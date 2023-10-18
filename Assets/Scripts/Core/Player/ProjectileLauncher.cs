using System.Threading;
using Coin;
using Combat;
using Input;
using Unity.Netcode;
using UnityEngine;

namespace PlayerCore
{
    public class ProjectileLauncher : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private GameObject serverProjectilePrefab;
        [SerializeField] private GameObject clientProjectilePrefab;
        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private Collider2D playerCollider;
        [SerializeField] private CoinWallet coinWallet;

        [Header("Setting")]
        [SerializeField] private float projectileSpeed;
        [SerializeField] private float fireRate;
        [SerializeField] private float muzzleFlashDuration;
        [SerializeField] private int costToFire;

        private bool shouldFire;
        private float timer;
        private float muzzleFlashTimer;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            inputReader.PrimaryFireEvent += HandlePrimaryFire;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;

            inputReader.PrimaryFireEvent -= HandlePrimaryFire;
        }

        // Update is called once per frame
        void Update()
        {
            if (muzzleFlashTimer > 0)
            {
                muzzleFlashTimer -= Time.deltaTime;
                if (muzzleFlashTimer <= 0)
                {
                    muzzleFlash.SetActive(false);
                }
            }

            if (!IsOwner) return;

            if (timer > 0)
                timer -= Time.deltaTime;

            if (!shouldFire) return;

            if (timer > 0) return;

            if (coinWallet.TotalCoins.Value < costToFire) return;

            PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
            SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

            timer = 1 / fireRate;
        }

        private void HandlePrimaryFire(bool shouldFire)
        {
            this.shouldFire = shouldFire;
        }

        [ServerRpc]
        private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
        {
            if (coinWallet.TotalCoins.Value < costToFire) return;

            coinWallet.SpendCoins(costToFire);

            GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);

            projectileInstance.transform.up = direction;

            Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

            if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
                dealDamage.SetOwner(OwnerClientId);

            if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                rb.velocity = rb.transform.up * projectileSpeed;

            PrimaryFireClientRpc(spawnPos, direction);
        }

        [ClientRpc]
        private void PrimaryFireClientRpc(Vector3 spawnPos, Vector3 direction)
        {
            if (IsOwner) return;
            SpawnDummyProjectile(spawnPos, direction);
        }

        private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
        {
            muzzleFlash.SetActive(true);
            muzzleFlashTimer = muzzleFlashDuration;

            GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

            projectileInstance.transform.up = direction;

            Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

            if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.velocity = rb.transform.up * projectileSpeed;
            }
        }
    }
}

