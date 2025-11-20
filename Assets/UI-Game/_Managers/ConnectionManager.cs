using System;
using System.Collections;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    private static ClientWebSocket socket;
    private static CancellationTokenSource cts;

    private async void Start()
    {
        socket = new ClientWebSocket();
        cts = new CancellationTokenSource();
        await socket.ConnectAsync(new Uri("wss://fbb9192efe60.ngrok-free.app/ws"), cts.Token);
        Debug.Log("WebSocket connected!");
        StartCoroutine(ReceiveLoop());
    }

    private IEnumerator ReceiveLoop()
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var task = socket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            yield return new WaitUntil(() => task.IsCompleted);

            var result = task.Result;
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Debug.Log($"Received: {message}");
            MenuManager.AddChatMessage(message);
            // Update UI or game state here
        }
    }

    private async void OnDestroy()
    {
        if (socket != null && socket.State == WebSocketState.Open)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        cts?.Cancel();
    }

    public static new async void SendMessage(string message)
    {
        if (socket != null && socket.State == WebSocketState.Open)
        {
            var encoded = Encoding.UTF8.GetBytes(message);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cts.Token);
        }
    }
}
