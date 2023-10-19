using Network;
using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private TMP_Text queueTimerText;
    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private Toggle teamToggle;
    [SerializeField] private Toggle lobbyVisiblityToggle;

    private bool isMatchmaking;
    private bool isCanceling;
    private bool isBusy;
    private float timeInQueue;

    private void Start()
    {
        if (ClientSingleton.Instance == null) return;

        // Reset cursor to default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        queueStatusText.text = string.Empty;
        queueTimerText.text = string.Empty;
    }

    private void Update()
    {
        if(isMatchmaking)
        {
            timeInQueue += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInQueue);
            queueTimerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        }
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
            isBusy = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            queueTimerText.text = string.Empty;
            return;
        }

        if (isBusy) return;

        // Start Queue
        ClientSingleton.Instance.ClientGameManager.MatchmakeAsync(teamToggle.isOn, OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching...";
        timeInQueue = 0;
        isMatchmaking = true;
        isBusy = true;

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
        if(isBusy) return;

        isBusy = true;

        await HostSingleton.Instance.HostGameManager.StartHostAsync(lobbyVisiblityToggle.isOn);

        isBusy = false;
    }

    public async void StartClient()
    {
        if (isBusy) return;

        isBusy = true;

        await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCodeField.text);

        isBusy = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isBusy) return;

        isBusy = true;

        try
        {
            // Get JoinCode from selected lobby
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            // Joining the lobby using JoinCode
            await ClientSingleton.Instance.ClientGameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        isBusy = false;
    }
}
