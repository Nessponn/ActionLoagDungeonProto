using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AttackableObjects
{
    //コンボ初動
    //ダッシュアタックの後隙、ダッシュアタックの発動時間、オブジェクトの攻撃時減速率
    void DashAttackHit(float _Afterglow, float Skill_LifeTime, float Deceleration_rate);

    //コンボ中
    //ダッシュアタックの後隙、オブジェクトの攻撃時減速率
    void DashAttackHit(float _Afterglow,float Deceleration_rate);

    //ダッシュアタックの後隙、ダッシュアタックの発動時間、オブジェクトの攻撃時減速率
    void SkillShot(float _Afterglow, float Skill_LifeTime, float Deceleration_rate);
}
