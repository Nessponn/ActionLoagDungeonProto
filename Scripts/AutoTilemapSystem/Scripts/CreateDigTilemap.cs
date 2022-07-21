using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DigMaze;

public class CreateDigTilemap : MonoBehaviour
{
    [Range(5, 121)] public int Width;
    [Range(5, 121)] public int Height;
    [Range(1, 5)] public int thickness;

    public GameObject StageGrid;

    public TileBase Basetile;//�ǂ⏰�̌��ƂȂ�^�C��
    public TileBase Chaintile;//�A�����̐����ʒu�i�f�o�b�O�p�j

    private Tilemap tilemap;

    private DigTilemap.MazeCreateor_Dig DT;

    private int[,] Maze;//���H�z��
    private int[,] EndPosition; //�I�[�ʒu�̏��


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
    void CellMap()
    {
        var Celltilemap = StageGrid.GetComponent<Tilemap>();
        var Cellbound = StageGrid.GetComponent<Tilemap>().cellBounds;

        //���H�̈ʒu�Q�Ɨp
        int mx,my;
        mx = my = 0;

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
                    tilemap.SetTile(position, Chaintile);
                }

                mx++;
            }

            mx = 0;
            my++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
