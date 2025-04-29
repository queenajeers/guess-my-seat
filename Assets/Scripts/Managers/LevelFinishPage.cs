using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;

public class LevelFinishPage : MonoBehaviour
{
    public ParticleImage main_coins_ps;
    public ParticleImage hammers_ps;
    public ParticleImage piggy_coins_ps;

    public Transform iconCoin;
    public Transform iconPiggy;
    public Transform iconHammer;



    public void BurstResources()
    {
        int coins = Random.Range(4, 9);
        int hammers = Random.Range(4, 9);
        int piggy = Random.Range(4, 9);


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

    }

    void AddToCoin()
    {
        PopTransform(iconCoin);
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

}
