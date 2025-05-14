using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int pitchMin, pitchMax;
    [SerializeField] bool invertY;
    public bool isMovable = true;

    float rotX;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
    }
}
