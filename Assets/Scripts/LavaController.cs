using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    [SerializeField] GameDialogue lavaDialogue;
    [SerializeField] List<GameObject> tiles;
    DialogueController dialogueController;
    private bool flooded = false;
    public void StartFlood()
    {
        dialogueController.EnqueueParagraph(lavaDialogue);
        StartCoroutine(SetActiveTiles());
    }

    private IEnumerator SetActiveTiles()
    {
        foreach (GameObject tile in tiles)
        {
            tile.SetActive(true);
            yield return new WaitForSeconds(1.5f);
        }
        flooded = true;
    }
    void Start()
    {
        dialogueController = FindObjectOfType<DialogueController>();
    }

    void Update()
    {

    }

    public bool PlayerNearLava(Vector2 pos)
    {
        if (!flooded) return false;
        return 20.5f <= pos.x && pos.x <= 26.5f && pos.y <= -28.5f;
    }
}
