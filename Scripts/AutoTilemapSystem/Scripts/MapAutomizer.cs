using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//�}�b�v�I�[�g�}�C�U�[
public class MapAutomizer : MonoBehaviour
{
    /// <summary>
    /// �T�v
    /// �P�D�^�C���}�b�v�I�u�W�F�N�g���쐬���A�������鏀�����s���B�i�c����A�Ȃǁj
    /// 
    /// 
    /// </summary>
    /// 

    public GameObject TileGrid;
    public TileBase Basetile;

    public int Thickness;//�Ԋu
    //�}�b�v���Ƃ̏��i�[��
    class MapInfo
    {
        GameObject mapobj;
        MapInfo(int Width)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TileMapDataCreate(15 * Thickness, 15 * Thickness);
    }

    void TileMapDataCreate(int Width,int Height)
    {
        //�}�b�v�f�[�^�̍쐬
        GameObject mapobj = new GameObject();//���̎����͐����������s���Ă���
        mapobj.AddComponent<Tilemap>();
        mapobj.AddComponent<TilemapRenderer>();
        
        //�^�C���}�b�v�R���|�[�l���g�̎擾
        var mapdata = mapobj.GetComponent<Tilemap>();

        //�c����̐ݒ�
        mapdata.size = new Vector3Int(Width,Height,mapdata.size.z);

        //�^�C���}�b�v�̈ʒu����
        mapobj.transform.position = new Vector3(-Width / 2, -Height / 2, 0);

        //�}�b�v�̍쐬
        //MapAutoCreate(mapobj);

        //�}�b�v�z��f�[�^�̍쐬
        MapBoundCreate(mapobj,Width, Height);
    }

    //���ƂȂ�}�b�v�f�[�^���쐬
    void MapBoundCreate(GameObject mapobj,int Width,int Height)
    {
        //�}�b�v�z��̍쐬
        int[,] mapdata = new int[Width,Height];

        //�}�b�v�̈ʒu�ۑ��p
        int Count;

        //�^�C���̐ݒu����̐����J�E���g�p
        int TiledCount;

        //�}�b�v�̒��S�_�i��΂ɖ��܂�Ȃ������B�܂��͂O�Őݒ�j
        int middlepoint = 0;

        for (int y = 0;y < Height;y++)
        {//��������E��ɂ����ă^�C�����č�����

            //�J�E���g�̃��Z�b�g
            Count = 2;
            TiledCount = 0;

            for (int x = 0; x < Width; x++)
            {
                //�}�b�v�̒��S�_�Ȃ��΂��A�c��J�E���g���O�ȉ��ł���΂�
                if (x != middlepoint && Count > 0 && TiledCount <= 0)
                {
                    //�����ݒ�
                    int rad = Random.Range(1, 10);


                    //�����œ���������^�C���𖄂߂�(1/5���x)
                    if (rad <= 2)
                    {
                        mapdata[x, y] = 1;

                        //�J�E���g���}�C�i�X
                        Count--;
                        TiledCount = 2;
                    }
                    //������x���̖��[���_�ł܂��񐔕��ݒ肵�Ă��ȏꍇ�A��O�I�Ɏ����Ŗ��߂�
                    else if (Count > 0 && x >= Width - 1)
                    {
                        mapdata[x, y] = 1;

                        //�J�E���g���}�C�i�X
                        Count--;
                        TiledCount = 2;
                    }
                    //��ڂ̃^�C����x���̖�������R�ڂ̎��_�Őݒu����Ă��Ȃ��ꍇ�A��O�I�Ɏ����Ŗ��߂�
                    else if (Count > 1 && x == Width - 3)
                    {
                        mapdata[x, y] = 1;

                        //�J�E���g���}�C�i�X
                        Count--;
                        TiledCount = 2;
                    }
                }

                TiledCount--;
            }

            if (Count > 0) Debug.Log("�J�E���g�܂��c���Ă�");
        }

        MapAutoCreate(mapobj, mapdata);
    }


    void MapAutoCreate(GameObject mapobj,int[,] mapdata)
    {
        //�^�C���}�b�v�R���|�[�l���g�̎擾
        var maptile = mapobj.GetComponent<Tilemap>();

        //���������
        var Cellbound = mapobj.GetComponent<Tilemap>().cellBounds;

        //�}�b�v�f�[�^�̂̈ʒu�Q�Ɨp
        int mx, my;
        mx = my = 0;

        for (int y = Cellbound.max.y - 1 - ((Thickness - 1) / 2); y >= Cellbound.min.y; y-= Thickness)
        {//��������E��ɂ����ă^�C�����č�����

            for (int x = Cellbound.min.x ; x < Cellbound.max.x; x+= Thickness)
            {
                //�ǁi�P�j�ł���΁A���߂�
                if (mapdata[mx, my] == 1)
                {
                    //�w�蕝���P�ȏ�w�肵�Ă���΁A��������߂�
                    //for (int ey = y + ((Thickness - 1) / 2); ey >= y - ((Thickness - 1) / 2); ey--)
                    //{
                        //�Q�Ƃ���u���b�N
                        var position = new Vector3Int(x - 1 - ((Thickness - 1) / 2), y, 0);
                        maptile.SetTile(position, Basetile);
                    //}
                }

                mx++;
            }
            Debug.Log(mx);

            mx = 0;
            my++;
        }

        Init_MapData(mapobj);
    }

    //���ƂȂ�}�b�v�f�[�^���쐬�i��� MapBoundCreate�j
    void MapAutoCreate(GameObject mapobj)
    {
        ///
        /// �܂��A���P��ɕt���A�Q�̃^�C����ݒu
        ///

        //�^�C���}�b�v�R���|�[�l���g�̎擾
        var mapdata = mapobj.GetComponent<Tilemap>();

        //���������
        var Cellbound = mapobj.GetComponent<Tilemap>().cellBounds;

        //�}�b�v�̈ʒu�ۑ��p
        int Count;


        //�}�b�v�̒��S�_�i��΂ɖ��܂�Ȃ������B�܂��͂O�Őݒ�j
        int middlepoint = 0;

        for (int y = Cellbound.max.y - 1; y >= Cellbound.min.y; y --)
        {//��������E��ɂ����ă^�C�����č�����

            //�J�E���g�̃��Z�b�g
            Count = 2;

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                //�}�b�v�̒��S�_�Ȃ��΂��A�c��J�E���g���O�ȉ��ł���΂�
                if(x != middlepoint && Count > 0)
                {
                    //�����ݒ�
                    int rad = Random.Range(1, 10);


                    //�����œ���������^�C���𖄂߂�(1/5���x)
                    if(rad <= 2)
                    {
                        var position = new Vector3Int(x, y, 0);
                        mapdata.SetTile(position, Basetile);

                        //�J�E���g���}�C�i�X
                        Count--;

                    }
                    //������x���̖��[���_�ł܂��񐔕��ݒ肵�Ă��ȏꍇ�A��O�I�Ɏ����Ŗ��߂�
                    else if (Count > 0 && x == Cellbound.max.x - 1)
                    {
                        var position = new Vector3Int(x, y, 0);
                        mapdata.SetTile(position, Basetile);

                        //�J�E���g���}�C�i�X
                        Count--;
                    }
                    //��ڂ̃^�C����x���̖�������R�ڂ̎��_�Őݒu����Ă��Ȃ��ꍇ�A��O�I�Ɏ����Ŗ��߂�
                    else if (Count > 1 && x == Cellbound.max.x - 3)
                    {
                        var position = new Vector3Int(x, y, 0);
                        mapdata.SetTile(position, Basetile);

                        //�J�E���g���}�C�i�X
                        Count--;

                    }
                }

            }

        }

        Init_MapData(mapobj);
    }

    //�}�b�v�̐����i�e�X�g�@�\�j
    void Init_MapData(GameObject mapobj)
    {
        //GameObject obj = Instantiate(mapdata.transform.gameObject, TileGrid.transform.position, Quaternion.identity);

        mapobj.transform.parent = TileGrid.transform;
    }
}
