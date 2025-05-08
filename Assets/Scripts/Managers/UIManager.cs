using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Transform personScrollView;
    [SerializeField] private GameObject personItemPrefab;
    [SerializeField] private TextMeshProUGUI seatsFilledIndicator;
    [SerializeField] private Transform seatsFilledIndicatorBG;
    [SerializeField] private GameObject winConfetti;
    [SerializeField] private GameObject gamePlayNavBar;
    [SerializeField] private GameObject gradientBarPanel;
    [SerializeField] private Animator gamePlayAnim;
    [SerializeField] private GameObject tutorialManager;

    [SerializeField] private GameObject winPage;
    [SerializeField] private GameObject outOfLivesPage;
    [SerializeField] private GameObject restartPage;
    [SerializeField] private GameObject goToMenuPage;

    [SerializeField] private List<GameObject> lives;

    [SerializeField] private GameObject complementBG;
    [SerializeField] private TextMeshProUGUI complementText;
    [SerializeField] private List<string> complements;

    [SerializeField] private TextMeshProUGUI hintsIndicator;
    [SerializeField] private TextMeshProUGUI coinsForAHintIndicator;

    [SerializeField] private GameObject useHintsForHints;
    [SerializeField] private GameObject useCoinsForHints;
    [SerializeField] private TextMeshProUGUI levelNumberIndicator;

    [SerializeField] private GameObject coinsShopPagePrefab;

    public Transform GamePlayPanel;

    private bool hintAnimationInPlay = false;
    private bool continuePlayClicked = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        coinsForAHintIndicator.text = GameData.CoinsToUseHint.ToString();
        UpdateLivesUI();
        UpdateHintsUI();
    }

    public PersonItem SpawnNewPerson(Sprite icon, string personName, Gender gender)
    {
        var personItemComp = Instantiate(personItemPrefab, personScrollView).GetComponent<PersonItem>();
        personItemComp.LoadData(personName, gender, icon);
        PeopleScrollManager.Instance.personItems.Add(personItemComp);

        return personItemComp;
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

        if (GameData.CurrentLevel == 0)
        {
            levelNumberIndicator.text = "Tutorial";
        }
        else
        {
            levelNumberIndicator.text = $"Level {GameData.CurrentLevel}";
        }

        GameData.CurrentLevel++;
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

    public void UseOneHint()
    {
        if (hintAnimationInPlay) return;

        if ((GameData.Hints > 0) || (GameData.Coins >= GameData.CoinsToUseHint))
        {
            var toBeSolvedSeatData = LevelLoader.Instance.SolveOneEligiblePersonItemsYetToBeSolved();

            if (toBeSolvedSeatData.Item1 != null)
            {
                if (GameData.Hints > 0)
                {
                    GameData.Hints--;
                }
                else
                {
                    GameData.Coins -= GameData.CoinsToUseHint;
                }

                UpdateHintsUI();
                StartCoroutine(UseOneHintCor(toBeSolvedSeatData));
            }
            else
            {
                Debug.Log("No seats to solve!");
            }
        }
        else
        {
            OpenShopPage();
        }
    }

    IEnumerator UseOneHintCor((Seat, PersonItem) toBeSolvedSeatData)
    {
        hintAnimationInPlay = true;

        yield return StartCoroutine(CameraDragMove.Instance.MoveToPosition(toBeSolvedSeatData.Item1.transform.position, .5f));

        toBeSolvedSeatData.Item2.SolveIt();
        toBeSolvedSeatData.Item1.PopUp();

        hintAnimationInPlay = false;
    }

    public void LoseLife()
    {
        int livesLeft = GameData.Lives;
        if (livesLeft > 0)
        {
            var target = lives[livesLeft - 1];
            target.transform.DOScale(0, .4f).SetEase(Ease.InBack).OnComplete(() =>
            {
                target.SetActive(false);
            });

            livesLeft--;
            GameData.Lives--;
        }

        if (livesLeft == 0)
        {
            Debug.Log("Game Over");
            OutOfLives();
        }
    }

    public void CheckForOutOfLives()
    {
        if (GameData.Lives <= 0)
        {
            OutOfLives();
        }
    }

    void OutOfLives()
    {
        outOfLivesPage.SetActive(true);
        PreventPersonItemsFromUse();
        CameraDragMove.Instance.preventPanAndZoom = true;
    }

    void PreventPersonItemsFromUse()
    {
        foreach (var item in PeopleScrollManager.Instance.personItems)
        {
            if (item != null)
            {
                item.preventFromUse = true;
            }
        }
    }

    void UpdateLivesUI()
    {
        foreach (var item in lives)
        {
            item.SetActive(false);
        }
        for (int i = 0; i < GameData.Lives; i++)
        {
            lives[i].SetActive(true);
        }
    }

    public void UpdateHintsUI()
    {
        hintsIndicator.text = GameData.Hints.ToString();
        if (GameData.Hints <= 0)
        {
            useHintsForHints.SetActive(false);
            useCoinsForHints.SetActive(true);
        }
        else
        {
            useHintsForHints.SetActive(true);
            useCoinsForHints.SetActive(false);
        }
    }

    public void ActivateRestartPage()
    {
        if (!continuePlayClicked)
        {
            LevelLoader.Instance.DeletePersistentLevelFile();
            restartPage.SetActive(true);
        }
    }

    void RefillLives()
    {
        GameData.Lives = 2;
    }

    public void GoToHomeScene()
    {
        goToMenuPage.SetActive(true);
    }

    public void ReloadCurrentScene()
    {
        RefillLives();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenuScene()
    {
        RefillLives();
        SceneManager.LoadScene(0);
    }

    public void ContinuePlaying()
    {
        var cg = outOfLivesPage.GetComponent<CanvasGroup>();
        continuePlayClicked = true;
        cg.DOFade(0f, .3f).OnComplete(() =>
        {
            outOfLivesPage.SetActive(false);
            cg.alpha = 1f;
            continuePlayClicked = false;
            ActivatePersonItemsAndCameraMovement();
        });
    }

    public void DisablePersonItems()
    {
        foreach (var item in PeopleScrollManager.Instance.personItems)
        {
            if (item != null)
            {
                item.preventFromUse = true;
            }
        }
    }

    public void EnablePersonItems()
    {
        foreach (var item in PeopleScrollManager.Instance.personItems)
        {
            if (item != null)
            {
                item.preventFromUse = false;
            }
        }
    }

    public void ActivatePersonItemsAndCameraMovement()
    {
        foreach (var item in PeopleScrollManager.Instance.personItems)
        {
            if (item != null)
            {
                item.preventFromUse = false;
            }
        }

        CameraDragMove.Instance.preventPanAndZoom = false;

        RefillLives();
        UpdateLivesUI();
        foreach (var item in lives)
        {
            item.transform.DOScale(1, .4f).SetEase(Ease.OutBack);
        }
    }

    public void SayComplement()
    {
        if (GameData.CurrentLevel == 0) return;
        complementBG.SetActive(true);
        complementText.text = complements[Random.Range(0, complements.Count)];
    }

    public void OpenShopPage()
    {
        Instantiate(coinsShopPagePrefab, canvasTransform);
    }
}
