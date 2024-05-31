using MapScripts.Scripts;
using UnityEngine;

public class MapLevel : MonoBehaviour
{
    private Vector3 _originalScale;
    private bool _isScaled;
    public float overScale = 1.5f;
    public float clickScale = 0.9f;

    public int number;
    public bool isLocked;
    public Transform Lock;
    public Transform pathPivot;
    public Object levelScene;
    public string sceneName;

    public int starsCount;
    public Transform starsHoster;
    public Transform star1;
    public Transform star2;
    public Transform star3;

    public void Awake()
    {
        _originalScale = transform.localScale;
    }

    #region Enable click

    public void OnMouseEnter()
    {
        if (LevelsMap.Instance.GetIsClickEnabled())
            Scale(overScale);
    }

    public void OnMouseDown()
    {
        if (LevelsMap.Instance.GetIsClickEnabled())
            Scale(clickScale);
    }

    public void OnMouseExit()
    {
        if (LevelsMap.Instance.GetIsClickEnabled())
            ResetScale();
    }

    private void Scale(float scaleValue)
    {
        transform.localScale = _originalScale * scaleValue;
        _isScaled = true;
    }

    public void OnDisable()
    {
        if (LevelsMap.Instance.GetIsClickEnabled())
            ResetScale();
    }

    public void OnMouseUpAsButton()
    {
        if (LevelsMap.Instance.GetIsClickEnabled())
        {
            ResetScale();
            LevelsMap.Instance.OnLevelSelected(number);
        }
    }

    private void ResetScale()
    {
        if (_isScaled)
            transform.localScale = _originalScale;
    }

    #endregion

    public void UpdateState(int starCount, bool locked)
    {
        this.starsCount = starCount;
        UpdateStars(starCount);
        this.isLocked = locked;
        Lock.gameObject.SetActive(locked);
    }

    public void UpdateStars(int starCount)
    {
        star1.gameObject.SetActive(starCount >= 1);
        star2.gameObject.SetActive(starCount >= 2);
        star3.gameObject.SetActive(starCount >= 3);
    }
}
