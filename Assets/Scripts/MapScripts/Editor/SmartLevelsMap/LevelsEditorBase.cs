using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public abstract class LevelsEditorBase : UnityEditor.Editor
{
    protected List<MapLevel> GetMapLevels()
    {
        return FindObjectsOfType<MapLevel>().OrderBy(ml => ml.number).ToList();
    }

    protected MapLevel CreateMapLevel(Vector3 position, int number, MapLevel mapLevelPrefab)
    {
        MapLevel mapLevel = PrefabUtility.InstantiatePrefab(mapLevelPrefab) as MapLevel;
        mapLevel.transform.position = position;
        return mapLevel;
    }

    protected void UpdateLevelsNumber(List<MapLevel> mapLevels)
    {
        for (int i = 0; i < mapLevels.Count; i++)
        {
            mapLevels[i].number = i + 1;
            mapLevels[i].name = string.Format("Level{0:00}", i + 1);
        }
    }

    protected void UpdatePathWaypoints(List<MapLevel> mapLevels)
    {
        MapScripts.Scripts.Path path = FindObjectOfType<MapScripts.Scripts.Path>();
        path.waypoints.Clear();
        foreach (MapLevel mapLevel in mapLevels)
            path.waypoints.Add(mapLevel.pathPivot);
    }

    protected void SetAllMapLevelsAsDirty()
    {
        GetMapLevels().ForEach(EditorUtility.SetDirty);
    }

    protected void SetStarsEnabled(LevelsMap levelsMap, bool isEnabled)
    {
        levelsMap.SetStarsEnabled(isEnabled);
        EditorUtility.SetDirty(levelsMap);
        SetAllMapLevelsAsDirty();
    }
}