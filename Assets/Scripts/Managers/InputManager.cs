using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// This method is called by the PlayerInput component when the "Select" action is performed.
    /// </summary>
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        // Perform a raycast from the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            // Check if the object we hit has a Monsters component
            Monsters monster = hit.collider.GetComponent<Monsters>();
            if (monster != null)
            {
                // Call the OnSelect method on the specific monster that was clicked
                monster.OnSelect();
            }
        }
    }
}
