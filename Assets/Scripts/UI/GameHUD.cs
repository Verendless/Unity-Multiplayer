using Network;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeText;

    private NetworkVariable<FixedString32Bytes> lobbyCode = new NetworkVariable<FixedString32Bytes>("");

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged += HandleLobbyCodeChanged;
            HandleLobbyCodeChanged("", lobbyCode.Value);
        }

        if (!IsHost) return;

        lobbyCode.Value = HostSingleton.Instance.HostGameManager.JoinCode;
    }

    private void HandleLobbyCodeChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        lobbyCodeText.text = newValue.ToString();
    }

    public override void OnNetworkDespawn()
    {
        lobbyCode.OnValueChanged -= HandleLobbyCodeChanged;
    }


    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.HostGameManager.Shutdown();
        }

        ClientSingleton.Instance.ClientGameManager.Disconnect();
    }
}
