namespace Frontend.Client.Models
{
    public static class ChannelManager
    {
        // main needs to be an always present channel and it's message history and userlist need to be initialized on load and displayed accordingly
        public class ChannelData
        {
            public List<UserMessage> MessageHistory { get; set; } = new List<UserMessage>();
            public List<string> UserList { get; set; } = new List<string>();
        }

        // function that sends message to channel that takes a message and target channel
        public static Dictionary<string, ChannelData> channels = new Dictionary<string, ChannelData>();
       
        
        public static string CurrentChannel { get; set; } = string.Empty;

        public static event Action? OnChannelChange;

        // here we are sending the message through the websocket and simultaneously adding it to the current channel' message history
        // we also set the message.Channel propety to the current channel it's being sent in

        public static void UpdateOrCreateChannel(string channelName, ChannelData channelData)
        {
            if (channels.ContainsKey(channelName))
            {
                channels[channelName] = channelData;
                return;
            }
            
            channels.Add(channelName, channelData); 
        }

        public static void UpdateChannelUserList(string channelName, List<string> userList)
        {

        }

        public static void UpdateChannelMessageHistory(string channelName, List<UserMessage> messageHistory)
        {

        }

        public static async Task SendMessage(UserMessage message)
        {
            if (channels.ContainsKey(message.Channel))
            {
                channels[message.Channel].MessageHistory.Add(message);
                message.Channel = CurrentChannel;
            }
            await WebSocketService.SendMessage(message);
        }

        public static void SetCurrentChannel(string channel)
        {
            CurrentChannel = channel;
            OnChannelChange?.Invoke();
        }

        public static ChannelData GetCurrentChannelData()
        {
            if (channels.TryGetValue(CurrentChannel, out ChannelData channelData))
            {
                return channelData;
            }

            // Return a new empty ChannelData if currentChannel is not found
            return new ChannelData();

        }

    }

}