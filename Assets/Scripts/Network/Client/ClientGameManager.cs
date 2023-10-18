using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class ClientGameManager : IDisposable
    {
        private const string MenuSceneName = "Menu";

        private JoinAllocation joinAllocation;

        private NetworkClient networkClient;

        private MatchplayMatchmaker matchmaker;

        private UserData userData;

        public async Task<bool> InitAsync()
        {
            // Authenticate Player
            await UnityServices.InitializeAsync();

            networkClient = new NetworkClient(NetworkManager.Singleton);
            matchmaker = new MatchplayMatchmaker();

            AuthState authState = await AuthenticationHandler.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                // Set user data
                userData = new UserData
                {
                    userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                    userAuthId = AuthenticationService.Instance.PlayerId,
                    userGamePreferences = new GameInfo()
                };
                return true;
            }
            return false;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        public void StartClient(string ip, int port)
        {
            // Set allocation transport protocol to relay
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            unityTransport.SetConnectionData(ip, (ushort)port);
            ConnectiClient();
        }

        public async Task StartClientAsync(string joinCode)
        {
            // Try join the server alloction with given joinCode
            try
            {
                joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return;
            }

            // Set allocation transport protocol to relay
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);

            ConnectiClient();
        }

        private void ConnectiClient()
        {
            // Convert userData from JSON to byte array
            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            // Send Username to the network
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

            // Start the client
            NetworkManager.Singleton.StartClient();
        }

        public async void MatchmakeAsync(Action<MatchmakerPollingResult> onMatchmakeResponse)
        {
            if(matchmaker.IsMatchmaking) { return; }

            MatchmakerPollingResult matchmakerPollingResult = await GetMatchAsync();
            onMatchmakeResponse?.Invoke(matchmakerPollingResult);
        }

        private async Task<MatchmakerPollingResult> GetMatchAsync()
        {
            MatchmakingResult matchmakingResult = await matchmaker.Matchmake(userData);

            if(matchmakingResult.result == MatchmakerPollingResult.Success)
            {
                StartClient(matchmakingResult.ip, matchmakingResult.port);
            }

            return matchmakingResult.result;
        }
        public async Task CancelMatchMaking()
        {
            await matchmaker.CancelMatchmaking();
        }

        public void Disconnect()
        {
            networkClient.Disconnect();
        }

        public void Dispose()
        {
            networkClient?.Dispose();
        }
    }
}
