using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Combat
{
    public class DealDamageOnContact : MonoBehaviour
    {

        [SerializeField] private Projectile projectile;
        [SerializeField] private int damage = 5;

        private ulong ownerClientId;

        public void SetOwner(ulong ownerClientId)
        {
            this.ownerClientId = ownerClientId;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.attachedRigidbody) return;

            if(projectile.TeamIndex != -1)
                if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer tankPlayer))
                    if (tankPlayer.TeamIndex.Value == projectile.TeamIndex)
                        return;

            if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
                health.TakeDamage(damage);
        }
    }

}
