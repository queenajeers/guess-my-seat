using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Transform GamePlayPanel;
    public Transform personScrollView;
    public GameObject personItemPrefab;
    public TextMeshProUGUI seatsFilledIndicator;
    public Transform seatsFilledIndicatorBG;
    public GameObject winConfetti;
    public GameObject gamePlayNavBar;
    public GameObject gradientBarPanel;
    [SerializeField] Animator gamePlayAnim;
    public GameObject tutorialManager;

    public GameObject winPage;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnNewPerson(Sprite icon, string personName)
    {
        var personItemComp = Instantiate(personItemPrefab, personScrollView).GetComponent<PersonItem>();
        personItemComp.LoadData(personName, icon);
        PeopleScrollManager.Instance.personItems.Add(personItemComp);

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
        CameraDragMove.Instance.ZoomOut();
        winConfetti.SetActive(true);
        yield return new WaitForSeconds(.8f);
        winPage.SetActive(true);
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
    public void GamePlayElementsOut()
    {
        gamePlayAnim.Play("GamePlayOut", 0, 0);
    }

}
