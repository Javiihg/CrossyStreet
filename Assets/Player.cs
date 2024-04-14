using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    public float jumpForce = 7f;
    public float moveSpeed = 2.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump(Vector3.forward);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Jump(Vector3.back);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Jump(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Jump(Vector3.right);
        }

        // Movimiento continuo hacia adelante
        rb.velocity = transform.forward * moveSpeed;
    }

    void Jump(Vector3 direction)
    {
        // Reiniciar la velocidad vertical antes de aplicar la fuerza del salto
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Aplicar fuerza de salto en la dirección especificada
        rb.AddForce(direction * jumpForce, ForceMode.Impulse);
    }
}