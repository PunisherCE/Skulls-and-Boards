using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameChat : MonoBehaviour
{

    TextField chatInput;
    static ListView chatRegistry;
    Button send;
    static List<string> chatMessages = new List<string>() { "Welcome", "" };
    public static bool isChatFocused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // You can use this boolean to control other game logic.
        // For example, you might want to disable player movement while the chat is focused.
        // if (isChatFocused)
        //    Debug.Log("Chat is focused!");
    }

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        chatInput = root.Q<TextField>("ChatText");
        send = root.Q<Button>("Send");
        send.clicked += SendClicked;

        chatRegistry = root.Q<ListView>("ChatRegistry");
        chatRegistry.itemsSource = chatMessages;
        chatRegistry.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        chatRegistry.makeItem = () =>
        {
            var label = new Label();
            label.style.whiteSpace = WhiteSpace.Normal;
            return label;
        };
        chatRegistry.bindItem = (element, i) => (element as Label).text = chatMessages[i];

        // Register callbacks to track the focus state of the chat input field.
        chatInput.RegisterCallback<FocusInEvent>(OnChatFocusIn);
        chatInput.RegisterCallback<FocusOutEvent>(OnChatFocusOut);

        // --- SOLUTION ---
        // By default, the TextField is not focusable. This prevents the UI navigation system
        // from giving it focus when you press 'A' or 'D' for monster movement.
        chatInput.focusable = false;

        // We only make it focusable when the mouse is over it, so you can click it.
        chatInput.RegisterCallback<PointerEnterEvent>(e => chatInput.focusable = true);
        chatInput.RegisterCallback<PointerLeaveEvent>(e => chatInput.focusable = false);
        
        // When the chat input is focused, stop keyboard events from propagating to the game.
        chatInput.RegisterCallback<KeyDownEvent>(evt =>
        {
            if (isChatFocused)
            {
                evt.StopPropagation();
            }
        });

        chatInput.RegisterCallback<KeyUpEvent>(evt =>
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                // prevent other handlers from seeing this Enter
                evt.StopPropagation();
                SendClicked();
            }
        });
    }

    private void OnChatFocusIn(FocusInEvent evt)
    {
        isChatFocused = true;
    }

    private void OnChatFocusOut(FocusOutEvent evt)
    {
        isChatFocused = false;
        chatInput.focusable = false; // Make it non-focusable again when we click away
    }
    private void SendClicked()
    {
        string message = chatInput.value;
        if (string.IsNullOrEmpty(message)) return;

        ConnectionManager.SendChatMessage(ConnectionManager.user_id, message);
        chatMessages.RemoveAt(chatMessages.Count - 1);
        chatMessages.Add(ConnectionManager.user_id + ": " + message);
        chatMessages.Add("");
        chatRegistry.RefreshItems();
        chatRegistry.ScrollToItem(chatMessages.Count - 1);
        if (!string.IsNullOrEmpty(message))
        {
            //ConnectionManager.SendMessage(message);
            chatInput.value = string.Empty;
        }
    }

    public static void AddChatMessage(string user_id, string message)
    {
        chatMessages.RemoveAt(chatMessages.Count - 1);
        chatMessages.Add(user_id + ": " + message);
        chatMessages.Add("");
        chatRegistry.RefreshItems();
        chatRegistry.ScrollToItem(chatMessages.Count - 1);
    }
}
