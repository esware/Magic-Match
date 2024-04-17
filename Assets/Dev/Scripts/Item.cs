using System.Collections.Generic;
using UnityEngine;

namespace Dev.Scripts
{ 
    public enum ItemsTypes
    {
        None = 0,
        VerticalStripped,
        HorizontalStripped,
        Package,
        Bomb,
        Ingredient
    }
    public class Item:MonoBehaviour
    {
        public Sprite[] items;
        public Sprite[] packageItems;
        public Sprite[] bombItems;
        public Sprite[] ingredientItems;
        
        public GameObject[] prefabs;
        public List<StripedItem> stripedItems = new List<StripedItem>();
        
        public SpriteRenderer sprRenderer;
        public Square square;
        public bool dragThis;
        public ItemsTypes currentType = ItemsTypes.None;
        public ItemsTypes debugType = ItemsTypes.None;
        public int COLORView;
        public Vector3 mousePos;
        public Vector3 deltaPos;
        public Vector3 switchDirection;
        public bool falling;
        
        private Square neighborSquare;
        private Item switchItem;
        private ItemsTypes nextType = ItemsTypes.None;
        private int COLOR;
    }
}