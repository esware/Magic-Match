using System;
using System.Collections.Generic;
using Dev.Scripts.Tiles;
using UnityEngine;
using Random = UnityEngine.Random;

public class TilePoolManager : MonoBehaviour
{
    [System.Serializable]
    public struct TilePool
    {
        public TileType type;
        public BaseTile prefab;
        public int size;
    }
    public static TilePoolManager Instance { get; private set; }
    
    [SerializeField] private List<TilePool> tilePools;
    private Dictionary<TileType, Queue<BaseTile>> poolDictionary;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
            DontDestroyOnLoad(gameObject); 
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<TileType, Queue<BaseTile>>();

        foreach (TilePool pool in tilePools)
        {
            Queue<BaseTile> objectPool = new Queue<BaseTile>();

            for (int i = 0; i < pool.size; i++)
            {
                BaseTile obj = Instantiate(pool.prefab);
                obj.gameObject.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.type, objectPool);
        }
    }
    
    public BaseTile GetRandomTile()
    {
        Array values = Enum.GetValues(typeof(TileType));
        TileType randomType = (TileType)values.GetValue(Random.Range(0, values.Length));
        return GetTileFromPool(randomType);
    }

    private BaseTile GetTileFromPool(TileType type)
    {
        if (poolDictionary.ContainsKey(type) && poolDictionary[type].Count > 0)
        {
            BaseTile tileToSpawn = poolDictionary[type].Dequeue();
            tileToSpawn.gameObject.SetActive(true);
            return tileToSpawn;
        }
        else
        {
            Debug.LogWarning("Pool for " + type.ToString() + " is empty!");
            return null;
        }
    }

    public void ReturnTileToPool(BaseTile tile)
    {
        if (tile != null && poolDictionary.ContainsKey(tile.tileType))
        {
            tile.gameObject.SetActive(false);
            poolDictionary[tile.tileType].Enqueue(tile);
        }
        else
        {
            Debug.LogWarning("ReturnTileToPool: Tile or TileType is invalid.");
        }
    }
}