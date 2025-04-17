using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Transform GamePlayPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform personScrollView;
    public GameObject personItemPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnNewPerson(Sprite icon, string personName)
    {
        var personItemComp = Instantiate(personItemPrefab, personScrollView).GetComponent<PersonItem>();
        personItemComp.LoadData(personName, icon);
    }

}
