using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

namespace Network
{
    public class ServerGameManager : IDisposable
    {
        private string serverIP;
        private int serverPort;
        private int serverQPort;
        private MatchplayBackfiller backfiller;
        private MultiplayAllocationService multiplayAllocationService;

        private Dictionary<string, int> teamIdToTeamIndex = new Dictionary<string, int>();

        public NetworkServer NetworkServer { get; private set; }

        public ServerGameManager(string serverIP, int serverPort, int serverQPort, 
            NetworkManager networkManager, NetworkObject playerPrefab)
        { 
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.serverQPort = serverQPort;
            NetworkServer = new NetworkServer(networkManager, playerPrefab);
            multiplayAllocationService = new MultiplayAllocationService();
        }

        public async Task StartGameServerAsync()
        {
            await multiplayAllocationService.BeginServerCheck();

            try
            {
                MatchmakingResults matchmakerPayload = await GetMatchmakingPayload();

                if(matchmakerPayload != null)
                {
                    await StartBackfill(matchmakerPayload);
                    NetworkServer.OnUserJoined += UserJoined;
                    NetworkServer.OnUserLeft += UserLeft;
                }
                else
                {
                    Debug.LogWarning("Matchmaker payload timed out");
                }

            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if(!NetworkServer.OpenConnection(serverIP, serverPort))
            {
                Debug.LogWarning("NetworkServer did not start as expected.");
                return;
            }
        }

        private async Task<MatchmakingResults> GetMatchmakingPayload()
        {
            Task<MatchmakingResults> matchmakerPlayloadTask =
                multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

            if(await Task.WhenAny(matchmakerPlayloadTask, Task.Delay(2000)) == matchmakerPlayloadTask)
            {
                return matchmakerPlayloadTask.Result;
            }

            return null;
        }

        // Do backfilling
        private async Task StartBackfill(MatchmakingResults matchmakerPayload)
        {
            backfiller = new MatchplayBackfiller($"{serverIP}:{serverPort}", 
                matchmakerPayload.QueueName, 
                matchmakerPayload.MatchProperties, 
                20);

            if(backfiller.NeedsPlayers())
                await backfiller.BeginBackfilling();
        }

        private void UserJoined(UserData userData)
        {
            // Assign new player to the team
            Team team = backfiller.GetTeamByUserId(userData.userAuthId);

            // Assign team index to the player
            if(!teamIdToTeamIndex.TryGetValue(team.TeamId, out int teamIndex))
            {
                // If there only the player in the team, add team to the dictionary
                teamIndex = teamIdToTeamIndex.Count;
                teamIdToTeamIndex.Add(team.TeamId, teamIndex);
            }
            userData.teamIndex = teamIndex;

            multiplayAllocationService.AddPlayer();

            // If there is no need to add other player, stop the backfilling
            if (!backfiller.NeedsPlayers() && backfiller.IsBackfilling)
               _ = backfiller.StopBackfill();
        }

        private void UserLeft(UserData userData)
        {
            // Remove the player from the match
            int playerCount = backfiller.RemovePlayerFromMatch(userData.userAuthId);
            multiplayAllocationService.RemovePlayer();

            // If there are no player, close the server
            if(playerCount <= 0)
            {
                // Close server
                CloseServer();
                return;
            }

            // If there are still player in the server, start backfilling if needed
            if (backfiller.NeedsPlayers() && !backfiller.IsBackfilling)
            {
                _ = backfiller.BeginBackfilling();
            }
        }

        private async void CloseServer()
        {
            await backfiller.StopBackfill();
            Dispose();
            Application.Quit();
        }

        public void Dispose()
        {
            NetworkServer.OnUserJoined -= UserJoined;
            NetworkServer.OnUserLeft -= UserLeft;

            backfiller?.Dispose();
            multiplayAllocationService?.Dispose();
            NetworkServer?.Dispose();
        }
    }
}
