using UnityEngine;
using UnityEngine.InputSystem; // new Input System namespace

public class SmoothZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 1f;
    public float moveSpeed = 1f;
    public float minZoom = 1f;
    public float maxZoom = 5f;
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
            if(mouseWorldPos.x < -4f || mouseWorldPos.x > 4f || mouseWorldPos.y < -4f || mouseWorldPos.y > 4f)
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

            // Interpolate target position towards the center based on zoom level
            float zoomRatio = (targetZoom - minZoom) / (maxZoom - minZoom);
            Vector3 centerPos = new Vector3(0f, 0f, transform.position.z);
            targetPos = Vector3.Lerp(targetPos, centerPos, zoomRatio);
        }
        if (scrollClickAction.ReadValue<float>() > 0)
        {
            if(cam.orthographicSize <= minZoom) return;
            targetZoom -= 0;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if(mouseWorldPos.x < -4f || mouseWorldPos.x > 4f || mouseWorldPos.y < -4f || mouseWorldPos.y > 4f)
            {
                mouseWorldPos = new Vector3(0f, 0f, transform.position.z);
            }
            mouseWorldPos.z = transform.position.z;
            targetPos = mouseWorldPos;
        }

        targetPos.x = Mathf.Clamp(targetPos.x, -2.8f, 2.8f);
        targetPos.y = Mathf.Clamp(targetPos.y, -2.8f, 2.8f);


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