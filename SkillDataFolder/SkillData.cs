using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill")]
public class SkillData : ScriptableObject
{
    public string Normal_Name;//技の名前
    [TextArea(3, 10)] public string Commentary;//技の内容の解説
    [TextArea(2, 10)] public string Skill_UsingTarget;//技の用途狙い

    public enum SkillType
    {
        ATTACK,SHIELD,HEAL,SUPPORT
    }//技の検索などで使う

    public SkillType skilltype = SkillType.ATTACK;

    public enum SkillAttribute
    {
        NOTHING,FIRE,ICE,SPARK,WIND,POISON,TIMESTOP
    }
    public SkillAttribute skillattribute = SkillAttribute.NOTHING;

    public int Power;//基礎攻撃力
    public float HitStun_Time;//基礎付与スタンタイム
    public float NockBack_Power;//基本ノックバックパワー
    public float Final_NockBack_Power;//コンボ最終段ノックバックパワー
    public float NockBack_Angle;//ノックバックの角度

    //通常段
    public float Normal_Frontglow;//発動所要時間(前隙)
    public float Normal_Collider_LifeTime;//攻撃判定持続時間
    public float Normal_Afterglow;//後隙
    public float Combo_ContinueTime;//後隙後のコンボ持続状態の維持時間
   
    //最終段
    public float Final_Frontglow;//発動所要時間(前隙)
    public float Final_Collider_LifeTime;//攻撃判定持続時間
    public float Final_Afterglow;//最終段の後隙


    public float Max_Combo = 1;//押しっぱなしで繋げられる最大コンボ数（最後に吹っ飛ばしてコンボ終了）

    public float Require_CoolTime;//次に使えるようになるまでの所要時間

    public float Deceleration_rate;//技発動時のオブジェクトの減速率
    public float Final_Deceleration_rate;//技の最終段発動時のオブジェクトの減速率

    public float ShootDistance;//射程距離（魔法）

    //以下は没にした変数

    //(以下４つの変数は　前隙、コライダー生成時間、後隙の合計で表現可能なため)
    //public float Skill_LifeTime;//スキル全体の持続時間
    //public float Final_Skill_LifeTime;//スキル全体の持続時間

    //public float Combo_LifeTime;//スキルの通常一発分の持続時間、指定時間後に攻撃がない場合はコンボが途切れる
    //public float Final_Combo_LifeTime;//スキルの通常一発分の持続時間、指定時間後に攻撃がない場合はコンボが途切れる


    //移動距離
    //移動速度（初速）
    //移動時間（持続）

    public string Advance_Name;//技の名前
    [TextArea(3, 10)] public string Commentary_A;//技の内容の解説
    public float Advance_PowerMagnitude = 1;//攻撃力増加倍率
    public float Advance_Require_StartSpan_DecreaseTime;//発動までにかかる所要時間の減少数
    public int Advance_CoolTime_DecreaseTime;//次に攻撃ができるようになるまでの所要時間の減少数

    public string Revolusion_Name;//技の名前
    [TextArea(3, 10)] public string Commentary_R;//技の内容の解説
    public float Revolusion_PowerMagnitude = 1;//攻撃力増加倍率
    public float Revolusion_Require_StartSpan_DecreaseTime;//発動までにかかる所要時間の減少数
    public int Revolusion_CoolTime_DecreaseTime;//次に攻撃ができるようになるまでの所要時間の減少数

    public string Technical_Name;//技の名前
    [TextArea(3, 10)] public string Commentary_T;//技の内容の解説
    public float Technical_PowerMagnitude = 1;//攻撃力増加倍率
    public float Technical_Require_StartSpan_DecreaseTime;//発動までにかかる所要時間の減少数
    public int Technical_CoolTime_DecreaseTime;//次に攻撃ができるようになるまでの所要時間の減少数

    public string Special_Name;//技の名前
    [TextArea(3, 10)] public string Commentary_S;//技の内容の解説
    public float Special_PowerMagnitude = 1;//攻撃力増加倍率
    public float Special_Require_StartSpan_DecreaseTime;//発動までにかかる所要時間の減少数
    public int Special_CoolTime_DecreaseTime;//次に攻撃ができるようになるまでの所要時間の減少数
}
