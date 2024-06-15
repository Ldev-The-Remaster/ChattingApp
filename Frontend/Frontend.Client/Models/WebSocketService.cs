namespace Frontend.Client.Models;

using System.Net.WebSockets;
using System.Text;

public class WebSocketService
{
    enum ActiveCommand
    {
        Auth,
        NotSet
    }

    public class Message
    {
        public string? User { get; set; }
        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }
    }

    private ClientWebSocket? _webSocket;
    private Task? _receiveMessagesTask;
    public delegate void MessagesEventHandler(Message messages);
    public event MessagesEventHandler? OnMessageReceived; 
    private bool _isAuthenticated = false;
    private ActiveCommand _activeCommand = ActiveCommand.NotSet;
    private readonly int AuthTimeOut = 10;

    public bool IsAuthenticated
    {
#if DEBUG
        get { return true; }
#else
        get { return _isAuthenticated; }
#endif
    }

    public async Task ConnectAsync(string uri)
    {
        if (_webSocket == null || _webSocket.State != WebSocketState.Open)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            _receiveMessagesTask = ReceiveMessagesAsync();
        }
    }

    private void OnMessage(string message)
    {
        if(_activeCommand == ActiveCommand.Auth)
        {
            _isAuthenticated = message.StartsWith("DO ACCEPT");
            _activeCommand = ActiveCommand.NotSet;
            return;
        }

        try
        {
            var lines = message.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            if (lines.Length >= 6 &&
                lines[0].StartsWith("DO SEND") &&
                lines[1].StartsWith("FROM ") &&
                lines[2].StartsWith("IN general-chat") &&
                lines[3].StartsWith("AT ") &&
                lines[4].StartsWith("WITH"))
            {
                var user = lines[1].Substring(5); // "FROM user"
                var timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(lines[3].Substring(3))).DateTime; // "AT timestamp"
                var content = string.Join("\n", lines.Skip(5)); // Content starts after "WITH" line
                OnMessageReceived?.Invoke(new Message
                {
                    User = user,
                    Content = content,
                    Timestamp = timestamp
                });
            }
            else
            {
                Console.WriteLine("Error: Message format is invalid.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error processing message: {e.Message}");
        }
    }

    private async Task ReceiveMessagesAsync()
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
                    OnMessage(message);
                }
            }
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public async Task<bool> RequestAuth(string username)
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