using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class PrePlay : MonoBehaviour {
    public GameObject ingrObject;
    public GameObject blocksObject;
    public GameObject scoreTargetObject;

	// Use this for initialization
	void OnEnable () {
        InitTargets();
	}

    void InitTargets()
    {
        blocksObject.SetActive(false);
        ingrObject.SetActive(false);
        scoreTargetObject.SetActive(false);
        GameObject ingr1 = ingrObject.transform.Find("Ingredient1").gameObject;
        GameObject ingr2 = ingrObject.transform.Find("Ingredient2").gameObject;

        ingr1.SetActive(true);
        ingr2.SetActive(true);
        ingr1.GetComponent<RectTransform>().localPosition = new Vector3(-74.37f, ingr1.GetComponent<RectTransform>().localPosition.y, ingr1.GetComponent<RectTransform>().localPosition.z);
        ingr2.GetComponent<RectTransform>().localPosition = new Vector3(50.1f, ingr2.GetComponent<RectTransform>().localPosition.y, ingr2.GetComponent<RectTransform>().localPosition.z);

        if (LevelManager.Instance.ingrCountTarget[0] == 0 && LevelManager.Instance.ingrCountTarget[1] == 0) ingrObject.SetActive(false);
        else if (LevelManager.Instance.ingrCountTarget[0] > 0 || LevelManager.Instance.ingrCountTarget[1] > 0)
        {
            blocksObject.SetActive(false);
            ingrObject.SetActive(true);
            ingr1 = ingrObject.transform.Find("Ingredient1").gameObject;
            ingr2 = ingrObject.transform.Find("Ingredient2").gameObject;
            if (LevelManager.Instance.target == Target.INGREDIENT)
            {
                if (LevelManager.Instance.ingrCountTarget[0] > 0 && LevelManager.Instance.ingrCountTarget[1] > 0 && LevelManager.Instance.ingrTarget[0] == LevelManager.Instance.ingrTarget[1])
                {
                    LevelManager.Instance.ingrCountTarget[0] += LevelManager.Instance.ingrCountTarget[1];
                    LevelManager.Instance.ingrCountTarget[1] = 0;
                    LevelManager.Instance.ingrTarget[1] = Ingredients.None;
                }
                ingr1.GetComponent<Image>().sprite = LevelManager.Instance.ingrediendSprites[(int)LevelManager.Instance.ingrTarget[0]];
                ingr2.GetComponent<Image>().sprite = LevelManager.Instance.ingrediendSprites[(int)LevelManager.Instance.ingrTarget[1]];
            }
            else if (LevelManager.Instance.target == Target.COLLECT)
            {
                if (LevelManager.Instance.ingrCountTarget[0] > 0 && LevelManager.Instance.ingrCountTarget[1] > 0 && LevelManager.Instance.collectItems[0] == LevelManager.Instance.collectItems[1])
                {
                    LevelManager.Instance.ingrCountTarget[0] += LevelManager.Instance.ingrCountTarget[1];
                    LevelManager.Instance.ingrCountTarget[1] = 0;
                    LevelManager.Instance.collectItems[1] = CollectItems.None;
                }
                ingr1.GetComponent<Image>().sprite = LevelManager.Instance.ingrediendSprites[(int)LevelManager.Instance.collectItems[0] + 2];
                ingr2.GetComponent<Image>().sprite = LevelManager.Instance.ingrediendSprites[(int)LevelManager.Instance.collectItems[1]+2];

            }
            if (LevelManager.Instance.ingrCountTarget[0] == 0 && LevelManager.Instance.ingrCountTarget[1] > 0)
            {
                ingr1.SetActive(false);
                ingr2.GetComponent<RectTransform>().localPosition = new Vector3(0, ingr2.GetComponent<RectTransform>().localPosition.y, ingr2.GetComponent<RectTransform>().localPosition.z);
            }
            else if (LevelManager.Instance.ingrCountTarget[0] > 0 && LevelManager.Instance.ingrCountTarget[1] == 0)
            {
                ingr2.SetActive(false);
                ingr1.GetComponent<RectTransform>().localPosition = new Vector3(0, ingr1.GetComponent<RectTransform>().localPosition.y, ingr1.GetComponent<RectTransform>().localPosition.z);
            }
        }
        if (LevelManager.Instance.targetBlocks > 0)
        {
            blocksObject.SetActive(true);
        }
        else if (LevelManager.Instance.ingrCountTarget[0] == 0 && LevelManager.Instance.ingrCountTarget[1] == 0)
        {
            ingrObject.SetActive(false);
            blocksObject.SetActive(false);
            scoreTargetObject.SetActive(true);
        }
    }
}
}