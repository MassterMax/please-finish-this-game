using UnityEngine;

public class LevelStateLoader : MonoBehaviour
{
    [SerializeField] GameDialogue levelStartDialogue;
    [SerializeField] float blockMoveTime;
    
    [SerializeField] GameDialogue nextSceneDialogue;
    [SerializeField] string nextSceneName;
    [SerializeField] GameObject finalPanel;

    private void Start()
    {
        LevelController.Instance.LoadLevel(levelStartDialogue, blockMoveTime, nextSceneName, nextSceneDialogue, finalPanel);
    }
}
