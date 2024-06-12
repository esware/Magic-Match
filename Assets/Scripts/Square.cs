using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FindSeparating
{
    NONE = 0,
    HORIZONTAL,
    VERTICAL
}

public enum SquareTypes
{
    NONE = 0,
    EMPTY,
    BLOCK,
    WIREBLOCK,
    SOLIDBLOCK,
    DOUBLEBLOCK,
    UNDESTROYABLE,
    THRIVING
}



public class Square : MonoBehaviour
{
    public Item item;
    public int row;
    public int col;
    public SquareTypes type;

    public List<GameObject> block = new List<GameObject>();

    void Start()
    {
        if (row == GameManager.Instance.maxRows - 1)
        {
            if (GameManager.Instance.target == Target.INGREDIENT && (GameManager.Instance.ingrTarget[0] == Ingredients.Ingredient1 || GameManager.Instance.ingrTarget[1] == Ingredients.Ingredient1 || GameManager.Instance.ingrTarget[0] == Ingredients.Ingredient2 || GameManager.Instance.ingrTarget[1] == Ingredients.Ingredient2))
            {
                GameObject obj = Instantiate(Resources.Load("Prefabs/arrow_ingredients")) as GameObject;
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero + Vector3.down * 0.8f;
            }
        }

    }

    public Item GenItem(bool falling = true)
    {
        if (IsNone() && !CanGoInto())
            return null;
        GameObject i = Instantiate(GameManager.Instance.itemPrefab) as GameObject;
        i.transform.localScale = Vector2.one * 0.6f;
        i.GetComponent<Item>().square = this;

        i.transform.SetParent(transform.parent);
        if (falling)
        {
            i.transform.position = transform.position + Vector3.back * 0.2f + Vector3.up * 3f;
            i.GetComponent<Item>().justCreatedItem = true;
        }
        else
            i.transform.position = transform.position + Vector3.back * 0.2f;
        this.item = i.GetComponent<Item>();
        return this.item;
    }

    public Square GetNeighborLeft(bool safe = false)
    {
        if (col == 0 && !safe)
            return null;
        return GameManager.Instance.GetSquare(col - 1, row, safe);
    }

    public Square GetNeighborRight(bool safe = false)
    {
        if (col >= GameManager.Instance.maxCols && !safe)
            return null;
        return GameManager.Instance.GetSquare(col + 1, row, safe);
    }

    public Square GetNeighborTop(bool safe = false)
    {
        if (row == 0 && !safe)
            return null;
        return GameManager.Instance.GetSquare(col, row - 1, safe);
    }

    public Square GetNeighborBottom(bool safe = false)
    {
        if (row >= GameManager.Instance.maxRows && !safe)
            return null;
        return GameManager.Instance.GetSquare(col, row + 1, safe);
    }

    Hashtable FindMoreMatches(int spr_COLOR, Hashtable countedSquares, FindSeparating separating, Hashtable countedSquaresGlobal = null)
    {
        bool globalCounter = true;
        if (countedSquaresGlobal == null)
        {
            globalCounter = false;
            countedSquaresGlobal = new Hashtable();
        }
        if (this.item == null)
            return countedSquares;
        if (this.item.destroying)
            return countedSquares;
        if (this.item.Color == spr_COLOR && !countedSquares.ContainsValue(this.item) && this.item.currentType != ItemsTypes.INGREDIENT && item.currentType != ItemsTypes.BOMB)
        { 
            if (GameManager.Instance.onlyFalling && this.item.justCreatedItem)
                countedSquares.Add(countedSquares.Count - 1, this.item);
            else if (!GameManager.Instance.onlyFalling)
                countedSquares.Add(countedSquares.Count - 1, this.item);
            else
                return countedSquares;

            if (separating == FindSeparating.VERTICAL)
            {
                if (GetNeighborTop() != null)
                    countedSquares = GetNeighborTop().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.VERTICAL);
                if (GetNeighborBottom() != null)
                    countedSquares = GetNeighborBottom().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.VERTICAL);
            }
            else if (separating == FindSeparating.HORIZONTAL)
            {
                if (GetNeighborLeft() != null)
                    countedSquares = GetNeighborLeft().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.HORIZONTAL);
                if (GetNeighborRight() != null)
                    countedSquares = GetNeighborRight().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.HORIZONTAL);
            }
        }
        return countedSquares;
    }

    public List<Item> FindMatchesAround(FindSeparating separating = FindSeparating.NONE, int matches = 3, Hashtable countedSquaresGlobal = null)
    {
        bool globalCounter = true;
        List<Item> newList = new List<Item>();
        if (countedSquaresGlobal == null)
        {
            globalCounter = false;
            countedSquaresGlobal = new Hashtable();
        }
        Hashtable countedSquares = new Hashtable();
        countedSquares.Clear();
        if (this.item == null)
            return newList;

        if (separating != FindSeparating.HORIZONTAL)
        {
            countedSquares = this.FindMoreMatches(this.item.Color, countedSquares, FindSeparating.VERTICAL, countedSquaresGlobal);
        }

        foreach (DictionaryEntry de in countedSquares)
        {
            GameManager.Instance.countedSquares.Add(GameManager.Instance.countedSquares.Count - 1, de.Value);
        }

        if (countedSquares.Count < matches)
            countedSquares.Clear();

        if (separating != FindSeparating.VERTICAL)
        {
            countedSquares = this.FindMoreMatches(this.item.Color, countedSquares, FindSeparating.HORIZONTAL, countedSquaresGlobal);
        }

        foreach (DictionaryEntry de in countedSquares)
        {
            GameManager.Instance.countedSquares.Add(GameManager.Instance.countedSquares.Count - 1, de.Value);
        }

        if (countedSquares.Count < matches)
            countedSquares.Clear();

        foreach (DictionaryEntry de in countedSquares)
        {
            newList.Add((Item)de.Value);
        }
        return newList;
    }
    public void FallOut()
    {
        if (item != null)
        {
            Square nextSquare = GetNeighborBottom();
            if (nextSquare != null)
            {
                if (nextSquare.IsNone())
                {
                    for (int i = row + 1; i < GameManager.Instance.maxRows; i++)
                    {
                        if (GameManager.Instance.GetSquare(col, i) != null)
                        {
                            if (!GameManager.Instance.GetSquare(col, i).IsNone())
                            {
                                nextSquare = GameManager.Instance.GetSquare(col, i);
                                break;
                            }
                        }
                    }
                }
                if (nextSquare.CanGoInto())
                {
                    if (nextSquare.item == null)
                    {
                        item.CheckNeedToFall(nextSquare);
                    }
                }
            }
        }
    }

    public bool IsNone()
    {
        return type == SquareTypes.NONE;
    }

    public bool IsHaveDestroybleObstacle()
    {
        return type == SquareTypes.SOLIDBLOCK || type == SquareTypes.THRIVING;

    }

    public bool CanGoOut()
    {
        return type != SquareTypes.WIREBLOCK;
    }

    public bool CanGoInto()
    {
        return type != SquareTypes.SOLIDBLOCK && type != SquareTypes.UNDESTROYABLE && type != SquareTypes.NONE && type != SquareTypes.THRIVING;
    }

    public void DestroyBlock()
    {
        if (type == SquareTypes.UNDESTROYABLE)
            return;
        if (type != SquareTypes.SOLIDBLOCK && type != SquareTypes.THRIVING)
        {
            List<Square> sqList = GetAllNeighbors();
            foreach (Square sq in sqList)
            {
                if (sq.type == SquareTypes.SOLIDBLOCK || sq.type == SquareTypes.THRIVING)
                    sq.DestroyBlock();
            }
        }
        if (block.Count > 0)
        {
            if (type == SquareTypes.BLOCK)
            {
                GameManager.Instance.CheckCollectedTarget(gameObject.transform.Find("Block(Clone)").gameObject);
                GameManager.Instance.PopupScore(GameManager.Instance.scoreForBlock, transform.position, 0);
                GameManager.Instance.TargetBlocks--;
                block[block.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
            }
            if (type == SquareTypes.WIREBLOCK)
            {
                GameManager.Instance.PopupScore(GameManager.Instance.scoreForWireBlock, transform.position, 0);
            }
            if (type == SquareTypes.SOLIDBLOCK)
            {
                GameManager.Instance.PopupScore(GameManager.Instance.scoreForSolidBlock, transform.position, 0);
            }
            if (type == SquareTypes.THRIVING)
            {
                GameManager.Instance.PopupScore(GameManager.Instance.scoreForThrivingBlock, transform.position, 0);
                GameManager.Instance.thrivingBlockDestroyed = true;
            }
            if (type != SquareTypes.BLOCK)
            {
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.block_destroy);

                block[block.Count - 1].GetComponent<Animation>().Play("BrickRotate");
                block[block.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 4;
                block[block.Count - 1].AddComponent<Rigidbody2D>();
                block[block.Count - 1].GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(Random.insideUnitCircle.x * Random.Range(30, 200), Random.Range(100, 150)), ForceMode2D.Force);
            }
            GameObject.Destroy(block[block.Count - 1], 1.5f);
            if (block.Count > 1)
                type = SquareTypes.BLOCK;
            block.Remove(block[block.Count - 1]);

            if (block.Count == 0)
                type = SquareTypes.EMPTY;
        }

    }

    public List<Square> GetAllNeighbors()
    {
        List<Square> sqList = new List<Square>();
        Square nextSquare = null;
        nextSquare = GetNeighborBottom();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborTop();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborLeft();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborRight();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        return sqList;
    }

    public bool IsHaveSolidAbove()
    {
        for (int i = row; i >= 0; i--)
        {
            if (GameManager.Instance.GetSquare(col, i).type == SquareTypes.SOLIDBLOCK || GameManager.Instance.GetSquare(col, i).type == SquareTypes.UNDESTROYABLE || GameManager.Instance.GetSquare(col, i).type == SquareTypes.THRIVING)
                return true;
        }
        return false;
    }
}
