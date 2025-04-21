using System.Collections;
using DG.Tweening;
using UnityEngine;

public class FingerMover : MonoBehaviour
{
    public static FingerMover Instance { get; private set; }
    [SerializeField] RectTransform fingerRect;

    private void Awake()
    {
        Instance = this;
    }

    public void Animate(Vector2 from, Vector2 to)
    {
        fingerRect.position = from;
        fingerRect.DOMove(to, 1.5f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Restart);
    }

    IEnumerator StartMotion(Vector2 fromPosition, Vector2 toPosition)
    {

        yield return null;
    }


}
