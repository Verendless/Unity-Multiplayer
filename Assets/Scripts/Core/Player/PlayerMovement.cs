using Input;
using Unity.Netcode;
using UnityEngine;

namespace PlayerCore
{
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform bodyTransform;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private ParticleSystem duskCloud;

        [Header("Settings")]
        [SerializeField] private float movementSpeed = 4f;
        [SerializeField] private float turningRate = 100f;
        [SerializeField] private float particleEmmisionValue = 10f;

        private ParticleSystem.EmissionModule emissionModule;
        private Vector2 previousMovementInput;
        private Vector3 previousPos;

        private const float ParticleStopThreshold = 0.005f;

        private void Awake()
        {
            emissionModule = duskCloud.emission;
        }

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
            emissionModule.rateOverTime =
                (transform.position - previousPos).sqrMagnitude > ParticleStopThreshold ?
                emissionModule.rateOverTime = particleEmmisionValue : emissionModule.rateOverTime = 0;

            previousPos = transform.position;

            if (!IsOwner) return;

            rb.velocity = movementSpeed * previousMovementInput.y * (Vector2)bodyTransform.up;
        }

        private void HandleMove(Vector2 movement)
        {
            previousMovementInput = movement;
        }


    }
}

