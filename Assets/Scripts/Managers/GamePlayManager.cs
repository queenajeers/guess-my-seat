using UnityEngine;

public class GamePlayManager : MonoBehaviour
{

    public static GamePlayManager Instance { get; private set; }
    int currentLevel;
    public int CurrentLevel
    {
        get { return currentLevel; }
    }

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetLevel(int level)
    {
        currentLevel = level;
    }


}
