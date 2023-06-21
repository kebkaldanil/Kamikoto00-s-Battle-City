using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    public event Action<GameObject, int> OnHit;
    public void Hit(GameObject by, int damage)
    {
        OnHit?.Invoke(by, damage);
    }
}
