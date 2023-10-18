using Network;
using System;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text queueTimeText;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_InputField joinCodeField;

    private bool isMatchmaking;
    private bool isCanceling;

    private void Start()
    {
        if (ClientSingleton.Instance == null) return;

        // Reset cursor to default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        queueStatusText.text = string.Empty;
        queueTimeText.text = string.Empty;
    }

    public async void FindMatchPressed()
    {
        if (isCanceling) return;

        if (isMatchmaking)
        {
            queueStatusText.text = "Cancelling...";
            isCanceling = true;

            //Cancel Matchmaking
            await ClientSingleton.Instance.ClientGameManager.CancelMatchMaking();
            isCanceling = false;
            isMatchmaking = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            return;
        }


        // Start Queue
        ClientSingleton.Instance.ClientGameManager.MatchmakeAsync(OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching...";
        isMatchmaking = true;

    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch(result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusText.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusText.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatusText.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusText.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusText.text = "MatchAssignmentError";
                break;
        }
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.HostGameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCodeField.text);
    }
}
