using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseLevelObject : MonoBehaviour
{
    public string objectID; // Уникальный ID объекта в сцене

    private void Awake()
    {
        Debug.Log("call RegisterObject " + objectID);
        LevelController.RegisterObject(this);
    }
}
