﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameStates;

public enum CombineType
{
	LShape,
	VShape
}

public class AI : MonoBehaviour
{
	public static AI THIS;
	public bool gotTip;
	public bool allowShowTip;
	public int corCount;
	
	private int _tipID;
	private List<Item> _nextMoveItems;

	void Start()
	{
		THIS = this;
	}

	public Vector3 vDirection;
	public CombineType combineType;
	public Item tipItem;

	/// <summary>
	/// Gets the square. Return square by row and column
	/// </summary>
	/// <param name="row">The row.</param>
	/// <param name="col">The column.</param>
	/// <returns></returns>
	Square GetSquare(int row, int col)
	{
		return GameManager.Instance.GetSquare(col, row);
	}

	/// <summary>
	/// Checks the square. Is the color of item of this square is equal to desired color. If so we add the item to nextMoveItems array.
	/// </summary>
	/// <param name="square">The square.</param>
	/// <param name="COLOR">The color.</param>
	/// <param name="moveThis">is the item should be movable?</param>
	void CheckSquare(Square square, int COLOR, bool moveThis = false)
	{
		if (square == null)
			return;
		if (square.item != null)
		{
			if (square.item.Color == COLOR)
			{
				if (moveThis && square.type != SquareTypes.WIREBLOCK)
				{
					_nextMoveItems.Add(square.item);
				}
				else if (!moveThis)
					_nextMoveItems.Add(square.item);
			}
		}

	}

	public List<Item> GetCombine()
	{
		return _nextMoveItems;
	}

	/// <summary>
	/// Loop of searching possible combines
	/// </summary>
	/// <returns></returns>
	public IEnumerator CheckPossibleCombines()
	{
		//waiting for 1 second just in case to be sure that field was built
		yield return new WaitForSeconds(1);

		//allow to show tips
		allowShowTip = true;

		//get max positions of squares
		int maxRow = GameManager.Instance.maxRows;
		int maxCol = GameManager.Instance.maxCols;

		//variable to check: are we got tip or not
		gotTip = false;

		//break, if the main scripts have not ready yet
		while (GameManager.Instance == null)
		{
			yield return new WaitForEndOfFrame();
		}
		//if game is not in Playing status - wait
		while (!GameManager.Instance.GetState<Playing>())
		{
			yield return new WaitForEndOfFrame();
		}

		//if drag have not blocked and game status Playing - continue
		if (!GameManager.Instance.DragBlocked && GameManager.Instance.GetState<Playing>())
		{
			_nextMoveItems = new List<Item>();

			if (!GameManager.Instance.GetState<Playing>())
				yield break;


			Item it = GameObject.FindGameObjectWithTag("Item").GetComponent<Item>();

			//Iteration for search possible combination 
			for (int COLOR = 0; COLOR < it.items.Length; COLOR++)
			{
				for (int col = 0; col < GameManager.Instance.maxCols; col++)
				{
					for (int row = 0; row < GameManager.Instance.maxRows; row++)
					{
						Square square = GameManager.Instance.GetSquare(col, row);
						if (square.type == SquareTypes.WIREBLOCK || square.item == null)
							continue;
						//current square called x
						//o-o-x
						//	  o
						vDirection = Vector3.zero;
						combineType = CombineType.LShape;
						if (col > 1 && row < maxRow - 1)
						{
							CheckSquare(GetSquare(row + 1, col), COLOR, true);
							CheckSquare(GetSquare(row, col - 1), COLOR);
							CheckSquare(GetSquare(row, col - 2), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							// StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
							ShowTip(_nextMoveItems);
							tipItem = _nextMoveItems[0];
							vDirection = Vector3.up;
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//    o
						//o-o x
						if (col > 1 && row > 0)
						{
							CheckSquare(GetSquare(row - 1, col), COLOR, true);
							CheckSquare(GetSquare(row, col - 1), COLOR);
							CheckSquare(GetSquare(row, col - 2), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							// StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
							vDirection = Vector3.down;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//x o o
						//o
						if (col < maxCol - 2 && row < maxRow - 1)
						{
							CheckSquare(GetSquare(row + 1, col), COLOR, true);
							CheckSquare(GetSquare(row, col + 1), COLOR);
							CheckSquare(GetSquare(row, col + 2), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							// StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
							vDirection = Vector3.up;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//o
						//x o o
						if (col < maxCol - 2 && row > 0)
						{
							CheckSquare(GetSquare(row - 1, col), COLOR, true);
							CheckSquare(GetSquare(row, col + 1), COLOR);
							CheckSquare(GetSquare(row, col + 2), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
							vDirection = Vector3.down;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//o
						//o
						//x o
						if (col < maxCol - 1 && row > 1)
						{
							CheckSquare(GetSquare(row, col + 1), COLOR, true);
							CheckSquare(GetSquare(row - 1, col), COLOR);
							CheckSquare(GetSquare(row - 2, col), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							// StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
							vDirection = Vector3.left;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//x o
						//o
						//o
						if (col < maxCol - 1 && row < maxRow - 2)
						{
							CheckSquare(GetSquare(row, col + 1), COLOR, true);
							CheckSquare(GetSquare(row + 1, col), COLOR);
							CheckSquare(GetSquare(row + 2, col), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
							vDirection = Vector3.left;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//	o
						//  o
						//o x
						if (col > 0 && row > 1)
						{
							CheckSquare(GetSquare(row, col - 1), COLOR, true);
							CheckSquare(GetSquare(row - 1, col), COLOR);
							CheckSquare(GetSquare(row - 2, col), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
							vDirection = Vector3.right;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//o x
						//  o
						//  o
						if (col > 0 && row < maxRow - 2)
						{
							CheckSquare(GetSquare(row, col - 1), COLOR, true);
							CheckSquare(GetSquare(row + 1, col), COLOR);
							CheckSquare(GetSquare(row + 2, col), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
							vDirection = Vector3.right;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//o-x-o-o
						if (col < maxCol - 2 && col > 0)
						{
							CheckSquare(GetSquare(row, col - 1), COLOR, true);
							CheckSquare(GetSquare(row, col + 1), COLOR);
							CheckSquare(GetSquare(row, col + 2), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//   StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
							vDirection = Vector3.right;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();
						//o-o-x-o
						if (col < maxCol - 1 && col > 1)
						{
							CheckSquare(GetSquare(row, col + 1), COLOR, true);
							CheckSquare(GetSquare(row, col - 1), COLOR);
							CheckSquare(GetSquare(row, col - 2), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//   StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
							vDirection = Vector3.left;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();
						//o
						//x
						//o
						//o
						if (row < maxRow - 2 && row > 0)
						{
							CheckSquare(GetSquare(row - 1, col), COLOR, true);
							CheckSquare(GetSquare(row + 1, col), COLOR);
							CheckSquare(GetSquare(row + 2, col), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
							vDirection = Vector3.down;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();

						//o
						//o
						//x
						//o
						if (row < maxRow - 2 && row > 1)
						{
							CheckSquare(GetSquare(row + 1, col), COLOR, true);
							CheckSquare(GetSquare(row - 1, col), COLOR);
							CheckSquare(GetSquare(row - 2, col), COLOR);
						}
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//   StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
							vDirection = Vector3.up;
							tipItem = _nextMoveItems[0];
							ShowTip(_nextMoveItems);
							yield break;
						}
						else
							_nextMoveItems.Clear();
						//  o
						//o x o
						//  o
						int h = 0;
						int v = 0;
						combineType = CombineType.VShape;

						if (row < maxRow - 1)
						{
							square = GetSquare(row + 1, col);
							if (square)
							{
								if (square.item != null)
								{
									if (square.item.Color == COLOR)
									{
										vDirection = Vector3.up;
										_nextMoveItems.Add(square.item);
										v++;
									}
								}
							}
						}
						if (row > 0)
						{
							square = GetSquare(row - 1, col);
							if (square)
							{
								if (square.item != null)
								{
									if (square.item.Color == COLOR)
									{
										vDirection = Vector3.down;
										_nextMoveItems.Add(square.item);
										v++;
									}
								}
							}
						}
						if (col > 0)
						{
							square = GetSquare(row, col - 1);
							if (square)
							{
								if (square.item != null)
								{
									if (square.item.Color == COLOR)
									{
										vDirection = Vector3.right;
										_nextMoveItems.Add(square.item);
										h++;
									}
								}
							}
						}
						if (col < maxCol - 1)
						{
							square = GetSquare(row, col + 1);
							if (square)
							{
								if (square.item != null)
								{
									if (square.item.Color == COLOR)
									{
										vDirection = Vector3.left;
										_nextMoveItems.Add(square.item);
										h++;
									}
								}
							}
						}

						//if we found 3or more items and they not lock show tip
						if (_nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto() && GetSquare(row, col).type != SquareTypes.WIREBLOCK)
						{
							if (v > h && _nextMoveItems[2].square.type != SquareTypes.WIREBLOCK)
							{ 
								tipItem = _nextMoveItems[2];
								if (tipItem.transform.position.x > _nextMoveItems[0].transform.position.x)
									vDirection = Vector3.left;
								else
									vDirection = Vector3.right;
								ShowTip(_nextMoveItems);
								yield break;

							}
							if (v < h && _nextMoveItems[0].square.type != SquareTypes.WIREBLOCK)
							{
								tipItem = _nextMoveItems[0];
								if (tipItem.transform.position.y > _nextMoveItems[0].transform.position.y)
									vDirection = Vector3.down;
								else
									vDirection = Vector3.up;

								ShowTip(_nextMoveItems);
								yield break;

							}
							_nextMoveItems.Clear();
						}
						else
							_nextMoveItems.Clear();

					}
				}


			}
			//if we don't get any tip.  call nomatches to regenerate level
			if (!GameManager.Instance.DragBlocked)
			{
				if (!gotTip)
					GameManager.Instance.NoMatches();
			}

		}
		yield return new WaitForEndOfFrame();
		//find possible combination again 
		if (!GameManager.Instance.DragBlocked)
			StartCoroutine(CheckPossibleCombines());

	}
	
	void ShowTip(List<Item> nextMoveItems)
	{
		StopCoroutine(ShowTipCor(nextMoveItems));
		StartCoroutine(ShowTipCor(nextMoveItems));
	}
	
	IEnumerator ShowTipCor(List<Item> nextMoveItems)
	{
		gotTip = true;
		corCount++;
		if (corCount > 1)
		{
			corCount--;
			yield break;
		}
		if (GameManager.Instance.DragBlocked && !allowShowTip)
		{
			corCount--;
			yield break;
		}
		_tipID = GameManager.Instance.moveID;
		//while (!LevelManager.THIS.DragBlocked && allowShowTip)
		//{
		yield return new WaitForSeconds(1);
		if (GameManager.Instance.DragBlocked && !allowShowTip && _tipID != GameManager.Instance.moveID)
		{
			corCount--;
			yield break;
		}
		foreach (Item item in nextMoveItems)
		{
			if (item == null)
			{
				corCount--;
				yield break;
			}

		}
		//call animation trigger for every found item to show tip
		foreach (Item item in nextMoveItems)
		{
			if (item != null)
				item.anim.SetTrigger("tip");
		}
		yield return new WaitForSeconds(0);
		StartCoroutine(CheckPossibleCombines());
		corCount--;
		// }
	}


}