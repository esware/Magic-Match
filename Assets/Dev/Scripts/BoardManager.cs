using System.Collections;
using System.Collections.Generic;
using Dev.Scripts.Tiles;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public RectTransform gridTransform;
    public int width = 8;
    public int height = 8; 
    public float tileSize = 100f;
    public float spacing = 10f; 
    public List<BaseTile> tilePrefabs;

    private BaseTile[,] tiles;

    void Start()
    {
        SetupBoard();
    }

    void SetupBoard()
    {
        tiles = new BaseTile[width, height];
        float totalWidth = (tileSize * width) + (spacing * (width - 1));
        float totalHeight = (tileSize * height) + (spacing * (height - 1));
        var cellSize = new Vector2(tileSize, tileSize);
        gridTransform.sizeDelta = new Vector2(totalWidth, totalHeight);
        
        Vector2 startPosition = -gridTransform.sizeDelta / 2f + cellSize / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tilePrefab = tilePrefabs[Random.Range(0, tilePrefabs.Count)];
                GameObject tileGo = Instantiate(tilePrefab.gameObject, gridTransform.GetChild(0));
                RectTransform tileRT = tileGo.GetComponent<RectTransform>();

                Vector2 position = new Vector2(startPosition.x + x * (cellSize.x + spacing), startPosition.y + y * (cellSize.y + spacing));
                tileRT.anchoredPosition = position;
                tileRT.sizeDelta = cellSize * 0.8f; 
                tileRT.localScale = Vector3.one;

                tileGo.name = $"Tile {x} {y}";
            }
        }
    }

    public void CheckForMatches()
    {
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                BaseTile currentTile = tiles[x, y];
                if (currentTile != null)
                {
                    List<BaseTile> matchList = FindHorizontalMatches(x, y);
                    if (matchList.Count >= 3)
                    {
                        foreach (BaseTile tile in matchList)
                        {
                            tile.OnMatch();
                        }
                    }
                }
            }
        }
    }

    List<BaseTile> FindHorizontalMatches(int startX, int y)
    {
        List<BaseTile> matchList = new List<BaseTile>();
        BaseTile startTile = tiles[startX, y];
        matchList.Add(startTile);
        
        for (int x = startX + 1; x < tiles.GetLength(0); x++)
        {
            BaseTile nextTile = tiles[x, y];
            if (nextTile != null && nextTile.tileType == startTile.tileType)
            {
                matchList.Add(nextTile);
            }
            else
            {
                break;
            }
        }
        return matchList;
    }
}


