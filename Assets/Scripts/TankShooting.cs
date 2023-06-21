using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShooting : MonoBehaviour
{
    public Projectile projectile;
    [Tooltip("In seconds")]
    public float reloadTime = 1f;
    public float projectileSpawnOffset = 0.6f;
    private float reloadTimeCountdown = 0f;
    private bool fire = false;

    void FixedUpdate()
    {
        if (reloadTimeCountdown > 0f)
        {
            reloadTimeCountdown -= Time.fixedDeltaTime;
        }
    }
    void LateUpdate()
    {
        if (fire)
        {
            if (reloadTimeCountdown <= 0f)
            {
                GameObject newProjectileObject = Instantiate(projectile.gameObject);
                Projectile newProjectile = newProjectileObject.GetComponent<Projectile>();
                newProjectile.velocity = transform.up * projectile.speed;
                newProjectile.transform.position = transform.up * projectileSpawnOffset + transform.position;
                reloadTimeCountdown = reloadTime;
            }
            fire = false;
        }
    }
    public void Fire()
    {
        fire = true;
    }
}
