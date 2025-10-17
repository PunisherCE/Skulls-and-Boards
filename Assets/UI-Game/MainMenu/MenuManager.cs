using System;
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
    Button about;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        //reload chat
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        matchMaking = root.Q<Button>("Match");
        create = root.Q<Button>("Create");
        buildDeck = root.Q<Button>("BuildDeck");
        about = root.Q<Button>("About");

        matchMaking.clicked += MatchMakingClicked;
        create.clicked += CreateClicked;
        buildDeck.clicked += BuildDeckClicked;
        about.clicked += AboutClicked;
    }

    private void AboutClicked()
    {
        throw new NotImplementedException();
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
}

//chatScrollView.ScrollTo(messageElement);
