using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject dmInitialization;
    Button back;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        //reload deck info
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        back = root.Q<Button>("Back");
        back.clicked += () =>
        {
            mainMenu.SetActive(true);
            dmInitialization.SetActive(false);
            this.gameObject.SetActive(false);
        };

    }

}

/*
        UIDocument mainMenuUI;
        UIDocument deckManagerUI;
        UIDocument boardUI;
        mainMenuUI = GameObject.Find("MainMenu").GetComponent<UIDocument>();
        deckManagerUI = GameObject.Find("DeckManager").GetComponent<UIDocument>();
        boardUI = GameObject.Find("Board").GetComponent<UIDocument>();
        deckManagerUI.rootVisualElement.style.display = DisplayStyle.None;
        boardUI.rootVisualElement.style.display = DisplayStyle.None;
*/
