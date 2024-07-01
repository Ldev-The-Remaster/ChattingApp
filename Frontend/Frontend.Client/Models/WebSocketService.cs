using System.Net.WebSockets;
using System.Text;

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

        private static void OnMessage(ClientManager.MessageParams message)
        {
            if (_activeCommand == ActiveCommand.Auth)
            {
                _isAuthenticated = message.Do.Equals("ACCEPT");
                _activeCommand = ActiveCommand.NotSet;
                return;
            }

            try
            {
                if (message.Do.Equals("INTRODUCE"))
                {
                    ClientManager.UpdateUserList(message.With);
                    return;
                }

                if(message.Do.Equals("SEND"))
                {
                    var hashAndMessageList = message.With.Split("\r\n");
                    if (hashAndMessageList.Length != 2) 
                    {
                        Console.WriteLine("Error: Message format is invalid, missing hash or content.");
                        return;
                    }

                    var user = message.From;
                    var timestamp = DateTimeOffset.FromUnixTimeSeconds(message.At).DateTime; 
                    var hash = hashAndMessageList[0];
                    var content = hashAndMessageList[1];

                    if (user.Equals(string.Empty))
                    {
                        OnMessageReceived?.Invoke(new UserMessage
                        (
                            user: null,
                            hash: hash,
                            content: content,
                            timestamp: timestamp,
                            isConfirmed: true
                        ));

                        return;
                    }

                    OnMessageReceived?.Invoke(new UserMessage
                    (
                        user: user,
                        hash: hash,
                        content: content,
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
            _receiveMessagesTask?.Dispose();
            _isAuthenticated = false;
            OnClosed?.Invoke();
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
                        OnMessage(new ClientManager.MessageParams(message));
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
            await SendMessage(message.Hash, message.Content);
        }

        public static async Task SendMessage(string hash, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await SendMessageAsync($"DO SEND\r\nWITH\r\n{hash}\r\n{message}");
            }
        }

        public static async Task<bool> RequestAuth(string username)
        {
            _activeCommand = ActiveCommand.Auth;
            await SendMessageAsync("DO AUTH\r\nFROM " + username);

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