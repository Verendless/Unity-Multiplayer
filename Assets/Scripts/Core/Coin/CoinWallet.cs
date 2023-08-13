using Unity.Netcode;

namespace Coin
{
    public class CoinWallet : NetworkBehaviour
    {
        public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

        private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
        {
            if (!collision.TryGetComponent(out Coin coin)) return;

            int tempCoinWallet = coin.Collect();

            if (!IsServer) return;

            TotalCoins.Value += tempCoinWallet;
        }

        public void SpenCoins(int value)
        {
            TotalCoins.Value -= value;
        }
    }

}
