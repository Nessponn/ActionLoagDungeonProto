using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TileDamagable
{
    //たいるのHP処理
    //HPはいくつか、試験段階で付与する変化色を設定するインスペクターを用意
    //HPが１の時、2の時などでスプライトが変わる仕組みを導入する

    //Hp精度は、各タイルのスプライトをスクリプトでチェック(GetSprite)、スプライトの番号によって次に入れるSprite、またはnullにするかを決める


    void TakeDamage(Vector3 pos,int num);//タイルの位置情報取得

    void RefreshTileState();//ゲームマネージャから状態回復命令を受けた際はこのメソッドで元に戻す（boolで戻さない選択も可能にする）
    //ゲットタイルで取得したタイルをfor文でどのTileBaseと同等かをチェック。
    //チェックが出来たら、そのタイルの１個下の番号のタイルに置き換える(SetTile)。もしも最後のタイルだったらnullに置き換える(SetTile)

}
