using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterDragger : MouseManipulator
{
    List<VisualElement> toSelect;
    private bool isActive;
    private Vector2 startPos;

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        throw new System.NotImplementedException();
    }

    protected void OnMouseDown(MouseDownEvent e)
    {
        // get mouse start pos
        startPos = e.localMousePosition;
        // get all selectable elements
        toSelect = target.parent.Query(className: "selectable").ToList();
        // mark this as active
        isActive = true;
        // capture the mouse to receive events even if it leaves the target
        target.CaptureMouse();
    }

    protected void OnMouseMove(MouseMoveEvent e)
    {
        if (!isActive) return;
        Vector2 diff = e.localMousePosition - startPos;
        foreach (var item in toSelect)
        {
            if (item.ClassListContains("selected"))
            {
                item.style.left = item.layout.x + diff.x;
                item.style.top = item.layout.y + diff.y;
            }
        }
        startPos = e.localMousePosition;
    }

    protected void OnMouseUp(MouseUpEvent e)
    {
        if (!isActive) return;
        isActive = false;
        target.ReleaseMouse();
    }
}
