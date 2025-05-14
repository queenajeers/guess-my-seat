using UnityEngine;
using UnityEngine.UI;

public class DailyStatsManager : MonoBehaviour
{
    [SerializeField] private Image infoButtonImage;
    [SerializeField] private Sprite infoSprite;
    [SerializeField] private Sprite closeSprite;

    [SerializeField] private Animator infoPanelAnimator;


    public void InfoClicked()
    {
        Debug.Log("Info button clicked.");

        if (infoPanelAnimator.gameObject.activeInHierarchy)
        {
            Debug.Log("Info Panel is active, closing it.");
            CloseInfoPanel();
        }
        else
        {
            Debug.Log("Info Panel is not active, opening it.");
            OpenInfoPanel();
        }
    }

    void OpenInfoPanel()
    {
        infoPanelAnimator.gameObject.SetActive(true);
        infoPanelAnimator.Play("Open", 0, 0);
        infoButtonImage.sprite = closeSprite;
    }
    void CloseInfoPanel()
    {
        infoPanelAnimator.Play("Close", 0, 0);
        infoButtonImage.sprite = infoSprite;
    }

}
