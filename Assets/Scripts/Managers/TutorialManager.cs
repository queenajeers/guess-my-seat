using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public SpriteRenderer maskBGSpriteRenderer;
    public SpriteRenderer borderSpriteRenderer1;
    public SpriteRenderer borderSpriteRenderer2;
    public GameObject mask1;
    public GameObject mask2;
    public SpriteRenderer instBG;

    [TextArea(10, 20)]
    public List<string> instructions;

    Dictionary<string, Seat> seatsByName = new Dictionary<string, Seat>();
    Dictionary<string, PersonItem> personItemsByName = new Dictionary<string, PersonItem>();

    [SerializeField] TextMeshPro instruction;

    [SerializeField] GameObject blackBG;

    public void Start()
    {
        StartCoroutine(TutorialCor());
    }

    IEnumerator TutorialCor()
    {
        float offset = SafeAreaUtils.GetSafeAreaTopOffsetWorld(Camera.main, Screen.safeArea);


        yield return new WaitForSeconds(.5f);
        CameraDragMove.Instance.preventPanAndZoom = true;
        FadeIn(maskBGSpriteRenderer, .95f);
        LoadWorldSeats();
        LoadPersonItems();
        mask1.SetActive(true);
        mask1.transform.position = seatsByName["Ricky"].transform.position;
        FadeIn(borderSpriteRenderer1, 1f);

        mask2.SetActive(true);
        mask2.transform.position = seatsByName["Nicky"].transform.position;
        FadeIn(borderSpriteRenderer2, 1f);

        FadeIn(instBG, 1f);
        LoadInstruction(instructions[0]);

        instBG.transform.position = VptoWP(0.5f, 1f) - new Vector2(0, .6f) - new Vector2(0, offset);
        seatsByName["Nicky"].holdSeat = false;
        personItemsByName["Nicky"].preventFromUse = false;
        FingerMover.Instance.ActivateFinger();
        FingerMover.Instance.Animate(personItemsByName["Nicky"].GetSP(), seatsByName["Nicky"].GetSP());

        while (!seatsByName["Nicky"].isPlaced)
        {
            if (personItemsByName["Nicky"].isDragging || personItemsByName["Nicky"].targetFound)
            {
                FingerMover.Instance.DeActivateFinger();
            }
            else if (!FingerMover.Instance.IsFingerActive)
            {
                FingerMover.Instance.ActivateFinger();
                FingerMover.Instance.Animate();
            }

            yield return null;
        }

        seatsByName["Lucky"].holdSeat = false;
        personItemsByName["Lucky"].preventFromUse = false;
        FingerMover.Instance.DeActivateFinger();
        mask1.transform.position = seatsByName["Lucky"].transform.position;
        FadeIn(borderSpriteRenderer1, 1f);
        instruction.transform.DOScale(1.1f, 0.3f).OnComplete((() =>
        {
            instruction.transform.DOScale(1f, 0.2f);

        }));

        LoadInstruction(instructions[1]);
        FadeIn(instruction, 1f);
        yield return new WaitForSeconds(.1f);
        FingerMover.Instance.ActivateFinger();
        FingerMover.Instance.Animate(personItemsByName["Lucky"].GetSP(), seatsByName["Lucky"].GetSP());

        while (!seatsByName["Lucky"].isPlaced)
        {
            if (personItemsByName["Lucky"].isDragging || personItemsByName["Lucky"].targetFound)
            {
                FingerMover.Instance.DeActivateFinger();
            }
            else if (!FingerMover.Instance.IsFingerActive)
            {
                FingerMover.Instance.ActivateFinger();
                FingerMover.Instance.Animate();
            }

            yield return null;
        }

        seatsByName["Vicky"].holdSeat = false;
        personItemsByName["Vicky"].preventFromUse = false;
        FingerMover.Instance.DeActivateFinger();
        mask1.transform.position = seatsByName["Vicky"].transform.position;
        FadeIn(borderSpriteRenderer1, 1f);
        instruction.transform.DOScale(1.1f, 0.3f).OnComplete((() =>
        {
            instruction.transform.DOScale(1f, 0.2f);

        }));

        LoadInstruction(instructions[2]);
        FadeIn(instruction, 1f);
        yield return new WaitForSeconds(.1f);
        FingerMover.Instance.ActivateFinger();
        FingerMover.Instance.Animate(personItemsByName["Vicky"].GetSP(), seatsByName["Vicky"].GetSP());

        while (!seatsByName["Vicky"].isPlaced)
        {
            if (personItemsByName["Vicky"].isDragging || personItemsByName["Vicky"].targetFound)
            {
                FingerMover.Instance.DeActivateFinger();
            }
            else if (!FingerMover.Instance.IsFingerActive)
            {
                FingerMover.Instance.ActivateFinger();
                FingerMover.Instance.Animate();
            }

            yield return null;
        }

        mask1.SetActive(false);
        mask2.SetActive(false);
        instBG.gameObject.SetActive(false);
        blackBG.SetActive(false);

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
            item.holdSeat = true;
            seatsByName[item.PersonName] = item;
        }
    }

    void LoadPersonItems()
    {
        foreach (var item in FindObjectsByType<PersonItem>(FindObjectsSortMode.None))
        {
            item.preventFromUse = true;
            personItemsByName[item.PersonName] = item;
        }
    }

    Vector2 VptoWP(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new(x, y, 0));
    }
}
