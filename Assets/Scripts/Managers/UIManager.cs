using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


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
    public GameObject outOfLivesPage;


    public GameObject restartPage;
    public GameObject goToMenuPage;

    public List<GameObject> lives;

    public GameObject complementBG;
    public TextMeshProUGUI complementText;
    public List<string> complements;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateLivesUI();
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

    bool hintAnimationInPlay = false;
    public void UseOneHint()
    {
        if (hintAnimationInPlay) return;


        var toBeSolvedSeatData = LevelLoader.Instance.SolveOneEligiblePersonItemsYetToBeSolved();

        if (toBeSolvedSeatData.Item1 != null)
        {
            StartCoroutine(UseOneHintCor(toBeSolvedSeatData));
        }
        else
        {
            Debug.Log("No seats to solve!");
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
        int livesLeft = LevelLoader.Instance.Lives;
        if (livesLeft > 0)
        {
            var target = lives[livesLeft - 1];
            target.transform.DOScale(0, .4f).SetEase(Ease.InBack).OnComplete(() =>
            {
                target.SetActive(false);
            });

            livesLeft--;
            LevelLoader.Instance.Lives--;
        }

        if (livesLeft == 0)
        {
            Debug.Log("Game Over");
            OutOfLives();
        }
    }

    public void CheckForOutOfLives()
    {
        if (LevelLoader.Instance.Lives <= 0)
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
        for (int i = 0; i < LevelLoader.Instance.Lives; i++)
        {
            lives[i].SetActive(true);
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
        LevelLoader.Instance.Lives = 2;
    }

    public void GoToHomeScene()
    {
        goToMenuPage.SetActive(true);
    }

    public void ReloadCurrentScene()
    {
        RefillLives();
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name);
    }
    public void LoadMenuScene()
    {
        RefillLives();
        SceneManager.LoadScene(0);
    }

    bool continuePlayClicked = false;

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
        complementBG.SetActive(true);
        complementText.text = complements[Random.Range(0, complements.Count)];

    }

}
