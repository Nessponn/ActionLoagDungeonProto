using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AttackableObjects
{
    //�R���{����
    //�_�b�V���A�^�b�N�̌㌄�A�_�b�V���A�^�b�N�̔������ԁA�I�u�W�F�N�g�̍U����������
    void DashAttackHit(float _Afterglow, float Skill_LifeTime, float Deceleration_rate);

    //�R���{��
    //�_�b�V���A�^�b�N�̌㌄�A�I�u�W�F�N�g�̍U����������
    void DashAttackHit(float _Afterglow,float Deceleration_rate);

    //�_�b�V���A�^�b�N�̌㌄�A�_�b�V���A�^�b�N�̔������ԁA�I�u�W�F�N�g�̍U����������
    void SkillShot(float _Afterglow, float Skill_LifeTime, float Deceleration_rate);
}
