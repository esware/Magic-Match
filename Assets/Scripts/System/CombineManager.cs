using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Scripts.System
{
    public class CombineManager
{
	private List<Combine> _combines = new List<Combine> ();
	private List<Combine> _tempCombines = new List<Combine> ();
	private Dictionary<Item, Combine> _dic = new Dictionary<Item, Combine> ();
	private int _maxCols;
	private int _maxRows;
	bool _vChecking;

	public List<List<Item>> GetCombine ()
	{

		List<List<Item>> combinedItems = new List<List<Item>> ();
		_maxCols = GameManager.Instance.maxCols;
		_maxRows = GameManager.Instance.maxRows;
		_combines.Clear ();
		_tempCombines.Clear ();
		_dic.Clear ();
		int color = -1;
		Combine combine = new Combine ();
		_vChecking = false;

		for (int row = 0; row < _maxRows; row++) {
			color = -1;
			for (int col = 0; col < _maxCols; col++) {
				Square square = GameManager.Instance.GetSquare (col, row);
				if (IsSquareNotNull (square)) {
					CheckMatches (square.item, color, ref combine);
					color = square.item.Color;
				}
			}
		}
		_vChecking = true;
		//Vertical searching
		for (int col = 0; col < _maxCols; col++) {
			color = -1;
			for (int row = 0; row < _maxRows; row++) {
				Square square = GameManager.Instance.GetSquare (col, row);
				if (IsSquareNotNull (square)) {
					CheckMatches (square.item, color, ref combine);
					color = square.item.Color;
				}
			}
		}

//		Debug.Log (" test combines detected " + tempCombines.Count);
		CheckCombines ();
//		Debug.Log ("combines detected " + combines.Count);
		//inspect combines
		foreach (Combine cmb in _combines) {
//			Debug.Log ("h: " + cmb.hCount + " v: " + cmb.vCount);
//			Debug.Log (cmb.items.Count);
//			Debug.Log (cmb.nextType);

			if (cmb.nextType != ItemsTypes.NONE) {
				Item item = cmb.items [UnityEngine.Random.Range (0, cmb.items.Count)];

				Item draggedItem = GameManager.Instance.lastDraggedItem;
				if (draggedItem) {
					if (draggedItem.Color != item.Color)
						draggedItem = GameManager.Instance.lastSwitchedItem;
					//check the dragged item found in this combine or not and change this type
					if (cmb.items.IndexOf (draggedItem) >= 0) {
						item = draggedItem;
					}
				}
				item.NextType = cmb.nextType;



			}
			combinedItems.Add (cmb.items);			
		}
		return combinedItems;
	}

	void CheckCombines ()
	{
		List<Combine> countedCombines = new List<Combine> ();

		//find and merge cross combines (+)
		foreach (Combine comb in _tempCombines) {
			if (_tempCombines.Count >= 2) {
				foreach (Item item in comb.items) {
					Combine newComb = FindCombineInDic (item);  
					if (comb != newComb && countedCombines.IndexOf (newComb) < 0 && countedCombines.IndexOf (comb) < 0 && IsCombineMatchThree (newComb)) {
						countedCombines.Add (newComb);
						countedCombines.Add (comb);
						Combine mergedCombine = MergeCombines (comb, newComb);
						_combines.Add (mergedCombine);
						foreach (Item item_ in comb.items) {
							_dic [item_] = mergedCombine;						
						}
						foreach (Item item_ in newComb.items) {
							_dic [item_] = mergedCombine;						
						}

						break;
					}
				}
			}
		} 

		//find simple combines (3,4,5) 
		foreach (Combine comb in _tempCombines) {
			if (_combines.IndexOf (comb) < 0 && IsCombineMatchThree (comb) && countedCombines.IndexOf (comb) < 0) {
				_combines.Add (comb);
				comb.nextType = SetNextItemType (comb);
			}
		}


//		foreach (var pair in dic) {
//
//			if (combines.IndexOf (pair.Value) < 0 && IsCombineMatchThree (pair.Value)) {
//				pair.Value.nextType = SetNextItemType (pair.Value);
//				combines.Add (pair.Value);
//			}
//		}
	}

	Combine MergeCombines (Combine comb1, Combine comb2)
	{ 
		Combine combine = new Combine ();
		combine.hCount = comb1.hCount + comb2.hCount - 1;
		combine.vCount = comb1.vCount + comb2.vCount - 1;
		combine.items.AddRange (comb1.items);
		combine.items.AddRange (comb2.items);
		combine.nextType = SetNextItemType (combine);
		return combine;
	}

	ItemsTypes SetNextItemType (Combine combine)
	{
//		Debug.Log (combine.hCount + " " + combine.vCount);
		if (combine.hCount > 4 || combine.vCount > 4)
			return ItemsTypes.BOMB;
		if (combine.hCount >= 3 && combine.vCount >= 3)
			return ItemsTypes.PACKAGE;
		if (combine.hCount > 3 || combine.vCount > 3) {
			if (GameManager.Instance.lastDraggedItem) {
				Vector2 dir = GameManager.Instance.lastDraggedItem.moveDirection;
				if (Math.Abs (dir.x) > Math.Abs (dir.y))
					return ItemsTypes.HORIZONTAL_STRIPPED;
				else
					return ItemsTypes.VERTICAL_STRIPPED;
				
			}
			return (ItemsTypes)UnityEngine.Random.Range (1, 3);
		}
		return ItemsTypes.NONE;
	}

	void CheckMatches (Item item, int color, ref Combine combine)
	{
		//Debug.Log("color " + item.color);
		//if (color != item.color) {

		combine = FindCombine (item);
		//}
		AddItemToCombine (combine, item);
	}

	void AddItemToCombine (Combine combine, Item item)
	{
		combine.AddingItem = item;
		_dic [item] = combine;

		if (IsCombineMatchThree (combine)) {
			if (_tempCombines.IndexOf (combine) < 0) {
				_tempCombines.Add (combine);
				//Debug.Log("add " + combine.GetHashCode());
			}
		}
	}

	bool IsCombineMatchThree (Combine combine)
	{
		if (combine.hCount > 2 || combine.vCount > 2) {
			return true;
		}
		return false;
	}

	bool IsSquareNotNull (Square square)
	{
		if (square == null)
			return false;
		if (square.item == null)
			return false;
		return true;
	}

	Combine FindCombine (Item item)
	{
		Combine combine = null;
		Item leftItem = item.GetLeftItem ();
		if (CheckColor (item, leftItem) && !_vChecking)
			combine = FindCombineInDic (leftItem);
		if (combine != null)
			return combine;
		Item topItem = item.GetTopItem ();
		if (CheckColor (item, topItem) && _vChecking)
			combine = FindCombineInDic (topItem);
		if (combine != null)
			return combine;

		return new Combine ();

	}

	Combine FindCombineInDic (Item item)
	{
		Combine combine;
		if (_dic.TryGetValue (item, out combine)) {
			return combine;
		}
		return new Combine ();
	}

	bool CheckColor (Item item, Item nextItem)
	{
		if (nextItem) {
			if (nextItem.Color == item.Color && nextItem.currentType != ItemsTypes.BOMB && nextItem.currentType != ItemsTypes.INGREDIENT)//2.0
				return true;
		}
		return false;
	}

}

public class Combine
{
	private Item addingItem;
	public List<Item> items = new List<Item> ();
	public int vCount;
	public int hCount;
	Vector2 latestItemPositionH = new Vector2 (-1, -1);
	Vector2 latestItemPositionV = new Vector2 (-1, -1);
	public ItemsTypes nextType;

	public Item AddingItem {
		get {
			return addingItem;
		}

		set {
			addingItem = value;
			if (CompareColumns (addingItem)) {
				if (latestItemPositionH.y != addingItem.square.row && latestItemPositionH.y > -1)
					hCount = 0;
				hCount++;
				latestItemPositionH = new Vector2 (addingItem.square.col, addingItem.square.row);

			} else if (CompareRows (addingItem)) {
				if (latestItemPositionV.x != addingItem.square.col && latestItemPositionV.x > -1)
					vCount = 0;
				vCount++;
				latestItemPositionV = new Vector2 (addingItem.square.col, addingItem.square.row);

			}
			if (hCount > 0 && vCount == 0) {
				vCount = 1;
			}
			items.Add (addingItem);
			//Debug.Log(" c: " + addingItem.square.col + " r: " + addingItem.square.row + " h: " + hCount + " v: " + vCount + " color: " + addingItem.color + " code: " + GetHashCode());
		}

	}

	bool CompareRows (Item item)
	{
		if (items.Count > 0) {
			if (item.square.row > PreviousItem ().square.row)
				return true;
		} else
			return true;

		return false;
	}

	bool CompareColumns (Item item)
	{
		if (items.Count > 0) {
			if (item.square.col > PreviousItem ().square.col)
				return true;
		} else
			return true;

		return false;
	}


	Item PreviousItem ()
	{
		return items [items.Count - 1];
	}
}
}