using System;
using Unity.Collections;
using Unity.Netcode;

namespace Leaderboard
{
    public struct LeaderboardEntity : INetworkSerializable, IEquatable<LeaderboardEntity>
    {
        public ulong ClientId;
        public FixedString32Bytes PlayerName;
        public int PlayerCoins;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref PlayerCoins);
        }

        public bool Equals(LeaderboardEntity other)
        {
            return ClientId == other.ClientId &&
                PlayerName.Equals(other.PlayerName) &&
                PlayerCoins == other.PlayerCoins;
        }

    }
}
