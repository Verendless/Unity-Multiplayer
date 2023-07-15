using Input;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Rigidbody2D rb;

        [Header("Settings")]
        [SerializeField] private float movementSpeed = 100f;
        [SerializeField] private float turningRate = 100f;

        private Vector2 previousMovementInput;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            inputReader.MovementEvent += HandleMove;

        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            inputReader.MovementEvent -= HandleMove;
        }  
        
        // Update is called once per frame
        private void Update()
        {
            if (!IsOwner) return;

            float zRotation = previousMovementInput.x * -turningRate * Time.deltaTime;
            bodyTransform.Rotate(0f, 0f, zRotation);
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            rb.velocity = (Vector2)bodyTransform.up * previousMovementInput.y * movementSpeed;
        }

        private void HandleMove(Vector2 movement)
        {
            previousMovementInput = movement;
        }


    }
}

