using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton instance;

        public HostGameManager HostGameManager { get; private set; }

        public static HostSingleton Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = FindObjectOfType<HostSingleton>();

                if (instance == null) return null;

                return instance;
            }
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void CreateHost(NetworkObject playerPrefab)
        {
            HostGameManager = new HostGameManager(playerPrefab);
        }

        private void OnDestroy()
        {
            HostGameManager.Dispose();
        }
    }

}
