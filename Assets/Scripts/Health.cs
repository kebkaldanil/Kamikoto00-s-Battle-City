using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hitable))]
public class Health : MonoBehaviour
{
    [Range(0f, 10f)]
    public int health = 1;
    private Hitable hitable;
    public event Action BeforeDeath;
    public event Action OnDeath;
    private bool beforeDeathCalled = false;
    void Awake()
    {
        hitable = GetComponent<Hitable>();
    }
    void Start()
    {
        hitable.OnHit += OnHit;
    }
    void OnHit(GameObject by, int damage)
    {
        health -= damage;
        if (!beforeDeathCalled && health <= 0)
        {
            beforeDeathCalled = true;
            BeforeDeath?.Invoke();
        }
    }
    void LateUpdate()
    {
        beforeDeathCalled = false;
    }
    void Update()
    {
        if (health <= 0)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}
