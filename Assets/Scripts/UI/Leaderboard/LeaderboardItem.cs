using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Leaderboard
{
    public class LeaderboardItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private Color myColor;

        public ulong ClientId { get; private set; }
        public int PlayerCoins { get; private set; }

        private FixedString32Bytes playerName;


        public void Initialize(ulong clientId, FixedString32Bytes playerName, int playerCoins)
        {
            ClientId = clientId;
            this.playerName = playerName;

            if(clientId.Equals(NetworkManager.Singleton.LocalClientId))
                displayText.color = myColor;

            UpdateCoin(playerCoins);
        }

        public void UpdateCoin(int coins)
        {
            PlayerCoins = coins;

            UpdateText();
        }

        public void UpdateText()
        {
            displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} - {PlayerCoins}";
        }
    }

}
