using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

namespace Network
{
    public class ClientNetworkTransform : NetworkTransform
    {

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            CanCommitToTransform = IsOwner;
        }

        protected override void Update()
        {
            CanCommitToTransform = IsOwner;
            base.Update();

            // Let client handle thier own character movement
            // This is for reduced the lag and rubber banding
            if(NetworkManager != null)
                if(NetworkManager.IsConnectedClient || NetworkManager.IsListening)
                    if (CanCommitToTransform)
                        TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
