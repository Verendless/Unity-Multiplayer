using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class HealingZone : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Image healPowerBar;

        [Header("Settings")]
        [SerializeField] private int maxHealPower = 30;
        [SerializeField] private float healCooldown = 60f;
        [SerializeField] private float healTickRate = 1f;
        [SerializeField] private int healCost = 10;
        [SerializeField] private int healPerTick = 10;

        private float RemainingCooldown;
        private float tickTimer;

        private List<TankPlayer> playerListInTheZone = new List<TankPlayer>();

        private NetworkVariable<int> healPower = new NetworkVariable<int>();

        public override void OnNetworkSpawn()
        {
            if (!IsClient) return;

            healPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, healPower.Value);

            if (!IsServer) return;

            healPower.Value = maxHealPower;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsClient) return;

            healPower.OnValueChanged -= HandleHealPowerChanged;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer) return;

            if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) return;

            playerListInTheZone.Add(player);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!IsServer) return;

            if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) return;

            playerListInTheZone.Remove(player);
        }

        private void Update()
        {
            if (!IsServer) return;

            if (RemainingCooldown > 0f)
            {
                RemainingCooldown -= Time.deltaTime;

                if(RemainingCooldown <= 0f)
                {
                    healPower.Value = maxHealPower;
                }
                else
                {
                    return;
                }
            }

            tickTimer += Time.deltaTime;
            if(tickTimer >= 1 / healTickRate)
            {
                foreach (TankPlayer player in playerListInTheZone)
                {
                    if (healPower.Value == 0) break;

                    if(player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue;

                    if (player.coinWallet.TotalCoins.Value < healCost) continue;

                    player.coinWallet.SpendCoins(healCost);
                    player.Health.RestoreHealth(healPerTick);

                    healPower.Value -= 1;

                    if(healPower.Value == 0)
                        RemainingCooldown = healCooldown;
                }

                tickTimer = tickTimer % (1 / healTickRate);
            }
        }

        private void HandleHealPowerChanged(int oldHealPower, int newHealPower)
        {
            healPowerBar.fillAmount = (float)newHealPower / maxHealPower;
        }

    }
}

