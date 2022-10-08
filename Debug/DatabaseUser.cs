using System;

namespace Debug
{
    public class DatabaseUser
    {
        public string telegramId;
        public DateTimeOffset expireAt;

        public bool IsExpired()
        {
            return expireAt <= DateTimeOffset.UtcNow;
        }
    }
}