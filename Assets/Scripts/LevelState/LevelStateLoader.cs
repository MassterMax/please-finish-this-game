using UnityEngine;

public class LevelStateLoader : MonoBehaviour
{
    [SerializeField] LevelState levelState;
    [SerializeField] GameDialogue levelStartDialogue;
    [SerializeField] float blockMoveTime;

    private void Start()
    {
        LevelController.Instance.LoadLevel(levelState, levelStartDialogue, blockMoveTime);
    }
}
