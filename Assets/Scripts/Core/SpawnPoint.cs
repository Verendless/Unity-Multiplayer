using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private void OnEnable()
    {
        spawnPoints.Add(this);
    }

    public static Vector3 GetRandomSpawnPointPos()
    {
        // if for some reason there are no spawn point, return Vector3.Zero (0, 0, 0)
        if (spawnPoints.Count == 0) return Vector3.zero;

        // Get random spawn point for players to spawn in
        int randIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randIndex].transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1);
    }

    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }
}
