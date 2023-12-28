using Combat;
using Unity.Netcode;
using UnityEngine;

namespace Coin
{
    public class CoinWallet : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Health health;

        [SerializeField] private BountyCoin bountyCoinPrefab;

        [Header("Settings")]
        [SerializeField] private float coinSpread = 3f;

        [SerializeField] private float bountyCoinPercentage = 50f;
        [SerializeField] private int bountyCoinCount = 10;
        [SerializeField] private int minBountyCoinValue = 5;
        [SerializeField] private LayerMask layerMask;

        private readonly Collider2D[] coinBuffer = new Collider2D[1];
        private float coinRadius;

        public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            coinRadius = bountyCoinPrefab.GetComponent<CircleCollider2D>().radius;

            health.OnDied += HandleDied;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;

            health.OnDied -= HandleDied;
        }

        public void SpendCoins(int value)
        {
            TotalCoins.Value -= value;
        }

        private void HandleDied(Health health)
        {
            int bountyValue = (int)(TotalCoins.Value * (bountyCoinPercentage / 100f));
            int bountyCoinValue = bountyValue / bountyCoinCount;

            if (bountyCoinValue < minBountyCoinValue) return;

            for (int i = 0; i < bountyCoinCount; i++)
            {
                BountyCoin coinInstance = Instantiate(bountyCoinPrefab, GetSpawnPoint(), Quaternion.identity);
                coinInstance.SetValue(bountyCoinValue);
                coinInstance.NetworkObject.Spawn();
            }
        }

        private Vector2 GetSpawnPoint()
        {
            while (true)
            {
                Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
                ContactFilter2D contactFilter2D = new()
                {
                    layerMask = layerMask
                };
                int numColliders = Physics2D.OverlapCircle(spawnPoint, coinRadius, contactFilter2D, coinBuffer);

                if (numColliders == 0)
                    return spawnPoint;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out Coin coin))
                return;

            int tempCoinWallet = coin.Collect();

            if (!IsServer)
                return;

            TotalCoins.Value += tempCoinWallet;
        }
    }
}