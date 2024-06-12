using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dev.Scripts.GUI;
using GameStates;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ItemsTypes
{
    NONE = 0,
    VERTICAL_STRIPPED,
    HORIZONTAL_STRIPPED,
    PACKAGE,
    BOMB,
    INGREDIENT
}

public class Item : MonoBehaviour
{
    //items sprites array
    public Sprite[] items;
    public GameObject[] prefabs;
    //stripe extra items array
    public List<StripedItem> stripedItems = new List<StripedItem>();
    //arrays for extra items
    public Sprite[] packageItems;
    public Sprite[] bombItems;
    public Sprite[] ingredientItems;

    //sprite rendered reference
    public SpriteRenderer sprRenderer;
    //square object reference
    public Square square;
    //is that item dragging
    public bool dragThis;

    public Vector3 mousePos;
    public Vector3 deltaPos;
    public Vector3 switchDirection;
    private Square neighborSquare;
    private Item switchItem;
    public bool falling;
    private ItemsTypes nextType = ItemsTypes.NONE;
    public ItemsTypes currentType = ItemsTypes.NONE;
    public ItemsTypes debugType = ItemsTypes.NONE;
    public int COLORView;
    private int _color;

    public int Color
    {
        get => _color;
        set => _color = value;
    }

    public ItemsTypes NextType
    {
        get => nextType;

        set => nextType = value;
    }

    public Animator anim;
    public bool destroying;
    public bool appeared;
    public bool animationFinished;
    public bool justCreatedItem;
    private float xScale;
    private float yScale;
    public bool boost;
    public Vector2 moveDirection;
    private static readonly int Appear = Animator.StringToHash("appear");
    private static readonly int DisAppear = Animator.StringToHash("disAppear");

    void Start()
    {
        gameObject.name = "item " + GetInstanceID();
        falling = true;
        GenColor();
        if (NextType != ItemsTypes.NONE)
        {
            debugType = NextType;
            currentType = NextType;
            NextType = ItemsTypes.NONE;
            transform.position = square.transform.position;
            falling = false;
        }
        else if (GameManager.Instance.limitType == LIMIT.TIME && Random.Range(0, 28) == 1)
        {
            GameObject fiveTimes = Instantiate(Resources.Load("Prefabs/5sec")) as GameObject;
            if (fiveTimes != null)
            {
                fiveTimes.transform.SetParent(transform);
                fiveTimes.name = "5sec";
                fiveTimes.transform.localScale = Vector3.one * 2;
                fiveTimes.transform.localPosition = Vector3.zero;
            }
        }
        xScale = transform.localScale.x;
        yScale = transform.localScale.y;

        //StartCoroutine(GenRandomSprite());
    }

    public void GenColor(int exceptColor = -1, bool onlyNONEType = false)
    {
        int row = square.row;
        int col = square.col;

        List<int> remainColors = new List<int>();
        for (int i = 0; i < GameManager.Instance.colorLimit; i++)
        {
            bool canGen = true;
            if (col > 1)
            {
                Square neighbor = GameManager.Instance.GetSquare(row, col - 1);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoInto() && neighbor.item.Color == i)
                            canGen = false;
                    }
                }
            }
            if (col < GameManager.Instance.maxCols - 1)
            {
                Square neighbor = GameManager.Instance.GetSquare(row, col + 1);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoOut() && neighbor.item.Color == i)
                            canGen = false;
                    }
                }
            }
            if (row < GameManager.Instance.maxRows)
            {
                Square neighbor = GameManager.Instance.GetSquare(row + 1, col);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoOut() && neighbor.item.Color == i)
                            canGen = false;
                    }
                }
            }
            if (canGen && i != exceptColor)
            {
                remainColors.Add(i);
            }
        }
        
        int randColor = Random.Range(0, GameManager.Instance.colorLimit);
        if (remainColors.Count > 0)
            randColor = remainColors[Random.Range(0, remainColors.Count)];
        if (exceptColor == randColor)
            randColor = (randColor++) % items.Length;
        GameManager.Instance.lastRandColor = randColor;
        sprRenderer.sprite = items[randColor];
        if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
            sprRenderer.sprite = stripedItems[Color].horizontal;
        else if (NextType == ItemsTypes.VERTICAL_STRIPPED)
            sprRenderer.sprite = stripedItems[Color].vertical;
        else if (NextType == ItemsTypes.PACKAGE)
            sprRenderer.sprite = packageItems[Color];
        else if (NextType == ItemsTypes.BOMB)
            sprRenderer.sprite = bombItems[0];
        else
        {
            var ingredients = GameManager.Instance.targetObject.Where(i=>i.type == Target.INGREDIENT).ToArray();
            if (ingredients.Any())
            {
                foreach (var targetObject in ingredients)
                {
                    if( Random.Range(0, GameManager.Instance.limit) == 0 && square.row + 1 < GameManager.Instance.maxRows && !onlyNONEType
                        && !targetObject.Done() && GameManager.Instance.GetIngredientsCount(targetObject.icon.name) < targetObject.GetCount())
                    {
                        StartCoroutine(FallingCor(square, true));
                        Color = 1000;
                        currentType = ItemsTypes.INGREDIENT;
                        sprRenderer.sprite = targetObject.icon;
                        name = "ingredient_" + Color;
                    }
                    else
                    {
                        StartCoroutine(FallingCor(square, true));
                        Color = Array.IndexOf(items, sprRenderer.sprite);
                    }
                }
            }
            else
            {
                StartCoroutine(FallingCor(square, true));
                Color = Array.IndexOf(items, sprRenderer.sprite);
            }
        }

        if (prefabs.Length > 0)
        {
            sprRenderer.enabled = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name != "Sprite")
                { Destroy(transform.GetChild(i)); }
            }
            GameObject obj = Instantiate(prefabs[Color]);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
        }

    }

    public void SetColor(int col)
    {
        Color = col;
        if (Color < items.Length)
            sprRenderer.sprite = items[Color];
    }

    public void SetAppeared()
    {
        appeared = true;
        StartIdleAnim();
        if (currentType == ItemsTypes.PACKAGE)
            anim.SetBool("package_idle", true);

    }

    public void StartIdleAnim()
    {
        StartCoroutine(AnimIdleStart());

    }

    IEnumerator AnimIdleStart()
    {
        float xScaleDest1 = xScale - 0.05f;
        float xScaleDest2 = xScale;
        float speed = Random.Range(0.02f, 0.07f);

        bool trigger = false;
        while (true)
        {
            if (!trigger)
            {
                if (xScale > xScaleDest1)
                {
                    xScale -= Time.deltaTime * speed;
                    yScale += Time.deltaTime * speed;
                }
                else
                    trigger = true;
            }
            else
            {
                if (xScale < xScaleDest2)
                {
                    xScale += Time.deltaTime * speed;
                    yScale -= Time.deltaTime * speed;
                }
                else
                    trigger = false;
            }
            transform.localScale = new Vector3(xScale, yScale, 1);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator GenRandomSprite()
    {
        Sprite spr = null;
        while (true)
        {
            spr = items[Random.Range(0, items.Length)];
            yield return new WaitForFixedUpdate();
            break;
        }

        sprRenderer.sprite = spr;
    }

    void ResetDrag()
    {
        dragThis = false;
        switchDirection = Vector3.zero;
        neighborSquare = null;
        switchItem = null;

    }

    void Update()
    {
        COLORView = Color;
        if (currentType != debugType && currentType != ItemsTypes.INGREDIENT)
        {
            NextType = debugType;
            ChangeType();

        }
        if (dragThis)
        {
            deltaPos = mousePos - GetMousePosition();
            if (switchDirection == Vector3.zero)
            {
                SwitchDirection(deltaPos);
            }
        }
    }

    public void SwitchDirection(Vector3 delta)
    {
        deltaPos = delta;
        if (Vector3.Magnitude(deltaPos) > 0.1f)
        {
            if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x > 0)
                switchDirection.x = 1;
            else if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x < 0)
                switchDirection.x = -1;
            else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y > 0)
                switchDirection.y = 1;
            else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y < 0)
                switchDirection.y = -1;
            if (switchDirection.x > 0)
            {
                neighborSquare = square.GetNeighborLeft();
            }
            else if (switchDirection.x < 0)
            {
                neighborSquare = square.GetNeighborRight();
            }
            else if (switchDirection.y > 0)
            {
                neighborSquare = square.GetNeighborBottom();
            }
            else if (switchDirection.y < 0)
            {
                neighborSquare = square.GetNeighborTop();
            }
            if (neighborSquare != null)
                switchItem = neighborSquare.item;
            if (switchItem != null)
            {
                if (switchItem.square.type != SquareTypes.WIREBLOCK)
                    StartCoroutine(Switching());
                else if ((currentType != ItemsTypes.NONE || switchItem.currentType != ItemsTypes.NONE) && switchItem.square.type != SquareTypes.WIREBLOCK)
                    StartCoroutine(Switching());
                else
                    ResetDrag();

            }
            else
                ResetDrag();
        }

    }

    IEnumerator Switching()
    {
        if (switchDirection != Vector3.zero && neighborSquare != null)
        {
            bool backMove = false;
            GameManager.Instance.DragBlocked = true;
            neighborSquare.item = this;
            square.item = switchItem;
            int matchesHere = neighborSquare.FindMatchesAround().Count;
            int matchesInThisItem = matchesHere;
            int matchesInNeithborItem = square.FindMatchesAround().Count;
            bool thisItemHaveMatch = matchesInThisItem >= 4;

            if (matchesInNeithborItem >= 4)
                thisItemHaveMatch = false;
            int matchesHereOneColor = matchesHere;
            matchesHere += matchesInNeithborItem;

            if ((currentType == ItemsTypes.BOMB || switchItem.currentType == ItemsTypes.BOMB) && (currentType != ItemsTypes.INGREDIENT && switchItem.currentType != ItemsTypes.INGREDIENT))
                matchesHere++;
            if (currentType > 0 && switchItem.currentType > 0)
                matchesHere++;
            if (currentType == ItemsTypes.INGREDIENT && switchItem.currentType == ItemsTypes.INGREDIENT)
                matchesHere = 0;
            float startTime = Time.time;
            Vector3 startPos = transform.position;

            float speed = 5;
            float distCovered = 0;
            while (distCovered < 1)
            {
                distCovered = (Time.time - startTime) * speed;
                transform.position = Vector3.Lerp(startPos, neighborSquare.transform.position + Vector3.back * 0.3f, distCovered);
                switchItem.transform.position = Vector3.Lerp(neighborSquare.transform.position + Vector3.back * 0.2f, startPos, distCovered);
                yield return new WaitForFixedUpdate();
            }
            if (matchesHere <= 0 && matchesInNeithborItem <= 0 && GameManager.Instance.ActivatedBoost.type != BoostType.Hand ||
                ((currentType == ItemsTypes.BOMB || switchItem.currentType == ItemsTypes.BOMB) && (currentType == ItemsTypes.INGREDIENT || switchItem.currentType == ItemsTypes.INGREDIENT) && (matchesHere + matchesInNeithborItem <= 2)) ||
                (((int)currentType >= 1 || (int)switchItem.currentType >= 1) && (currentType == ItemsTypes.INGREDIENT || switchItem.currentType == ItemsTypes.INGREDIENT) && (matchesHere + matchesInNeithborItem <= 2)))
            {

                neighborSquare.item = switchItem;
                square.item = this;
                backMove = true;
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.wrongMatch);
            }
            else
            {
                if (GameManager.Instance.ActivatedBoost.type != BoostType.Hand)
                {
                    if (GameManager.Instance.limitType == LIMIT.MOVES)
                        GameManager.Instance.limit--;
                    GameManager.Instance.moveID++;
                }
                if (GameManager.Instance.ActivatedBoost.type == BoostType.Hand)
                    GameManager.Instance.ActivatedBoost = null;
                switchItem.square = square;
                square = neighborSquare;
                GameManager.Instance.lastDraggedItem = this;
                GameManager.Instance.lastSwitchedItem = switchItem;

                if (matchesHereOneColor == 4 || matchesInNeithborItem == 4)
                {
                    if (thisItemHaveMatch)
                        SetStrippedExtra(startPos - neighborSquare.transform.position);
                    else
                    {
                        GameManager.Instance.lastDraggedItem = switchItem;
                        GameManager.Instance.lastSwitchedItem = this;
                        switchItem.SetStrippedExtra(startPos - square.transform.position);
                        if (matchesInThisItem == 4)
                            SetStrippedExtra(startPos - neighborSquare.transform.position);
                    }
                }

                if (matchesHere >= 5)
                {
                    if (thisItemHaveMatch && matchesHereOneColor >= 5)
                        NextType = ItemsTypes.BOMB;
                    else if (!thisItemHaveMatch && matchesInNeithborItem >= 5)
                    {
                        GameManager.Instance.lastDraggedItem = switchItem;
                        GameManager.Instance.lastSwitchedItem = this;
                        switchItem.NextType = ItemsTypes.BOMB;
                        if (matchesInThisItem >= 5)
                            NextType = ItemsTypes.BOMB;
                    }

                }
                if (currentType != ItemsTypes.INGREDIENT && switchItem.currentType != ItemsTypes.INGREDIENT)
                {
                    CheckChocoBomb(this, switchItem);
                    if (currentType != ItemsTypes.BOMB || switchItem.currentType != ItemsTypes.BOMB)
                        CheckChocoBomb(switchItem, this);
                }

                if ((currentType == ItemsTypes.HORIZONTAL_STRIPPED || currentType == ItemsTypes.VERTICAL_STRIPPED) && (switchItem.currentType == ItemsTypes.HORIZONTAL_STRIPPED || switchItem.currentType == ItemsTypes.VERTICAL_STRIPPED))
                {
                    switchItem.currentType = ItemsTypes.NONE; 
                    switchItem.nextType = ItemsTypes.NONE;  
                    switchItem.debugType = ItemsTypes.NONE; 
                    DestroyHorizontal();
                    DestroyVertical();
                }

                CheckPackageAndStripped(this, switchItem);
                CheckPackageAndStripped(switchItem, this);


                CheckPackageAndPackage(this, switchItem);
                CheckPackageAndPackage(switchItem, this);

                if (currentType != ItemsTypes.BOMB || switchItem.currentType != ItemsTypes.BOMB)
                    GameManager.Instance.FindMatches();
            }

            startTime = Time.time;
            distCovered = 0;
            while (distCovered < 1 && backMove)
            {
                distCovered = (Time.time - startTime) * speed;
                transform.position = Vector3.Lerp(neighborSquare.transform.position + Vector3.back * 0.3f, startPos, distCovered);
                switchItem.transform.position = Vector3.Lerp(startPos, neighborSquare.transform.position + Vector3.back * 0.2f, distCovered);
                yield return new WaitForFixedUpdate();
            }

            if (backMove)
            {
                GameManager.Instance.DragBlocked = false;
                StartCoroutine(AI.THIS.CheckPossibleCombines());
            }
        }
        ResetDrag();
    }

    void CheckPackageAndPackage(Item item1, Item item2)
    {
        if (item1.currentType == ItemsTypes.PACKAGE && item2.currentType == ItemsTypes.PACKAGE)
        {
            int i = 0;
            List<Item> bigList = new List<Item>();
            List<Item> itemsList = GameManager.Instance.GetItemsAround(item2.square);
            foreach (Item item in itemsList)
            {
                if (item != null)
                {

                    bigList.AddRange(GameManager.Instance.GetItemsAround(item.square));
                }
            }
            foreach (Item item in bigList)
            {
                if (item != null)
                {
                    if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                        item.DestroyItem(true, "destroy_package");
                }
            }

            item1.DestroyPackage();
            item2.DestroyPackage();
        }
    }


    void CheckPackageAndStripped(Item item1, Item item2)
    {
        if (((item1.currentType == ItemsTypes.HORIZONTAL_STRIPPED || item1.currentType == ItemsTypes.VERTICAL_STRIPPED) && item2.currentType == ItemsTypes.PACKAGE))
        {
            int i = 0;
            List<Item> itemsList = GameManager.Instance.GetItemsAround(item2.square);
            int direction = 1;
            foreach (Item item in itemsList)
            {
                if (item != null)
                {
                    if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                    {
                        if (direction > 0)
                            item.currentType = ItemsTypes.HORIZONTAL_STRIPPED;
                        else
                            item.currentType = ItemsTypes.VERTICAL_STRIPPED;
                        direction *= -1;
                        i++;
                    }
                }
            }
            item2.DestroyPackage();
        }
    }

    public void CheckChocoBomb(Item item1, Item item2)
    {
        if (item1.currentType == ItemsTypes.INGREDIENT || item2.currentType == ItemsTypes.INGREDIENT)
            return;
        if (item1.currentType == ItemsTypes.BOMB)
        {
            if (item2.currentType == ItemsTypes.NONE)
                DestroyColor(item2.Color);
            else if (item2.currentType == ItemsTypes.HORIZONTAL_STRIPPED || item2.currentType == ItemsTypes.VERTICAL_STRIPPED)
                GameManager.Instance.SetTypeByColor(item2.Color, ItemsTypes.HORIZONTAL_STRIPPED);
            else if (item2.currentType == ItemsTypes.PACKAGE)
                GameManager.Instance.SetTypeByColor(item2.Color, ItemsTypes.PACKAGE);
            else if (item2.currentType == ItemsTypes.BOMB)
                GameManager.Instance.DestroyDoubleBomb(square.col);



            item1.DestroyItem();
        }

    }

    void SetStrippedExtra(Vector3 dir)
    {
        moveDirection = dir;

        if (Math.Abs(dir.x) > Math.Abs(dir.y))
            NextType = ItemsTypes.HORIZONTAL_STRIPPED;
        else
            NextType = ItemsTypes.VERTICAL_STRIPPED;
    }

    Vector3 GetDeltaPositionFromSquare(Square sq, Vector3 delta)
    {
        Vector3 newPos = (sq.transform.position - delta) + Vector3.back * 0.3f;
        newPos.x = Mathf.Clamp(newPos.x, sq.GetNeighborLeft(true).transform.position.x, sq.GetNeighborRight(true).transform.position.x);
        newPos.y = Mathf.Clamp(newPos.y, sq.GetNeighborBottom(true).transform.position.y, sq.GetNeighborTop(true).transform.position.y);
        return newPos;
    }


    public Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void CheckNeedToFall(Square _square)
    {
        _square.item = this;
        square.item = null;
        square = _square;  
    }

    public void StartFalling()
    {
        if (!falling)
            StartCoroutine(FallingCor(square, true));
    }

    IEnumerator FallingCor(Square _square, bool animate)
    {
        falling = true;
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        float speed = 10;
        if (GameManager.Instance.GetState<PreWinAnimations>())
            speed = 10;
        float distance = Vector3.Distance(startPos, _square.transform.position);
        float fracJourney = 0;
        if (distance > 0.5f)
        {
            while (fracJourney < 1)
            {
                speed += 0.2f;
                float distCovered = (Time.time - startTime) * speed;
                fracJourney = distCovered / distance;
                transform.position = Vector3.Lerp(startPos, _square.transform.position + Vector3.back * 0.2f, fracJourney);
                yield return new WaitForFixedUpdate();

            }
        }
        transform.position = _square.transform.position + Vector3.back * 0.2f;
        if (distance > 0.5f && animate)
        {
            anim.SetTrigger("stop");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.drop[Random.Range(0, SoundBase.Instance.drop.Length)]);
        }
        falling = false;
        justCreatedItem = false;
        transform.position = _square.transform.position + Vector3.back * 0.2f;
    }

    public bool GetNearEmptySquares()
    {
        bool nearEmptySquareDetected = false;
        if (square.row < GameManager.Instance.maxRows - 1 && square.col < GameManager.Instance.maxCols)
        {
            Square checkingSquare = GameManager.Instance.GetSquare(square.col + 1, square.row + 1, true);
            if (checkingSquare.CanGoInto() && checkingSquare.item == null && !falling)
            {//2.0
                checkingSquare = GameManager.Instance.GetSquare(square.col + 1, square.row + 1, true);
                if (checkingSquare.CanGoInto())
                {
                    if (checkingSquare.item == null)
                    {
                        square.item = null;
                        checkingSquare.item = this;
                        square = checkingSquare;
                        StartFalling();//2.0
                        nearEmptySquareDetected = true;
                    }
                }
            }
        }
        if (square.row < GameManager.Instance.maxRows - 1 && square.col > 0)
        {
            Square checkingSquare = GameManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
            if (checkingSquare.CanGoInto() && checkingSquare.item == null && !falling)
            {
                checkingSquare = GameManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
                if (checkingSquare.CanGoInto())
                {
                    if (checkingSquare.item == null)
                    {
                        square.item = null;
                        checkingSquare.item = this;
                        square = checkingSquare;
                        StartFalling();
                        nearEmptySquareDetected = true;
                    }
                }
            }
        }

        return nearEmptySquareDetected;
    }

    public bool GetNearEmptySquaresLeft()
    {
        bool nearEmptySquareDetected = false;
        if (square.row < GameManager.Instance.maxRows - 1 && square.col > 0)
        {
            Square checkingSquare = GameManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
            if (checkingSquare.CanGoInto() && checkingSquare.item == null)
            {
                checkingSquare = GameManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
                if (checkingSquare.CanGoInto())
                {
                    if (checkingSquare.item == null)
                    {
                        square.item = null;
                        checkingSquare.item = this;
                        square = checkingSquare;
                        StartCoroutine(FallingCor(square, true));
                        nearEmptySquareDetected = true;
                    }
                }
            }
        }
        return nearEmptySquareDetected;
    }
    
    public Item GetLeftItem()
    {
        Square sq = square.GetNeighborLeft();
        if (sq != null)
        {
            if (sq.item != null)
                return sq.item;
        }
        return null;
    }

    public Item GetTopItem()
    {
        Square sq = square.GetNeighborTop();
        if (sq != null)
        {
            if (sq.item != null)
                return sq.item;
        }
        return null;
    }


    public void ChangeType()
    {
        if (this != null)
            StartCoroutine(ChangeTypeCor());
    }

    IEnumerator ChangeTypeCor()
    {
        if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
        {
            anim.SetTrigger(Appear);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
        }
        else if (NextType == ItemsTypes.VERTICAL_STRIPPED)
        {
            anim.SetTrigger(Appear);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
        }
        else if (NextType == ItemsTypes.PACKAGE)
        {
            anim.SetTrigger(Appear);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearPackage);

        }
        else if (NextType == ItemsTypes.BOMB)
        {
            anim.SetTrigger(Appear);
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
            Color = 555;
        }
        while (!appeared)
            yield return new WaitForFixedUpdate();

        if (NextType == ItemsTypes.NONE)
            yield break;
        
        if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
            sprRenderer.sprite = stripedItems[Color].horizontal;
        else if (NextType == ItemsTypes.VERTICAL_STRIPPED)
            sprRenderer.sprite = stripedItems[Color].vertical;
        else if (NextType == ItemsTypes.PACKAGE)
            sprRenderer.sprite = packageItems[Color];
        else if (NextType == ItemsTypes.BOMB)
            sprRenderer.sprite = bombItems[0];
        
        debugType = NextType;
        currentType = NextType;
        NextType = ItemsTypes.NONE;

    }

    public void SetAnimationDestroyingFinished()
    {
        GameManager.Instance.itemsHided = true;
        animationFinished = true;
    }

    #region Destroying

    public void DestroyItem(bool showScore = false, string animName = "", bool explEffect = false)
    {

        if (destroying)
            return;
        if (this == null)
            return;
        StopCoroutine(AnimIdleStart());
        destroying = true;
        square.item = null;
        GetComponent<BoxCollider2D>().enabled = false;

        if (this == null)
            return;

        StartCoroutine(DestroyCor(showScore, animName, explEffect));


    }

    IEnumerator DestroyCor(bool showScore = false, string animName = "", bool explEffect = false)
    {
        if (currentType == ItemsTypes.HORIZONTAL_STRIPPED)
            PlayDestroyAnimation("destroy");
        else if (currentType == ItemsTypes.VERTICAL_STRIPPED)
            PlayDestroyAnimation("destroy");
        else if (currentType == ItemsTypes.PACKAGE)
        {
            PlayDestroyAnimation("destroy");
            yield return new WaitForSeconds(0.1f);

            GameObject partcl = Instantiate(Resources.Load("Prefabs/Effects/Firework"), transform.position, Quaternion.identity) as GameObject;
            partcl.GetComponent<ParticleSystem>().startColor = GameManager.Instance.scoresColors[Color];
            Destroy(partcl, 1f);
        }
        else if (currentType != ItemsTypes.INGREDIENT && currentType != ItemsTypes.BOMB)
        {
            PlayDestroyAnimation("destroy");
            GameObject partcl = GameManager.Instance.GetExplFromPool();
            if (partcl != null)
            {
                partcl.GetComponent<ItemAnimEvents>().item = this;
                partcl.transform.localScale = Vector3.one * 0.5f;
                partcl.transform.position = transform.position;
                partcl.GetComponent<Animator>().SetInteger("color", Color);
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.destroy[Random.Range(0, SoundBase.Instance.destroy.Length)]);
            }
            if (explEffect)
            {
                GameObject partcl1 = Instantiate(Resources.Load("Prefabs/Effects/Replace"), transform.position, Quaternion.identity) as GameObject;
                Destroy(partcl1, 1f);

            }
        }
        if (GameManager.Instance.limitType == LIMIT.TIME && transform.Find("5sec") != null)
        {
            GameObject FiveSec = transform.Find("5sec").gameObject;
            FiveSec.transform.SetParent(null);
#if UNITY_5
			FiveSec.GetComponent<Animation>().clip.legacy = true;
#endif

            FiveSec.GetComponent<Animation>().Play("5secfly");
            Destroy(FiveSec, 1);
            if (GameManager.Instance.GetState<Playing>())
                GameManager.Instance.limit += 5;
        }
        
        if (showScore)
            GameManager.Instance.PopupScore(GameManager.Instance.scoreForItem, transform.position, Color);

        GameManager.Instance.CheckCollectedTarget(gameObject);

        while (!animationFinished && currentType == ItemsTypes.NONE)
            yield return new WaitForFixedUpdate();

        square.DestroyBlock();
        if (currentType == ItemsTypes.HORIZONTAL_STRIPPED)
            DestroyHorizontal();
        else if (currentType == ItemsTypes.VERTICAL_STRIPPED)
            DestroyVertical();
        else if (currentType == ItemsTypes.PACKAGE)
            DestroyPackage();
        else if (currentType == ItemsTypes.BOMB && GameManager.Instance.GetState<PreWinAnimations>())
            CheckChocoBomb(this, GameManager.Instance.GetRandomItems(1)[0]);

        if (NextType != ItemsTypes.NONE)
        {
            Item i = square.GenItem();
            i.NextType = NextType;
            i.SetColor(Color);
            i.ChangeType();
        }

        if (destroying)
        {
            Destroy(gameObject);
        }
    }

    public void DestroyHorizontal()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.strippedExplosion);
        GameManager.Instance.StrippedShow(gameObject, true);
        List<Item> itemsList = GameManager.Instance.GetRow(square.row);
        foreach (Item item in itemsList)
        {
            if (item != null)
            {
                if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                    item.DestroyItem(true);
            }
        }
        List<Square> sqList = GameManager.Instance.GetRowSquaresObstacles(square.row);
        foreach (Square item in sqList)
        {
            if (item != null)
                item.DestroyBlock();
        }
    }

    public void DestroyVertical()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.strippedExplosion);
        GameManager.Instance.StrippedShow(gameObject, false);
        List<Item> itemsList = GameManager.Instance.GetColumn(square.col);
        foreach (Item item in itemsList)
        {
            if (item != null)
            {
                if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                    item.DestroyItem(true);
            }
        }
        List<Square> sqList = GameManager.Instance.GetColumnSquaresObstacles(square.col);
        foreach (Square item in sqList)
        {
            if (item != null)
                item.DestroyBlock();
        }


    }

    public void DestroyPackage()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.destroyPackage);

        List<Item> itemsList = GameManager.Instance.GetItemsAround(square);
        foreach (Item item in itemsList)
        {
            if (item != null)
                if (item != null)
                {
                    if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                        item.DestroyItem(true, "destroy_package");
                }
        }
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.explosion);
        currentType = ItemsTypes.NONE;
        DestroyItem(true);
    }

    public void DestroyColor(int p)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.colorBombExpl);

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().Color == p)
                item.GetComponent<Item>().DestroyItem(true, "", true);
        }
    }

    void PlayDestroyAnimation(string anim_name)
    {
        anim.SetTrigger(anim_name);

    }

    public void SmoothDestroy()
    {
        StartCoroutine(SmoothDestroyCor());
    }

    IEnumerator SmoothDestroyCor()
    {
        square.item = null;
        GetComponent<BoxCollider2D>().enabled = false;
        anim.SetTrigger(DisAppear);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    #endregion

}

[Serializable]
public class StripedItem
{
    public Sprite horizontal;
    public Sprite vertical;
}
