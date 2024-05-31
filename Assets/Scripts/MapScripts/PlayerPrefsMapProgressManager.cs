using UnityEngine;

public class PlayerPrefsMapProgressManager : IMapProgressManager
{
    private string GetLevelKey(int number)
    {
        return $"Level.{number:000}.StarsCount";
    }

    public int LoadLevelStarsCount(int level)
    {
        return PlayerPrefs.GetInt(GetLevelKey(level), 0);
    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        PlayerPrefs.SetInt(GetLevelKey(level), starsCount);
    }

    public void ClearLevelProgress(int level)
    {
        PlayerPrefs.DeleteKey(GetLevelKey(level));
    }
}