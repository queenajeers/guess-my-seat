using TMPro;
using UnityEngine;


public class CoinsShopPage : MonoBehaviour
{
    public TextMeshProUGUI coinsText;
    public GameObject coinsRewardPage;
    public TextMeshProUGUI coinsRewardText;
    public TextMeshProUGUI hintsRewardText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnEnable()
    {
        coinsText.text = GameData.Coins.ToString();
    }

    public void BuyCoins(string coinsAndHints)
    {
        string[] parts = coinsAndHints.Split(',');
        int coins = int.Parse(parts[0]);
        int hints = int.Parse(parts[1]);

        Debug.Log("Coming Here!");
        GameData.Coins += coins;
        GameData.Hints += hints;
        coinsText.text = GameData.Coins.ToString();
        hintsRewardText.text = GameData.Hints.ToString();
        coinsRewardText.text = coins.ToString();
        coinsRewardPage.SetActive(true);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateNavBarCoins();
            UIManager.Instance.UpdateHintsUI();
        }
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.UpdateMenuCoins();
        }
        if (OutOfLivesPage.Instance != null)
        {
            OutOfLivesPage.Instance.UpdateCoins();
        }

        SoundManager.Play(SoundNames.Click, .6f);

    }


    public void ClosePage()
    {
        SoundManager.Play(SoundNames.Click, .6f);
        Destroy(gameObject);
    }
}
