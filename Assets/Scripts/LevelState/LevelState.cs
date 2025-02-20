using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelState/NewLevelState")]
public class LevelState : ScriptableObject
{
    // first phase is always what object we should hide at first
    public List<LevelPhase> levelPhases;
}