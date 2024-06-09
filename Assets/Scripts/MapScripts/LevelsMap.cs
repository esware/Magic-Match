using System;
using System.Collections.Generic;
using System.Linq;
using MapScripts.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMap : MonoBehaviour 
{
	public static LevelsMap Instance;
	private static IMapProgressManager _mapProgressManager = new PlayerPrefsMapProgressManager ();

	public bool isGenerated;

	public MapLevel mapLevelPrefab;
	public Transform characterPrefab;
	public int count = 10;

	public WaypointsMover waypointsMover;
	public MapLevel characterLevel;
	public TranslationType translationType;

	public bool starsEnabled;

	public bool scrollingEnabled;
	public MapCamera mapCamera;

	public bool isClickEnabled;
	public bool isConfirmationEnabled;

	public void Awake () {
		Instance = this;
	}

	public void OnDestroy () {
		Instance = null;
	}

	public void OnEnable () {
		if (isGenerated) {
			Reset ();
		}
	}

	public List<MapLevel> GetMapLevels ()
	{
		return FindObjectsOfType<MapLevel> ().OrderBy (ml => ml.number).ToList ();
	}

	public void Reset () {
		UpdateMapLevels ();
		PlaceCharacterToLastUnlockedLevel ();
		SetCameraToCharacter ();
	}

	private void UpdateMapLevels () 
	{
		foreach (MapLevel mapLevel in GetMapLevels()) 
		{
			mapLevel.UpdateState (
				_mapProgressManager.LoadLevelStarsCount (mapLevel.number),
				IsLevelLocked (mapLevel.number));
		}
	}

	private void PlaceCharacterToLastUnlockedLevel () 
	{
		int lastUnlockedNumber = GetMapLevels ().Where (l => !l.isLocked).Select (l => l.number).Max ();
		TeleportToLevelInternal (lastUnlockedNumber, true);
	}

	public int GetLastestReachedLevel () 
	{
		return GetMapLevels ().Where (l => !l.isLocked).Select (l => l.number).Max ();
	}

	private void SetCameraToCharacter () 
	{
		MapCamera mapCamera = FindObjectOfType<MapCamera> ();
		if (mapCamera != null)
			mapCamera.SetPosition (waypointsMover.transform.position);
	}

	#region API

	public  void CompleteLevel (int number) 
	{
		CompleteLevelInternal (number, 1);
	}

	public  void CompleteLevel (int number, int starsCount) 
	{
		CompleteLevelInternal (number, starsCount);
	}

	internal void OnLevelSelected (int number) 
	{
		/*if (LevelSelected != null && !IsLevelLocked (number))
            LevelSelected (Instance, new LevelReachedEventArgs (number));
*/
		
		if (!IsLevelLocked(number))
		{
			GameEvents.OnLevelSelected?.Invoke(number);
		}
		if (!Instance.isConfirmationEnabled)
			GoToLevel (number);
	}

	private static void GoToLevel (int number)
	{
		if (Instance.translationType == TranslationType.Teleportation)
			Instance.TeleportToLevelInternal(number, false);
		else if (Instance.translationType == TranslationType.Walk) Instance.WalkToLevelInternal(number);
	}

	private bool IsLevelLocked (int number) 
	{
		return number > 1 && _mapProgressManager.LoadLevelStarsCount (number - 1) == 0;
	}

	public void OverrideMapProgressManager (IMapProgressManager mapProgressManager) 
	{
		_mapProgressManager = mapProgressManager;
	}

	public  void ClearAllProgress () {
		Instance.ClearAllProgressInternal ();
	}

	public  bool IsStarsEnabled () {
		return Instance.starsEnabled;
	}

	public  bool GetIsClickEnabled () 
	{
		return Instance.isClickEnabled;
	}

	public bool GetIsConfirmationEnabled () {
		return Instance.isConfirmationEnabled;
	}

	#endregion

	private void CompleteLevelInternal (int number, int starsCount) {
		if (IsLevelLocked (number)) {
			Debug.Log (string.Format ("Can't complete locked level {0}.", number));
		} else if (starsCount < 1 || starsCount > 3) {
			Debug.Log (string.Format ("Can't complete level {0}. Invalid stars count {1}.", number, starsCount));
		} else {
			int curStarsCount = _mapProgressManager.LoadLevelStarsCount (number);
			int maxStarsCount = Mathf.Max (curStarsCount, starsCount);
			_mapProgressManager.SaveLevelStarsCount (number, maxStarsCount);

			if (Instance != null)
				Instance.UpdateMapLevels ();
		}
	}

	private void TeleportToLevelInternal (int number, bool isQuietly) {
		MapLevel mapLevel = GetLevel (number);
		if (mapLevel.isLocked) {
			Debug.Log (string.Format ("Can't jump to locked level number {0}.", number));
		} else {
			waypointsMover.transform.position = mapLevel.pathPivot.transform.position; 
			characterLevel = mapLevel;
			if (!isQuietly)
				RaiseLevelReached (number);
		}
	}

	private void WalkToLevelInternal (int number) {
		MapLevel mapLevel = GetLevel (number);
		if (mapLevel.isLocked) {
			Debug.Log (string.Format ("Can't go to locked level number {0}.", number));
		} else {
			waypointsMover.Move (characterLevel.pathPivot, mapLevel.pathPivot,
				() => {
					RaiseLevelReached (number);
					characterLevel = mapLevel;
				});
		}
	}

	private void RaiseLevelReached (int number) 
	{
		MapLevel mapLevel = GetLevel (number);
		if (!string.IsNullOrEmpty (mapLevel.sceneName))
			SceneManager.LoadScene (mapLevel.sceneName);

		GameEvents.OnLevelReached?.Invoke(number);
	}

	public MapLevel GetLevel (int number) {
		return GetMapLevels().FirstOrDefault(ml => ml.number == number);
	}

	private void ClearAllProgressInternal () {
		foreach (MapLevel mapLevel in GetMapLevels())
			_mapProgressManager.ClearLevelProgress (mapLevel.number);
		Reset ();
	}

	public void SetStarsEnabled (bool bEnabled) 
	{
		starsEnabled = bEnabled;
		int starsCount = 0;
		foreach (MapLevel mapLevel in GetMapLevels()) {
			mapLevel.UpdateStars (starsCount);
			starsCount = (starsCount + 1) % 4;
			mapLevel.starsHoster.gameObject.SetActive (bEnabled);
			mapLevel.starsHoster.gameObject.SetActive (bEnabled);
		}
	}
	
}
