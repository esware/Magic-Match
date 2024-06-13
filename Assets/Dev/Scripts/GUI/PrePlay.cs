using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class PrePlay : MonoBehaviour 
    {
        public GameObject ingrObject;
        public GameObject blocksObject;
        public GameObject scoreTargetObject;
        
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

            if (GameManager.Instance.ingrCountTarget[0] == 0 && GameManager.Instance.ingrCountTarget[1] == 0) ingrObject.SetActive(false);
            else if (GameManager.Instance.ingrCountTarget[0] > 0 || GameManager.Instance.ingrCountTarget[1] > 0)
            {
                blocksObject.SetActive(false);
                ingrObject.SetActive(true);
                ingr1 = ingrObject.transform.Find("Ingredient1").gameObject;
                ingr2 = ingrObject.transform.Find("Ingredient2").gameObject;
                if (GameManager.Instance.target == Target.INGREDIENT)
                {
                    if (GameManager.Instance.ingrCountTarget[0] > 0 && GameManager.Instance.ingrCountTarget[1] > 0 && GameManager.Instance.ingrTarget[0] == GameManager.Instance.ingrTarget[1])
                    {
                        GameManager.Instance.ingrCountTarget[0] += GameManager.Instance.ingrCountTarget[1];
                        GameManager.Instance.ingrCountTarget[1] = 0;
                        GameManager.Instance.ingrTarget[1] = Ingredients.None;
                    }
                    ingr1.GetComponent<Image>().sprite = GameManager.Instance.ingrediendSprites[(int)GameManager.Instance.ingrTarget[0]];
                    ingr2.GetComponent<Image>().sprite = GameManager.Instance.ingrediendSprites[(int)GameManager.Instance.ingrTarget[1]];
                }
                else if (GameManager.Instance.target == Target.COLLECT)
                {
                    if (GameManager.Instance.ingrCountTarget[0] > 0 && GameManager.Instance.ingrCountTarget[1] > 0 && GameManager.Instance.collectItems[0] == GameManager.Instance.collectItems[1])
                    {
                        GameManager.Instance.ingrCountTarget[0] += GameManager.Instance.ingrCountTarget[1];
                        GameManager.Instance.ingrCountTarget[1] = 0;
                        GameManager.Instance.collectItems[1] = CollectItems.None;
                    }
                    ingr1.GetComponent<Image>().sprite = GameManager.Instance.ingrediendSprites[(int)GameManager.Instance.collectItems[0] + 2];
                    ingr2.GetComponent<Image>().sprite = GameManager.Instance.ingrediendSprites[(int)GameManager.Instance.collectItems[1]+2];

                }
                if (GameManager.Instance.ingrCountTarget[0] == 0 && GameManager.Instance.ingrCountTarget[1] > 0)
                {
                    ingr1.SetActive(false);
                    ingr2.GetComponent<RectTransform>().localPosition = new Vector3(0, ingr2.GetComponent<RectTransform>().localPosition.y, ingr2.GetComponent<RectTransform>().localPosition.z);
                }
                else if (GameManager.Instance.ingrCountTarget[0] > 0 && GameManager.Instance.ingrCountTarget[1] == 0)
                {
                    ingr2.SetActive(false);
                    ingr1.GetComponent<RectTransform>().localPosition = new Vector3(0, ingr1.GetComponent<RectTransform>().localPosition.y, ingr1.GetComponent<RectTransform>().localPosition.z);
                }
            }
            if (GameManager.Instance.targetBlocks > 0)
            {
                blocksObject.SetActive(true);
            }
            else if (GameManager.Instance.ingrCountTarget[0] == 0 && GameManager.Instance.ingrCountTarget[1] == 0)
            {
                ingrObject.SetActive(false);
                blocksObject.SetActive(false);
                scoreTargetObject.SetActive(true);
            }
        }
    }
}