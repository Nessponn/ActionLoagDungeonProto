using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[Serializable]
[CreateAssetMenu(menuName = "EnemyScriptable/Create EnemyData")]
public class EnemyData : ScriptableObject
{
	/*
	public string enemyName;
	public int maxHp;
	public int atk;
	public int def;
	public int exp;
	public int gold;
	*/

	//MonoBehaviourを継承したクラスのように、クラスをSerializableしてできるようなやつじゃあない


	//敵ステータスの設定(HP、リスポーン状態、）
	[SerializeField] public int HP = 1;
	[SerializeField] public bool Respawn;//やられてもリスポーン地点に再びカメラが入れば


	public Animation Animpatch;//Animatorでアニメーションさせる敵のアニメーションデータ

	public Sprite Image;//エディターに表示するスプライト
	public float Scale;//表示するときの倍率


	//敵がどう行動するかの選択

	//移動するか
	public bool move;

	//移動するとしたら、どんなときにどのくらい動くか
	public float Speed;
	public float Speed_InAir;
	public float Speed_OnIce;

	public float MaxSpeed;

	//ジャンプするか
	public bool Jump;

	public int JumpPower;


	//アニメーションクリップを参照し、あったら敵のオブジェクトにAnimatorコンポーネントをくっつける、

	public bool Animation;

	public AnimationClip _move;
	public AnimationClip _jump;
	//着地した時にちょっと加速するか
	//

	//がつがつ攻める
	//リスポーン地点でジャンプしまくる
	//
	//

	//特定の地点に来るまで待つか（眠り動作を入れ、イベントを受け入れるメソッドも作成しておく）



	//基本的な当たり判定
	public float Circleoffset_y;
	public float radius;

	//床の判定で着地を行う場合はこのboolをOnにする
	public bool BoxCol;//着地判定を有効にするか（接地できるようにする）

	public Vector2 Boxsize;
	public Vector2 Boxoffset;

	//敵リジッドボディーの設定（質量、重力、コンスタント状態、コンスタントの解除条件）
	public float Mass;
	public float Gravity;

	//コンスタント設定
	public bool constant_x;
	public bool constant_y;
	public bool constant_z;

	//プレイヤーのサーチ範囲に入ったら解除する

	public bool PlayerRangeWithconstant_x;
	public bool PlayerRangeWithconstant_y;
	public bool PlayerRangeWithconstant_z;


	//対プレイヤー設定
	public int PlayerDamage;//ダメージ量（最大４。４は即死、大抵１）
	public float PlayerSearchRange;//プレイヤーのサーチ範囲
	public bool PlayerSpinPenetration;//スピンを貫通する
	public bool PlayerCameraOut_thisObjectDelete;//この敵オブジェクトがプレイヤーカメラから外れた時、消えるようにするか
}
