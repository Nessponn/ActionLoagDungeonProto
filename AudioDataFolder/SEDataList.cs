using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEDataList")]
public class SEDataList : ScriptableObject
{
    public List<SEData> seDatas;
}
[System.Serializable]
public class SEData
{
    public SEID seID;
    public AudioClip audioClip;
}