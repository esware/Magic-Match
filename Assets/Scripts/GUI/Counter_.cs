using UnityEngine;
using Dev.Scripts.Targets;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class Counter_ : MonoBehaviour
{
	Text txt;
	private float lastTime;
	bool alert;

	private int levelNum;
	private TargetLevel targetLevel;

	// Use this for initialization
	void Start ()
	{
		txt = GetComponent<Text> ();
	}

	void OnEnable ()
	{
		lastTime = 0;
		alert = false;
		levelNum = PlayerPrefs.GetInt("OpenLevel");
		targetLevel = Resources.Load<TargetLevel>("Targets/Level" + levelNum);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (name == "Score") {
			txt.text = "" + LevelManager.Score;
		}
		if (name == "BestScore") {
			txt.text = "Best score:" + PlayerPrefs.GetInt ("Score" + PlayerPrefs.GetInt ("OpenLevel"));
		}

		if (name == "Limit") {
			if (LevelManager.Instance.limitType == LIMIT.MOVES) {
				txt.text = "" + LevelManager.Instance.limit;
				txt.transform.localScale = Vector3.one;
				if (LevelManager.Instance.limit <= 5) {
//					txt.color = new Color (216f / 255f, 0, 0);
//					txt.GetComponent<Outline> ().effectColor = Color.white;
					if (!alert) {
//						alert = true;
////						SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.alert);
					}

				} else {
					alert = false;
					txt.color = Color.white;
					txt.GetComponent<Outline> ().effectColor = new Color (148f / 255f, 61f / 255f, 95f / 255f);
				}

			} else {
				int minutes = Mathf.FloorToInt (LevelManager.Instance.limit / 60F);
				int seconds = Mathf.FloorToInt (LevelManager.Instance.limit - minutes * 60);
				txt.text = "" + string.Format ("{0:00}:{1:00}", minutes, seconds);
				txt.transform.localScale = Vector3.one * 0.68f;
				if (LevelManager.Instance.limit <= 30 && LevelManager.Instance.GameStatus == GameState.Playing) {
					txt.color = new Color (216f / 255f, 0, 0);
					txt.GetComponent<Outline> ().effectColor = Color.white;
					if (lastTime + 30f < Time.time) {
						lastTime = Time.time;
						SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.timeOut);
					}

				} else {
					txt.color = Color.white;
					txt.GetComponent<Outline> ().effectColor = new Color (148f / 255f, 61f / 255f, 95f / 255f);
				}

			}
		}
		if (name == "TargetBlocks") {
			txt.text = "" + LevelManager.Instance.TargetBlocks;
		}
		if (name == "TargetIngr1") {
			txt.text = "" + LevelManager.Instance.ingrCountTarget [0];
		}
		if (name == "TargetIngr2") {
			txt.text = "" + LevelManager.Instance.ingrCountTarget [1];
		}
		if (name == "Lifes") {
			txt.text = "" + InitScript.Instance.GetLife ();
		}

		if (name == "Gems") {
			txt.text = "" + InitScript.Gems;
		}
		if (name == "TargetScore") {
			txt.text = "" + LevelManager.Instance.star1;
		}
		if (name == "Level") {
			txt.text = "" + levelNum;
		}
		
		if (name == "TargetDescription1") {
			if (LevelManager.Instance.target == Target.SCORE)
				txt.text = LevelManager.Instance.targetDiscriptions [0].Replace ("%n", "" + LevelManager.Instance.star1);
			else if (LevelManager.Instance.target == Target.BLOCKS)
				txt.text = LevelManager.Instance.targetDiscriptions [1];
			else if (LevelManager.Instance.target == Target.INGREDIENT)
				txt.text = LevelManager.Instance.targetDiscriptions [2];
			else if (LevelManager.Instance.target == Target.COLLECT)
				txt.text = LevelManager.Instance.targetDiscriptions [3];

		}


	}
}
}