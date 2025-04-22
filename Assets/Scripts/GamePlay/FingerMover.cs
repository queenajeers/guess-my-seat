using System.Collections;
using DG.Tweening;
using UnityEngine;

public class FingerMover : MonoBehaviour
{
    public static FingerMover Instance { get; private set; }
    [SerializeField] RectTransform fingerRect;
    public bool IsFingerActive { get { return fingerRect.gameObject.activeInHierarchy; } }

    Vector2 from;
    Vector2 to;

    private void Awake()
    {
        Instance = this;
    }



    public void Animate(Vector2 from, Vector2 to)
    {
        this.from = from;
        this.to = to;
        fingerRect.DOKill();
        fingerRect.position = from;
        fingerRect.DOMove(to, 1.5f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Restart);
    }
    public void Animate()
    {
        fingerRect.DOKill();
        fingerRect.position = from;
        fingerRect.DOMove(to, 1.5f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Restart);
    }

    public void ActivateFinger()
    {
        fingerRect.gameObject.SetActive(true);
    }

    public void DeActivateFinger()
    {
        fingerRect.gameObject.SetActive(false);
    }



}
