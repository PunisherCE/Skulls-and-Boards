using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DeckManager : MonoBehaviour
{
    Button info;
    VisualElement popup;
    Button popupBack;


    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        info = root.Q<Button>("Info");
        popup = root.Q<VisualElement>("PopupOverlay");
        popupBack = popup.Q<Button>("Back");
        info.clicked += InfoClicked;
        popupBack.clicked += PopupBackClicked;
    }

    private void PopupBackClicked()
    {
        popup.style.display = DisplayStyle.None; // fully hidden
    }

    private void InfoClicked()
    {
        popup.style.display = DisplayStyle.Flex; // shown and interactive
    }

}
