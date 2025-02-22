using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextTrigger : MonoBehaviour
{
    [SerializeField] ConsequentTriggerHandler handler;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))  // Check if it's a Player
        {
            handler.NextOnTrigger();
        }
    }
}
