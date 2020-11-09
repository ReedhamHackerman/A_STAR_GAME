using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveForce;
  
    

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector2.up * moveForce * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-Vector2.up * moveForce * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * moveForce * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(-Vector2.left * moveForce * Time.deltaTime);
        }
    }
}
