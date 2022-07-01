using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Stage")]
public class TileStageData : ScriptableObject
{

    //�^�C���}�b�v�̑S�̃T�C�Y
    public int x_GridRange;
    public int y_GridRange;
    public List<StageDetail> Stage = new List<StageDetail>();
}

[System.Serializable]
public class StageDetail
{
    public Vector2 StageSize;
    [Range(1, 3)] public int OutlineNum = 1;
}
