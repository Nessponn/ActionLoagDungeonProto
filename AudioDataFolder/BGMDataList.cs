using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BGMDataList")]
public class BGMDataList : ScriptableObject
{
    public List<BGMData> bgmDatas;
}
[System.Serializable]
public class BGMData
{
    public BGMID bgmID;
    public AudioClip audioClip;
}