using UnityEngine;

public class PlayerPrefsMapProgressManager : IMapProgressManager
{
    private string GetLevelKey(int number)=>$"Level.{number:000}.StarsCount";
    private string GetLevelScoreKey(int number) => $"Level.{number:000}.Score";


    public int LoadLevelStarsCount(int level)
    {
        return PlayerPrefs.GetInt(GetLevelKey(level), 0);
    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        PlayerPrefs.SetInt(GetLevelKey(level), starsCount);
    }
    public int LoadLevelScoreCount(int level)
    {
        return PlayerPrefs.GetInt(GetLevelScoreKey(level), 0);
    }
    public void SaveLevelScoreCount(int level, int score)
    {
        PlayerPrefs.SetInt(GetLevelScoreKey(level),score);
    }

    public void ClearLevelProgress(int level)
    {
        PlayerPrefs.DeleteKey(GetLevelKey(level));
    }
}