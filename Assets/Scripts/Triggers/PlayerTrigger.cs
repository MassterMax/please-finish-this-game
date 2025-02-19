using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] BaseTriggerHandler handler;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))  // Check if it's a Player
        {
            handler.OnTrigger();
        }
    }
}
