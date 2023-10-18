using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

namespace Network
{
    public class ServerSingleton : MonoBehaviour
    {
        private static ServerSingleton instance;

        public ServerGameManager ServerGameManager { get; private set; }

        public static ServerSingleton Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = FindObjectOfType<ServerSingleton>();

                if (instance == null) return null;

                return instance;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task CreateServer(NetworkObject playerPrefab)
        {
            await UnityServices.InitializeAsync();
            ServerGameManager = new ServerGameManager(
                ApplicationData.IP(),
                ApplicationData.Port(),
                ApplicationData.QPort(),
                NetworkManager.Singleton,
                playerPrefab);
        }

        private void OnDestroy()
        {
            ServerGameManager?.Dispose();
        }
    }

}
