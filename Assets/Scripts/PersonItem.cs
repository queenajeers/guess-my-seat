using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class PersonItem : PersonDraggable
{
    [Space(20)]

    [Header("Person Data")]
    public string personName;
    public Image personIcon;
}
