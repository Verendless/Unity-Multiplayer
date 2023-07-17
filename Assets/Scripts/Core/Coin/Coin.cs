using Unity.Netcode;
using UnityEngine;

namespace Coin
{
    public abstract class Coin : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        protected int coinValue = 10;
        protected bool isCollected;

        public abstract int Collect();

        public void SetValue(int value)
        {
            coinValue = value;
        }

        protected void Show(bool show)
        {
            spriteRenderer.enabled = show;
        }
    }

}
