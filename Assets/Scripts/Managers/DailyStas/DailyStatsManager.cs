using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyStatsManager : MonoBehaviour
{
    [SerializeField] private Image infoButtonImage;
    [SerializeField] private Sprite infoSprite;
    [SerializeField] private Sprite closeSprite;

    [SerializeField] private Animator infoPanelAnimator;

    [Header("FillColors")]
    [SerializeField] List<Image> images_1;
    [SerializeField] List<Image> images_2;
    [SerializeField] List<Image> images_3;

    [SerializeField] Color fillColor_1;
    [SerializeField] Color fillColor_2;
    [SerializeField] Color fillColor_3;




    [Header("References")]
    [SerializeField] private Image fill_levelsPlayed;
    [SerializeField] private Image fill_solvedOnFirstTry;
    [SerializeField] private Image fill_minuitesPlayed;

    [SerializeField] private TextMeshProUGUI levelsPlayedTargetText;
    [SerializeField] private TextMeshProUGUI solvedOnFirstTryTargetText;
    [SerializeField] private TextMeshProUGUI minuitesPlayedTargetText;


    [Header("Targets")]
    [SerializeField] private int levelsPlayedTarget = 15;
    [SerializeField] private int solvedOnFirstTryTarget = 5;
    [SerializeField] private int minuitesPlayedTarget = 20;

    void Start()
    {
        FillColors();
        InitialsieUI();
    }

    void FillColors()
    {
        foreach (Image image in images_1)
        {
            image.color = fillColor_1;
        }
        foreach (Image image in images_2)
        {
            image.color = fillColor_2;
        }
        foreach (Image image in images_3)
        {
            image.color = fillColor_3;
        }
    }

    void InitialsieUI()
    {
        int levelsPlayed = GameData.LevelsPlayed;
        int solvedOnFirstTry = GameData.SolvedOnFirstTry;
        int minuitesPlayed = GameData.MinutesPlayed;

        levelsPlayedTargetText.text = $"{levelsPlayed}/{levelsPlayedTarget}";
        solvedOnFirstTryTargetText.text = $"{solvedOnFirstTry}/{solvedOnFirstTryTarget}";
        minuitesPlayedTargetText.text = $"{minuitesPlayed}/{minuitesPlayedTarget}";
        StartCoroutine(FillMeters());

    }

    IEnumerator FillMeters()
    {
        yield return new WaitForSeconds(0.5f); // Wait for the UI to be visible

        transform.DOScale(1.05f, 0.14f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        });

        float duration = 2.0f; // Duration of the animation
        float elapsed = 0f;

        int levelsPlayed = GameData.LevelsPlayed;
        int solvedOnFirstTry = GameData.SolvedOnFirstTry;
        int minutesPlayed = GameData.MinutesPlayed;

        float targetFillLevels = Mathf.Clamp01((float)levelsPlayed / levelsPlayedTarget);
        float targetFillFirstTry = Mathf.Clamp01((float)solvedOnFirstTry / solvedOnFirstTryTarget);
        float targetFillMinutes = Mathf.Clamp01((float)minutesPlayed / minuitesPlayedTarget);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = EaseOutCubic(t);

            fill_levelsPlayed.fillAmount = easedT * targetFillLevels;
            fill_solvedOnFirstTry.fillAmount = easedT * targetFillFirstTry;
            fill_minuitesPlayed.fillAmount = easedT * targetFillMinutes;

            yield return null;
        }

        // Snap to final values
        fill_levelsPlayed.fillAmount = targetFillLevels;
        fill_solvedOnFirstTry.fillAmount = targetFillFirstTry;
        fill_minuitesPlayed.fillAmount = targetFillMinutes;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }




    public void InfoClicked()
    {
        Debug.Log("Info button clicked.");

        if (infoPanelAnimator.gameObject.activeInHierarchy)
        {
            Debug.Log("Info Panel is active, closing it.");
            CloseInfoPanel();
        }
        else
        {
            Debug.Log("Info Panel is not active, opening it.");
            OpenInfoPanel();
        }
    }

    void OpenInfoPanel()
    {
        infoPanelAnimator.gameObject.SetActive(true);
        infoPanelAnimator.Play("Open", 0, 0);
        infoButtonImage.sprite = closeSprite;
    }
    void CloseInfoPanel()
    {
        infoPanelAnimator.Play("Close", 0, 0);
        infoButtonImage.sprite = infoSprite;
    }

}
