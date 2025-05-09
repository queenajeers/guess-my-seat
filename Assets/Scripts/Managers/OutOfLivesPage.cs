using TMPro;
using UnityEngine;

public class OutOfLivesPage : MonoBehaviour
{
    public static OutOfLivesPage Instance { get; private set; }
    public TextMeshProUGUI coinsText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        UpdateCoins();
    }

    public void ContinueClicked()
    {
        if (GameData.Coins >= 100)
        {
            GameData.Coins -= 100;
            UpdateCoins();
            UIManager.Instance.UpdateNavBarCoins();
            UIManager.Instance.ContinuePlaying();
        }
        else
        {

            UIManager.Instance.OpenShopPage();
        }
    }

    public void UpdateCoins()
    {
        coinsText.text = GameData.Coins.ToString();
    }

}
