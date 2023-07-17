using System;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public class Health : NetworkBehaviour
    {
        [field: SerializeField] public int MaxHealth { get; private set; } = 100;

        public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

        private bool isDead;

        public Action<Health> OnDied;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            CurrentHealth.Value = MaxHealth;
        }

        public void TakeDamage(int damage)
        {
            ModifiedHealth(-damage);
        }

        public void RestoreHealth(int health)
        {
            ModifiedHealth(+health);
        }

        private void ModifiedHealth(int value)
        {
            if (isDead) return;

            int modifiedHealth = CurrentHealth.Value + value;
            CurrentHealth.Value = Math.Clamp(modifiedHealth, 0, MaxHealth);

            if (CurrentHealth.Value <= 0)
            {
                isDead = true;
                OnDied?.Invoke(this);
            }

        }
    }
}
