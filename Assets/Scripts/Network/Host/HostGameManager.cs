using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class HostGameManager
    {
        private Allocation allocation;
        private const int MaxConnections = 20;

        public string JoinCode { get; private set; }

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

            // Start the host
            NetworkManager.Singleton.StartHost();

            // Load to Game scene
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
    }
}
