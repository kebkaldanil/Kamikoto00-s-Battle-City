using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankMove), typeof(TankShooting))]
public class PlayerTankControl : MonoBehaviour
{
    [Range(0f, 1f)]
    public float moveInputThreshold = 0.25f;
    public float shootInputThrehold = 0.25f;
    public string shootAxis = "Fire1";
    public string moveHorizontalAxis = "Horizontal";
    public string moveVerticalAxis = "Vertical";
    private TankMove tankMove;
    private TankShooting tankShooting;
    void Awake()
    {
        tankShooting = GetComponent<TankShooting>();
        tankMove = GetComponent<TankMove>();
    }
    void FixedUpdate()
    {
        ProceedMove();
        if (Input.GetAxis(shootAxis) != 0f)
        {
            tankShooting.Fire();
        }
    }
    void ProceedMove()
    {
        Vector2 input = new(Input.GetAxis(moveHorizontalAxis), Input.GetAxis(moveVerticalAxis));
        if (tankMove.velocity != Vector2.zero)
        {
            if (tankMove.velocity.x != 0 && Mathf.Abs(input.x) >= moveInputThreshold)
            {
                tankMove.velocity.x = Mathf.Sign(input.x);
                return;
            }
            if (tankMove.velocity.y != 0 && Mathf.Abs(input.y) >= moveInputThreshold)
            {
                tankMove.velocity.y = Mathf.Sign(input.y);
                return;
            }
        }
        if (input.Abs().MaxAxis() >= moveInputThreshold)
        {
            tankMove.velocity = input.LeftOnlyMaxAxis().normalized;
            return;
        }
        tankMove.velocity = Vector2.zero;
    }
}
