using Cinemachine;
using Network;
using System.Threading.Tasks;
using UnityEngine;

public class HandleCameraPriority : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainVirtualCamera;

    private NetworkServer networkServer;

    private void Awake()
    {
        networkServer = HostSingleton.Instance.HostGameManager.NetworkServer;
        if (networkServer != null)
            networkServer.OnUserJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(UserData data)
    {
        Debug.Log("Test");
        mainVirtualCamera.Priority = 100;
        Task.Delay(250);
        mainVirtualCamera.Priority = 9;
    }

    private void OnDestroy()
    {
        networkServer.OnUserJoined -= OnPlayerJoined;
    }
}