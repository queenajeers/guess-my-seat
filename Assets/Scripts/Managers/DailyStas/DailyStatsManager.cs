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

    [SerializeField] private Slider fill_overallFill;


    [SerializeField] private TextMeshProUGUI levelsPlayedTargetText;
    [SerializeField] private TextMeshProUGUI solvedOnFirstTryTargetText;
    [SerializeField] private TextMeshProUGUI minuitesPlayedTargetText;


    [Header("Targets")]
    [SerializeField] private int levelsPlayedTarget = 15;
    [SerializeField] private int solvedOnFirstTryTarget = 5;
    [SerializeField] private int minuitesPlayedTarget = 20;


    [SerializeField] private TextMeshProUGUI currentIQ;
    [SerializeField] private TextMeshProUGUI prevIQ;
    [SerializeField] private TextMeshProUGUI nextIQ;

    [SerializeField] private GameObject IQUpPage;
    [SerializeField] private TextMeshProUGUI newIQText;


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

    public void InitialsieUI()
    {

        currentIQ.text = $"IQ {GameData.IQ}";

        int levelsPlayed = GameData.LevelsPlayed;
        int solvedOnFirstTry = GameData.SolvedOnFirstTry;
        int minuitesPlayed = GameData.MinutesPlayed;

        levelsPlayedTargetText.text = $"{levelsPlayed}/{levelsPlayedTarget}";
        solvedOnFirstTryTargetText.text = $"{solvedOnFirstTry}/{solvedOnFirstTryTarget}";
        minuitesPlayedTargetText.text = $"{minuitesPlayed}/{minuitesPlayedTarget}";
        prevIQ.text = GameData.IQ.ToString();
        nextIQ.text = (GameData.IQ + 1).ToString();

        StartCoroutine(FillMeters());

    }

    IEnumerator FillMeters()
    {
        // Previous values
        int prevLevelsPlayed = GameData.PreviousLevelsPlayed;
        int prevSolvedOnFirstTry = GameData.PreviousSolvedOnFirstTry;
        int prevMinutesPlayed = GameData.PreviousMinutesPlayed;

        // Initial fill amounts based on previous values
        float startFillLevels = Mathf.Clamp01((float)prevLevelsPlayed / levelsPlayedTarget);
        float startFillFirstTry = Mathf.Clamp01((float)prevSolvedOnFirstTry / solvedOnFirstTryTarget);
        float startFillMinutes = Mathf.Clamp01((float)prevMinutesPlayed / minuitesPlayedTarget);
        float startFillOverall = (startFillLevels + startFillFirstTry + startFillMinutes) / 3f;

        // ⬇️ Set initial fill values (before waiting and animating)
        fill_levelsPlayed.fillAmount = startFillLevels;
        fill_solvedOnFirstTry.fillAmount = startFillFirstTry;
        fill_minuitesPlayed.fillAmount = startFillMinutes;
        fill_overallFill.value = startFillOverall;

        // Small delay to allow user to see initial state
        yield return new WaitForSeconds(0.5f);

        float duration = 1f;
        float elapsed = 0f;

        // Current values
        int levelsPlayed = GameData.LevelsPlayed;
        int solvedOnFirstTry = GameData.SolvedOnFirstTry;
        int minutesPlayed = GameData.MinutesPlayed;

        float endFillLevels = Mathf.Clamp01((float)levelsPlayed / levelsPlayedTarget);
        float endFillFirstTry = Mathf.Clamp01((float)solvedOnFirstTry / solvedOnFirstTryTarget);
        float endFillMinutes = Mathf.Clamp01((float)minutesPlayed / minuitesPlayedTarget);
        float endFillOverall = (endFillLevels + endFillFirstTry + endFillMinutes) / 3f;

        if (endFillOverall >= 1f)
        {
            endFillOverall = 1f;
            newIQText.text = $"{GameData.IQ + 1}";
            GameData.IQ++;
            GameData.TimeSpentInSeconds = 0;
            GameData.SolvedOnFirstTry = 0;
            GameData.LevelsPlayed = 0;
            GameData.MinutesPlayed = 0;
        }

        if (endFillOverall > startFillOverall)
        {
            transform.DOScale(1.05f, 1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                transform.DOScale(1f, .2f).SetEase(Ease.OutBack);
            });
        }

        // Animate from previous (start) to current (end)
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = EaseOutCubic(t);

            fill_levelsPlayed.fillAmount = Mathf.Lerp(startFillLevels, endFillLevels, easedT);
            fill_solvedOnFirstTry.fillAmount = Mathf.Lerp(startFillFirstTry, endFillFirstTry, easedT);
            fill_minuitesPlayed.fillAmount = Mathf.Lerp(startFillMinutes, endFillMinutes, easedT);
            fill_overallFill.value = Mathf.Lerp(startFillOverall, endFillOverall, easedT);

            yield return null;
        }

        // Snap to final values
        fill_levelsPlayed.fillAmount = endFillLevels;
        fill_solvedOnFirstTry.fillAmount = endFillFirstTry;
        fill_minuitesPlayed.fillAmount = endFillMinutes;
        fill_overallFill.value = endFillOverall;

        if (endFillOverall >= 1f)
        {
            IQUpPage.SetActive(true);
        }

        // Save current as previous
        GameData.PreviousLevelsPlayed = levelsPlayed;
        GameData.PreviousSolvedOnFirstTry = solvedOnFirstTry;
        GameData.PreviousMinutesPlayed = minutesPlayed;
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

        SoundManager.Play(SoundNames.Click, 1f);
    }
    void CloseInfoPanel()
    {
        infoPanelAnimator.Play("Close", 0, 0);
        infoButtonImage.sprite = infoSprite;

        SoundManager.Play(SoundNames.Click, 1f);
    }

}
