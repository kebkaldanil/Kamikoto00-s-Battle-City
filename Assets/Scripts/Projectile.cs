using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public int damage = 1;
    [Range(0f, 30f)]
    public float speed = 1.5f;
    public Vector2 velocity;
    [Range(0f, 30f)]
    public float lifetime = 10f;
    [NonSerialized] public new Rigidbody2D rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = velocity;
    }
    void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Hitable hitable))
        {
            hitable.Hit(gameObject, damage);
        }
        Destroy(gameObject);
    }
}
