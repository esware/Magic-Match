using UnityEngine;
using System.Collections;
using GameStates;
using TMPro;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class BoostIcon : MonoBehaviour
{
	public TextMeshProUGUI boostCount;
	public BoostType type;
	bool check;

	void OnEnable ()
	{
		if (name != "Main Camera")
		{
			var mapState = GameManager.Instance.GetState<Map>();
			if (mapState != null)
			{
				transform.Find("Indicator/Image/Check").gameObject.SetActive(false);
			}
		}
	}

	public void ActivateBoost ()
	{
		if (LevelManager.Instance.ActivatedBoost == this) {
			UnCheckBoost ();
			return;
		}

		if (check )
		{
			if (type == BoostType.Colorful_bomb) {
				LevelManager.Instance.boostColorfullBomb = 0;
				UnCheck();
				return;
			}
			if (type == BoostType.Packages) {
				LevelManager.Instance.boostPackage = 0;
				UnCheck();
				return;
			}
			if (type == BoostType.Stripes) {
				LevelManager.Instance.boostStriped = 0;
				UnCheck();
				return;
			}
		}

		if (IsLocked () || check || (!GameManager.Instance.GetState<Playing>() && !GameManager.Instance.GetState<Map>()))
			return;
		if (!check && BoostCount () > 0) {
			if (type != BoostType.Colorful_bomb && type != BoostType.Packages && type != BoostType.Stripes && !LevelManager.Instance.DragBlocked)
				LevelManager.Instance.ActivatedBoost = this;
			if (type == BoostType.Colorful_bomb) {
				LevelManager.Instance.boostColorfullBomb = 1;
				Check ();
			}
			if (type == BoostType.Packages) {
				LevelManager.Instance.boostPackage = 5;
				Check ();
			}
			if (type == BoostType.Stripes) {
				LevelManager.Instance.boostStriped = 5;
				Check ();
			}

		} else {
			OpenBoostShop (type);
		}
	}

	void UnCheckBoost ()
	{
		LevelManager.Instance.activatedBoost = null;
		LevelManager.Instance.UnLockBoosts ();
	}

	public void InitBoost ()
	{
		transform.Find ("Indicator/Image/Check").gameObject.SetActive (false);
		transform.Find ("Indicator/Image/Count").gameObject.SetActive (true);
		LevelManager.Instance.boostColorfullBomb = 0;
		LevelManager.Instance.boostPackage = 0;
		LevelManager.Instance.boostStriped = 0;
		check = false;
	}

	void Check ()
	{
		check = true;
		transform.Find ("Indicator/Image/Check").gameObject.SetActive (true);
		transform.Find ("Indicator/Image/Count").gameObject.SetActive (false);
		//InitScript.Instance.SpendBoost(type);
	}	
	
	void UnCheck ()//2.2.1
	{
		check = false;
		transform.Find ("Indicator/Image/Check").gameObject.SetActive (false);
		transform.Find ("Indicator/Image/Count").gameObject.SetActive (true);
	}

	public void LockBoost ()
	{
		transform.Find ("Lock").gameObject.SetActive (true);
		transform.Find ("Indicator").gameObject.SetActive (false);
	}

	public void UnLockBoost ()
	{
		transform.Find ("Lock").gameObject.SetActive (false);
		transform.Find ("Indicator").gameObject.SetActive (true);
	}

	bool IsLocked ()
	{
		return transform.Find ("Lock").gameObject.activeSelf;
	}

	int BoostCount ()
	{
		return int.Parse (boostCount.text);
	}

	public void OpenBoostShop (BoostType boosType)
	{
		SoundBase.Instance.GetComponent<AudioSource> ().PlayOneShot (SoundBase.Instance.click);
		GameObject.Find ("CanvasGlobal").transform.Find ("BoostShop").gameObject.GetComponent<BoostShop> ().SetBoost (boosType);
	}

	void ShowPlus (bool show)
	{
		transform.Find ("Indicator").Find ("Plus").gameObject.SetActive (show);
	}


	void Update ()
	{
		if (boostCount != null) {
			boostCount.text = "" + PlayerPrefs.GetInt ("" + type);
			if (!check) {
				if (BoostCount () > 0)
					ShowPlus (false);
				else
					ShowPlus (true);
			}
		}
	}
}
}