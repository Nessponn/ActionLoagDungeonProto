using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapInfo
{
    public GameObject mapobj { get; set; }

    public Tilemap map { get; set; }

    public Vector2Int size { get; set; }

    public List<Vector3Int> GimmickStartPosx { get; set; }

    public List<Vector3Int> GimmickGoalPosx { get; set; }

    public List<Vector3Int> GimmickStartPosy { get; set; }

    public List<Vector3Int> GimmickGoalPosy { get; set; }

    public List<Vector3Int> GimmickposLeft { get; set; }

    public List<Vector3Int> GimmickposRight { get; set; }
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
    public TileBase Itemtile_X;
    public TileBase Itemtile_Y;

    public int Thickness;//�Ԋu

    [Range(0,2)]public float Reaction;

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
        //�}�b�v�f�[�^�̍쐬
        GameObject mapobj = new GameObject();//���̎����͐����������s���Ă���
        mapobj.AddComponent<Tilemap>();
        mapobj.AddComponent<TilemapRenderer>();

        

        mapobj.transform.parent = TileGrid.transform;//�ǐ��������̂��߂̍��p�𐳂����o�^���邽�߁A�I�u�W�F�N�g�̎q�o�^�����Ă���

        //�^�C���}�b�v�R���|�[�l���g�̎擾
        var mapdata = mapobj.GetComponent<Tilemap>();

        //�c����̐ݒ�
        mapdata.size = new Vector3Int(Width,Height,mapdata.size.z);



        //mapInfo�̍쐬��������
        MapInfo mapInfo = new MapInfo();
        mapInfo.map = mapdata;
        mapInfo.mapobj = mapobj;//�}�b�v��Ԃ�ۑ�
        mapInfo.size = new Vector2Int(Width, Height);
        mapInfo.GimmickposLeft = new List<Vector3Int>();
        mapInfo.GimmickposRight = new List<Vector3Int>();

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
        List<Vector3Int> posLeft = LeftPointerFromMiddle(mapInfo);//�����̃^�C���}�b�v
        List<Vector3Int> posRight = RightPointerFromMiddle(mapInfo);//�E���̃^�C���}�b�v

        MapAutoCreate(mapInfo, posLeft, posRight);
    }

    //�������獶�[�֓_��ł�
    List<Vector3Int> LeftPointerFromMiddle(MapInfo mapInfo)
    {
        List<Vector3Int> posLeft = new List<Vector3Int>();//�����̃^�C���}�b�v

        //���������
        var Cellbound = mapInfo.map.cellBounds;


        //Debug.Log(Cellbound.max.x);

        for (int y = Cellbound.max.y - 1 - ((Thickness - 1) / 2); y >= Cellbound.min.y; y -= Thickness)
        {//��������E��ɂ����ă^�C�����č�����

            //�����܂łőł��~��
            for (int x = Cellbound.max.x / 2 - 1; x >= 0; x--)
            {

                //�����ݒ�
                int rad = Random.Range(1, 10);


                //�����œ���������^�C���𖄂߂�(1/5���x)
                if (rad > 8)
                {
                    var position = new Vector3Int(x, y, 0);
                    posLeft.Add(position);

                    break;///�_���ł��ꂽ���_�ŁA����x���ł̐��������͏I��
                }

                if(x == 0)
                {
                    var position = new Vector3Int(x, y, 0);
                    posLeft.Add(position);

                    break;///�_���ł��ꂽ���_�ŁA����x���ł̐��������͏I��
                }
            }
        }

        return posLeft;
    }

    //��������E�[�֓_��ł�
    List<Vector3Int> RightPointerFromMiddle(MapInfo mapInfo)
    {
        List<Vector3Int> posRight = new List<Vector3Int>();//�E���̃^�C���}�b�v

        //���������
        var Cellbound = mapInfo.map.cellBounds;

        for (int y = Cellbound.max.y - 1 - ((Thickness - 1) / 2); y >= Cellbound.min.y; y -= Thickness)
        {//��������E��ɂ����ă^�C�����č�����

            //�����܂łőł��~��
            for (int x = Cellbound.max.x / 2 + 1; x <= Cellbound.max.x; x++)
            {
                //�����ݒ�
                int rad = Random.Range(1, 10);

                //�����œ���������^�C���𖄂߂�(1/5���x)
                if (rad > 8)
                {
                    var position = new Vector3Int(x, y, 0);
                    posRight.Add(position);

                    break;///�_���ł��ꂽ���_�ŁA����x���ł̐��������͏I��
                }

                //���̎��_�łǂ����ł���Ă��Ȃ���΁A�E�[�őł�
                if(x == Cellbound.max.x)
                {
                    var position = new Vector3Int(x, y, 0);
                    posRight.Add(position);

                    break;///�_���ł��ꂽ���_�ŁA����x���ł̐��������͏I��
                }
            }
        }

        return posRight;
    }
    void LeftPointerToMiddle(MapInfo mapInfo, int[,] mapdata, int[,] mappoint)
    {
        for (int y = 0; y < mapInfo.size.y; y++)
        {//��������E��ɂ����ă^�C�����č�����

            for (int x = 0; x < mapInfo.size.x; x++)
            {
                //�����ݒ�
                int rad = Random.Range(1, 10);

                //�����œ���������^�C���𖄂߂�(1/5���x)
                if (rad > 8)
                {

                }
            }
        }
    }

    //�E�[���璆���֓_��ł�
    void RightPointerToMiddle(MapInfo mapInfo, int[,] mapdata, int[,] mappoint)
    {
        for (int y = 0; y < mapInfo.size.y; y++)
        {//��������E��ɂ����ă^�C�����č�����

            for (int x = 0; x < mapInfo.size.x; x++)
            {
                //�����ݒ�
                int rad = Random.Range(1, 10);

                //�����œ���������^�C���𖄂߂�(1/5���x)
                if (rad > 8)
                {

                }
            }
        }
    }


    void MapAutoCreate(MapInfo mapInfo, List<Vector3Int> posLeft, List<Vector3Int> posRight)
    {
        //�^�C���}�b�v�R���|�[�l���g�̎擾
        var maptile = mapInfo.mapobj.GetComponent<Tilemap>();

        //�����Əo���ƂȂ镔��������ɐ�������Ă���΁A�����𑱍s����
        if (!(posLeft[0].y == posRight[0].y && posLeft[posLeft.Count - 1].y == posRight[posRight.Count - 1].y))
        {
            //������Ɠ_����������Ă��Ȃ���΁A�Đ����������s��
            Debug.LogError("����ɐ�������܂���ł����B�Đ������s���܂�");

            return;
        }

        mapInfo.GimmickStartPosx = new List<Vector3Int>();
        mapInfo.GimmickGoalPosx = new List<Vector3Int>();
        mapInfo.GimmickStartPosy = new List<Vector3Int>();
        mapInfo.GimmickGoalPosy = new List<Vector3Int>();
        mapInfo.GimmickStartPosx.Add(new Vector3Int(-9999, -9999, 0));
        mapInfo.GimmickStartPosy.Add(new Vector3Int(-9999, -9999, 0));

        //�ǂ���鏈��
        //����for���ł́A�K�v�ȏ�ɓʉ����Ȃ��悤�ɒ������邽�߂̂���
        for (int x = 0; x < posLeft.Count - 1; x++)
        {
            for (int ex = x + 1; ex < posLeft.Count - 1; ex++)
            {
                //���̓_�Ƃ��āA�K�؂��ǂ��������߂�
                if (Mathf.Abs(posLeft[x].x - posLeft[ex].x) < size.x + ((int)((-Mathf.Pow(x, 2) + (size.y * Thickness * x)) / size.y * Thickness)) * (Reaction - 1) / Thickness || posLeft[ex] == posLeft[posLeft.Count - 1])
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
        //Gimmick_posset(mapInfo, 0);


        for (int x = 0; x < posRight.Count - 1; x++)
        {
            for (int ex = x + 1; ex < posRight.Count - 1; ex++)
            {
                //���̓_�Ƃ��āA�K�؂��ǂ��������߂�
                if (Mathf.Abs(posRight[x].x - posRight[ex].x) < size.x + ((int)((-Mathf.Pow(x, 2) + (size.y * Thickness * x)) / size.y * Thickness)) * (Reaction - 1) / Thickness || posRight[ex] == posRight[posRight.Count - 1])
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

            AStarPath.Instance.astarSearchPathFinding(mapInfo, posRight[x], posRight[x + 1], 1);//�E
        }

        //Gimmick_posset(mapInfo, 1);
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

    public void Gimmickpos_Add()
    {

    }

    //�M�~�b�N��ݒu���鏈��
    public void Gimmick_posset(MapInfo mapInfo,int dir,List<Vector3Int> GimmickStartPosx, List<Vector3Int> GimmickGoalPosx, List<Vector3Int> GimmickStartPosy, List<Vector3Int> GimmickGoalPosy)
    {
        //���������Ă��Ȃ���΁A�������΂�
        if (mapInfo.GimmickStartPosy[0] != new Vector3Int(-9999, -9999, 0))
        {
            for (int x = 0; x < mapInfo.GimmickStartPosy.Count - 1; x++)//��ԍŌ�͂��肦�Ȃ��l�Ȃ̂ŁA���O����Ӗ���-1����
            {
                int Ypos = mapInfo.GimmickStartPosy[x].y - mapInfo.GimmickGoalPosy[x].y;

                //���ȏ㕝������΁A�M�~�b�N��ݒu����
                if (Ypos >= 3)
                {
                    if (dir == 0)//�E���ɐ���
                    {
                        var position = new Vector3Int(mapInfo.GimmickStartPosy[x].x + 1, mapInfo.GimmickGoalPosy[x].y + Ypos / 2, 0);

                        mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile_Y);
                        mapInfo.GimmickposLeft.Add(mapInfo.map.WorldToCell(position));
                    }
                    else//�����ɐ���
                    {
                        var position = new Vector3Int(mapInfo.GimmickStartPosy[x].x - 1, mapInfo.GimmickGoalPosy[x].y + Ypos / 2, 0);

                        mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile_Y);
                        mapInfo.GimmickposRight.Add(mapInfo.map.WorldToCell(position));
                    }
                }
            }
        }

        if (mapInfo.GimmickStartPosx[0] != new Vector3Int(-9999, -9999, 0))
        {
            for (int x = 0; x < mapInfo.GimmickStartPosx.Count - 1; x++)//��ԍŌ�͂��肦�Ȃ��l�Ȃ̂ŁA���O����Ӗ���-1����
            {
                int Ypos = mapInfo.GimmickStartPosx[x].y - mapInfo.GimmickGoalPosx[x].y;
                int Xpos = mapInfo.GimmickStartPosx[x].x - mapInfo.GimmickGoalPosx[x].x;

                //���ȏ㕝������΁A�M�~�b�N��ݒu����
                if (Xpos >= 3 * Thickness)
                {
                    var position = new Vector3Int(mapInfo.GimmickStartPosx[x].x - Xpos / 2, mapInfo.GimmickGoalPosx[x].y + Ypos / 2, 0);

                    mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile_X);//�ݒu

                    //�������ɐ������邽�߂̏���z���ɓ����(�g���Ƃ��́@0�@�ɂ���̂�Y�ꂸ��)
                    position.z = -1;//��
                    mapInfo.GimmickposRight.Add(mapInfo.map.WorldToCell(position));
                }
                else if (Xpos <= -3 * Thickness)
                {
                    var position = new Vector3Int(mapInfo.GimmickStartPosx[x].x - Xpos / 2, mapInfo.GimmickGoalPosx[x].y + Ypos / 2, 0);

                    mapInfo.map.SetTile(mapInfo.map.WorldToCell(position), Itemtile_X);//�ݒu

                    //������ɐ������邽�߂̏���z���ɓ����(�g���Ƃ��́@0�@�ɂ���̂�Y�ꂸ��)
                    position.z = 1;//��
                    mapInfo.GimmickposRight.Add(mapInfo.map.WorldToCell(position));
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

        for (int y = Cellbound.max.y - 1 ; y >= Cellbound.min.y; y --)
        {//��������E��ɂ����ă^�C�����č�����

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                var mapcell = map.GetTile(new Vector3Int(x, y, 0));

                if (mapcell)
                {
                    //�Q�Ƃ���u���b�N
                    var position = new Vector3Int(y, x, 0);

                    //�Ԃ��_�Ɠ����Ƃ���ɂ���������
                    if (mapcell == Itemtile_X)
                    {
                        mapdata.SetTile(position, map.GetTile(new Vector3Int(x, y, 0)));
                    }
                    //�ΐF�̓_�Ɠ����Ƃ���ɂ���������
                    else if (mapcell == Itemtile_Y)
                    {
                        mapdata.SetTile(position, map.GetTile(new Vector3Int(x, y, 0)));
                    }
                    else mapdata.SetTile(position, map.GetTile(new Vector3Int(x, y, 0)));
                }
            }
        }

        Destroy(map.transform.gameObject);

        return mapobj;
    }

    //���ƂȂ�}�b�v�f�[�^���쐬�i��� MapBoundCreate�j
    
    void TileDataDone(GameObject mapobj, int Width, int Height)
    {

        //�^�C���}�b�v�̈ʒu����
        mapobj.transform.position = new Vector3(-Width / 2, -Height / 2, 0);

    }
}
