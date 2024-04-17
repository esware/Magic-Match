﻿using System.Collections.Generic;
using UnityEngine;

namespace Dev.Scripts
{ 
    public enum FindSeparating
    {
        None = 0,
        Horizontal,
        Vertical
    }

    public enum SquareTypes
    {
        None = 0,
        Empty,
        Block,
        WireBlock,
        SolidBlock,
        DoubleBlock,
        UndestroyableBlock,
        Thriving
    }
    public class Square:MonoBehaviour
    {
        public Item item;
        public int row;
        public int col;
        public SquareTypes type;
        public List<GameObject> block = new List<GameObject>();
        
        
    }
}