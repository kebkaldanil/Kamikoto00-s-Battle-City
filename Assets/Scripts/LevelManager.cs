using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Vector2Int levelSize = new(10, 10);
    public Vector2Int LevelSize { get => levelSize; }
    public int? seed;
    public int enemiesMin = 10;
    public int enemiesMax = 20;
    public int enemiesOnMapMax = 4;
    public float enemySpawnCooldown = 5f;
    public int destructableWallMin = 30;
    public int destructableWallMax = 100;
    public int undestructableWallMin = 4;
    public int undestructableWallMax = 20;
    public Grid grid;
    public PlayerTankControl playerSource;
    public Base baseSource;
    public Health destrucableWallSource;
    public GameObject undestrucableWallSource;
    public AI_TankControl enemySource;
    public uint skipTries = 100;
    public uint maxSkipTries = 1000;

    public static LevelManager Instance { get; private set; }

    private GameObject[,] map;
    private GameObject[] border;
    public PlayerTankControl Player { get; private set; }
    public Base Base { get; private set; }
    public IList<AI_TankControl> Enemies { get; private set; }
    private float enemySpawnCooldownCounter = 0;
    public int EnemiesLeft { get; private set; }
    public bool Running
    {
        get => map != null;
        set
        {
            if (map == null)
            {
                if (value)
                {
                    BuildLevel();
                }
            }
            else if (!value)
            {
                ClearManager();
            }
        }
    }
    void Awake()
    {
        if (Instance != null)
        {
            throw new System.Exception("Can not create more than one manager");
        }
        Instance = this;
        if (Mathf.Min(levelSize.x, levelSize.y) < 0)
        {
            grid = null;
            throw new System.Exception("Level size can be negative");
        }
        if (grid == null)
        {
            grid = GetComponent<Grid>();
        }
        Random.InitState(seed ?? Random.Range(0, 0xffffff));
        //modules = new ModuleSet();
    }
    void OnDestroy()
    {
        Instance = null;
    }
    public Vector2 CellToWorld(Vector2Int pos) => grid.CellToWorld((Vector3Int)pos);
    public Vector2Int WorldToCell(Vector2 pos) => (Vector2Int)grid.WorldToCell(pos);
    public GameObject InstantiateAtCell(MonoBehaviour src, Vector2Int pos) => InstantiateAtCell(src.gameObject, pos);
    public GameObject InstantiateAtCell(GameObject src, Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= levelSize.x || pos.y < 0 || pos.y >= levelSize.y)
        {
            return null;
        }
        if (GetObjectInCell(pos) != null)
        {
            return null;
        }
        var obj = Instantiate(src, grid.transform);
        if (obj == null)
        {
            return obj;
        }
        obj.transform.position = CellToWorld(pos);
        map[pos.x, pos.y] = obj;
        return obj;
    }
    public GameObject GetObjectInCell(Vector2Int pos) => map[pos.x, pos.y];
    void Start()
    {
        BuildLevel();
    }
    void BuildLevel()
    {
        FindObjectOfType<Canvas>().enabled = false;
        ClearManager();
        map = new GameObject[levelSize.x, levelSize.y];
        Enemies = new List<AI_TankControl>(enemiesOnMapMax);
        EnemiesLeft = Random.Range(enemiesMin, enemiesMax);
        border = new GameObject[(levelSize.x + levelSize.y + 2) * 2];
        Base = GenerateBase(out var basePosition, out int baseSide);
        Player = GeneratePlayer(out var playerPosition, basePosition, baseSide);
        GenerateUndestructableWalls();
        GenerateDestructableWalls();
        GenerateBorder();
        enemySpawnCooldownCounter = enemySpawnCooldown;
    }
    Base GenerateBase(out Vector2Int basePosition, out int baseSide)
    {
        Base result = null;
        baseSide = Random.Range(0, 4);
        basePosition = baseSide switch
        {
            0 => new(Random.Range(0, levelSize.x), levelSize.y - 1),
            1 => new(levelSize.x - 1, Random.Range(0, levelSize.y)),
            2 => new(Random.Range(0, levelSize.x), 0),
            3 => new(0, Random.Range(0, levelSize.y)),
            _ => Vector2Int.zero,
        };
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x | y) == 0)
                {
                    result = InstantiateAtCell(baseSource, basePosition).GetComponent<Base>();
                    result.GetComponent<Health>().OnDeath += Lose;
                }
                else
                {
                    InstantiateAtCell(destrucableWallSource, basePosition + new Vector2Int(x, y));
                }
            }
        }
        return result;
    }
    PlayerTankControl GeneratePlayer(out Vector2Int playerPosition, Vector2Int basePosition, int baseSide)
    {
        Vector2Int offset = baseSide switch
        {
            0 => Vector2Int.down,
            1 => Vector2Int.left,
            2 => Vector2Int.up,
            3 => Vector2Int.right,
            _ => Vector2Int.zero,
        } * 2;
        playerPosition = basePosition + offset;
        var result = InstantiateAtCell(playerSource, playerPosition).GetComponent<PlayerTankControl>();
        if (result.TryGetComponent(out Respawnable respawnable))
        {
            respawnable.spawn = result.transform.position;
            respawnable.health.OnDeath += Lose;
        }
        return result;
    }
    void GenerateUndestructableWalls()
    {
        int count = 0;
        int target = Random.Range(undestructableWallMin, undestructableWallMax);
        uint skiped = 0;
        while (count < target && count <= undestructableWallMin && skiped < skipTries && skiped < maxSkipTries)
        {
            if (InstantiateAtCell(undestrucableWallSource, new(Random.Range(0, levelSize.x), Random.Range(0, levelSize.y))))
            {
                count++;
                skiped = 0;
            }
            else
            {
                skiped++;
            }
        }
    }
    void GenerateDestructableWalls()
    {
        int count = 0;
        int target = Random.Range(undestructableWallMin, destructableWallMax);
        uint skiped = 0;
        while (count < target && count <= destructableWallMin && skiped < skipTries && skiped < maxSkipTries)
        {
            var wall = InstantiateAtCell(destrucableWallSource.gameObject, new(Random.Range(0, levelSize.x), Random.Range(0, levelSize.y)));
            if (wall)
            {
                wall.GetComponent<Health>().BeforeDeath += () => wall = null;
                count++;
                skiped = 0;
            }
            else
            {
                skiped++;
            }
        }
    }
    bool TrySpawnEnemy(out AI_TankControl enemy)
    {
        enemy = null;
        if (Enemies.Count >= EnemiesLeft || enemySpawnCooldownCounter > 0)
        {
            enemySpawnCooldownCounter -= Time.fixedDeltaTime;
            return false;
        }
        Vector2Int position = new(Random.Range(0, levelSize.x), Random.Range(0, levelSize.y));
        if (
            GetObjectInCell(position) == null &&
            Vector2Int.Distance(position, WorldToCell(Base.transform.position)) > Mathf.Abs(levelSize.MinAxis()) / 3 &&
            Vector2Int.Distance(position, WorldToCell(Player.transform.position)) > Mathf.Abs(levelSize.MinAxis()) / 4 &&
            Enemies.All((enemy) => Vector2Int.Distance(position, WorldToCell(enemy.transform.position)) >= 2)
        )
        {
            var ai = Instantiate(enemySource, grid.transform);
            ai.transform.position = CellToWorld(position);
            enemySpawnCooldownCounter = enemySpawnCooldown;
            Enemies.Add(ai);
            ai.GetComponent<Health>().OnDeath += () =>
            {
                Enemies.Remove(ai);
                EnemiesLeft--;
                if (EnemiesLeft <= 0)
                {
                    Win();
                }
            };
            enemy = ai;
            return true;
        }
        enemySpawnCooldownCounter = enemySpawnCooldown * Random.Range(0.2f, 0.5f);
        return false;
    }
    GameObject InstantiateBorder(int x, int y)
    {
        var result = Instantiate(undestrucableWallSource, grid.transform);
        result.transform.position = grid.CellToWorld(new(x, y));
        return result;
    }
    void GenerateBorder()
    {
        int i = 0;
        for (int x = -1; x < levelSize.x;)
        {
            border[i++] = InstantiateBorder(x, -1);
            border[i++] = InstantiateBorder(++x, levelSize.y);
        }
        for (int y = -1; y < levelSize.y;)
        {
            border[i++] = InstantiateBorder(levelSize.x, y);
            border[i++] = InstantiateBorder(-1, ++y);
        }
    }
    void FixedUpdate()
    {
        if (map != null)
        {
            if (TrySpawnEnemy(out var enemy))
            {
            }
        }
    }
    void ClearManager()
    {
        if (map != null)
        {
            foreach (var item in map)
            {
                if (item)
                {
                    Destroy(item);
                }
            }
            foreach (var item in border)
            {
                Destroy(item);
            }
            foreach (var enemy in Enemies)
            {
                Destroy(enemy.gameObject);
            }
            map = null;
            border = null;
            Enemies = null;
        }
    }
    public void Win()
    {
        FindObjectOfType<Canvas>().enabled = true;
        FindObjectOfType<ShowMessage>().Show("You won!");
        ClearManager();
    }
    public void Lose()
    {
        FindObjectOfType<Canvas>().enabled = true;
        FindObjectOfType<ShowMessage>().Show("You lose!");
        ClearManager();
    }
    public void Restart()
    {
        FindObjectOfType<EventSystem>()?.SetSelectedGameObject(null);
        BuildLevel();
    }
}
