using UnityEngine;
using UnityEngine.InputSystem; // new Input System namespace

public class SmoothZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float moveSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    public InputAction scrollClickAction;


    private Camera cam;
    private Vector3 targetPos;
    private float targetZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
        targetPos = transform.position;
    }

    void Update()
    {
        // New Input System scroll
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0) // zoom in
        {
            if(cam.orthographicSize <= minZoom) return;
            targetZoom -= 1f;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if(mouseWorldPos.x < -4.5f || mouseWorldPos.x > 4.5f || mouseWorldPos.y < -4.5f || mouseWorldPos.y > 4.5f)
            {
                mouseWorldPos = new Vector3(0f, 0f, transform.position.z);
            }
            mouseWorldPos.z = transform.position.z;
            targetPos = mouseWorldPos;
        }
        else if (scroll < 0) // zoom out
        {
            if(cam.orthographicSize >= maxZoom) return;
            targetZoom += 1f;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            
            targetPos = new Vector3(0f, 0f, transform.position.z);
        }
        if (scrollClickAction.ReadValue<float>() > 0)
        {
            if(cam.orthographicSize <= minZoom) return;
            targetZoom -= 0;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if(mouseWorldPos.x < -4.5f || mouseWorldPos.x > 4.5f || mouseWorldPos.y < -4.5f || mouseWorldPos.y > 4.5f)
            {
                mouseWorldPos = new Vector3(0f, 0f, transform.position.z);
            }
            mouseWorldPos.z = transform.position.z;
            targetPos = mouseWorldPos;
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
    }

    void OnEnable()
    {
        // Bind directly to the middle mouse button
        scrollClickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/middleButton");
        scrollClickAction.Enable();
    }

    void OnDisable()
    {
        scrollClickAction.Disable();
    }
}