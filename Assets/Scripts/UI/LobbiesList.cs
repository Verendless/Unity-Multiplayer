using System;
using System.Collections.Generic;
using Network;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem lobbyItemPrefab;
    [SerializeField] private Transform lobbyItemParent;

    private bool isJoining;
    private bool isRefreshing;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefreshing) return;

        isRefreshing = true;

        try
        {
            // Show 25 Lobbies that meet the options bellow
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Only show lobby with more than 0 AvailableSlots
            // Don't show locked lobby
            options.Filters = new List<QueryFilter>()
            {
            new QueryFilter(
                field: QueryFilter.FieldOptions.AvailableSlots,
                // OpOption.GT mean Greater Than
                op: QueryFilter.OpOptions.GT,
                value: "0"),
            new QueryFilter(
                field: QueryFilter.FieldOptions.IsLocked,
                // OpOption.EQ mean Equal to
                op: QueryFilter.OpOptions.EQ,
                value: "0")
            };

            // Get the lobby that meet the criteria
            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            // Destory all lobbyItemParent child if anny
            foreach (Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            // Instantiate lobbyItem
            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.Initialize(this, lobby);
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;

        isJoining = true;

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

        isJoining = false;
    }
}
