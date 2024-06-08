namespace Frontend.Client.Models;
using System;
using System.Net.WebSockets;
using System.Text;

public static class ServerComms
{
    public static ClientWebSocket? _webSocket;
    public static async Task ConnectWebSocket(string url, string username)
    {
        _webSocket = new ClientWebSocket();
        await _webSocket.ConnectAsync(new Uri(url), CancellationToken.None);
        await SendMessage("DO AUTH\r\nFROM " + username);
        Console.WriteLine("CONNECTED TO SERVER!");
    }

    public static async Task SendMessage(string message)
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            Console.WriteLine("message before encoding " + message);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            string reconvertedMessage = System.Text.Encoding.UTF8.GetString(segment);
            Console.WriteLine("Message after encoding and reconverting: " + reconvertedMessage);
            await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else
        {
            Console.WriteLine("websocket not connected");
            return;
        }
    }

    public static async Task<string> ReceiveMessage()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            var buffer = new byte[1024];
            WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string responseMessage = Encoding.UTF8.GetString(buffer,0, buffer.Length);
            Console.WriteLine("Received message from server: " + responseMessage);
            return responseMessage;
        }
        else
        {
            Console.WriteLine("Could not receive a message");
            return null;
        }
    }
}

