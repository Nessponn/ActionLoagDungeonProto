using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapInfo
{
    public GameObject mapobj { get; set; }

    public Tilemap map { get; set; }

    public Vector2Int size { get; set; }

    public List<Vector3Int> GimmickStartPos { get; set; }

    public List<Vector3Int> GimmickGoalPos { get; set; }
}
//�}�b�v�I�[�g�}�C�U�[
public class MapAutomizer : SingletonMonoBehaviourFast<MapAutomizer>
{
    /// <summary>
    /// �T�v
    /// �P�D�^�C���}�b�v�I�u�W�F�N�g���쐬���A�������鏀�����s���B�i�c����A�Ȃǁj
    /// 
    /// 
    /// </summary>
    /// 

    public Vector2Int size;
    public GameObject TileGrid;
    public TileBase Basetile;
    public TileBase Itemtile;

    public int Thickness;//�Ԋu

    private List<MapInfo> mapInfo = new List<MapInfo>();
    
    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> mapobj = new List<GameObject>();

        for(int x = 0;x < 10; x++) mapobj.Add(TileMapDataCreate(size.x * Thickness, size.y * Thickness));

        for (int x = 0; x < 10; x++) 
        { 
            if(x % 2 == 0)mapobj[x].transform.position = new Vector2(size.x * x * 2 - 75, size.y * 2);
            else mapobj[x].transform.position = new Vector2(size.x * x * 2 - 75, -size.y * 2);
        }
    }

    

    GameObject TileMapDataCreate(int Width,int Height)
    {
        MapInfo mapInfo = new MapInfo();

        //�}�b�v�f�[�^�̍쐬
        GameObject mapobj = new GameObject();//���̎����͐����������s���Ă���
        mapobj.AddComponent<Tilemap>();
        mapobj.AddComponent<TilemapRenderer>();

        mapobj.transform.parent = TileGrid.transform;//�ǐ��������̂��߂̍��p�𐳂����o�^���邽�߁A�I�u�W�F�N�g�̎q�o�^�����Ă���

        //�^�C���}�b�v�R���|�[�l���g�̎擾
        var mapdata = mapobj.GetComponent<Tilemap>();
        mapInfo.map = mapdata;

        //�c����̐ݒ�
        mapdata.size = new Vector3Int(Width,Height,mapdata.size.z);

        //�}�b�v��Ԃ�ۑ�
        mapInfo.mapobj = mapobj;
        mapInfo.size = new Vector2Int(Width, Height);

        //�}�b�v�z��f�[�^�̍쐬
        MapBoundCreate(mapInfo);

        //��]����
        //mapobj = Rotate(mapobj.GetComponent<Tilemap>());

        TileDataDone(mapobj, Width, Height);

        this.mapInfo.Add(mapInfo);



        return mapobj;
    }

    //���ƂȂ�}�b�v�f�[�^���쐬
    void MapBoundCreate(MapInfo mapInfo)
    {
        //�}�b�v�z��̍쐬
        int[,] mapdata = new int[mapInfo.size.x, mapInfo.size.y];
        int[,] mappoint = new int[mapInfo.size.x, mapInfo.size.y];

        //�}�b�v�̈ʒu�ۑ��p
        int Count;

        //�^�C���̐ݒu����̐����J�E���g�p
        int TiledCount;

        //�}�b�v�̒��S�_�i��΂ɖ��܂�Ȃ������B�܂��͂O�Őݒ�j
        int middlepoint = 0;

        //Debug.Log("Width =" + Width);
        for (int y = 0; y < mapInfo.size.y; y++)
        {//��������E��ɂ����ă^�C�����č�����

            //�J�E���g�̃��Z�b�g
            Count = 2;
            TiledCount = 0;
            //Debug.Log("x[" + y + "] =" + ((int)((-Mathf.Pow(y, 2) + (Height * y)) / Height)));

            for (int x = 0; x < mapInfo.size.x; x++)
            {
                //�}�b�v�̒��S�_�Ȃ��΂��A�c��J�E���g���O�ȉ��ł���΂�
                if (x != middlepoint && Count > 0 && TiledCount <= 0)
                {
                    //�����ݒ�
                    int rad = Random.Range(1, 10);


                    //�����œ���������^�C���𖄂߂�(1/5���x)
                    if (rad > 8)
                    {
                        if (Count == 2)
                        {
                            //�}�b�v�̐^�񒆂����肪�c��ޗl�ɁA����

                            mapdata[x, y] = 1;
                            mappoint[x, y] = Count;
                        }
                        else if (Count == 1)
                        {
                            int X = x + ((int)((-Mathf.Pow(y, 2) + (mapInfo.size.y * y)) / mapInfo.size.y)) * 2;
                            if (X > mapInfo.size.x - 1) X = mapInfo.size.x - 1;

                            //�}�b�v�̐^�񒆂����肪�c��ޗl�ɁA����
                            mapdata[X, y] = 1;
                            mappoint[X, y] = Count;
                        }


                        /*mapdata[x, y] = 1;
                        mappoint[x, y] = Count;*/

                        //�J�E���g���}�C�i�X
                        Count--;
                        TiledCount = 2;
                    }
                    //������x���̖��[���_�ł܂��񐔕��ݒ肵�Ă��ȏꍇ�A��O�I�Ɏ����Ŗ��߂�
                    else if (Count > 0 && x == mapInfo.size.x - 1)
                    {
                        mapdata[x, y] = 1;
                        mappoint[x, y] = Count;

                        //�J�E���g���}�C�i�X
                        Count--;
                        TiledCount = 2;
                    }
                    //��ڂ̃^�C����x���̖�������R�ڂ̎��_�Őݒu����Ă��Ȃ��ꍇ�A��O�I�Ɏ����Ŗ��߂�
                    else if (Count > 1 && x == mapInfo.size.x - 3)
                    {
                        mapdata[x, y] = 1;
                        mappoint[x, y] = Count;

                        //�J�E���g���}�C�i�X
                        Count--;
                        TiledCount = 2;
                    }
                }

                TiledCount--;
            }

            if (Count > 0) Debug.Log("�J�E���g�܂��c���Ă�");
        }

        MapAutoCreate(mapInfo, mapdata, mappoint);
    }

    void MapAutoCreate(MapInfo mapInfo, int[,] mapdata, int[,] mappoint)
    {
        //�^�C���}�b�v�R���|�[�l���g�̎擾
        var maptile = mapInfo.mapobj.GetComponent<Tilemap>();

        //���������
        var Cellbound = mapInfo.mapobj.GetComponent<Tilemap>().cellBounds;

        List<Vector3Int> posLeft = new List<Vector3Int>();//�����̃^�C���}�b�v
        List<Vector3Int> posRight = new List<Vector3Int>();//�E���̃^�C���}�b�v

        //�}�b�v�f�[�^�̂̈ʒu�Q�Ɨp
        int mx, my;
        mx = my = 0;

        for (int y = Cellbound.max.y - 1 - ((Thickness - 1) / 2); y >= Cellbound.min.y; y -= Thickness)
        {//��������E��ɂ����ă^�C�����č�����

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                //�ǁi�P�j�ł���΁A���߂�
                if (mapdata[mx, my] == 1)
                {
                    //�w�蕝���P�ȏ�w�肵�Ă���΁A��������߂�
                    /*for (int ey = y + ((Thickness - 1) / 2); ey >= y - ((Thickness - 1) / 2); ey--)
                    {
                        //�Q�Ƃ���u���b�N
                        var position = new Vector3Int(x - 1 - ((Thickness - 1) / 2), y, 0);
                        maptile.SetTile(position, Basetile);
                    }*/

                    //�Q�Ƃ���u���b�N
                    var position = new Vector3Int(x - 1 - ((Thickness - 1) / 2), y, 0);
                    maptile.SetTile(position, Basetile);

                    //�^�C�������E�ɕ�����
                    if (mappoint[mx, my] == 2)//���ł����
                    {
                        posLeft.Add(position);
                    }
                    else//�E�ł����
                    {
                        posRight.Add(position);
                    }
                }
                mx++;
            }
            mx = 0;
            my++;
        }

        //�����Əo���ƂȂ镔��������ɐ�������Ă���΁A�����𑱍s����
        if (!(posLeft[0].y == posRight[0].y && posLeft[posLeft.Count - 1].y == posRight[posRight.Count - 1].y))
        {
            //������Ɠ_����������Ă��Ȃ���΁A�Đ����������s��
            Debug.LogError("����ɐ�������܂���ł����B�Đ������s���܂�");

            //Destroy(mapobj);

            //TileMapDataCreate(size.x * Thickness, size.y * Thickness);

            return;
        }

        mapInfo.GimmickStartPos = new List<Vector3Int>();
        mapInfo.GimmickGoalPos = new List<Vector3Int>();
        mapInfo.GimmickStartPos.Add(new Vector3Int(-9999, -9999, 0));

        //�ǂ���鏈��
        for (int x = 0; x < posLeft.Count - 1; x++)
        {
            float X = posLeft[x].y / Thickness;

            for (int ex = x + 1; ex < posLeft.Count - 1; ex++)
            {
                //���̓_�Ƃ��āA�K�؂��ǂ��������߂�
                if (Mathf.Abs(posLeft[x].x - posLeft[ex].x) < size.x - ((int)((-Mathf.Pow(x, 2) + (size.y * Thickness * x)) / size.y * Thickness)) / Thickness || posLeft[ex] == posLeft[posLeft.Count - 1])
                {
                    //�K�v�ȏ�ɗ���Ă��Ȃ���΁A���p�_�Ƃ��Ĉ���
                    //�܂��́A���̓_���Ō�̓_�ł���΁A����͒��p�_�Ƃ��Ĉ���

                    break;
                }
                else
                {
                    //�s�K���ł���΁A���Ƃ��ď���
                    maptile.SetTile(posLeft[ex], null);
                    posLeft.RemoveAt(ex);
                    //���炵���������Aex����������
                    ex--;
                }
            }

            AStarPath.Instance.astarSearchPathFinding(mapInfo, posLeft[x], posLeft[x + 1], 0);//��
        }

        //MapAutomizer.Instance.Gimmick_Init(map,GimmickStartPos, GimmickGoalPos, dir);

        Gimmick_Init(mapInfo, 0);

        //AStarPath.Instance.astarSearchPathFinding(maptile, posLeft[0], posLeft[1], 0);//��

        /*for (int x = 0; x < posRight.Count - 1; x++)
        {
            float X = posRight[x].y / Thickness;

            for (int ex = x + 1; ex < posRight.Count - 1; ex++)
            {
                //���̓_�Ƃ��āA�K�؂��ǂ��������߂�
                if (Mathf.Abs(posRight[x].x - posRight[ex].x) < size.x - ((int)((-Mathf.Pow(x, 2) + (size.y * Thickness * x)) / size.y * Thickness)) / Thickness || posRight[ex] == posRight[posRight.Count - 1])
                {
                    //�K�v�ȏ�ɗ���Ă��Ȃ���΁A���p�_�Ƃ��Ĉ���
                    //�܂��́A���̓_���Ō�̓_�ł���΁A����͒��p�_�Ƃ��Ĉ���

                    break;
                }
                else
                {
                    //�s�K���ł���΁A���Ƃ��ď���
                    maptile.SetTile(posRight[ex], null);
                    posRight.RemoveAt(ex);
                    //���炵���������Aex����������
                    ex--;
                }
            }

            AStarPath.Instance.astarSearchPathFinding(maptile, posRight[x], posRight[x + 1], 1);//�E
        }*/
        /*
                //�㉺�̘g�����
                for (int x = posLeft[0].x; x <= posRight[0].x; x++)
                {
                    var position = posLeft[0];
                    position.x = x;

                    maptile.SetTile(position, Basetile);
                }

                for (int x = posLeft[posLeft.Count - 1].x; x <= posRight[posRight.Count - 1].x; x++)
                {
                    var position = posLeft[posLeft.Count - 1];
                    position.x = x;

                    if (maptile.HasTile(position)) maptile.SetTile(position, null);
                    else maptile.SetTile(position, Basetile);
                }*/
    }
    //�M�~�b�N��ݒu���鏈��
    public void Gimmick_Init(MapInfo mapInfo,int dir)
    {
        //���������Ă��Ȃ���΁A�������΂�
        if (mapInfo.GimmickStartPos[0] != new Vector3Int(-9999, -9999, 0))
        {
            for (int x = 0; x < mapInfo.GimmickStartPos.Count - 1; x++)//��ԍŌ�͂��肦�Ȃ��l�Ȃ̂ŁA���O����Ӗ���-1����
            {
                //Debug.Log("x[" + x + "]" + (mapInfo.GimmickStartPos[x].y - mapInfo.GimmickGoalPos[x].y));

                int Ypos = mapInfo.GimmickStartPos[x].y - mapInfo.GimmickGoalPos[x].y;

                //���ȏ㕝������΁A�M�~�b�N��ݒu����
                if (Ypos >= 3)
                {
                    if (dir == 0)//�E���ɐ���
                    {
                        Debug.Log("�g���W���[�b�I�I�I");

                        var position = new Vector3Int(mapInfo.GimmickStartPos[x].x + 1, mapInfo.GimmickGoalPos[x].y + Ypos / 2, 0);

                        mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile);
                    }
                    else//�����ɐ���
                    {
                        var position = new Vector3Int(mapInfo.GimmickStartPos[x].x - 1, mapInfo.GimmickGoalPos[x].y + Ypos / 2, 0);

                        mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile);
                    }
                }
            }
        } 
    }

    //�����}�b�v�̉�]����(���ɂX�O�x��])
    GameObject Rotate(Tilemap map)
    {
        //�}�b�v�f�[�^�̍쐬
        GameObject mapobj = new GameObject();//���̎����͐����������s���Ă���
        mapobj.AddComponent<Tilemap>();
        mapobj.AddComponent<TilemapRenderer>();

        mapobj.transform.parent = TileGrid.transform;//�ǐ��������̂��߂̍��p�𐳂����o�^���邽�߁A�I�u�W�F�N�g�̎q�o�^�����Ă���

        //�^�C���}�b�v�R���|�[�l���g�̎擾
        var mapdata = mapobj.GetComponent<Tilemap>();

        //�c����̐ݒ�
        mapdata.size = new Vector3Int(map.size.x, map.size.y, mapdata.size.z);

        //���������
        var Cellbound = map.cellBounds;

        //GameObject obj = Instantiate(mapdata.transform.gameObject, TileGrid.transform.position, Quaternion.identity);
        for (int y = Cellbound.max.y - 1 ; y >= Cellbound.min.y; y --)
        {//��������E��ɂ����ă^�C�����č�����

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                if(map.GetTile(new Vector3Int(x, y, 0)))
                {
                    //�Q�Ƃ���u���b�N
                    var position = new Vector3Int(y, x, 0);
                    mapdata.SetTile(position, map.GetTile(new Vector3Int(x, y, 0)));
                }

            }
        }
        //mapobj.transform.position = new Vector3(-map.size.x / 2, -map.size.y / 2, 0);

        Destroy(map.transform.gameObject);

        return mapobj;
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
        
    }

    void TileDataDone(GameObject mapobj, int Width, int Height)
    {

        //�^�C���}�b�v�̈ʒu����
        mapobj.transform.position = new Vector3(-Width / 2, -Height / 2, 0);

    }
}
