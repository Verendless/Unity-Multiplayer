using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class NetworkServer
    {
        private NetworkManager networkManager;

        public NetworkServer(NetworkManager networkManager)
        {
            this.networkManager = networkManager;

            networkManager.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            // Convert Payload to JSON String then get that JSON String to UserData
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            UserData userData = JsonUtility.FromJson<UserData>(payload);

            Debug.Log(userData.userName);

            response.Approved = true;
            response.CreatePlayerObject = true;
        }
    }

}
