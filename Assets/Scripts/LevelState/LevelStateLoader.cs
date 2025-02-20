using UnityEngine;

public class LevelStateLoader : MonoBehaviour
{
    [SerializeField] LevelState levelState;

    private void Start()
    {
        LevelController.Instance.LoadLevel(levelState);
    }
}
