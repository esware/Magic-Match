using System.Collections;
using System.Collections.Generic;
using Dev.Scripts.Tiles;
using DG.Tweening;
using Unity.VisualScripting;
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

    private void Start()
    {
        SetupBoard();
    }

    private void SetupBoard()
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

                var position = new Vector2(startPosition.x + x * (cellSize.x + spacing),
                    startPosition.y + y * (cellSize.y + spacing));
                var tile = tileGo.GetComponent<BaseTile>();
                tile.Position = position;
                tile.X = x;
                tile.Y = y;
                
                RectTransform tileRT = tile.GetComponent<RectTransform>();
                
                tileRT.anchoredPosition = position;
                tileRT.sizeDelta = cellSize * 0.8f; 
                tileRT.localScale = Vector3.one;

                tile.name = $"Tile {x} {y}";
                tiles[x, y] = tile;
            }
        }
    }
    
    public void SwapTiles(BaseTile tile1, BaseTile tile2)
    {
        float duration = 0.5f;
        
        Vector3 tile1TargetPosition = tile2.transform.position;
        Vector3 tile2TargetPosition = tile1.transform.position;
        
        tile1.transform.DOMove(tile1TargetPosition, duration);
        tile2.transform.DOMove(tile2TargetPosition, duration);
        
        DOVirtual.DelayedCall(duration, () =>
        {
            (int x1, int y1) = (tile1.X, tile1.Y);

            tiles[tile1.X, tile1.Y] = tile2;
            tiles[tile2.X, tile2.Y] = tile1;

            (tile1.X, tile1.Y) = (tile2.X,tile2.Y);
            (tile2.X, tile2.Y) = (x1, y1);
        }).OnComplete(CheckForMatches);
        
        StartCoroutine(RefillBoard());

    }
    private void CheckForMatches()
    {
        for (int x = 0;x  < width; x++)
        {
            for (int y = 0; y < height; y++)
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
    private List<BaseTile> FindHorizontalMatches(int startX, int y)
    {
        List<BaseTile> matchList = new List<BaseTile>();
        BaseTile startTile = tiles[startX, y];
        matchList.Add(startTile);

        for (int x = startX + 1; x < width; x++)
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

        for (int x = startX - 1; x >= 0; x--)
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
    
    private IEnumerator RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (tiles[x, y].IsUnityNull())
                {
                    for (int i = y + 1; i < height; i++)
                    {
                        if (tiles[x, i] != null)
                        {
                            tiles[x, i].GetComponent<RectTransform>().DOAnchorPos(new Vector2(tiles[x, i].X, tiles[x, i].Y - 1) * tileSize, 0.5f);
                            tiles[x, i - 1] = tiles[x, i];
                            tiles[x, i] = null;
                            break;
                        }
                    }
                }
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    
                    var newTile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Count)], gridTransform.GetChild(0));
                    newTile.X = x;
                    newTile.Y = y;
                    var cellSize = new Vector2(tileSize, tileSize);
                    Vector2 startPosition = -gridTransform.sizeDelta / 2f + cellSize / 2f;
                    var position = new Vector2(startPosition.x + x * (cellSize.x + spacing),
                        startPosition.y + y * (cellSize.y + spacing));
                    
                    var tile = newTile.GetComponent<BaseTile>();
                    tiles[x, y] = tile;
                    tile.name = $"Tile {x} {y}";
                    
                    var rectTransform = newTile.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = cellSize * 0.8f; 
                    rectTransform.localScale = Vector3.one;
                    rectTransform.DOAnchorPos(position, 0.5f);
                    
                    tile.Position = position;
                    tile.X = x;
                    tile.Y = y;
                }
            }
        }
    }


}


