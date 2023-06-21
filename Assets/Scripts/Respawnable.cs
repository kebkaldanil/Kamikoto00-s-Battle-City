using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid), typeof(Health))]
public class Respawnable : MonoBehaviour
{
    [NonSerialized]
    public Grid grid;
    [NonSerialized]
    public Health health;
    public Vector3 spawn = Vector3.zero;
    public int withHealth = 0;
    public int lifes = 3;
    void Awake()
    {
        grid = GetComponent<Grid>();
        health = GetComponent<Health>();
    }
    void Start()
    {
        if (withHealth <= 0)
        {
            withHealth = health.health;
        }
        health.BeforeDeath += BeforeDeath;
    }
    void BeforeDeath()
    {
        if (lifes > 0)
        {
            health.health = withHealth;
            transform.position = spawn;
            lifes--;
        }
    }
}
