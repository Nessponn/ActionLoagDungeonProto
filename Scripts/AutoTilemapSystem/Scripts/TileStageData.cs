using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Stage")]
public class TileStageData : ScriptableObject
{
    public List<Vector2> StagesSize;
}
