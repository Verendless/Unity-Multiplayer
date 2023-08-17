using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    private const int TimeToRespawn = 3;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // Search for existing player in the scene
        TankPlayer[] players = FindObjectsOfType<TankPlayer>();
        foreach (TankPlayer player in players)
        {
            HandlerPlayerSpawned(player);
        }

        // This is for to be spawned player and for despawned player
        TankPlayer.OnPlayerSpawn += HandlerPlayerSpawned;
        TankPlayer.OnPlayerSpawn += HandlerPlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        TankPlayer.OnPlayerSpawn -= HandlerPlayerSpawned;
        TankPlayer.OnPlayerSpawn -= HandlerPlayerDespawned;
    }

    private void HandlerPlayerSpawned(TankPlayer player)
    {
        player.Health.OnDied += (health) => HandlePlayerDeath(player);
    }

    private void HandlerPlayerDespawned(TankPlayer player)
    {
        player.Health.OnDied = (health) => HandlePlayerDeath(player);
    }

    private void HandlePlayerDeath(TankPlayer player)
    {
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    // Wait for next frame to respawn the player who died
    private IEnumerator RespawnPlayer(ulong OwnerClientId)
    {
        yield return new WaitForSecondsRealtime(TimeToRespawn);

        NetworkObject playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPointPos(), Quaternion.identity);
        playerInstance.SpawnAsPlayerObject(OwnerClientId);
    }
}
