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
        private string joinCode;
        private const int MaxConnections = 20;

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
                joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log(joinCode);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return;
            }

            // Set allocation transport protocol to relay
            UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayServerData = new RelayServerData(allocation, "udp");
            unityTransport.SetRelayServerData(relayServerData);

            // Start the host
            NetworkManager.Singleton.StartHost();

            // Load to Game scene
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
    }
}
