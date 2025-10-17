using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class MonsterSelectorManipulator : PointerManipulator
{
    private VisualElement floatingPreview;
    private readonly Sprite sprite;
    private readonly int monsterIndex;
    private readonly VisualElement root;

    public MonsterSelectorManipulator(VisualElement root, Sprite sprite, int monsterIndex)
    {
        this.root = root;
        this.sprite = sprite;
        this.monsterIndex = monsterIndex;
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        floatingPreview = CreateFloatingPreview(sprite);
        root.Add(floatingPreview);
        UpdatePreviewPosition(evt.position);
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (floatingPreview != null)
            UpdatePreviewPosition(evt.position);
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        if (floatingPreview == null) return;

        VisualElement dropTarget = FindGridCellUnderMouse(evt.position);
        if (dropTarget != null && dropTarget.childCount == 0)
        {
            var monsterSlot = new MonsterSlotControl(sprite, monsterIndex);
            dropTarget.Add(monsterSlot);
        }

        floatingPreview.RemoveFromHierarchy();
        floatingPreview = null;
    }

    private void UpdatePreviewPosition(Vector2 mousePos)
    {
        floatingPreview.style.left = mousePos.x - 50;
        floatingPreview.style.top = mousePos.y - 50;
    }

    private VisualElement CreateFloatingPreview(Sprite sprite)
    {
        var ve = new VisualElement();
        ve.style.position = Position.Absolute;
        ve.style.width = 100;
        ve.style.height = 100;
        ve.style.backgroundImage = sprite.texture;
        ve.AddToClassList("monster-slot");
        //ve.style.backgroundSize = BackgroundSize.Contain;
        return ve;
    }

    private VisualElement FindGridCellUnderMouse(Vector2 mousePos)
    {
        return root.Query<VisualElement>()
               .Where(el => el.ClassListContains("tiles"))
               .ToList()
               .FirstOrDefault(cell => cell.worldBound.Contains(mousePos));
    }

}
