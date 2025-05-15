using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    public TextMeshProUGUI coinsIndicator;

    public Transform canvasTransform;

    public GameObject shopPagePrefab;

    public TextMeshProUGUI playButtonText;

    public GameObject takeToGamePlayPage;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateMenuCoins();
        UpdatePlayButtonText();
    }


    void UpdatePlayButtonText()
    {
        int levelPlaying = GameData.CurrentLevel;
        if (GameData.IsNewLevel(levelPlaying))
        {
            playButtonText.text = $"Play <u>Lvl.{levelPlaying}</u>";
        }
        else
        {
            playButtonText.text = $"Continue <u>Lvl.{levelPlaying}</u>";
        }
    }


    public void OpenCoinsShop()
    {
        GameObject shopPage = Instantiate(shopPagePrefab, canvasTransform);
        shopPage.transform.SetAsLastSibling();

    }

    public void UpdateMenuCoins()
    {
        coinsIndicator.text = GameData.Coins.ToString();
    }

    public void LoadGamePlay()
    {
        SceneManager.LoadScene(2);

    }

    public void TakeToGamePlay()
    {
        takeToGamePlayPage.SetActive(true);
        ClickSound();
    }

    public void ClickSound()
    {
        SoundManager.Play(SoundNames.Click, 1f);
    }
}
