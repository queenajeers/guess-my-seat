using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Transform GamePlayPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform personScrollView;
    public GameObject personItemPrefab;

    public TextMeshProUGUI seatsFilledIndicator;
    public Transform seatsFilledIndicatorBG;

    public GameObject winConfetti;

    public GameObject gamePlayNavBar;
    public GameObject gradientBarPanel;

    [SerializeField] Animator gamePlayAnim;

    public GameObject tutorialManager;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnNewPerson(Sprite icon, string personName)
    {
        var personItemComp = Instantiate(personItemPrefab, personScrollView).GetComponent<PersonItem>();
        personItemComp.LoadData(personName, icon);
    }

    public void SetSeatsIndicator(int seatsFilled, int totalSeats)
    {
        seatsFilledIndicator.text = $"{seatsFilled}/{totalSeats}";
        seatsFilledIndicatorBG.DOKill();
        seatsFilledIndicatorBG.DOScale(1.1f, .1f).OnComplete(() =>
        {

            seatsFilledIndicatorBG.DOScale(1f, .2f).SetEase(Ease.InBack);
        });
    }

    public void FinishActivate()
    {
        StartCoroutine(FinishActivateCor());
    }

    IEnumerator FinishActivateCor()
    {

        yield return new WaitForSeconds(.5f);
        SoundManager.Play(SoundNames.Win);
        winConfetti.SetActive(true);
    }

    public void GamePlayElementsIn()
    {
        if (GamePlayManager.Instance.CurrentLevel == 0)
        {
            gamePlayNavBar.SetActive(false);
            gradientBarPanel.SetActive(false);
            Instantiate(tutorialManager, Vector2.zero, Quaternion.identity);
        }

        gamePlayAnim.Play("GamePlayIn", 0, 0);
    }

}
