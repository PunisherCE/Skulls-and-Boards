using UnityEngine;
using UnityEngine.InputSystem;

public class TestOnMouse : MonoBehaviour
{
    /// <summary>
    /// This method is called by the PlayerInput component when the "Select" action is performed.
    /// In your case, this is the left mouse button.
    /// </summary>
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Left mouse button ('Select') clicked on " + gameObject.name);
        }
    }

    public void OnView(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Right mouse button ('View') clicked on " + gameObject.name);
        }
    }

    public void OnCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Middle mouse button ('Camera') clicked on " + gameObject.name);
        }
    }
}
