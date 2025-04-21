using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class TutorialManager : MonoBehaviour
{

    public SpriteRenderer maskBGSpriteRenderer;
    public SpriteRenderer borderSpriteRenderer;
    public GameObject mask;
    public SpriteRenderer instBG;

    public List<string> namesByOrder;
    [TextArea(10, 20)]
    public List<string> instructions;
    public int currentNameOrder;

    Dictionary<string, Seat> seatsByName = new Dictionary<string, Seat>();
    Dictionary<string, PersonItem> personItemsByName = new Dictionary<string, PersonItem>();

    [SerializeField] TextMeshPro instruction;


    public void Start()
    {
        StartCoroutine(TutorialCor());
    }

    IEnumerator TutorialCor()
    {
        yield return new WaitForSeconds(.5f);
        FadeIn(maskBGSpriteRenderer, .95f);
        LoadWorldSeats();
        LoadPersonItems();
        mask.SetActive(true);
        mask.transform.position = seatsByName[namesByOrder[currentNameOrder]].transform.position;
        FadeIn(borderSpriteRenderer, 1f);
        FadeIn(instBG, 1f);
        LoadInstruction(instructions[currentNameOrder]);
        instBG.transform.position = VptoWP(0.5f, 1f) - new Vector2(0, .6f);
        FingerMover.Instance.Animate(personItemsByName["Nicky"].GetSP(), seatsByName["Nicky"].GetSP());

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

    void LoadInstruction(string inst)
    {
        instruction.text = inst;
        FadeIn(instruction, 1f);
    }
    void FadeIn(TextMeshPro spriteRenderer, float targetAlpha)
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

    void LoadPersonItems()
    {
        foreach (var item in FindObjectsByType<PersonItem>(FindObjectsSortMode.None))
        {
            personItemsByName[item.PersonName] = item;
        }
    }

    Vector2 VptoWP(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new(x, y, 0));

    }
}
