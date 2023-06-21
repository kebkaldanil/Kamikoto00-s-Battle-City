using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UpdateData : MonoBehaviour
{
    public TileBase enemyDefaultTile;
    public TileBase enemyTile;
    public TileBase HP0;
    public TileBase HP1;
    public TileBase HP2;
    public TileBase HP3;
    [NonSerialized]
    public Tilemap tilemap;
    public LevelManager levelManager;
    public Vector3Int lifesCountPosition = new(1, 0);
    public Vector3Int enemiesStart = new(0, 6);
    private Respawnable respawner;
    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
    void Start()
    {
        if (levelManager == null)
        {
            levelManager = LevelManager.Instance;
            if (levelManager == null)
            {
                Debug.LogError("LevelManager not found");
            }
        }
        respawner = levelManager.Player.GetComponent<Respawnable>();
    }
    void Update()
    {
        var healthNumberTile = respawner.lifes switch {
            1 => HP1,
            2 => HP2,
            3 => HP3,
            _ => HP0,
        };
        tilemap.SetTile(lifesCountPosition, healthNumberTile);
        for (int i = 0; i < 20; i++)
        {
            tilemap.SetTile(enemiesStart + new Vector3Int(i / 10, i % 10), levelManager.EnemiesLeft > i ? enemyTile : enemyDefaultTile);
        }
    }
}
