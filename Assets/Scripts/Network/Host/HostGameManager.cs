using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using Unity.Services.Authentication;

namespace Network
{
    public class HostGameManager
    {
        private Allocation allocation;
        private const int MaxConnections = 20;

        private NetworkServer networkServer;

        public string JoinCode { get; private set; }
        private string lobbyId;

        private const string GameSceneName = "Game";

        public async Task StartHostAsync()
        {
            // Try get server allocation with certain MaxConnection
            try
            {
                allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return;
            }

            // Try get allocated join code if we get the server allocation
            try
            {
                JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log(JoinCode);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return;
            }

            // Set allocation transport protocol to relay
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            unityTransport.SetRelayServerData(relayServerData);

            // Setup lobby
            try
            {
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
                lobbyOptions.IsPrivate = false;
                lobbyOptions.Data = new Dictionary<string, DataObject>()
                {
                  {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: JoinCode
                    )
                  }
                };
                string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Unkown");
                Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MaxConnections, lobbyOptions);
                lobbyId = lobby.Id;

                // Ping UGS every x seconds to keep the lobbies alive
                HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
                return;
            }

            networkServer = new NetworkServer(NetworkManager.Singleton);

            // Set Username on the network
            UserData userData = new UserData
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };

            // Convert userData from JSON to byte array
            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            // Send Username to the network
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

            // Start the host
            NetworkManager.Singleton.StartHost();

            // Load to Game scene
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }

        private IEnumerator HeartbeatLobby(float waitTimeSec)
        {
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(waitTimeSec);
            while (true)
            {
                Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
                yield return wait;
            }
        }
    }
}
