using System.Collections;
using System.Collections.Generic;
using Dev.Scripts;
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

        gridTransform.sizeDelta = new Vector2(totalWidth, totalHeight);
        
        Vector2 startPosition = -gridTransform.sizeDelta / 2f + new Vector2(tileSize,tileSize) / 2f;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var position = new Vector2(startPosition.x + x * (tileSize + spacing),
                    startPosition.y + y * (tileSize + spacing));
                
                BaseTile tile = TilePoolManager.Instance.GetRandomTile();
                tile.transform.SetParent(gridTransform.GetChild(0));
                tile.Position = position;
                tile.X = x;
                tile.Y = y;
                tile.name = $"Tile {x} {y}";
                tiles[x, y] = tile;
                
                RectTransform tileRT = tile.GetComponent<RectTransform>();
                
                tileRT.anchoredPosition = position;
                tileRT.sizeDelta = new Vector2(tileSize,tileSize) * 0.8f; 
                tileRT.localScale = Vector3.one;
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
        tile1.Position = new Vector2(tile1TargetPosition.x, tile1TargetPosition.y);
        tile2.Position = new Vector2(tile2TargetPosition.x, tile2TargetPosition.y);
        
        DOVirtual.DelayedCall(duration, () =>
        {
            (int x1, int y1) = (tile1.X, tile1.Y);

            tiles[tile1.X, tile1.Y] = tile2;
            tiles[tile2.X, tile2.Y] = tile1;

            (tile1.X, tile1.Y) = (tile2.X,tile2.Y);
            (tile2.X, tile2.Y) = (x1, y1);
        }).OnComplete(CheckForMatches);
        
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
                        tiles[tile.X, tile.Y] = null;
                    }
                }
            }
        }
        
        StartCoroutine(RefillBoard());
    }
    private List<BaseTile> FindHorizontalMatches(int startX, int y)
    {
        List<BaseTile> matchList = new List<BaseTile>();
        BaseTile startTile = tiles[startX, y];
        
        if (startTile == null) return matchList;
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
    IEnumerator RefillBoard()
    {
        yield return new WaitForSeconds(1f);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    for (int i = y + 1; i < height; i++)
                    {
                        if (tiles[x, i] != null)
                        {
                            float totalWidth = (tileSize * width) + (spacing * (width - 1));
                            float totalHeight = (tileSize * height) + (spacing * (height - 1));

                            gridTransform.sizeDelta = new Vector2(totalWidth, totalHeight);
        
                            Vector2 startPosition = -gridTransform.sizeDelta / 2f + new Vector2(tileSize,tileSize) / 2f;
                            var position = new Vector2(startPosition.x + x * (tileSize + spacing),
                                startPosition.y + y * (tileSize + spacing));
                            
                            tiles[x, i].GetComponent<RectTransform>().DOAnchorPos(position, 1f);
                            tiles[x, y] = tiles[x, i];
                            tiles[x, y].Position = new Vector2(position.x , position.y);
                            tiles[x, y].X = x;
                            tiles[x, y].Y = y;
                            tiles[x, i] = null;
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(1f);
        
         for (int x = 0; x < width; x++)
         {
             for (int y = 0; y < height; y++)
             {
                 if (tiles[x, y] == null)
                 {
                     float totalWidth = (tileSize * width) + (spacing * (width - 1));
                     float totalHeight = (tileSize * height) + (spacing * (height - 1));

                     gridTransform.sizeDelta = new Vector2(totalWidth, totalHeight);
        
                     Vector2 startPosition = -gridTransform.sizeDelta / 2f + new Vector2(tileSize,tileSize) / 2f;
                     var position = new Vector2(startPosition.x + x * (tileSize + spacing),
                         startPosition.y + y * (tileSize + spacing));
                     
                     BaseTile newTile = TilePoolManager.Instance.GetRandomTile();
                     newTile.transform.SetParent(gridTransform.GetChild(0), false);
                     newTile.Position = position;
                     newTile.X = x;
                     newTile.Y = y;
                     Vector2 spawnPosition = new Vector2(position.x,-startPosition.y+50f);
                     
                     RectTransform tileRT = newTile.GetComponent<RectTransform>();
                     
                     tileRT.sizeDelta = new Vector2(tileSize,tileSize) * 0.8f; 
                     tileRT.localScale = Vector3.one;
                     tileRT.anchoredPosition = spawnPosition;
                     tileRT.DOAnchorPos(position, 1f);
  
                     tiles[x, y] = newTile;
                     yield return new WaitForSeconds(0.1f);
                 }
                 
             }
         }

         CheckForMatches();
    }


}


