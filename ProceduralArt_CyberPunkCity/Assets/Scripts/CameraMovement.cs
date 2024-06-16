using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;
    public float zoomSpeed = 50f;

    private Vector3 lastMousePosition;

    void Update()
    {
        // Move camera with WASD keys
        float horizontal = Input.GetAxis("Horizontal"); // A and D keys
        float vertical = Input.GetAxis("Vertical"); // W and S keys

        Vector3 move = new Vector3(horizontal, 0, vertical);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.Self);

        
        if (Input.GetMouseButtonDown(1)) // Last mouse position for when it was first clicked
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) //rotate the camera
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationX = delta.y * rotationSpeed * Time.deltaTime;
            float rotationY = -delta.x * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.right, rotationX);
            transform.Rotate(Vector3.up, rotationY, Space.World);

            lastMousePosition = Input.mousePosition;
        }

        // Zoom camera with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(0, 0, scroll * zoomSpeed * Time.deltaTime, Space.Self);
    }
}
