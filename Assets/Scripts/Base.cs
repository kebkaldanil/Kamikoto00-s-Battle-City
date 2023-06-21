using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(Animator))]
public class Base : MonoBehaviour
{
    private Health health;
    private Animator animator;
    void Awake()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        health.BeforeDeath += BeforeDeath;
    }

    private void BeforeDeath()
    {
        animator.SetFloat("explode", 1f);
        health.health = 1000;
    }

    void OnDestroy()
    {
        LevelManager.Instance?.Lose();
    }
}
