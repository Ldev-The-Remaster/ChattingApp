namespace Frontend.Client.Models
{
    public static class UsernameColorizer
    {
        // Dictionary to cache username-color mappings
        private static readonly Dictionary<string, string> UsernameColorCache = new Dictionary<string, string>();

        public static string GenerateColorFromUsername(string? username)
        {
            // Check if the username is already in the cache
            if (UsernameColorCache.TryGetValue(username, out string cachedColor))
            {
                return cachedColor;
            }

            int hash = GetDeterministicHashCode(username);

            byte r = (byte)(hash >> 16 & 0xFF);
            byte g = (byte)(hash >> 8 & 0xFF);
            byte b = (byte)(hash & 0xFF);

            string hexColor = $"#{r:X2}{g:X2}{b:X2}";

            UsernameColorCache[username] = hexColor;

            return hexColor;
        }

        private static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in str)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

    }
}
