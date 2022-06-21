using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private int result_distance;
    private int result_enemy;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Get_GameData(int dis,int ene)
    {
        result_distance = dis;
        result_enemy = ene;

        //var GM = GameObject.FindWithTag("GameMaster").GetComponent<GameMaster>();
        //if (GM != null)
        //{
        //result_distance = GM.Result_distance;
        //result_enemy = GM.Result_enemy;
        // }
    }

    public int Get_distance()
    {
        return result_distance;
    }

    public int Get_Enemy()
    {
        return result_enemy;
    }
}
