using System;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.SimpleDAO
{
    public static class TimestampHelper
    {
        public static Timestamp GetUtcNow()
        {
            return Timestamp.FromDateTime(DateTime.UtcNow);
        }
    }
} 