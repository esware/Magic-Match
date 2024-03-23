using System.Collections;
using System.Collections.Generic;
using Dev.Scripts.Tiles;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
   public BoardManager boardManager;
   
       private BaseTile _selectedTile = null;
       private bool _inputLocked = false;
   
       private void Update()
       {
           if (Input.GetMouseButtonDown(0) && !_inputLocked)
           {
               HandleInput();
           }
       }
   
       private void HandleInput()
       {
           if (Input.GetMouseButtonDown(0))
           {
               PointerEventData pointerData = new PointerEventData(EventSystem.current)
               {
                   position = Input.mousePosition
               };

               List<RaycastResult> results = new List<RaycastResult>();
               EventSystem.current.RaycastAll(pointerData, results);

               if (results.Count > 0)
               {
                   foreach (var result in results)
                   {
                       if (result.gameObject.CompareTag("Tile"))
                       {
                           TileSelected(result.gameObject.GetComponent<BaseTile>());
                           break;
                       }
                   }
               }
           }
       }
       
       private void TileSelected(BaseTile tile)
       {
           if (_selectedTile == null)
           {
               _selectedTile = tile;
               _selectedTile.transform.DOScale(Vector3.one * 1.2f, 0.5f);
           }
           else
           {
               _inputLocked = true;
               boardManager.SwapTiles(_selectedTile, tile);
               tile.transform.DOScale(Vector3.one * 1.2f, 0.25f).OnComplete(() =>
               {
                   tile.transform.DOScale(Vector3.one, 0.25f);
               });
               _selectedTile.transform.DOScale(Vector3.one, 0.25f).OnComplete(() =>
               {
                   _inputLocked = false;
                   _selectedTile = null;
               });
              
           }
       }
}
