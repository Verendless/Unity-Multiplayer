using UnityEngine;

namespace Utils
{
    public class DestroySelfOnContact : MonoBehaviour
    {
        [SerializeField] private Projectile projectile;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(projectile.TeamIndex != -1)
            {
                if (collision.attachedRigidbody != null)
                    if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
                        if (player.TeamIndex.Value == projectile.TeamIndex)
                            return;
            }
            Destroy(this.gameObject);
        }
    }

}
