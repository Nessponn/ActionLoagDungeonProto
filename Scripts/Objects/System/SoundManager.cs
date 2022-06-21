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
    /// 引数を2つ入れて動作させます
    /// PlaySE(BGMのID、鳴らすオーディオのコンポーネントオブジェクト)
    /// </summary>
    /// <param name="bgmID"></param>
    /// <param name="AS"></param>
    /// <param name="Duplicate"></param>
    public void PlayBGM(BGMID bgmID, AudioSource AS)
    {
        var bgmData = bgmDataList.bgmDatas.Find(b => b.bgmID == bgmID);
        if (bgmData == null)
        {
            Debug.LogError("bgmがないよ");
        }
        AS.clip = bgmData.audioClip;
        AS.Play();
    }

    /// <summary>
    /// 引数を３つ入れて動作させます
    /// PlaySE(効果音のID、鳴らすオーディオのコンポーネントオブジェクト、音が重複するか)
    /// </summary>
    /// <param name="seID"></param>
    /// <param name="AS"></param>
    /// <param name="Duplicate"></param>
    public void PlaySE(SEID seID, AudioSource AS,bool Duplicate)
    {
        var seData = seDataList.seDatas.Find(s => s.seID == seID);
        if (seData == null)
        {
            Debug.LogError("seがないよ");
        }

        if (Duplicate)
        {
            //重複
            AS.PlayOneShot(seData.audioClip);
        }
        else
        {
            //未重複
            AS.clip = seData.audioClip;
            AS.Play();
        }
    }
    public void StopBGM(AudioSource AS)
    {
        AS.Stop();
    }

}
