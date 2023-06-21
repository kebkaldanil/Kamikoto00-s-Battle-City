using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TankMove), typeof(TankShooting))]
public class AI_TankControl : MonoBehaviour
{
    private TankMove tankMove;
    private TankShooting tankShooting;
    private LevelManager levelManager;
    private Vector2 currentTarget;
    void Awake()
    {
        tankShooting = GetComponent<TankShooting>();
        tankMove = GetComponent<TankMove>();
    }
    void Start()
    {
        if (levelManager == null)
        {
            levelManager = LevelManager.Instance;
            if (tankShooting == null)
            {
                Debug.LogError("LevelManager not found");
            }
        }
        currentTarget = generateTarget();
    }
    void FixedUpdate()
    {
        var raycastHit = Physics2D.Raycast(transform.position, transform.up, 20);
        if (raycastHit && (raycastHit.collider.gameObject == levelManager.Player.gameObject || raycastHit.collider.gameObject == levelManager.Base))
        {
            tankShooting.Fire();
            return;
        }
        if (TryRotateToShoot(levelManager.Player.gameObject))
        {
            return;
        }
        if (TryRotateToShoot(levelManager.Base.gameObject))
        {
            return;
        }
        var vectorToTarget = (currentTarget - (Vector2)transform.position);
        if (vectorToTarget.magnitude < 0.01)
        {
            currentTarget = generateTarget();
        }
        raycastHit = Physics2D.Raycast(transform.position, vectorToTarget);
        if (raycastHit.collider && raycastHit.collider.GetComponent<Health>())
        {
            tankShooting.Fire();
        }
        tankMove.velocity = vectorToTarget.normalized;
    }
    bool TryRotateToShoot(GameObject obj)
    {
        Vector2 diff = obj.transform.position - transform.position;
        if (diff.Abs().MinAxis() >= 1)
        {
            return false;
        }
        var posibleMovement = diff.LeftOnlyMaxAxis();
        var raycastHits = Physics2D.RaycastAll(transform.position, posibleMovement, 20);
        if (raycastHits.Any((hit) => hit.collider.gameObject == obj))
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (raycastHits.Where((hit) => hit.collider.GetComponent<Health>() == null && Vector2.Distance(transform.position, hit.collider.transform.position) < distance).Count() == 0)
            {
                currentTarget = (Vector2)transform.position + posibleMovement;
                tankMove.velocity = posibleMovement.normalized;
                tankShooting.Fire();
                return true;
            }
        }
        return false;
    }
    Vector2 generateTarget()
    {
        Vector2 result;
        Vector2Int targetCell = Vector2Int.zero;
        if (Random.Range(0, 2) != 0)
        {
            targetCell.x = Random.Range(0, levelManager.LevelSize.x);
            result = levelManager.grid.CellToWorld((Vector3Int)targetCell);
            result.y = transform.position.y;
        }
        else
        {
            targetCell.y = Random.Range(0, levelManager.LevelSize.y);
            result = levelManager.grid.CellToWorld((Vector3Int)targetCell);
            result.x = transform.position.x;
        }
        return result;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        currentTarget = generateTarget();
    }
}
