using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PersonItem : MonoBehaviour
{
    [SerializeField] Transform personTransform;
    [SerializeField] Image personIcon;
    [SerializeField] TextMeshProUGUI personName;

    [SerializeField] float selectScale = 1.2f;
    [SerializeField] float neutralScale = 1.0f;

    [SerializeField] Color selectColor;
    [SerializeField] Color neutralColor;

    [SerializeField] Image selectBG;

    [SerializeField] float tweenDuration = 0.3f;
    [SerializeField] Ease tweenEase = Ease.OutBack;

    bool isSelected;

    void Start()
    {
        Neutral();
    }

    public void Select()
    {
        isSelected = true;
        selectBG.gameObject.SetActive(true);
        selectBG.color = new Color(selectColor.r, selectColor.g, selectColor.b, selectBG.color.a);
        personName.color = selectColor;

        // Cancel any existing tween
        personTransform.DOKill();

        personTransform.DOScale(selectScale, tweenDuration).SetEase(tweenEase);
    }

    public void Neutral()
    {
        isSelected = false;
        personName.color = neutralColor;
        selectBG.gameObject.SetActive(false);

        // Cancel any existing tween
        personTransform.DOKill();

        personTransform.DOScale(neutralScale, tweenDuration).SetEase(tweenEase);
    }

    public void Clicked()
    {
        if (!isSelected)
        {
            PersonSelector.Instance.SelectThis(this);
        }
    }
}
