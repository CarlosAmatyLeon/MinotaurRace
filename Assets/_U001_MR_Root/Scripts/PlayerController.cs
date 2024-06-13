using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction playerMovement;
    public float speed = 10f;
    public Rigidbody rb;
    public float rotationSpeed = 0.2f;
    public AudioSource robotWalk;

    Vector2 moveDirection = Vector2.zero;
    

    void OnEnable()
    {
        playerMovement.Enable();
    }
    private void OnDisable()
    {
        playerMovement.Disable();        
    }

    void Update()
    {
        moveDirection = playerMovement.ReadValue<Vector2>();

        if (moveDirection.magnitude > 0)
        {
            robotWalk.enabled = true;
            float angleRadians = Mathf.Atan2(moveDirection.x, moveDirection.y);
            Quaternion newRotation = Quaternion.Euler(new Vector3(0, angleRadians * Mathf.Rad2Deg, 0));
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            robotWalk.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(moveDirection.x * speed, 0f, moveDirection.y * speed);
    }
}
