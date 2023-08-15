using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class ClientGameManager
    {
        private const string MenuSceneName = "Menu";

        private JoinAllocation joinAllocation;

        public async Task<bool> InitAsync()
        {
            // Authenticate Player
            await UnityServices.InitializeAsync();
            AuthState authState = await AuthenticationHandler.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                return true;
            }
            return false;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
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

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "udp");
            unityTransport.SetRelayServerData(relayServerData);

            // Start the client
            NetworkManager.Singleton.StartClient();
        }
    }

}
