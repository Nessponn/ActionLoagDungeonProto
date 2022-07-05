using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAuto : MonoBehaviour
{
    #region//�ϐ��ꗗ

    public TileStageData StageData;
    public GameObject StageGrid;//�����_�����O��ՂƂȂ�^�C���t�B�[���h

    public GameObject PreStages;//�}�b�v�����̊�ՂƂȂ����

    private Tilemap tilemap;

    public TileBase Basetile;//�ǂ⏰�̌��ƂȂ�^�C��
    public TileBase Chaintile;//�A�����̐����ʒu�i�f�o�b�O�p�j

    //�r���h��(���������锠�̐�)
    private int BuildCount;

    //�^�C���}�b�v�̑S�̃T�C�Y
    private int x_GridRange;
    private int y_GridRange;

    //�ǂ̂��炢�̃T�C�Y�̘g����邩
    private int Outline_scale_x;
    private int Outline_scale_y;

    //���삷��}�b�v�̋󓴂̃T�C�Y
    private int mapbox_scale_x;
    private int mapbox_scale_y;


    //�A�����ۑ��p�N���X�ϐ�
    private List<PrePoints_Info> Preinfo = new List<PrePoints_Info>();

    #endregion

    private void Start()
    {
        TileSetup();    //�}�b�v�̐ݒ�̃Z�b�g�A�b�v

        //Debug.Log(tilemap.size);

        Premap_Init();   //�v���Z�b�g�̃}�X�𐶐�����


        PrePoints_Detection();  //�v���Z�b�g�̃}�X���璆�p�n�_���`����


        //(���p�n�_���o�R���Ȃ���)�^�C���𖄂߂�
        //TileCellar();

        //�^�C���̏�Ԃ��m�F
        CheckTileCell();
    }

    //�v���Z�b�g�̃^�C����F�����A������ǉ����钆�p�ʒu���`����

    void TileSetup()
    {
        //�^�C���}�b�v�̃R���|�[�l���g�擾
        tilemap = StageGrid.GetComponent<Tilemap>();

        //�^�C���}�b�v�̑S�̃T�C�Y
        x_GridRange = StageData.x_GridRange;
        y_GridRange = StageData.y_GridRange;

        //�P���Ɋg�傷��ƉE��ɂ���Ă����̂ŁA�����Ŋg�債�����������炷
        StageGrid.transform.position = new Vector3(-x_GridRange / 2, -y_GridRange / 2, 0);
        //gameObject.transform.position = new Vector3(-x_GridRange / 2, -y_GridRange / 2, 0);

        tilemap.size = new Vector3Int(x_GridRange, y_GridRange, tilemap.size.z);
        //tilemap.CompressBounds();
    }

    //�Z�b�g���ꂽ�v���Z�b�g�X�e�[�W���Z�b�g����
    void Premap_Init()
    {
        if (!PreStages) return;
        
        var Pretilemap = PreStages.GetComponent<Tilemap>();
        var Prebound = PreStages.GetComponent<Tilemap>().cellBounds;

        Pretilemap.CompressBounds();//���T�C�Y


        //�Z�b�g���ꂽ�v���X�e�[�W���^�C���}�b�v�̑S�̃T�C�Y���傫���ꍇ�A�傫�����㏑������
        if (Pretilemap.size.x > x_GridRange) x_GridRange = Pretilemap.size.x;
        if (Pretilemap.size.y > y_GridRange) y_GridRange = Pretilemap.size.y;

        //�T�C�Y�̍X�V
        tilemap.size = new Vector3Int(x_GridRange, y_GridRange, tilemap.size.z);


        for (int y = Prebound.max.y - 1; y >= Prebound.min.y; --y)
        {//���ォ��E���ɂ����ă^�C�����č�����

            for (int x = Prebound.min.x; x < Prebound.max.x; ++x)
            {
                //�Q�Ƃ���u���b�N
                var position = new Vector3Int(x + (x_GridRange - Pretilemap.size.x) / 2, y + (y_GridRange - Pretilemap.size.y) / 2, 0);

                //�^�C���̎擾
                TileBase tile = Pretilemap.GetTile(new Vector3Int(x, y, 0));

                //��Ղ̃^�C���}�b�v�Ƀv���Z�b�g�n�`��]��
                tilemap.SetTile(position, tile);
                
            }
        }

    }

    //�v���Z�b�g�̃^�C���}�b�v���璆�p�n�_��{������
    void PrePoints_Detection()
    {
        var tilemap = StageGrid.GetComponent<Tilemap>();
        var bound = StageGrid.GetComponent<Tilemap>().cellBounds;

        //�^����ꂽ�}�b�v�ɏd�݂�t����
        int num = 0;
        Vector3Int StartPos = new Vector3Int();
        Vector3Int EndPos = new Vector3Int(); ;

        //X���̌���
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {//���ォ��E���ɂ����ă^�C�����č�����

            //�H�����ς�邲�Ƃ�num�̒l�����Z�b�g
            num = 0;
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                var position = new Vector3Int(x, y, 0);//�Q�Ƃ���u���b�N

                //�^�C���̎擾
                TileBase tile = tilemap.GetTile(position);

                if(tile != null)
                {
                    //num��0�̎��A���݂̎Q�ƈʒu���n�_��ݒ�
                    if(num == 0)StartPos = position;

                    //�^�C���̃J�E���g
                    num++;
                }
                else
                {
                    //���݂̎Q�ƈʒu���I�_��ݒ�
                    EndPos = position;
                    EndPos.x = EndPos.x - 1;//��O�̈ʒu��ݒ�

                    //�؂ꂽ���_�ŁA���p�n�_�Ƃ��Ďg�p���邩�ǂ����̌��蔻�f����������
                    PrePoints_Init(num,StartPos,EndPos,true);

                    //num�̒l�����Z�b�g
                    num = 0;
                }
            }
        }

        //y���̌���
        for (int x = bound.min.x; x < bound.max.x; ++x)
        {//���ォ��E���ɂ����ă^�C�����č�����

            //�H�����ς�邲�Ƃ�num�̒l�����Z�b�g
            num = 0;
            for (int y = bound.max.y - 1; y >= bound.min.y; --y)
            {
                var position = new Vector3Int(x, y, 0);//�Q�Ƃ���u���b�N

                //�^�C���̎擾
                TileBase tile = tilemap.GetTile(position);

                if (tile != null)
                {
                    //num��0�̎��A���݂̎Q�ƈʒu���n�_��ݒ�
                    if (num == 0) StartPos = position;

                    //�^�C���̃J�E���g
                    num++;
                }
                else
                {
                    //���݂̎Q�ƈʒu���I�_��ݒ�
                    EndPos = position;
                    EndPos.y = EndPos.y + 1;//��O�̈ʒu��ݒ�

                    //�؂ꂽ���_�ŁA���p�n�_�Ƃ��Ďg�p���邩�ǂ����̌��蔻�f����������
                    PrePoints_Init(num, StartPos, EndPos, false);

                    //num�̒l�����Z�b�g
                    num = 0;
                }
            }
        }
    }

    //���p�n�_�̐���
    void PrePoints_Init(int num, Vector3Int StartPos, Vector3Int EndPos ,bool Xmark)
    {
        //����̒������@��萔�ȉ��@�ł���΁A����𕔉��Ƃ��Č��Ȃ��Ȃ�
        if (num <= 8) return;

        var position = new Vector3Int();

        //�����Ώۂ�x����y�����ŋ��
        if(Xmark) position = new Vector3Int(EndPos.x - ((EndPos.x - StartPos.x) / 2), StartPos.y, StartPos.z);
        else position = new Vector3Int(StartPos.x, EndPos.y - ((EndPos.y - StartPos.y) / 2), StartPos.z);

        //�f�o�b�O�p�̘A���ʒu�ۑ�
        tilemap.SetTile(position, Chaintile);

        //�A�������̕ۑ�
        var info = new PrePoints_Info(position, true);
        Preinfo.Add(info);

    }

    class PrePoints_Info
    {
        Vector3Int position;//�ʒu
        bool Xmark;//X���̘A������

        public PrePoints_Info(Vector3Int position,bool Xmark)
        {
            this.position = position;
            this.Xmark = Xmark;
        }
    }






    //�^�C���𖄂߂邽�߂̃��\�b�h�B�A���S���Y���ɕK�v�ȕ����̍쐬
    void TileCellar()
    {
        //�^����ꂽ�}�b�v�ɏd�݂�t����
        int num = 0;
        var tilemap = StageGrid.GetComponent<Tilemap>();
        var bound = StageGrid.GetComponent<Tilemap>().cellBounds;

        //���ォ��^�C����������ɂ����A�n�_����́@'offset'�@�Ɓ@�g�̑傫��'�@�Ɓ@'�g�����ۂ̉E����̕ϐ�'�@�����߂�(�ŏ��̓N���X�Ƃ����Ȃ��ł���)

        int offsetX,offsetY;//�g�̈ʒu�����炷�l

        for (int i = 0; i < StageData.Stage.Count; i++)
        {
            //�r���h��(���������锠�̐�)
            BuildCount = StageData.Stage.Count;

            //�ǂ̂��炢�̃T�C�Y�̘g����邩
            Outline_scale_x = StageData.Stage[i].OutlineNum;
            Outline_scale_y = StageData.Stage[i].OutlineNum;

            //���삷��}�b�v�̋󓴂̃T�C�Y
            mapbox_scale_x = (int)StageData.Stage[i].StageSize.x;
            mapbox_scale_y = (int)StageData.Stage[i].StageSize.y;

            //��������g��offset���r���h�񐔂��ƂɌ��肷��
            offsetX = Random.Range(0, x_GridRange - mapbox_scale_x);
            offsetY = Random.Range(0, y_GridRange - mapbox_scale_y);

            for (int y = mapbox_scale_y - 1; y >= bound.min.y; --y){//��������E��ɂ����ă^�C���������Ă���

                for (int x = bound.min.x; x < mapbox_scale_x; ++x)
                {
                    var position = new Vector3Int(x + offsetX, y + offsetY, 0);//�u���b�N��z�u����ʒu�̌���
                    //Vector3 rotation = tilemap.GetTransformMatrix(position).rotation.eulerAngles;//��]�����

                    //�^�C�����̕ۑ�
                    //var info = new TileInfo(position, 0, rotation, tile, MAP_SPEED);
                    //ist.Add(info);

                    if((x < bound.min.x + Outline_scale_x || x > mapbox_scale_x - 1 - Outline_scale_x) || (y >= mapbox_scale_y - Outline_scale_y) || y <= bound.min.y + Outline_scale_y - 1)
                    {
                        tilemap.SetTile(position, Basetile);
                    }

                    num++;
                }
            }
        }
    }

    /*
     
    void TileCellar()
    {
        var bound = tilemap.cellBounds;
        for (int y = bound.max.y - 1 - scale_y; y >= bound.min.y + scale_y; --y)
        {
            for (int x = bound.min.x + scale_x + Zurashi_x; x < bound.max.x - scale_x + Zurashi_x; ++x)
            {
                if((x >= x + 2 && x < x - 2) && (y >= y + 2 && y < y - 2))
                {
                    tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), null);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), Basetile);
                }
            }
        }
    }
     */

    /*
     Grid�̒��ɋ󂫘g�Q�}�X�̘g�ň͂������ɁA�Q�}�X�g�̃{�b�N�X�P�����v���O�����̗�
      
    void TileCellar()
    {
        var bound = tilemap.cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                if((x >= bound.min.x + 2 && x < bound.max.x - 2) && (y >= bound.min.y + 2 && y < bound.max.y - 2))
                {
                    if (!((x >= bound.min.x + 4 && x < bound.max.x - 4) && (y >= bound.min.y + 4 && y < bound.max.y - 4)))
                    {
                        tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), Basetile);
                    }
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, tilemap.size.z), null);
                }
            }
        }
    }
     */

    //���܂��Ă���^�C�����m�F���邽�߂̃��\�b�h
    void CheckTileCell()
    {
        var builder = new StringBuilder();
        var bound = StageGrid.GetComponent<Tilemap>().cellBounds;
        for (int y = bound.max.y - 1; y >= bound.min.y; --y)
        {
            for (int x = bound.min.x; x < bound.max.x; ++x)
            {
                builder.Append(StageGrid.GetComponent<Tilemap>().HasTile(new Vector3Int(x, y, 0)) ? "�Q" : "�P");
            }
            builder.Append("\n");
        }
        Debug.Log(builder.ToString());
    }

    void CreateMap()
    {
        //�^�C����Ճ{�[�h�̐���
        //�O���b�h�̃T�C�Y����Ղ̃T�C�Y�ɍ��킹��
    }


    public class TileInfo
    {
        public readonly Vector3Int m_position;
        public readonly Quaternion m_rotation;
        public readonly TileBase m_tile;
        public readonly bool cell;

        //�^�C���̈ʒu�A�^�C���̉�]�A�^�C����ݒu���邩�ǂ����i0�c���Ȃ��A�P�c����j
        public TileInfo(Vector3Int position,Quaternion rotation,TileBase tile,int num)
        {
            m_position = position;
            m_rotation = rotation;
            m_tile = tile;
            cell = num == 1 ? true : false;
        }
    }
    public void PassTile()
    {
        //�^�C���̏������炷
        //0����-1�S�����u�ԁA���W��0��Ń^�C���������ɂ��炷


        //�^�C�����牽��ڂɂ��邩�iy���̒l�j�̏��������o��
        int num = 0;
        //Vector3Int vec;
        var tilemap = StageGrid.GetComponent<Tilemap>();
        var bound = StageGrid.GetComponent<Tilemap>().cellBounds;

        int gridnum = bound.max.x;

        //bound.max = new Vector3Int(bound.max.x - 1, bound.max.y, bound.max.z);

        var list = new List<TileInfo>();


        for (int y = bound.max.y - 1; y >= bound.min.y; --y)//���ォ��E���ɂ����ă^�C���������Ă���
        {
            for (int x = bound.min.x; x < gridnum; ++x)
            {
                //Debug.Log("x = " + x);
                //�^�C���̏������L�̂P�s�̃R�[�h�Ɋi�[����
                //�^�C���̍��W�A���\�b�h���Ǝv���Ă��������Ŏ���̃r�r������₯�ǁc
                //var tile = StageGrid.GetComponent<Tilemap>().GetTile<Tile>(vec = new Vector3Int(x, y, 0));

                //vec.x -= 1;

                var tile = StageGrid.GetComponent<Tilemap>().GetTile<Tile>(new Vector3Int(x, y, 0));

                var position = new Vector3Int(x, y, 0);
                Vector3 rotation = tilemap.GetTransformMatrix(position).rotation.eulerAngles;//��]�����

                Quaternion rota = tilemap.GetTransformMatrix(position).rotation;

                Debug.Log("rotation = " + rota);

                //�^�C����x�̈ʒu���^�C���̍ŏ��̒l���傫���i�����Ɉʒu���Ă���j�Ȃ�
                if (position.x >= bound.min.x)
                {
                    //
                    var info = new TileInfo(position,rota, tile, 1);
                    list.Add(info);

                    //�{�}�b�v�̕���17�ȉ��ɂȂ�����}�b�v���[����
                }


                if (list.Count <= 0) return;

                num++;
            }

            foreach (var data in list)
            {
                var position = data.m_position;
                var rotation = data.m_rotation;

                if (position.x >= bound.min.x)
                {
                    tilemap.SetTile(position, data.m_tile);
                    Matrix4x4 matrix = Matrix4x4.TRS(Vector3Int.zero, rotation, Vector3.one);
                    tilemap.SetTransformMatrix(position, matrix);
                }
                else
                {
                    //�����ŁA�{�}�b�v�ȍ~�̃}�X�͂��ׂč폜���Ă���
                    tilemap.SetTile(position, null);
                }


                position.x += 1;
                tilemap.SetTile(position, null);
            }
        }

        //
        /*
        if (distance > 1)
        {
            tilemap.size = new Vector3Int(tilemap.size.x - distance, 10, tilemap.size.z);
            tilemap.ResizeBounds();
        }
        */
        StageGrid.GetComponent<Tilemap>().size = tilemap.size;
        StageGrid.GetComponent<Tilemap>().CompressBounds();

        //Create_Outline_Maps(list);
    }

    public static int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }
    public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
    {
        //�}�b�v���N���A����i�d�����Ȃ��悤�ɂ���j
        tilemap.ClearAllTiles();
        //�}�b�v�̕��̕��A���񂷂�
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            //�}�b�v�̍����̕��A���񂷂�
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                // 1 = �^�C������A0 = �^�C���Ȃ�
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }
    public static void UpdateMap(int[,] map, Tilemap tilemap) //�}�b�v�ƃ^�C���}�b�v���擾���Anull �^�C����K�v�ӏ��ɐݒ肷��
    {
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                //�ă����_�����O�ł͂Ȃ��A�}�b�v�̍X�V�݂̂��s��
                //����́A���ꂼ��̃^�C���i����яՓ˃f�[�^�j���ĕ`�悷��̂ɔ�ׂ�
                //�^�C���� null �ɍX�V����ق����g�p���\�[�X�����Ȃ��čςނ��߂ł��B
                if (map[x, y] == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }
    public static int[,] PerlinNoise(int[,] map, float seed)
    {
        int newPoint;
        //�p�[�����m�C�Y�̃|�C���g�̈ʒu�������邽�߂Ɏg�p�����
        float reduction = 0.5f;
        //�p�[�����m�C�Y�𐶐�����
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));

            //�����̔����̈ʒu�t�߂���m�C�Y���n�܂�悤�ɂ���
            newPoint += (map.GetUpperBound(1) / 2);
            for (int y = newPoint; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }
        return map;
    }

    public static int[,] PerlinNoiseSmooth(int[,] map, float seed, int interval)
    {
        //�m�C�Y�𕽊������Đ����̔z����ɕۑ�����
        if (interval > 1)
        {
            int newPoint, points;
            //�p�[�����m�C�Y�̃|�C���g�̈ʒu�������邽�߂Ɏg�p�����
            float reduction = 0.5f;

            //�������̃v���Z�X�Ŏg�p�����
            Vector2Int currentPos, lastPos;
            //�������̍ۂɑΉ�����|�C���g�ix �̃��X�g�� y �̃��X�g�� 1 ���j
            List<int> noiseX = new List<int>();
            List<int> noiseY = new List<int>();

            //�m�C�Y�𐶐�����
            for (int x = 0; x < map.GetUpperBound(0); x += interval)
            {
                newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, (seed * reduction))) * map.GetUpperBound(1));
                noiseY.Add(newPoint);
                noiseX.Add(x);
            }

            points = noiseY.Count;// 1 �ŊJ�n����̂Ŋ��ɒ��O�̈ʒu�����邱�ƂɂȂ�
            for (int i = 1; i < points; i++)
            {
                //���݂̈ʒu���擾����
                currentPos = new Vector2Int(noiseX[i], noiseY[i]);
                //���O�̈ʒu���擾����
                lastPos = new Vector2Int(noiseX[i - 1], noiseY[i - 1]);

                // 2 �̊Ԃ̍��ق���肷��
                Vector2 diff = currentPos - lastPos;

                //�����ύX�̒l��ݒ肷��
                float heightChange = diff.y / interval;
                //���݂̍�������肷��
                float currHeight = lastPos.y;

                //�Ō�� x ���猻�݂� x �܂ł̏������s��
                for (int x = lastPos.x; x < currentPos.x; x++)
                {
                    for (int y = Mathf.FloorToInt(currHeight); y > 0; y--)
                    {
                        map[x, y] = 1;
                    }
                    currHeight += heightChange;
                }
            }
        }
        else
        {
            //�f�t�H���g�ł͒ʏ�̃p�[�����m�C�Y�������g�p�����
            map = PerlinNoise(map, seed);
        }

        return map;

    }
    public static int[,] RandomWalkTop(int[,] map, float seed)
    {
        //�����̃V�[�h�l��^����
        System.Random rand = new System.Random(seed.GetHashCode());

        //�����̊J�n�l��ݒ肷��
        int lastHeight = Random.Range(0, map.GetUpperBound(1));

        //���̌J��Ԃ�����
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            //�R�C���𓊂���
            int nextMove = rand.Next(2);

            //�\�ŁA�ŉ����t�߂łȂ��ꍇ�́A��������������
            if (nextMove == 0 && lastHeight > 2)
            {
                lastHeight--;
            }
            //���ŁA�ŏ㕔�t�߂łȂ��ꍇ�́A�����𑝉�����
            else if (nextMove == 1 && lastHeight < map.GetUpperBound(1) - 2)
            {
                lastHeight++;
            }

            //���O�̍�������ŉ����܂ŌJ��Ԃ���������
            for (int y = lastHeight; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }
        //�}�b�v��߂�
        return map;
    }
    public static int[,] RandomWalkTopSmoothed(int[,] map, float seed, int minSectionWidth)
    {
        //�����̃V�[�h�l��^����
        System.Random rand = new System.Random(seed.GetHashCode());

        //�J�n�ʒu����肷��
        int lastHeight = Random.Range(0, map.GetUpperBound(1));

        //�ǂ̕����ɐi�ނ��̓���Ɏg�p�����
        int nextMove = 0;
        //���݂̃Z�N�V�������̔c���Ɏg�p�����
        int sectionWidth = 0;

        //�z��̕��ɂ����ď������s��
        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            //���̓�������肷��
            nextMove = rand.Next(2);

            //�Z�N�V�������̍ŏ����̒l���傫�����݂̍������g�p�����ꍇ�ɂ̂݁A������ύX����
            if (nextMove == 0 && lastHeight > 0 && sectionWidth > minSectionWidth)
            {
                lastHeight--;
                sectionWidth = 0;
            }
            else if (nextMove == 1 && lastHeight < map.GetUpperBound(1) && sectionWidth > minSectionWidth)
            {
                lastHeight++;
                sectionWidth = 0;
            }
            //�Z�N�V���������C���N�������g����
            sectionWidth++;

            //�������� 0 �܂ŏ������J��Ԃ�
            for (int y = lastHeight; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }

        //�C�����ꂽ�}�b�v��߂�
        return map;
    }
}

