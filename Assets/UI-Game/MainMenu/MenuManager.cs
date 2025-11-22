using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject deckManagerUI;
    [SerializeField] private GameObject dmInitialization;
    [SerializeField] private GameObject matchMakingUI;
    [SerializeField] private GameObject aboutUI;
    [SerializeField] private GameObject createUI;

    Button matchMaking;
    Button create;
    Button buildDeck;
    Button social;
    Button send;
    TextField chatInput;

    static ListView chatRegistry;

    static List<string> chatMessages = new List<string>() { "Welcome", "" };

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnEnable()
    {
        //reload chat
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        matchMaking = root.Q<Button>("Match");
        create = root.Q<Button>("Create");
        buildDeck = root.Q<Button>("BuildDeck");
        social = root.Q<Button>("Social");
        send = root.Q<Button>("Send");
        chatInput = root.Q<TextField>("ChatText");

        chatRegistry = root.Q<ListView>("ChatRegistry");
        chatRegistry.itemsSource = chatMessages;
        chatRegistry.makeItem = () =>
        {
            var label = new Label();
            return label;
        };
        chatRegistry.bindItem = (element, i) => (element as Label).text = chatMessages[i];

        //matchMaking.clicked += MatchMakingClicked;
        //create.clicked += CreateClicked;
        buildDeck.clicked += BuildDeckClicked;
        //about.clicked += AboutClicked;

        send.clicked += SendClicked;

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

    private void BuildDeckClicked()
    {
        deckManagerUI.SetActive(true);
        dmInitialization.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void CreateClicked()
    {
        throw new NotImplementedException();
    }

    private void MatchMakingClicked()
    {
        throw new NotImplementedException();
    }

    public static void AddChatMessage(string message)
    {
        chatMessages.RemoveAt(chatMessages.Count - 1);
        chatMessages.Add(message);
        chatMessages.Add("");
        chatRegistry.RefreshItems();
        chatRegistry.ScrollToItem(chatMessages.Count - 1);
    }

    private void SendClicked()
    {
        string message = chatInput.value;
        if (string.IsNullOrEmpty(message)) return;

        ConnectionManager.SendMessage(message);
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

//chatScrollView.ScrollTo(messageElement);
