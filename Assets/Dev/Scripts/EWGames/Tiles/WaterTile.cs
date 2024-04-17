﻿using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.Tiles
{
    public class WaterTile:BaseTile
    {
        public override void OnMatch()
        {
            base.OnMatch();
            transform.DOScale(Vector3.one*1.5f, 0.5f).OnComplete((() =>
            {
                transform.DOScale(Vector3.zero, 0.5f).OnComplete(()=>TilePoolManager.Instance.ReturnTileToPool(this));
            }));
        }
    }
}