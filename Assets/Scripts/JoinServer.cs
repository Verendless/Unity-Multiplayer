using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace UI
{
    public class JoinServer : MonoBehaviour
    {
        public void Join()
        {
            NetworkManager.Singleton.StartClient();
        }
    }

}
