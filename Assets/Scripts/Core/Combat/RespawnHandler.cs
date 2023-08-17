using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float keptCoinPercentage;

    private const int TimeToRespawn = 3;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // Search for existing player in the scene
        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        // This is for to be spawned player and for despawned player
        TankPlayer.OnPlayerSpawn += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawn += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        TankPlayer.OnPlayerSpawn -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawn -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.OnDied += (health) => HandlePlayerDeath(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDied = (health) => HandlePlayerDeath(player);
    }

    private void HandlePlayerDeath(TankPlayer player)
    {
        int keptCoint = (int)(player.coinWallet.TotalCoins.Value * (keptCoinPercentage / 100));

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoint));
    }

    // Wait for x seconds to respawn the player who died
    private IEnumerator RespawnPlayer(ulong OwnerClientId, int keptCoints)
    {
        yield return new WaitForSecondsRealtime(TimeToRespawn);

        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPointPos(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(OwnerClientId);
        playerInstance.coinWallet.TotalCoins.Value += keptCoints;
    }
}
