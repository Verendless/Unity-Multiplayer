using Unity.Netcode.Components;

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

            // Let client handle their own character movement
            // This is for reduced the lag and rubber banding
            // NOTE: This is creating ticks duplication
            // Disabled for now
            /*            if (NetworkManager == null) return;
                        if (!NetworkManager.IsConnectedClient && !NetworkManager.IsListening) return;
                        if (CanCommitToTransform)
                            TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);*/
        }

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
