using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable 
{
    void TakeDamage(int Damage);
    //自機や敵にHPを設定できる。
    //しょぼんは基本HP1だが、アイテムなどでHPを増やした時も大丈夫である

}
