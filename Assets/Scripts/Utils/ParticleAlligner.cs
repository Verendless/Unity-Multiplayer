using UnityEngine;

[RequireComponent (typeof(ParticleSystem))]
public class ParticleAlligner : MonoBehaviour
{
    private ParticleSystem.MainModule psMain;

    // Start is called before the first frame update
    private void Start()
    {
        psMain = GetComponent<ParticleSystem>().main;
    }

    // Update is called once per frame
    private void Update()
    {
        psMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
