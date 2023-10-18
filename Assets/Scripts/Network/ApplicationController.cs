using System.Collections;
using System.Threading.Tasks;
using Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientSingletonPrefab;
    [SerializeField] private HostSingleton hostSingletonPrefab;
    [SerializeField] private ServerSingleton serverSingletonPrefab;
    [SerializeField] private NetworkObject playerPrefab;

    private ApplicationData applicationData;

    private const string GameSceneName = "Game";


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
            Application.targetFrameRate = 60;

            applicationData = new ApplicationData();

            ServerSingleton serverSingleton = Instantiate(serverSingletonPrefab);

            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostSingletonPrefab);
            hostSingleton.CreateHost(playerPrefab);

            ClientSingleton clientSingleton = Instantiate(clientSingletonPrefab);
            bool authenticated = await clientSingleton.CreateClient();

            // Go to Main Menu
            if (authenticated)
                clientSingleton.ClientGameManager.GoToMenu();
        }
    }

    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameSceneName);

        while(!asyncOperation.isDone)
        {
            yield return null;
        }

        Task createServerTask = serverSingleton.CreateServer(playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        Task startServerTask = serverSingleton.ServerGameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServerTask.IsCompleted);

    }
}
