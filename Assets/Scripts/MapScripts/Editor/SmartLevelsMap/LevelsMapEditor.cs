using System.Collections.Generic;
using MapScripts.Scripts;
using UnityEditor;
using UnityEngine;

namespace MapScripts.Editor.SmartLevelsMap
{
	[CustomEditor (typeof(LevelsMap))]
	public class LevelsMapEditor : LevelsEditorBase
	{
		private LevelsMap _levelsMap;

		private float _width;
		private float _height;
		bool fixToggle;

	public void OnEnable() {
	    _levelsMap = target as LevelsMap;
	}

	public override void OnInspectorGUI() {
	    GUILayout.BeginVertical();
	    fixToggle = EditorGUILayout.Toggle("fix map", fixToggle);
	    if (!fixToggle) {
	        if (_levelsMap != null && _levelsMap.isGenerated) {
	            DrawLevelsSettings();
	            DrawStarsSettings();
	            DrawMapCameraSettings();

	            DrawLevelClickSettings();

	            if (GUILayout.Button("Clear all", GUILayout.MinWidth(120)) &&
	                EditorUtility.DisplayDialog("Delete all?",
	                    "Are you sure that you want to delete all levels map settings?", "Delete", "Cancel")) {
	                Clear();
	            }
	        } else {
	            DrawGenerateDraft();
	        }
	    } else {
	        DrawDefaultInspector();
	    }
	    GUILayout.Space(16);
	    GUILayout.EndVertical();

	    if (_levelsMap != null) {
	        EditorUtility.SetDirty(_levelsMap);
	    }
	}

	private void DrawLevelsSettings() {
	    if (_levelsMap == null) return;

	    GUILayout.BeginVertical("Box");
	    EditorGUILayout.LabelField("General:");

	    if (_levelsMap.waypointsMover != null) {
	        _levelsMap.waypointsMover.speed = EditorGUILayout.FloatField("Character speed", _levelsMap.waypointsMover.speed);

	        _levelsMap.translationType = (TranslationType)EditorGUILayout.EnumPopup("Translation type", _levelsMap.translationType);

	        if (_levelsMap.waypointsMover.path == null) {
	            Transform pathTransform = _levelsMap.transform.Find("Path");
	            if (pathTransform != null) {
	                _levelsMap.waypointsMover.path = pathTransform.GetComponent<MapScripts.Scripts.Path>();
	            }
	        }

	        if (_levelsMap.waypointsMover.path != null) {
	            MapScripts.Scripts.Path path = _levelsMap.waypointsMover.path;
	            path.isCurved = EditorGUILayout.Toggle("Curved", path.isCurved);
	            path.gizmosColor = EditorGUILayout.ColorField("Gizmos Path Color", path.gizmosColor);
	            path.gizmosRadius = EditorGUILayout.FloatField("Gizmos Path Pivot Radius", path.gizmosRadius);

	            EditorUtility.SetDirty(path);
	        }
	    }

	    GUILayout.EndVertical();
	}


		private void Clear () {
			while (_levelsMap.transform.childCount > 0) {
				DestroyImmediate (_levelsMap.transform.GetChild (0).gameObject);
			}
			_levelsMap.isGenerated = false;
			DisableScrolling ();
		}

		#region Generation

		private void DrawGenerateDraft () {
			GUILayout.BeginVertical ("Box");
			_levelsMap.count = EditorGUILayout.IntField ("Count", _levelsMap.count);
			_levelsMap.mapLevelPrefab = EditorGUILayout.ObjectField ("Level prefab", _levelsMap.mapLevelPrefab, typeof(MapLevel), false) as MapLevel;
			_levelsMap.characterPrefab = EditorGUILayout.ObjectField ("Character prefab", _levelsMap.characterPrefab, typeof(Transform), false) as Transform;
			GUILayout.EndVertical ();

			if (GUILayout.Button ("Generate draft", GUILayout.MinWidth (120))) {
				Generate ();
				_levelsMap.isGenerated = true;
				SetStarsEnabled (_levelsMap, false);
			}
		}

		private void Generate () {
			InitBounds ();
			List<MapLevel> levels = GenerateLevels ();
			MapScripts.Scripts.Path path = GeneratePath (levels);
			_levelsMap.waypointsMover = GenerateCharacter (path);
			_levelsMap.waypointsMover.transform.position = levels [0].transform.position;
		}

		private void InitBounds () {
			_height = Camera.main.orthographicSize * 2 * 0.9f;
			_width = _height * 1.33333f * 0.9f;
		}

		private List<MapLevel> GenerateLevels () {
			GameObject goLevels = new GameObject ("Levels");
			goLevels.transform.parent = _levelsMap.transform;
			float[] points = DevideLineToPoints (_levelsMap.count);
			List<MapLevel> levels = new List<MapLevel> ();
			for (int i = 0; i < _levelsMap.count; i++) {
				MapLevel mapLevel = CreateMapLevel (points [i], i + 1);
				mapLevel.transform.parent = goLevels.transform;
				levels.Add (mapLevel);
			}
			UpdateLevelsNumber (levels);
			return levels;
		}

		private MapLevel CreateMapLevel (float point, int number) {
			Vector3 position;
			if (point < 1f / 3f)
				position = GetPosition (point * 3f, _width, 0, _height / 3f, 0);
			else if (point < 2f / 3f)
				position = GetPosition ((point - 1f / 3f) * 3f, -_width, _width, _height / 3f, _height / 3f);
			else
				position = GetPosition ((point - 2f / 3f) * 3f, _width, 0, _height / 3f, _height * 2f / 3f);
			return CreateMapLevel (position, number, _levelsMap.mapLevelPrefab);
		}

		private Path GeneratePath (List<MapLevel> levels) {
			Path path = new GameObject ("Path").AddComponent<Path> ();
			path.isCurved = false;
			path.gizmosRadius = Camera.main.orthographicSize / 40f;
			path.transform.parent = _levelsMap.transform;
			UpdatePathWaypoints (levels);
			return path;
		}

		private Vector3 GetPosition (float p, float width, float xOffset, float height, float yOffset) {
			return new Vector3 (
				xOffset + p * width - _width / 2f,
				yOffset + p * height - _height / 2f,
				0f);
		}

		/// <summary>
		/// Devide [0,1] line to array of points.
		/// If count = 1, ret {0}
		/// If count = 2, ret {0, 1}
		/// If count = 3, ret {0, 0.5, 1}
		/// If count = 4, ret {0, 0.25, 0.25, 1}
		/// </summary>
		private float[] DevideLineToPoints (int pointsCount) {
			if (pointsCount <= 0)
				return new float[0];

			float[] points = new float[pointsCount];
			for (int i = 0; i < pointsCount; i++)
				points [i] = i * 1f / (pointsCount - 1);

			return points;
		}

		private WaypointsMover GenerateCharacter (MapScripts.Scripts.Path path) {
			Transform character = PrefabUtility.InstantiatePrefab (_levelsMap.characterPrefab) as Transform;
			character.transform.parent = _levelsMap.transform;
			WaypointsMover waypointsMover = character.gameObject.AddComponent<WaypointsMover> ();
			waypointsMover.path = path;
			waypointsMover.speed = Camera.main.orthographicSize;
			return waypointsMover;
		}

		#endregion

		#region Stars

		private void DrawStarsSettings () {
			if (_levelsMap.starsEnabled) {
				if (GUILayout.Button ("Disable stars")) {
					SetStarsEnabled (_levelsMap, false);
				} else {
					DrawEnableState ();
				}
			} else {
				if (GUILayout.Button ("Enable stars")) {
					SetStarsEnabled (_levelsMap, true);
				}
			}
		}

		private void DrawEnableState () {
			GUILayout.BeginVertical ("Box");
			GUILayout.Label ("Stars enabled:");
			GUILayout.EndVertical ();
		}

		#endregion

		#region Map camera

		private void DrawMapCameraSettings () {
			if (_levelsMap.scrollingEnabled) {
				if (GUILayout.Button ("Disable map scrolling"))
					DisableScrolling ();
				else
					DrawMapCameraBounds ();
			} else {
				if (GUILayout.Button ("Enable map scrolling"))
					EnableScrolling (Camera.main);
			}
		}
		
		private void EnableScrolling(Camera camera)
		{
			_levelsMap.scrollingEnabled = true;
			MapCamera mapCameraComponent = camera.gameObject.AddComponent<MapCamera>();
			mapCameraComponent.camera = camera;
			_levelsMap.mapCamera = mapCameraComponent;
			_levelsMap.mapCamera.bounds.size = new Vector2(camera.orthographicSize * 3f, camera.orthographicSize * 3f);
			EditorUtility.SetDirty(_levelsMap);
		}


		private void DisableScrolling () {
			_levelsMap.scrollingEnabled = false;
			DestroyImmediate (_levelsMap.mapCamera);
			EditorUtility.SetDirty (_levelsMap);
		}

		private void DrawMapCameraBounds () {
			MapCamera mapCamera = _levelsMap.mapCamera;
			if (!mapCamera) return;
			GUILayout.BeginVertical ("Box");

			EditorGUILayout.LabelField ("Map bounds:");

			mapCamera.bounds.center = new Vector3 (
				EditorGUILayout.FloatField ("Center X", mapCamera.bounds.center.x),
				mapCamera.bounds.center.y,
				mapCamera.bounds.center.z);
			mapCamera.bounds.center = new Vector3 (
				mapCamera.bounds.center.x,
				EditorGUILayout.FloatField ("Center Y", mapCamera.bounds.center.y),
				mapCamera.bounds.center.z);
			mapCamera.bounds.size = new Vector3 (
				EditorGUILayout.FloatField ("Size X", mapCamera.bounds.size.x),
				mapCamera.bounds.size.y,
				mapCamera.bounds.size.z);
			mapCamera.bounds.size = new Vector3 (
				mapCamera.bounds.size.x,
				EditorGUILayout.FloatField ("Size Y", mapCamera.bounds.size.y),
				mapCamera.bounds.size.z);

			GUILayout.EndVertical ();

			EditorUtility.SetDirty (mapCamera);

			Camera camera = EditorGUILayout.ObjectField ("Map Camera", mapCamera.GetComponent<Camera>(), typeof(Camera), true) as Camera;
			if (camera != mapCamera.GetComponent<Camera>()) {
				if (camera == null) {
					DisableScrolling ();
				} else {
					Bounds bounds = mapCamera.bounds;
					DisableScrolling ();
					EnableScrolling (camera);
					mapCamera = _levelsMap.mapCamera;
					mapCamera.bounds = bounds;
					EditorUtility.SetDirty (mapCamera);
				}
			}
		}

		#endregion

		#region Level selection confirmation

		private void DrawLevelClickSettings () {
			if (_levelsMap.isClickEnabled) {
				if (GUILayout.Button ("Disable levels click/tap")) {
					_levelsMap.isClickEnabled = false;
					EditorUtility.SetDirty (_levelsMap);
				}
				DrawConfirmationSettings ();
			} else {
				if (GUILayout.Button ("Enable levels click/tap")) {
					_levelsMap.isClickEnabled = true;
					EditorUtility.SetDirty (_levelsMap);
				}
			}
		}

		private void DrawConfirmationSettings () {
			GUILayout.BeginVertical ("Box");
			string helpString = "Level click/tap enabled.\n";

			if (_levelsMap.isConfirmationEnabled) {
				helpString +=
					"Confirmation enabled: Click/tap level on map and catch 'LevelsMap.LevelSelected' event. After confirmation call 'LevelsMap.GoToLevel' method.";
				GUILayout.Box (helpString);
				if (GUILayout.Button ("Disable confirmation")) {
					_levelsMap.isConfirmationEnabled = false;
					EditorUtility.SetDirty (_levelsMap);
				}
			} else {
				helpString += "Confirmation disabled: Click/tap level on map for character moving to level.";
				GUILayout.Box (helpString);
				if (GUILayout.Button ("Enable confirmation")) {
					_levelsMap.isConfirmationEnabled = true;
					EditorUtility.SetDirty (_levelsMap);
				}
			}

			GUILayout.EndVertical ();
		}

		#endregion
	}
}
