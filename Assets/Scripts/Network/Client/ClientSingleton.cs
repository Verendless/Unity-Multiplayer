using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class ClientSingleton : MonoBehaviour
    {
        private static ClientSingleton instance;

        public ClientGameManager ClientGameManager { get; private set; }

        public static ClientSingleton Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = FindFirstObjectByType<ClientSingleton>();

                if (instance == null) return null;

                return instance;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task<bool> CreateClient()
        {
            ClientGameManager = new ClientGameManager();
            return await ClientGameManager.InitAsync();
        }

        private void OnDestroy()
        {
            ClientGameManager?.Dispose();
        }
    }

}
