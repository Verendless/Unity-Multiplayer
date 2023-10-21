using TMPro;
using Unity.Collections;
using UnityEngine;

namespace Leaderboard
{
    public class LeaderboardItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text displayText;

        private FixedString32Bytes displayName;

        public int TeamIndex {  get; private set; }
        public ulong ClientId { get; private set; }
        public int Coins { get; private set; }

        public void Initialize(ulong clientId, FixedString32Bytes displayName, int playerCoins)
        {
            ClientId = clientId;
            this.displayName = displayName;

            UpdateCoin(playerCoins);
        }

        public void Initialise(int teamIndex, FixedString32Bytes displayName, int playerCoins)
        {
            TeamIndex = teamIndex;
            this.displayName = displayName;

            UpdateCoin(playerCoins);
        }

        public void SetColor(Color color)
        {
            displayText.color = color;
        }

        public void UpdateCoin(int coins)
        {
            Coins = coins;

            UpdateText();
        }

        public void UpdateText()
        {
            displayText.text = $"{transform.GetSiblingIndex() + 1}. {displayName} - {Coins}";
        }
    }

}
