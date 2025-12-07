using UnityEngine;
using UnityEngine.InputSystem;

public class TileInputManager : MonoBehaviour
{
    private Camera mainCamera;

    public static GameObject tileSelected;

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
            TilesSelection tile = hit.collider.GetComponent<TilesSelection>();
            if (tile != null)
            {
                // If a tile was previously selected, reset its color first.
                if (tileSelected != null)
                {
                    SpriteRenderer sr = tileSelected.GetComponent<SpriteRenderer>();
                    sr.color = Color.white;
                }

                // Now, select the new tile. This will update tileSelected and set the new color.
                tile.OnSelect();
            }
        }
    }
}
