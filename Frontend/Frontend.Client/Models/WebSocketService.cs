using System.Net.WebSockets;
using System.Text;
using Frontend.Client.Pages;
using LSMP;
using LSMP.Utils;

namespace Frontend.Client.Models
{
    public static class WebSocketService
    {
        enum ActiveCommand
        {
            Auth,
            NotSet
        }

        private static ClientWebSocket? _webSocket;
        private static Task? _receiveMessagesTask;
        public delegate void MessagesEventHandler(UserMessage messages);
        public static event MessagesEventHandler? OnMessageReceived;
        public static event Action? OnClosed;
        private static bool _isAuthenticated = false;
        private static ActiveCommand _activeCommand = ActiveCommand.NotSet;
        private static readonly int AuthTimeOut = 10;

        public static bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        public static async Task ConnectAsync(string uri)
        {
            if (_webSocket == null || _webSocket.State != WebSocketState.Open)
            {
                _webSocket = new ClientWebSocket();
                await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                _receiveMessagesTask = ReceiveMessagesAsync();
            }
        }

        private static void OnMessage(MessageParser rawMessage)
        {
            if (_activeCommand == ActiveCommand.Auth)
            {
                _isAuthenticated = rawMessage.Do.Equals("ACCEPT");
                _activeCommand = ActiveCommand.NotSet;
                return;
            }

            try
            {
                if (rawMessage.Do.Equals("INTRODUCE"))
                {
                    ChannelManager.UpdateChannelUserList("general-chat", Messaging.DecodeUserList(rawMessage.With));
                    return;
                }

                if (rawMessage.Do.Equals("REMIND"))
                {
                    List<UserMessage> messagesDetails = new List<UserMessage>();
                    foreach (var item in Messaging.DecodeMessageHistory(rawMessage.With))
                    {
                        MessageParser messageDetails = new MessageParser(item);
                        (string hash, string content) = Messaging.GetHashAndMessage(messageDetails.With);
                        UserMessage message = new UserMessage(messageDetails.From, messageDetails.In, hash, content, messageDetails.At, true);
                        messagesDetails.Add(message);
                    }

                    ChannelManager.UpdateChannelMessageHistory(rawMessage.In, messagesDetails);
                }


                if (rawMessage.Do.Equals("RETRIEVE"))
                {
                    List<string> banWithReason = rawMessage.With.Split(new[] {"/*$*/"}, StringSplitOptions.None).ToList();

                    if (rawMessage.From.Equals("BANNEDUSERS"))
                    {
                        foreach (var ban in banWithReason)
                        {
                            var user = ban.Split(new[] {"\r\n"},StringSplitOptions.None);
                            var username = user[0];
                            var reason = user[1];

                            ClientManager.AddUserBan(username, reason);
                        }
                    }
                    else 
                    {
                        foreach (var ban in banWithReason)
                        {
                            var ip = ban.Split(new[] {"\r\n"}, StringSplitOptions.None);
                            var ipAddress = ip[0];
                            var reason = ip[1];

                            ClientManager.AddIpBan(ipAddress, reason);

                        }
                    }

                }

                    if (rawMessage.Do.Equals("SEND"))
                {
                    var user = rawMessage.From;
                    var timestamp = rawMessage.At;
                    var channel = rawMessage.In;
                    var (hash, message) = Messaging.GetHashAndMessage(rawMessage.With);
                    if (hash == string.Empty) 
                    {
                        Console.WriteLine("Error: Message format is invalid, missing hash or content.");
                        return;
                    }

                    if (user.Equals(string.Empty))
                    {
                        OnMessageReceived?.Invoke(new UserMessage
                        (
                            user: null,
                            hash: hash,
                            channel: channel,
                            content: message,
                            timestamp: timestamp,
                            isConfirmed: true
                        )); ;

                        return;
                    }

                    OnMessageReceived?.Invoke(new UserMessage
                    (
                        user: user,
                        hash: hash,
                        channel: channel,
                        content: message,
                        timestamp: timestamp,
                        isConfirmed: true
                    ));

                    return;
                }

                    Console.WriteLine("Error: Message format is invalid.");
            }
            catch (Exception e)
            {
                 Console.WriteLine($"Error processing message: {e.Message}");
            }
        }

        private static void OnClose() 
        {
            _isAuthenticated = false;
            OnClosed?.Invoke();
            _receiveMessagesTask?.Dispose();
        }

        private static async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket?.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    if (message != null)
                    {                          
                        OnMessage(new MessageParser(message));
                    }
                }
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    OnClose();
                }
            }
        }

        public static async Task SendMessageAsync(string message)
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public static async Task SendMessage(UserMessage message)
        {
            await SendMessageAsync(Messaging.EncodeMessageToString(message));
        }

        public static async Task<bool> RequestAuth(string username)
        {
            _activeCommand = ActiveCommand.Auth;
            await SendMessageAsync(Messaging.AuthMessage(username));

            var authTimeout = TimeSpan.FromSeconds(AuthTimeOut);
            var startTime = DateTime.Now;
            while (!IsAuthenticated && DateTime.Now - startTime < authTimeout)
            {
                await Task.Delay(100);
            }

            return IsAuthenticated;
        }
    }
}