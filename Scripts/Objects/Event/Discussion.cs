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
    //��b�V�X�e���̂���
    //�p�������I�u�W�F�N�g�ɋ߂Â��ăv���C���[������̓��͂������
    //�b�������邱�Ƃ��ł���

    public NovelData data;

    protected int novelNumber;//���ݕ\������Ă���e�L�X�g�̔ԍ�
    public Text Noveltext;//
    public Text NameText;

    protected List<string> Text_Logs = new List<string>();//�e�L�X�g�̃��O

    // Start is called before the first frame update
    protected virtual void Start()
    {
        StartCoroutine(Novel());
    }


    public IEnumerator Novel()
    {
        

        string noveltext = data.Data[novelNumber].Text;//���͂��ꂽ�e�L�X�g�̓��͂��󂯕t����
        WaitForSeconds sec = new WaitForSeconds(data.Data[novelNumber].Passeage_RequireSendTime);

        //�e�L�X�g��\������O�ɁA�e�L�X�g�����ׂĂ܂������
        Noveltext.text = "";
        NameText.text = "";

        for (int i = 0;i < noveltext.Length; i++)
        {
            Noveltext.text += noveltext[i];
            yield return sec;
        }

        //�������͂����܂ő҂�
        yield return new WaitUntil(() => Input.anyKeyDown);
        Debug.Log(data.Data.Length);
        novelNumber++;

        //���̕��͂�����΁A������x��������
        if(data.Data.Length > novelNumber)
        {
            StartCoroutine(Novel());
            yield break;
        }

        //���̕��͂͂Ȃ����A���̕��̓f�[�^������΁A���݂̃m�x���f�[�^�ƈڂ��ւ��čēxNovel���g��
        if (data.Next)
        {
            data = data.Next;
            StartCoroutine(Novel());
        }

        novelNumber = 0;
    }
}
