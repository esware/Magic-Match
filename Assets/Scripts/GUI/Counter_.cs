using System.Collections;
using UnityEngine;
using Dev.Scripts.Targets;
using GameStates;
using TMPro;

namespace Dev.Scripts.GUI
{
    public class Counter_ : MonoBehaviour
{
	private TextMeshProUGUI txt;
	private float lastTime;
	private int levelNum;
	private Coroutine blinkCoroutine;
	private bool isAnimating = false;

	void Start ()
	{
		txt = GetComponent<TextMeshProUGUI> ();
	}

	void OnEnable ()
	{
		lastTime = 0;
		levelNum = PlayerPrefs.GetInt("OpenLevel");
	}
	
	void Update ()
	{
		if (name == "Score") 
		{
			txt.text = "" + GameManager.Score;
		}
		if (name == "BestScore")
		{
			txt.text = "Best score:" + PlayerPrefs.GetInt ("Score" + PlayerPrefs.GetInt ("OpenLevel"));
		}

		if (name == "Limit")
        {
            if (GameManager.Instance.limitType == LIMIT.MOVES)
            {
                txt.text = "" + GameManager.Instance.limit;
                txt.transform.localScale = Vector3.one;
                if (GameManager.Instance.limit <= 5)
                {
                    if (!isAnimating)
                    {
                        blinkCoroutine = StartCoroutine(AnimateText());
                        isAnimating = true;
                    }
                }
                else
                {
                    if (isAnimating)
                    {
                        StopCoroutine(blinkCoroutine);
                        txt.color = Color.white;
                        txt.transform.localScale = Vector3.one;
                        isAnimating = false;
                    }
                }
            }
            else
            {
                int minutes = Mathf.FloorToInt(GameManager.Instance.limit / 60F);
                int seconds = Mathf.FloorToInt(GameManager.Instance.limit - minutes * 60);
                txt.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);
                txt.transform.localScale = Vector3.one * 0.68f;
                if (GameManager.Instance.limit <= 30 && GameManager.Instance.GetState<Playing>())
                {
                    txt.color = new Color(216f / 255f, 0, 0);
                    if (lastTime + 30f < Time.time)
                    {
                        lastTime = Time.time;
                        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.timeOut);
                    }
                }
                else
                {
                    txt.color = Color.white;
                }
            }
        }

		if (name == "TargetBlocks") 
		{
			txt.text = "" + GameManager.Instance.TargetBlocks;
		}
		if (name == "TargetIngr1") 
		{
			txt.text = "" + GameManager.Instance.ingrCountTarget [0];
		}
		if (name == "TargetIngr2")
		{
			txt.text = "" + GameManager.Instance.ingrCountTarget [1];
		}
		if (name == "Lifes") 
		{
			txt.text = "" + InitScript.Instance.GetLife ();
		}

		if (name == "Gems") 
		{
			txt.text = "" + InitScript.Gems;
		}
		if (name == "TargetScore") 
		{
			txt.text = "" + GameManager.Instance.star1;
		}
		if (name == "Level") 
		{
			txt.text = "" + levelNum;
		}
		
		if (name == "TargetDescription1") 
		{
			if (GameManager.Instance.target == Target.SCORE)
				txt.text = GameManager.Instance.targetDiscriptions [0].Replace ("%n", "" + GameManager.Instance.star1);
			else if (GameManager.Instance.target == Target.BLOCKS)
				txt.text = GameManager.Instance.targetDiscriptions [1];
			else if (GameManager.Instance.target == Target.INGREDIENT)
				txt.text = GameManager.Instance.targetDiscriptions [2];
			else if (GameManager.Instance.target == Target.COLLECT)
				txt.text = GameManager.Instance.targetDiscriptions [3];

		}


	}
	
	private IEnumerator AnimateText()
	{
		while (true)
		{
			txt.color = Color.Lerp(Color.red, new Color(1, 0, 0, 0.5f), Mathf.PingPong(Time.time * 2, 1));
			
			txt.transform.localScale = Vector3.one * (1.0f + Mathf.PingPong(Time.time, 0.3f));

			yield return null;
		}
	}
	}
}