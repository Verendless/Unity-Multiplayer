using Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    // Start is called before the first frame update
    void Start()
    {
        inputReader.MovementEvent += HandleMovement;
    }

    private void OnDestroy()
    {
        inputReader.MovementEvent -= HandleMovement;
    }

    private void HandleMovement(Vector2 movement)
    {
        Debug.Log(movement);
    }
}
