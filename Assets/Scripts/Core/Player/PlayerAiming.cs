using Input;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerAiming : NetworkBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform turretTransfrom;

        private void LateUpdate()
        {
            if (!IsOwner) return;

            Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(inputReader.AimPosition);
            turretTransfrom.up = new Vector2(
                cursorPosition.x - turretTransfrom.position.x,
                cursorPosition.y - turretTransfrom.position.y
                );
        }
    }
}