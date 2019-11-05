using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{

    public float mouseSensitivity;
    public float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        ManageRotation();
        ManageMovement();
    }

    void ManageRotation()
    {
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * mouseSensitivity);
    }

    void ManageMovement()
    {
        transform.position = transform.position + Camera.main.transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        transform.position = transform.position + Camera.main.transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        transform.position = transform.position + Camera.main.transform.up * Input.GetAxis("Up") * Time.deltaTime * moveSpeed;
    }
}
