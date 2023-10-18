using System;
using Cinemachine;
using Coin;
using Combat;
using Network;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private SpriteRenderer playerMiniMapIcon;
    [SerializeField] private Texture2D crossHair;
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet coinWallet { get; private set; }

    [Header("Setting")]
    [SerializeField] private int ownerPriority = 15;
    [SerializeField] private Color playerMiniMapColor;


    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawn;
    public static event Action<TankPlayer> OnPlayerDespawn;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = null;
            if (IsHost)
            {
                userData = HostSingleton.Instance.HostGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                userData = ServerSingleton.Instance.ServerGameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
           
            playerName.Value = userData.userName;

            OnPlayerSpawn?.Invoke(this);
        }

        if (IsOwner)
        {
            cinemachineVirtualCamera.Priority = ownerPriority;

            playerMiniMapIcon.color = playerMiniMapColor;

            Cursor.SetCursor(crossHair, new Vector2(crossHair.width/2, crossHair.height/2), CursorMode.Auto);
        }
    }

    override public void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawn?.Invoke(this);
        }
    }
}
