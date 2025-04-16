using UnityEngine;

public class Seat : MonoBehaviour
{
    public SpriteRenderer mySeatSR;
    public Canvas worldCanvas;

    public Vector2 SeatingPos
    {
        get { return mySeatSR.transform.position; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


}
