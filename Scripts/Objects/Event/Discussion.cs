using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Person_ID
{
    Anriel,
    mother,
    Dorochy,

}

public enum Discussion_Kind
{
    Discussion,
    Shop,
}

public abstract class Discussion : MonoBehaviour
{
    //会話システムのもと
    //継承したオブジェクトに近づいてプレイヤーが特定の入力をすると
    //話しかけることができる

    public NovelData data;

    protected int novelNumber;//現在表示されているテキストの番号
    public Text Noveltext;//
    public Text NameText;

    protected List<string> Text_Logs = new List<string>();//テキストのログ

    // Start is called before the first frame update
    protected virtual void Start()
    {
        StartCoroutine(Novel());
    }


    public IEnumerator Novel()
    {
        

        string noveltext = data.Data[novelNumber].Text;//入力されたテキストの入力を受け付ける
        WaitForSeconds sec = new WaitForSeconds(data.Data[novelNumber].Passeage_RequireSendTime);

        //テキストを表示する前に、テキストをすべてまっさらに
        Noveltext.text = "";
        NameText.text = "";

        for (int i = 0;i < noveltext.Length; i++)
        {
            Noveltext.text += noveltext[i];
            yield return sec;
        }

        //何か入力されるまで待つ
        yield return new WaitUntil(() => Input.anyKeyDown);
        Debug.Log(data.Data.Length);
        novelNumber++;

        //次の文章があれば、もう一度文字送り
        if(data.Data.Length > novelNumber)
        {
            StartCoroutine(Novel());
            yield break;
        }

        //次の文章はないが、次の文章データがあれば、現在のノベルデータと移し替えて再度Novelを使う
        if (data.Next)
        {
            data = data.Next;
            StartCoroutine(Novel());
        }

        novelNumber = 0;
    }
}
