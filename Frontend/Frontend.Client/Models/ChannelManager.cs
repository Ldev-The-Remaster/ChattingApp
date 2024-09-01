using LSMP;

namespace Frontend.Client.Models
{
    public static class ChannelManager
    {
        public class ChannelData
        {
            public List<UserMessage> MessageHistory { get; set; } = new List<UserMessage>();
            public List<string> UserList { get; set; } = new List<string>();
        }

        public static Dictionary<string, ChannelData> channels = new Dictionary<string, ChannelData>
        {
            {"general-chat", new ChannelData() }
        };

        public static string CurrentChannel { get; set; } = "general-chat";

        public static event Action? OnStateChange;
        public static event Action? OnChannelChanged;
        public static event Action? OnFinishLoadingFullMessageHistory;
        public static event Action? OnFinishedCurrentLoadingOperation;

        public static string CreateDmChannel(string targetUser)
        {
            var dmChannelName = Messaging.GetDirectMessageChannelHash(ClientManager.CurrentUser, targetUser);
            if (channels.ContainsKey(dmChannelName))
            {
                return dmChannelName;
            }

            var channelData = new ChannelData
            {
                UserList = new List<string>
                {
                    ClientManager.CurrentUser,
                    targetUser
                },
            };

            channels.Add(dmChannelName, channelData);
            OnStateChange?.Invoke();
            return dmChannelName;
        }

        public static void RequestMessageHistory(string channelName, int count)
        {
            if (channels.ContainsKey(channelName))
            {
                int from = channels[channelName].MessageHistory.Count + 1;
                int to = from + count - 1;
                _ = WebSocketService.SendMessageAsync(Messaging.RememberMessage(channelName, from, to));
            }
        }

        public static bool UpdateChannelUserList(string channelName, List<string> userList)
        {
            if (channels.ContainsKey(channelName))
            {
                channels[channelName].UserList= userList;
                OnStateChange?.Invoke();
                return true;
            }

            return false;
        }

        public static bool UpdateChannelMessageHistory(string channelName, List<UserMessage> messageHistory)
        {
            if (channels.ContainsKey(channelName))
            {
                if (messageHistory.Count == 0)
                {
                    OnFinishLoadingFullMessageHistory?.Invoke();
                }

                channels[channelName].MessageHistory.AddRange(messageHistory);
                OnStateChange?.Invoke();
                OnFinishedCurrentLoadingOperation.Invoke();
                return true;
            }

            return false;
        }

        public static async Task SendMessage(UserMessage message)
        {
            if (channels.ContainsKey(message.Channel))
            {
                channels[message.Channel].MessageHistory.Insert(0, message);
                message.Channel = CurrentChannel;
            }

            await WebSocketService.SendMessage(message);
        }

        public static void SetCurrentChannel(string channel)
        {
            CurrentChannel = channel;
            OnChannelChanged?.Invoke();
        }

        public static ChannelData GetCurrentChannelData()
        {
            if (channels.TryGetValue(CurrentChannel, out ChannelData channelData))
            {
                return channelData;
            }

            return new ChannelData();
        }
    }
}