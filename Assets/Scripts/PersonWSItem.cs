using TMPro;
using UnityEngine;

public class PersonWSItem : MonoBehaviour
{

    [SerializeField] SpriteRenderer mySR;
    [SerializeField] TextMeshPro myTMP;

    public void LoadData(Sprite personSprite, string nameOfPerson)
    {
        mySR.sprite = personSprite;
        myTMP.text = nameOfPerson;
    }

}
