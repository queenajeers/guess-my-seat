using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class LettersAnimation : MonoBehaviour
{
    public List<TextMeshProUGUI> letters;

    void OnEnable()
    {
        SoundManager.Play(SoundNames.Win2);
        foreach (var letter in letters)
        {
            letter.transform.localScale = Vector2.zero;
        }

        for (int i = 0; i < letters.Count; i++)
        {
            int index = i; // Capture the index to avoid closure issues
            float appearDelay = 0.4f + (index * 0.06f);
            float disappearDelay = 0.4f + ((letters.Count - 1 - index) * 0.06f);

            letters[index].transform.DOScale(1, 0.3f)
                .SetDelay(appearDelay)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    letters[index].transform.DOScale(0, 0.3f)
                        .SetDelay(disappearDelay)
                        .SetEase(Ease.InBack);
                });
        }

    }

}
