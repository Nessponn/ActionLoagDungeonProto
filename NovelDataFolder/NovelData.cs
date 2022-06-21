using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NovelData")]
public class NovelData : ScriptableObject
{
    public Discussion_Kind discussion_kind;
    public NovelData_Novel[] Data;

    public NovelData Prev;
    public NovelData Next;
}
[System.Serializable]
public class NovelData_Novel
{
    public Person_ID person;
    [TextArea(3, 10)] public string Text;

    public float Passeage_RequireSendTime;//�P�����\��������̂ɕK�v�Ȏ���

    

    public AudioClip character_voice;//�L����������ׂ邱�Ƃɔ�������ʉ�
}
