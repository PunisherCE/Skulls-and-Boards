using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class PopupWindow : VisualElement
{

    [UxmlAttribute]
    string prompt;
    public string Prompt
    {
        get => prompt;
        set
        {
            prompt = value;
            UpdatePrompt();
        }
    }

    Label msgLabel;

    private const string styleResource = "Stylesheet_PopupPanel";
    private const string ussContainer = "popup_container";
    private const string ussPopup = "popup_window";
    private const string ussPopupMsg = "popup_msg";
    private const string ussPopupButton = "popup_button";
    private const string ussCancel = "button_cancel";
    private const string ussHorContainer = "horizontal_container";

    public PopupWindow()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
        AddToClassList(ussContainer);

        VisualElement window = new VisualElement();
        hierarchy.Add(window);
        window.AddToClassList(ussPopup);

        VisualElement horizontalContainerText = new VisualElement();
        window.Add(horizontalContainerText);
        horizontalContainerText.AddToClassList(ussHorContainer);



        msgLabel = new Label();
        horizontalContainerText.Add(msgLabel);
        msgLabel.AddToClassList(ussPopupMsg);

        VisualElement horizontalContainerButton = new VisualElement();
        window.Add(horizontalContainerButton);
        horizontalContainerButton.AddToClassList(ussHorContainer);

        Button cancelButton = new Button() { text = "OK" };
        horizontalContainerButton.Add(cancelButton);


        cancelButton.AddToClassList(ussPopupButton);
        cancelButton.AddToClassList(ussCancel);

        msgLabel.text = "An error ocurred";

        cancelButton.clicked += OnCancel;


    }

    public event Action cancelled;

    void OnCancel()
    {
        cancelled?.Invoke();
    }

    void UpdatePrompt()
    {
        msgLabel.text = prompt;
    }
}