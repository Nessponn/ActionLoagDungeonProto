using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DigMaze;

public class CreateDigTilemap : MonoBehaviour
{
    [Range(5, 121)] public int Width;
    [Range(5, 121)] public int Height;
    [Range(1, 9)] public int thickness;

    public GameObject StageGrid;

    public List<GameObject> PresetMaps = new List<GameObject>();

    public TileBase Basetile;//�ǂ⏰�̌��ƂȂ�^�C��
    public TileBase Chaintile;//�A�����̐����ʒu�i�f�o�b�O�p�j
    public TileBase AddMaptile;//�v���}�b�v�̐����ʒu�i�f�o�b�O�p�j

    private Tilemap tilemap;

    private DigTilemap.MazeCreateor_Dig DT;

    private int[,] Maze;//���H�z��
    private int[,] EndPosition; //�I�[�ʒu�̏��

    private List<Vector3Int> SetMapPosition = new List<Vector3Int>();//�v���Z�b�g�}�b�v���Z�b�g����ʒu���

    // Start is called before the first frame update
    void Start()
    {
        //�^�C���}�b�v�̃R���|�[�l���g�擾
        tilemap = StageGrid.GetComponent<Tilemap>();
        tilemap.CompressBounds();//�}�b�v���ǂ̊��ł��K�������ɃZ�b�g�����悤�ɃZ�b�g�A�b�v

        //���H�̃T�C�Y���Z�b�g
        DT = new DigTilemap.MazeCreateor_Dig(Width, Height,thickness);

        //���H�f�[�^�̐���
        Maze = DT.CreateMaze();

        EndPosition = DT.EndPosition;

        //�^�C���}�b�v�̃T�C�Y����
        tilemap.size = new Vector3Int(DT.Width * DT.Thickness, DT.Height * DT.Thickness, tilemap.size.z);

        //�}�b�v�ɖ��H�f�[�^�̓K�p
        CellMap();

        //�^�C���}�b�v�̈ʒu����
        //tilemap.CompressBounds();
        StageGrid.transform.position = new Vector3(-tilemap.size.x / 2, -tilemap.size.y / 2, 0);
    }

    //���H�z����Z�b�g
    //�^�C���Z�b�g�̏����͌�ŏ���
    void CellMap()
    {
        var Cellbound = StageGrid.GetComponent<Tilemap>().cellBounds;

        //���H�̈ʒu�Q�Ɨp
        int mx,my;
        mx = my = 0;
       
        //�^�C���̐ݒu�A����������΁A�������f�[�^�̃^�C�������������A�������������^�C����傫������B
        for (int y = Cellbound.max.y - 1 - ((DT.Thickness - 1) / 2); y >= Cellbound.min.y; y -= DT.Thickness)
        {//���ォ��E���ɂ����ă^�C�����č�����

            for (int x = Cellbound.min.x + ((DT.Thickness - 1) / 2); x < Cellbound.max.x; x += DT.Thickness)
            {
                //�ǁi�P�j�ł���΁A���߂�
                if (Maze[mx, my] == 1)
                {
                    //�w�蕝���P�ȏ�w�肵�Ă���΁A��������߂�
                    for (int ey = y + ((DT.Thickness - 1) / 2); ey >= y - ((DT.Thickness - 1) / 2); ey--)
                    {
                        for (int ex = x - ((DT.Thickness - 1) / 2); ex <= x + ((DT.Thickness - 1) / 2); ex++)
                        {
                            //�Q�Ƃ���u���b�N
                            var position = new Vector3Int(ex, ey, 0);
                            tilemap.SetTile(position, Basetile);
                        }
                    }
                }

                if(EndPosition[mx,my] == 2)
                {
                    //�Q�Ƃ���u���b�N
                    var position = new Vector3Int(x, y, 0);
                    tilemap.SetTile(position, Chaintile);//�f�o�b�O�p 

                    //�w�肳�ꂽ�ʒu�𒆐S�Ƀv���Z�b�g�}�b�v��ݒu������ɓ����
                    SetMapPosition.Add(position);
                }

                mx++;
            }

            mx = 0;
            my++;
        }
        
        //�}�b�v�Ƀv���Z�b�g�}�b�v��ݒu����
        TranscriptionMaps(Cellbound);
    }

    //�Z�b�g�����v���Z�b�g�}�b�v���}�b�v�ɓ]�ʂ���
    private void TranscriptionMaps(BoundsInt Cellbound)
    {
        while (true)
        {
            int index = Random.Range(0, PresetMaps.Count);
            var PreCell = PresetMaps[index].GetComponent<Tilemap>();
            PreCell.CompressBounds();
            var PreCellbound = PresetMaps[index].GetComponent<Tilemap>().cellBounds;



            //�v���}�b�v�̃u���b�N�̈ʒu�Q��
            int mx, my;
            my = PreCellbound.max.y - 1;
            mx = PreCellbound.min.x;

            int posindex = Random.Range(0, SetMapPosition.Count);
            var Setpos = SetMapPosition[posindex];//���H�̐����̃}�b�v�ݒu�ʒu
            tilemap.SetTile(Setpos, AddMaptile);

            PresetMaps.RemoveAt(index);
            SetMapPosition.RemoveAt(index);

            //Vector3Int[,] PrePosition = new Vector3Int[PreCell.size.x, PreCell.size.y];
            TileBase[,] PreTile = new TileBase[PreCell.size.x, PreCell.size.y];

            //�v���Z�b�g�}�b�v�̒��g������
            bool EraseTile = false;

            for (int y = Setpos.y + PreCellbound.max.y - 1; y >= Setpos.y + PreCellbound.min.y; y--)
            {
                var LeftWallposition = LeftWallPosition(PreCell, my);
                var RightWallposition = RightWallPosition(PreCell, my);

                for (int x = Setpos.x + PreCellbound.min.x; x < Setpos.x + PreCellbound.max.x; x++)
                {



                    //Debug.Log("LeftWallposition = " + LeftWallposition);
                    //Debug.Log("RightWallposition = " + RightWallposition);
                    if (EraseTile)
                    {
                        var position = new Vector3Int(x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0);
                        tilemap.SetTile(position, null);

                        //LeftWallPosition ������LWP
                        /*var LWP = new Vector3Int(LeftWallposition.x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0);

                        var RWP = new Vector3Int(LeftWallposition.x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0);
                        Debug.Log("LeftWallposition.x + 2 - 1 = " + (LeftWallposition.x + 2 - 1)
                        + "\nLeftWallposition.y + y - 2 - (PreCell.size.y % 2) + 1 = " + ( y - 2 - (PreCell.size.y % 2) + 1)
                        + "\nx + 2 + (PreCell.size.x % 2) - 1 = " + (x + 2 + (PreCell.size.x % 2) - 1)
                        + "\ny - 2 - (PreCell.size.y % 2) + 1 = " + (y - 2 - (PreCell.size.y % 2) + 1));
                        if (LWP == position)
                        {
                            Debug.Log("�����Ă�");
                        }*/



                        //Debug.Log("position = " + position);
                        //���蔲�������̃}�b�v��o�^
                        //PrePosition[mx, my] = position;
                        //if(TileCheck(PreCellbound, PreCell, new Vector3Int(mx, my, 0))) PreTile[mx, my] = AddMaptile;
                    }

                    //�Q�Ƃ���v���}�b�v�̃u���b�N�̈ʒu�Ƀu���b�N�����邩�`�F�b�N
                    if (TileCheck(PreCellbound, PreCell, new Vector3Int(mx, my, 0)))
                    {
                        var position = new Vector3Int(x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0);

                        /*//�ݒu����ʒu�Ƀ^�C���}�b�v�̃u���b�N�����łɂ���΁A����
                        if (!tilemap.GetTile(position))
                        {
                            tilemap.SetTile(position, null);
                        }
                        else
                        {
                            tilemap.SetTile(position, AddMaptile);
                        }*/

                        //�����}�b�v��]�ʂ��邾���Ȃ炱�̈�s���A�N�e�B�u���A���if�����R�����g�A�E�g����
                        tilemap.SetTile(position, AddMaptile);

                    }

                    //�v���Z�b�g�}�b�v�̒��g���������߂̐^�U�l��ݒ�
                    /*if (LeftWallposition == new Vector3Int(x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0))
                    {
                        EraseTile = true;
                    }

                    if(RightWallposition == new Vector3Int(x + 2 + (PreCell.size.x % 2) - 1, y - 2 - (PreCell.size.y % 2) + 1, 0))
                    {
                        EraseTile = false;
                    }
    */
                    if (!TileCheck(PreCellbound, PreCell, new Vector3Int(mx + 1, my, 0)))
                    {
                        EraseTile = true;
                    }
                    else
                    {
                        EraseTile = false;
                    }


                    mx++;
                }
                //�ݒ�����Z�b�g����
                EraseTile = false;

                mx = PreCellbound.min.x;
                my--;
            }

            if (PresetMaps.Count <= 0 || SetMapPosition.Count <= 0) break;
        }

        /*for (int y = Cellbound.max.y - 1 - ((DT.Thickness - 1) / 2); y >= Cellbound.min.y; y -= DT.Thickness)
        {//���ォ��E���ɂ����ă^�C�����č�����

            for (int x = Cellbound.min.x + ((DT.Thickness - 1) / 2); x < Cellbound.max.x; x += DT.Thickness)
            {
                var position = new Vector3Int(x, y, 0);

                if (Setpos == position)
                {
                    for (int ey = PreCellbound.max.y - 1; y >= PreCellbound.min.y; y--)
                    {//���ォ��E���ɂ����ă^�C�����č�����

                        for (int ex = PreCellbound.min.x; x < PreCellbound.max.x; x++)
                        {
                            var Preposition = new Vector3Int(ex, ey, 0);
                            tilemap.SetTile(Preposition, Basetile);
                        }
                    }
                    *//*for (int ey =  PreCellbound.max.y - y; ey >= PreCellbound.min.y; ey--)
                    {
                        for (int ex = PreCellbound.min.x + x; ex <= PreCellbound.max.x; ex++)
                        {
                            //�Q�Ƃ���u���b�N
                            //var position = new Vector3Int(ex, ey, 0);
                            tilemap.SetTile(position, Basetile);
                        }
                    }*//*
                }


                mx++;
            }

            mx = 0;
            my++;
        }*/
    }

    private bool TileCheck(BoundsInt Prebound, Tilemap Pretilemap,Vector3Int position)
    {
        TileBase tile = Pretilemap.GetTile(position);

        if (tile != null) return true;
        return false;
    }

    //�Q�Ƃ���s�́A�����̕ǂ����o����
    private Vector3Int LeftWallPosition(Tilemap PreTilemap,int y)
    {
        var bound = PreTilemap.cellBounds;

        for (int x = bound.min.x; x < bound.max.x; ++x)
        {
            //�^�C���̎擾
            TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
            TileBase tile2 = tilemap.GetTile(new Vector3Int(x + 1, y, 0));

            if (tile != null && tile2 == null)
            {
                return new Vector3Int(x, y, 0);
            }
        }

        return new Vector3Int(999, 999, 0);
    }

    //�Q�Ƃ���s�́A�E���̕ǂ����o����
    private Vector3Int RightWallPosition(Tilemap PreTilemap, int y)
    {
        var bound = PreTilemap.cellBounds;

        for (int x = bound.max.x; x > bound.min.x; --x)
        {
            //�^�C���̎擾
            TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
            TileBase tile2 = tilemap.GetTile(new Vector3Int(x - 1, y, 0));

            if (tile != null && tile2 == null)
            {
                return new Vector3Int(x, y, 0);
            }
        }

        return new Vector3Int(999, 999, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
