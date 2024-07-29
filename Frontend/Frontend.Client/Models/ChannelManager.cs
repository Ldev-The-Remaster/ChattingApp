namespace Frontend.Client.Models
{
    public static class ChannelManager
    {
        public  struct ChannelData
        {
            

            public  List<UserMessage> MessageHistory {  get; set; } = new List<UserMessage>();
            public  List<string> userList { get; set; } = new List<string>();

        }

        public static string currentChannel = string.Empty;

        public static event Action? OnChannelChange;


        public static ChannelData GetCurrentChannelData() {
            ChannelData channelData;
            channels.TryGetValue(currentChannel, out channelData);

            return channelData;

        }

        // public static ChatType CurrentChatType { get; set; }

        //public static void SwitchToDm()
        //{
        //    CurrentChatType = ChatType.DmChat;
        //    onChannelChange?.Invoke();
        //}

        //public static void SwitchToMain()
        //{
        //    CurrentChatType = ChatType.MainChat;
        //    onChannelChange?.Invoke();
        //}

        public static Dictionary<string, ChannelData> channels = new Dictionary<string, ChannelData>();
        

    }
}
