using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;

public class MonsterSlotControl : VisualElement
{
    public int MonsterIndex { get; private set; }

    public MonsterSlotControl(Sprite sprite, int index)
    {
        MonsterIndex = index;
        AddToClassList("monster-slot");

        style.backgroundImage = sprite.texture;
        //style.backgroundSize = BackgroundSize.Contain;
        style.width = 100;
        style.height = 100;
        style.position = Position.Relative;

        var removeButton = new Button(() => this.RemoveFromHierarchy())
        {
            text = "X"
        };
        removeButton.AddToClassList("remove-button");
        Add(removeButton);
    }
}
