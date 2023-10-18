using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Network
{
    public class NetworkClient : IDisposable
    {
        private NetworkManager networkManager;

        private const string MenuSceneName = "Menu";

        public NetworkClient(NetworkManager networkManager)
        {
            this.networkManager = networkManager;

            networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            // Check if the client who disconnected is the Host
            if (clientId != 0 && clientId != networkManager.LocalClientId) return;

            Disconnect();
        }

        public void Disconnect()
        {
            // Go back to main menu if got disconnected from the game scene
            if (SceneManager.GetActiveScene().name != MenuSceneName)
                SceneManager.LoadScene(MenuSceneName);

            // When failed to connect in main menu then stop trying to connect
            if (networkManager.IsConnectedClient)
                networkManager.Shutdown();
        }

        public void Dispose()
        {
            if (networkManager != null)
                networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}