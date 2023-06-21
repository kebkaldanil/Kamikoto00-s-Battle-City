using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankMove : MonoBehaviour
{
    [Range(0f, 30f)]
    public float speed = 1f;
    [NonSerialized]
    public new Rigidbody2D rigidbody;
    [NonSerialized]
    public Vector2 velocity = Vector2.zero;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        rigidbody.velocity = velocity * speed;
        if (velocity != Vector2.zero)
        {
            transform.up = velocity;
        }
    }
}
