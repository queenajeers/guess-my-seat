using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{

    public SpriteRenderer maskBGSpriteRenderer;
    public SpriteRenderer borderSpriteRenderer;
    public GameObject mask;

    public List<string> namesByOrder;
    public int currentNameOrder;

    Dictionary<string, Seat> seatsByName = new Dictionary<string, Seat>();

    public void Start()
    {
        StartCoroutine(TutorialCor());
    }

    IEnumerator TutorialCor()
    {
        yield return new WaitForSeconds(1f);
        FadeIn(maskBGSpriteRenderer, .4f);
        LoadWorldSeats();
        mask.SetActive(true);
        mask.transform.position = seatsByName[namesByOrder[currentNameOrder]].transform.position;
        // maskBGSpriteRenderer.DOColor(color, .4f);
        // borderSpriteRenderer.DOColor(color, .4f);

        yield return null;
    }

    void FadeIn(SpriteRenderer spriteRenderer, float targetAlpha)
    {
        Color c = spriteRenderer.color;

        c.a = 0f;
        spriteRenderer.color = c;
        c.a = targetAlpha;

        spriteRenderer.DOColor(c, .4f);

    }


    void LoadWorldSeats()
    {
        foreach (var item in FindObjectsByType<Seat>(FindObjectsSortMode.None))
        {
            seatsByName[item.PersonName] = item;
        }
    }
}
