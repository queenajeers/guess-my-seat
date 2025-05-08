using System.Collections;
using AssetKits.ParticleImage;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelFinishPage : MonoBehaviour
{
    public ParticleImage main_coins_ps;
    public ParticleImage hammers_ps;
    public ParticleImage piggy_coins_ps;

    public Transform iconCoin;
    public Transform iconPiggy;
    public Transform iconHammer;

    public TextMeshProUGUI coinsIndicator;

    public TextMeshProUGUI reward_coinsMainIndicator;

    int coins;
    int hammers;
    int piggy;

    void Start()
    {
        coinsIndicator.text = GameData.Coins.ToString();
        MakeRewardReady();
    }

    void MakeRewardReady()
    {
        coins = Random.Range(4, 9);
        hammers = Random.Range(4, 9);
        piggy = Random.Range(4, 9);

        reward_coinsMainIndicator.text = coins.ToString();
    }

    public void BurstResources()
    {

        for (int i = 0; i < coins; i++)
        {
            main_coins_ps.AddBurst(0f + (i * 0.14f), 1);
        }

        for (int i = 0; i < hammers; i++)
        {
            hammers_ps.AddBurst(0f + (i * 0.14f), 1);
        }

        for (int i = 0; i < piggy; i++)
        {
            piggy_coins_ps.AddBurst(0f + (i * 0.14f), 1);
        }

        main_coins_ps.gameObject.SetActive(true);
        hammers_ps.gameObject.SetActive(true);
        piggy_coins_ps.gameObject.SetActive(true);



        main_coins_ps.Play();
        hammers_ps.Play();
        piggy_coins_ps.Play();

        main_coins_ps.onAnyParticleFinished.AddListener(AddToCoin);
        hammers_ps.onAnyParticleFinished.AddListener(AddToHammer);
        piggy_coins_ps.onAnyParticleFinished.AddListener(AddToPiggy);

        main_coins_ps.onLastParticleFinished.AddListener(GoToMenuScene);



    }

    void AddToCoin()
    {
        PopTransform(iconCoin);
        GameData.Coins++;
        coinsIndicator.text = GameData.Coins.ToString();
    }
    void AddToHammer()
    {
        PopTransform(iconHammer);
    }
    void AddToPiggy()
    {
        PopTransform(iconPiggy);
    }

    void PopTransform(Transform t)
    {
        t.DOKill();
        t.transform.localScale = Vector2.one;

        t.DOShakeScale(1, .2f).SetEase(Ease.OutBack);
    }
    void GoToMenuScene()
    {
        StartCoroutine(GoToMenuSceneCor());
    }
    IEnumerator GoToMenuSceneCor()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.ActivateMenuRestartPage();
    }


}
