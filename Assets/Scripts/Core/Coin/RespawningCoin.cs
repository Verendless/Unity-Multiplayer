namespace Coin
{
    public class RespawningCoin : Coin
    {
        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }

            if (isCollected) return 0;

            isCollected = true;
            return coinValue;
        }
    }
}

