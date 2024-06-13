
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Dev.Scripts.GUI
{
    public class PreFailed : MonoBehaviour
{
	public Sprite[] buyButtons;
	public Image buyButton;
	private int _failedCost;
	
	void OnEnable ()
	{
		_failedCost = GameManager.Instance.failedCost;
		transform.Find ("Buy/Price").GetComponent<Text> ().text = "" + _failedCost;
		if (GameManager.Instance.limitType == LIMIT.MOVES)
			buyButton.sprite = buyButtons [0];
		else if (GameManager.Instance.limitType == LIMIT.TIME)
			buyButton.sprite = buyButtons [1];
		if (!GameManager.Instance.enableInApps)
			transform.Find ("Buy").gameObject.SetActive (false);
		
		SetTargets ();
	}

	void SetTargets ()
	{
		Transform TargetCheck1 = transform.Find ("Banner/Targets/TargetCheck1");
		Transform TargetCheck2 = transform.Find ("Banner/Targets/TargetCheck2");
		Transform TargetUnCheck1 = transform.Find ("Banner/Targets/TargetUnCheck1");
		Transform TargetUnCheck2 = transform.Find ("Banner/Targets/TargetUnCheck2");
		if (GameManager.Score < GameManager.Instance.star1) {
			TargetCheck2.gameObject.SetActive (false);
			TargetUnCheck2.gameObject.SetActive (true);
		} else {
			TargetCheck2.gameObject.SetActive (true);
			TargetUnCheck2.gameObject.SetActive (false);
		}
		if (GameManager.Instance.target == Target.BLOCKS) {
			if (GameManager.Instance.TargetBlocks > 0) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		} else if (GameManager.Instance.target == Target.INGREDIENT || GameManager.Instance.target == Target.COLLECT) {
			if (GameManager.Instance.ingrCountTarget [0] > 0 || GameManager.Instance.ingrCountTarget [1] > 0) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		} else if (GameManager.Instance.target == Target.SCORE) {
			if (GameManager.Score < GameManager.Instance.star1) {
				TargetCheck1.gameObject.SetActive (false);
				TargetUnCheck1.gameObject.SetActive (true);
			} else {
				TargetCheck1.gameObject.SetActive (true);
				TargetUnCheck1.gameObject.SetActive (false);
			}
		}


	}

}
}