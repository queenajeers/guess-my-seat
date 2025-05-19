using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PersonData))]
public class PersonDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PersonData personData = (PersonData)target;

        if (GUILayout.Button("Load Random Sprite"))
        {
            personData.LOADSPRITE();
            EditorUtility.SetDirty(personData); // Marks as dirty so the change is saved
        }
    }
}
