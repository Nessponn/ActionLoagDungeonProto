using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameMasterInterFace
{
    void Player_MissCount();//プレイヤーがミスした時に実行

    Vector3 RespawnPoint_Hoge//プレイヤーのリスポーン地点を読み込みor設定するプロパティ。getで読み込み、setで設定
    {
        get;
        set;
    }
    
    IEnumerator Player_Restart();//プレイヤーがミスした後にリスポーン地点に戻すために。

    
}
