using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class SpeedAnimationUpdate : MonoBehaviour
{
    private Animator animator;
    [NonSerialized]
    public new Rigidbody2D rigidbody;
    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        animator.SetFloat("speed", Vector2.Dot(rigidbody.velocity, transform.up));
    }
}
