using System;
using UnityEngine;

namespace Coin
{
    public class RespawningCoin : Coin
    {
        public event Action<RespawningCoin> OnCollected;
        private Vector3 previousPos;

        public override void OnNetworkSpawn()
        {
            previousPos = transform.position;
        }

        private void Update()
        {
            if(!IsServer) { return; }
            if(transform.position != previousPos)
            {
                Show(true);
            }
            previousPos = transform.position;

        }

        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }

            if (isCollected) return 0;

            isCollected = true;
            OnCollected?.Invoke(this);

            return coinValue;
        }

        public void Reset()
        {
            isCollected = false;
        }
    }
}

