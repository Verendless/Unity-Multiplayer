using System.Collections;
using System.Collections.Generic;
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
                if (instance == null) return instance;

                instance = FindObjectOfType<ClientSingleton>();

                if (instance == null)
                {
                    Debug.LogError("No ClientSingleton in the Scene!");
                    return null;
                }

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
    }

}