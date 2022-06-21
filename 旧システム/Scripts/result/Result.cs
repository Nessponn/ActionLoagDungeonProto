using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    private int result_distance;
    private int result_enemy;

    public Text Distance;
    public Text Enemy_Num;
    
    // Start is called before the first frame update
    void Start()
    {
        var Data = GameObject.FindWithTag("Data").GetComponent<DataManager>();
        if (Data != null)
        {
            result_distance = Data.Get_distance();
            result_enemy = Data.Get_Enemy();
        }

        Distance.text =  " すすんだ距離　…" + result_distance;
        Enemy_Num.text = " 倒した敵の数　…" + result_enemy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
