using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Network
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton instance;

        private HostGameManager hostGameManager;

        public static HostSingleton Instance
        {
            get
            {
                if (instance == null) return instance;

                instance = FindObjectOfType<HostSingleton>();

                if (instance == null)
                {
                    Debug.LogError("No HostSingleton in the Scene!");
                    return null;
                }

                return instance;
            }
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void CreateHost()
        {
            hostGameManager = new HostGameManager();
        }
    }

}
