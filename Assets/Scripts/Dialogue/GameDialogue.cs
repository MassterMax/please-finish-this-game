using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NewDialogueContainer")]
public class GameDialogue : ScriptableObject
{
    [TextArea(5, 10)]
    public string[] paragraphs;
    public bool force = false;
    public bool randomParagraph = false;
}
