using System.Threading.Tasks;
using Network;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientSingletonPrefab;
    [SerializeField] private HostSingleton hostSingletonPrefab;
    [SerializeField] private ServerSingleton serverSingletonPrefab;

    private ApplicationData applicationData;


    // Start is called before the first frame update
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            applicationData = new ApplicationData();

            ServerSingleton serverSingleton = Instantiate(serverSingletonPrefab);
            await serverSingleton.CreateServer();

            await serverSingleton.ServerGameManager.StartGameServerAsync();
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostSingletonPrefab);
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(clientSingletonPrefab);
            bool authenticated = await clientSingleton.CreateClient();

            // Go to Main Menu
            if (authenticated)
                clientSingleton.ClientGameManager.GoToMenu();
        }
    }
}
