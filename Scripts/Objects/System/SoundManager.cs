using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BGMID
{


}

public enum SEID
{
    SLASH_Hit,
    SLASH_Burst,

}
public class SoundManager : SingletonMonoBehaviourFast<SoundManager>
{
    [SerializeField] BGMDataList bgmDataList = null;
    [SerializeField] SEDataList seDataList = null;

    /// <summary>
    /// ������2����ē��삳���܂�
    /// PlaySE(BGM��ID�A�炷�I�[�f�B�I�̃R���|�[�l���g�I�u�W�F�N�g)
    /// </summary>
    /// <param name="bgmID"></param>
    /// <param name="AS"></param>
    /// <param name="Duplicate"></param>
    public void PlayBGM(BGMID bgmID, AudioSource AS)
    {
        var bgmData = bgmDataList.bgmDatas.Find(b => b.bgmID == bgmID);
        if (bgmData == null)
        {
            Debug.LogError("bgm���Ȃ���");
        }
        AS.clip = bgmData.audioClip;
        AS.Play();
    }

    /// <summary>
    /// �������R����ē��삳���܂�
    /// PlaySE(���ʉ���ID�A�炷�I�[�f�B�I�̃R���|�[�l���g�I�u�W�F�N�g�A�����d�����邩)
    /// </summary>
    /// <param name="seID"></param>
    /// <param name="AS"></param>
    /// <param name="Duplicate"></param>
    public void PlaySE(SEID seID, AudioSource AS,bool Duplicate)
    {
        var seData = seDataList.seDatas.Find(s => s.seID == seID);
        if (seData == null)
        {
            Debug.LogError("se���Ȃ���");
        }

        if (Duplicate)
        {
            //�d��
            AS.PlayOneShot(seData.audioClip);
        }
        else
        {
            //���d��
            AS.clip = seData.audioClip;
            AS.Play();
        }
    }
    public void StopBGM(AudioSource AS)
    {
        AS.Stop();
    }

}
