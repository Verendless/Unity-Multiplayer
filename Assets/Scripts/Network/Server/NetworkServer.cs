using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Network
{
    public class NetworkServer : IDisposable
    {
        private NetworkManager networkManager;
        private NetworkObject playerPrefab;
        private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
        private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
       
        public Action<UserData> OnUserJoined;
        public Action<UserData> OnUserLeft;

        public Action<string> OnClientLeft;


        public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab)
        {
            this.networkManager = networkManager;
            this.playerPrefab = playerPrefab;

            networkManager.ConnectionApprovalCallback += ApprovalCheck;
            networkManager.OnServerStarted += OnNetworkReady;
        }

        public bool OpenConnection(string ip, int port)
        {
            UnityTransport unityTransport = networkManager.gameObject.GetComponent<UnityTransport>();
            unityTransport.SetConnectionData(ip, (ushort)port);
            return networkManager.StartServer();
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            // Convert Payload to JSON String then get that JSON String to UserData
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            UserData userData = JsonUtility.FromJson<UserData>(payload);

            // Save user Id to the dictionary
            clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
            authIdToUserData[userData.userAuthId] = userData;
            
            // Invoke the event
            OnUserJoined?.Invoke(userData);

            _ = SpawnPlayerDelayed(request.ClientNetworkId);

            response.Approved = true;
            response.CreatePlayerObject = false;
        }

        // Spawn new Player within certain delay
        private async Task SpawnPlayerDelayed(ulong clientId)
        {
            await Task.Delay(100);

            NetworkObject playerInstance = GameObject.Instantiate(playerPrefab, 
                SpawnPoint.GetRandomSpawnPointPos(), 
                Quaternion.identity);

            playerInstance.SpawnAsPlayerObject(clientId);

        }

        private void OnNetworkReady()
        {
            networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        public UserData GetUserDataByClientId(ulong clientId)
        {
            if (clientIdToAuth.TryGetValue(clientId, out string authId))
            {
                if (authIdToUserData.TryGetValue(authId, out UserData data))
                    return data;

                return null;
            }

            return null;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (clientIdToAuth.TryGetValue(clientId, out string authId))
            {
                clientIdToAuth.Remove(clientId);
                OnUserLeft?.Invoke(authIdToUserData[authId]);
                authIdToUserData.Remove(authId);
                OnClientLeft?.Invoke(authId);
            }
        }

        public void Dispose()
        {
            if (networkManager == null) return;
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnServerStarted -= OnNetworkReady;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

            if (networkManager.IsListening)
                networkManager.Shutdown();
        }
    }

}
