using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGameChat : MonoBehaviour
{

    TextField chatInput;
    static ListView chatRegistry;
    Button send;
    static List<string> chatMessages = new List<string>() { "Welcome", "" };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void SendClicked()
    {
        string message = chatInput.value;
        if (string.IsNullOrEmpty(message)) return;

        //ConnectionManager.SendMessage(message);
        chatMessages.RemoveAt(chatMessages.Count - 1);
        chatMessages.Add(message);
        chatMessages.Add("");
        chatRegistry.RefreshItems();
        chatRegistry.ScrollToItem(chatMessages.Count - 1);
        if (!string.IsNullOrEmpty(message))
        {
            //ConnectionManager.SendMessage(message);
            chatInput.value = string.Empty;
        }
    }
}
