using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class NetworkServer : IDisposable
    {
        private NetworkManager networkManager;
        private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
        private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

        public NetworkServer(NetworkManager networkManager)
        {
            this.networkManager = networkManager;

            networkManager.ConnectionApprovalCallback += ApprovalCheck;
            networkManager.OnServerStarted += OnNetworkReady;
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

            response.Approved = true;
            response.Position = SpawnPoint.GetRandomSpawnPointPos();
            response.Rotation = Quaternion.identity;
            response.CreatePlayerObject = true;
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
                authIdToUserData.Remove(authId);
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
