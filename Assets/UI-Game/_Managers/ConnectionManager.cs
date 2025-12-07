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
    public static string user_id = "Begone";
    public static GameObject boardObject;

    private async void Start()
    {
        socket = new ClientWebSocket();
        cts = new CancellationTokenSource();
        await socket.ConnectAsync(new Uri("wss://e3757495d076.ngrok-free.app/ws "), cts.Token);
        Debug.Log("WebSocket connected!");
        StartCoroutine(ReceiveLoop());
    }
    private async void OnDestroy()
    {
        if (socket != null && socket.State == WebSocketState.Open)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        cts?.Cancel();
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
            // Step 1: parse envelope
            PacketEnvelope envelope = JsonUtility.FromJson<PacketEnvelope>(message);

            // Step 2: branch by type
            if (envelope.type == 0)
            {
                // Parse as ChatPacket
                ChatPayload chat = JsonUtility.FromJson<ChatPayload>(message);
                if (boardObject.activeSelf)
                    InGameChat.AddChatMessage(chat.user_id, chat.message);
                else MenuManager.AddChatMessage(chat.user_id, chat.message);
                //MenuManager.AddChatMessage(chat.user_id + ": " + chat.message);
            }
            else if (envelope.type == 1)
            {
                // Parse as UserPacket
                MovementPayload movement = JsonUtility.FromJson<MovementPayload>(message);
                Monsters monster = BoardManager.Instance.GetMonster(movement.monster_id);
                if (monster != null && movement.success)
                {
                    monster.currentIndex = movement.tile_destination;
                    monster.transform.position = BoardManager.Instance.tilePrefab[monster.currentIndex].transform.position;
                    monster.onMonsterMoved?.Invoke(monster.currentIndex);
                } else {
                    Debug.LogWarning($"Monster with ID {movement.monster_id} not found or movement failed.");
                }
            }
        }
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

    public static void SendChatMessage(string user_id, string message)
    {
        if(socket != null && socket.State == WebSocketState.Open)
        {
            ChatPayload payload = new ChatPayload
            {
                user_id = user_id,
                message = message
            };
            string jsonMessage = JsonUtility.ToJson(payload);
            SendMessage(jsonMessage);
        }
    }
    public static void SendMovement(int monster_id, int tileOrigin, int tileDestination, string action)
    {
        if(socket != null && socket.State == WebSocketState.Open)
        {
            MovementPayload payload = new MovementPayload
            {
                monster_id = monster_id,
                tile_origin = tileOrigin,
                tile_destination = tileDestination,
                action = action,
                success = true
            };
            string jsonMessage = JsonUtility.ToJson(payload);
            SendMessage(jsonMessage);
        }
    }

    [System.Serializable]
    public class PacketEnvelope
    {
        public int type;
    }

    //Sendable payloads
    [System.Serializable]
    public class ChatPayload
    {
        public int type = 0;
        public string user_id;
        public string message;
    }
    [System.Serializable]
    public class MovementPayload
    {
        public int type = 1;
        public int monster_id;
        public int tile_origin;
        public int tile_destination;
        public string action;
        public bool success;
    }

    //Receivable payloads
}
