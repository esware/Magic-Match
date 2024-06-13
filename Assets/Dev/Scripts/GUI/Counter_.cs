using System.Collections;
using UnityEngine;
using Dev.Scripts.Targets;
using GameStates;
using TMPro;

namespace Dev.Scripts.GUI
{
    public class Counter_ : MonoBehaviour
{
	private TextMeshProUGUI _text;
	private float _lastTime;
	private int _levelNum;

	private Coroutine _blinkCoroutine;
	private bool _isAnimating = false;

	void Start ()
	{
		_text = GetComponent<TextMeshProUGUI> ();
	}

	void OnEnable ()
	{
		_lastTime = 0;
		_levelNum = PlayerPrefs.GetInt(PlayerPrefsKeys.OpenLevel);
	}
	
	void Update ()
	{
		if (name == "Score") 
		{
			_text.text = "" + GameManager.Score;
		}
		if (name == "BestScore")
		{
			int currentLevel = PlayerPrefs.GetInt(PlayerPrefsKeys.OpenLevel);
			int bestScore = PlayerPrefs.GetInt(PlayerPrefsKeys.Score + currentLevel);
			_text.text = "Best score: " + bestScore;
		}

		if (name == "Limit")
        {
            if (GameManager.Instance.limitType == LIMIT.MOVES)
            {
                _text.text = "" + GameManager.Instance.limit;
                _text.transform.localScale = Vector3.one;
                if (GameManager.Instance.limit <= 5)
                {
                    if (!_isAnimating)
                    {
                        _blinkCoroutine = StartCoroutine(AnimateText());
                        _isAnimating = true;
                    }
                }
                else
                {
                    if (_isAnimating)
                    {
                        StopCoroutine(_blinkCoroutine);
                        _text.color = Color.white;
                        _text.transform.localScale = Vector3.one;
                        _isAnimating = false;
                    }
                }
            }
            else
            {
                int minutes = Mathf.FloorToInt(GameManager.Instance.limit / 60F);
                int seconds = Mathf.FloorToInt(GameManager.Instance.limit - minutes * 60);
                _text.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);
                _text.transform.localScale = Vector3.one * 0.68f;
                if (GameManager.Instance.limit <= 30 && GameManager.Instance.GetState<Playing>())
                {
                    _text.color = new Color(216f / 255f, 0, 0);
                    if (_lastTime + 30f < Time.time)
                    {
                        _lastTime = Time.time;
                        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.timeOut);
                    }
                }
                else
                {
                    _text.color = Color.white;
                }
            }
        }

		if (name == "TargetBlocks") 
		{
			_text.text = "" + GameManager.Instance.TargetBlocks;
		}
		if (name == "TargetIngr1") 
		{
			_text.text = "" + GameManager.Instance.ingrCountTarget [0];
		}
		if (name == "TargetIngr2")
		{
			_text.text = "" + GameManager.Instance.ingrCountTarget [1];
		}
		if (name == "Lifes") 
		{
			_text.text = "" + InitScript.Instance.GetLife ();
		}

		if (name == "Gems") 
		{
			_text.text = "" + InitScript.Gems;
		}
		if (name == "TargetScore") 
		{
			_text.text = "" + GameManager.Instance.star1;
		}
		if (name == "Level") 
		{
			_text.text = "" + _levelNum;
		}
		
		if (name == "TargetDescription1") 
		{
			if (GameManager.Instance.target == Target.SCORE)
				_text.text = GameManager.Instance.targetDiscriptions [0].Replace ("%n", "" + GameManager.Instance.star1);
			else if (GameManager.Instance.target == Target.BLOCKS)
				_text.text = GameManager.Instance.targetDiscriptions [1];
			else if (GameManager.Instance.target == Target.INGREDIENT)
				_text.text = GameManager.Instance.targetDiscriptions [2];
			else if (GameManager.Instance.target == Target.COLLECT)
				_text.text = GameManager.Instance.targetDiscriptions [3];

		}


	}
	
	private IEnumerator AnimateText()
	{
		while (true)
		{
			_text.color = Color.Lerp(Color.red, new Color(1, 0, 0, 0.5f), Mathf.PingPong(Time.time * 2, 1));
			
			_text.transform.localScale = Vector3.one * (1.0f + Mathf.PingPong(Time.time, 0.3f));

			yield return null;
		}
	}
	}
}