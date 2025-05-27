using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] PlayerController playerController;

    [SerializeField] int sensitivity;
    [SerializeField] int pitchMin, pitchMax;
    [SerializeField] bool invertY;
    [SerializeField] float normalFOV = 60f;
    [SerializeField] float sprintFOV = 75f;
    [SerializeField] float fovSmoothSpeed = 10f;

    public bool isMovable = true;

    float rotX;
    Camera cam;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = normalFOV;
        }
    }

    void Update()
    {
        if (!isMovable) return;
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        if (invertY)
        {
            rotX += mouseY;
        } else
        {
            rotX -= mouseY;
        }

        rotX = Mathf.Clamp(rotX, pitchMin, pitchMax);

        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);

        UpdateFOV();
    }

    void UpdateFOV()
    {
        if (cam == null || playerController == null)
        {
            return;
        }

        float targetFOV = playerController.isSprinting ? sprintFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
    }
}
