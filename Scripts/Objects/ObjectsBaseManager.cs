using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObjectsBaseManager
{
    //ここでは、当に指示や命令はせず、全ての可能動物の基盤メソッドを記述する

    //プレイヤーも敵も破壊可能なブロックも、みんなここを通して他のオブジェクトにダメージや地形効果などを受ける

    void OnDamaged(int Damage,float stoptime, float NockBack_Power, float NockBack_Angle);

    //持続ダメージ、ダメージの単発火力が高い。氷系の敵に有効
    void FIRE();
    //発動した場合、敵を一定時間動けなくする
    void FREEZE();
    //ショック中、一定確率で敵を一定時間動けなくする
    void SHOCK();
    //持続ダメージ、発動中は攻撃力が半減する。
    void POIZON();
    //持続する連続ダメージ。他の持続ダメージ系より連打力が高く、装備によっては最も火力が出る
    void WIND();
}
