using UnityEngine;

namespace Dev.Scripts.Manager
{
    public enum Target
    {
        SCORE,
        COLLECT,
        INGREDIENT,
        BLOCKS
    }

    public enum LIMIT
    {
        MOVES,
        TIME
    }

    public enum Ingredients
    {
        None = 0,
        Ingredient1,
        Ingredient2
    }

    public enum CollectItems
    {
        None = 0,
        Item1,
        Item2,
        Item3,
        Item4,
        Item5,
        Item6
    }

    public enum RewardedAdsType
    {
        GetLifes,
        GetGems,
        GetGoOn
    }
    public class InitScript:MonoBehaviour
    {
        
    }
}