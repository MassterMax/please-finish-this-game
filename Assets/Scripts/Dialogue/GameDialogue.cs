using System.Collections.Generic;
using UnityEngine;

public enum Emotions
{
    Neutral,
    Joy,
    Angry,
    Irritation,
    Confusion,
}

[CreateAssetMenu(menuName = "Dialogue/NewDialogueContainer")]
public class GameDialogue : ScriptableObject
{
    [TextArea(5, 10)]
    public string[] paragraphs;
    public Emotions[] emotions;
    public bool force = false;
    public bool randomParagraph = false;
}
