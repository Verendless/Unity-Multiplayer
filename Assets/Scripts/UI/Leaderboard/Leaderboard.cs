using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Leaderboard
{
    public class Leaderboard : NetworkBehaviour
    {
        [SerializeField] private Transform leaderboardItemHolder;
        [SerializeField] private LeaderboardItem leaderboardItemPrefab;
        [SerializeField] private int itemToDisplay = 8;

        private NetworkList<LeaderboardEntity> leaderboardEntities;
        private List<LeaderboardItem> leaderboardItems = new List<LeaderboardItem>();

        private void Awake()
        {
            leaderboardEntities = new NetworkList<LeaderboardEntity>();
        }

        public override void OnNetworkSpawn()
        {
            if(IsClient)
            {
                leaderboardEntities.OnListChanged += HandleLeaderboardEntitiesChanged;

                foreach(LeaderboardEntity entity in leaderboardEntities)
                {
                    HandleLeaderboardEntitiesChanged(new NetworkListEvent<LeaderboardEntity>
                    {
                        Type = NetworkListEvent<LeaderboardEntity>.EventType.Add,
                        Value = entity
                    });
                }
            }

            if (!IsServer) return;

            // Search all player
            TankPlayer[] tankPlayers = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
            foreach (TankPlayer tankPlayer in tankPlayers)
            {
                HandlePlayerSpawned(tankPlayer);
            }

            // This is for to be spawned player and for despawned player
            TankPlayer.OnPlayerSpawn += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawn += HandlePlayerDespawned;
            TankPlayer.OnPlayerDespawn -= HandlePlayerDespawned;
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                leaderboardEntities.OnListChanged -= HandleLeaderboardEntitiesChanged;
            }

            if (!IsServer) return;

            TankPlayer.OnPlayerSpawn -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawn -= HandlePlayerDespawned;
        }

        private void HandleLeaderboardEntitiesChanged(NetworkListEvent<LeaderboardEntity> changeEvent)
        {
            switch(changeEvent.Type)
            {
                // When there are new players joining the room, add them to the leaderboard list
                case NetworkListEvent<LeaderboardEntity>.EventType.Add:
                    if(!leaderboardItems.Any(x => x.ClientId.Equals(changeEvent.Value.ClientId)))
                    {
                        LeaderboardItem leaderboardItem = Instantiate(leaderboardItemPrefab, leaderboardItemHolder);
                        leaderboardItem.Initialize(
                            changeEvent.Value.ClientId, 
                            changeEvent.Value.PlayerName, 
                            changeEvent.Value.PlayerCoins);
                        leaderboardItems.Add(leaderboardItem);
                    }
                    break;
                // Remove player's leaderboard when they disconnected from the server
                case NetworkListEvent<LeaderboardEntity>.EventType.Remove:
                    LeaderboardItem itemToRemove = 
                        leaderboardItems.FirstOrDefault(x => x.ClientId.Equals(changeEvent.Value.ClientId));
                    if (itemToRemove != null)
                    {
                        itemToRemove.transform.SetParent(null);
                        leaderboardItems.Remove(itemToRemove);
                        Destroy(itemToRemove.gameObject);
                    }
                    break;
                // Update player's leaderboard value
                case NetworkListEvent<LeaderboardEntity>.EventType.Value:
                    LeaderboardItem itemToUpdate = 
                        leaderboardItems.FirstOrDefault(x => x.ClientId.Equals(changeEvent.Value.ClientId));
                    if(itemToUpdate != null ) 
                    {
                        itemToUpdate.UpdateCoin(changeEvent.Value.PlayerCoins);
                    }
                    break;
            }

            // Sort the Leaderboard
            leaderboardItems.Sort((x, y) => y.PlayerCoins.CompareTo(x.PlayerCoins));

            for (int i = 0; i < leaderboardItems.Count; i++)
            {
                leaderboardItems[i].transform.SetSiblingIndex(i);
                leaderboardItems[i].UpdateText();

                bool shouldShow = i <= itemToDisplay - 1;
                leaderboardItems[i].gameObject.SetActive(shouldShow);
            }

            // If my leaderboard score is greater than itemToDisplay, hide the 8th player and show mine
            LeaderboardItem myDisplayItem = leaderboardItems.FirstOrDefault(
                x => x.ClientId.Equals(NetworkManager.Singleton.LocalClientId));

            if(myDisplayItem != null)
            {
                if(myDisplayItem.transform.GetSiblingIndex() >= itemToDisplay)
                {
                    leaderboardItemHolder.GetChild(itemToDisplay - 1).gameObject.SetActive(false);
                    myDisplayItem.gameObject.SetActive(true);
                }
            }
        }

        // Create new Leaderboard Entity for new player
        private void HandlePlayerSpawned(TankPlayer player)
        {
            leaderboardEntities.Add(new LeaderboardEntity
            {
                ClientId = player.OwnerClientId,
                PlayerName = player.playerName.Value,
                PlayerCoins = 0
            });

            player.coinWallet.TotalCoins.OnValueChanged += (oldCoin, newCoins) =>
                HandleCoinChanged(player.OwnerClientId, newCoins);
        }

        // Delete player's leaderboard entity when dispawned
        private void HandlePlayerDespawned(TankPlayer player)
        {
            if (!gameObject.scene.isLoaded) return;

            foreach (LeaderboardEntity entity in leaderboardEntities)
            {
                if (entity.ClientId != player.OwnerClientId) continue;

                leaderboardEntities.Remove(entity);
                break;
            }


            player.coinWallet.TotalCoins.OnValueChanged -= (oldCoin, newCoins) =>
                HandleCoinChanged(player.OwnerClientId, newCoins);
        }

        private void HandleCoinChanged(ulong clientId, int newCoins)
        {
            for (int i = 0; i < leaderboardEntities.Count; i++)
            {
                if (leaderboardEntities[i].ClientId != clientId) continue;

                leaderboardEntities[i] = new LeaderboardEntity
                {
                    ClientId = leaderboardEntities[i].ClientId,
                    PlayerName = leaderboardEntities[i].PlayerName,
                    PlayerCoins = newCoins,
                };

                return;
            }    
        }
    }
}

