using UnityEngine;

public class FreeCamController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;

    float rotX;
    float rotY;

    void Update()
    {
        rotX += Input.GetAxis("Mouse X") * lookSpeed;
        rotY -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotY = Mathf.Clamp(rotY, -80f, 80f);
        transform.rotation = Quaternion.Euler(rotY, rotX, 0);

        Vector3 dir = new Vector3(
            Input.GetAxis("Horizontal"),
            (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.LeftShift) ? 1 : 0),
            Input.GetAxis("Vertical")
        );

        transform.Translate(dir * moveSpeed * Time.unscaledDeltaTime);
    }
}
