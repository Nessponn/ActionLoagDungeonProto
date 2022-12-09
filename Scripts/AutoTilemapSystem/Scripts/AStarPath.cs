using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class cellInfo
{
    public Vector3 pos { get; set; }        // �Ώۂ̈ʒu���
    public float cost { get; set; }         // ���R�X�g(���܂ŉ�����������)
    public float heuristic { get; set; }    // ����R�X�g(�S�[���܂ł̋���)
    public float sumConst { get; set; }     // ���R�X�g = ���R�X�g + ����R�X�g
    public Vector3 parent { get; set; }     // �e�Z���̈ʒu���
    public bool isOpen { get; set; }        // �����ΏۂƂȂ��Ă��邩�ǂ���
}

public class AStarPath : SingletonMonoBehaviourFast<AStarPath>
{
    //public Tilemap map;                     // �ړ��͈�
    public TileBase replaceTile;            // �ړ�����Ɉʒu����^�C���̐F��ウ��
    //public TileBase player;               // �v���C���[�̃Q�[���I�u�W�F�N�g
    //public TileBase enemy;                // �G�̃Q�[���I�u�W�F�N�g
    //private List<cellInfo> cellInfoList = new List<cellInfo>();    // �����Z�����L�����Ă������X�g
    private Vector3 goal;                   // �S�[���̈ʒu���
    private bool exitFlg;                   // �������I���������ǂ���

    // Start is called before the first frame update
    void Start()
    {
        //cellInfoList = new List<cellInfo>();

        //astarSearchPathFinding();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// AStar�A���S���Y���ł��B
    /// </summary>
    /*public void astarSearchPathFinding()
    {
        var maptile = map.GetComponent<Tilemap>();

        //���������
        var Cellbound = map.GetComponent<Tilemap>().cellBounds;

        // �X�^�[�g�̏���ݒ肷��(�X�^�[�g�͓G)
        cellInfo start = new cellInfo();
        //start.pos = map.WorldToCell(enemy.transform.position);
        start.cost = 0;
        //start.heuristic = Vector2.Distance(enemy.transform.position, goal);
        start.sumConst = start.cost + start.heuristic;
        start.parent = new Vector3(-9999, -9999, 0);    // �X�^�[�g���̐e�̈ʒu�͂��肦�Ȃ��l�ɂ��Ă����܂�
        start.isOpen = true;
        cellInfoList.Add(start);

        exitFlg = false;

        for (int y = Cellbound.max.y - 1; y >= Cellbound.min.y; y--)
        {//��������E��ɂ����ă^�C�����č�����

            for (int x = Cellbound.min.x; x < Cellbound.max.x; x++)
            {
                //�Q�Ƃ���u���b�N
                var position = new Vector3Int(x, y, 0);

                // �S�[���̓v���C���[�̈ʒu���
                if (map.GetTile(position) == player) goal = map.WorldToCell(position);
                if (map.GetTile(position) == enemy) start.pos = map.WorldToCell(position);
            }
        }

        start.heuristic = Vector2.Distance(start.pos, goal);

        //goal = player.transform.position;



        //�I�[�v�������݂�����胋�[�v
        //�����ΏۂƂ��āA�܂܂�Ă���}�X�ڂ��������A���ꂪ�I���̃}�X�ڂłȂ���Α���
        //�Ȃ��AcellInfo��while���ł�����������
        while (cellInfoList.Where(x => x.isOpen == true).Select(x => x).Count() > 0 && exitFlg == false)
        {
            //�ŏ��R�X�g�̃m�[�h��T��
            //�o�H�T���̑O�ɁA�����Ώۂ݂̂������Ƃ��A�T���ς݂̐i�s�����̐���R�X�g���A�ł����Ȃ����̂�I�o�B
            cellInfo minCell = cellInfoList.Where(x => x.isOpen == true).OrderBy(x => x.sumConst).Select(x => x).First();

            //�����Ώۂ̊J����s��
            openSurround(minCell);

            // ���S�̃m�[�h�����
            minCell.isOpen = false;
        }
    }*/

    //MapAutomizer���瓾��ꂽ�f�[�^����A�ǐ������s��
    public void astarSearchPathFinding(MapInfo mapInfo, Vector3Int Startpos, Vector3Int Goalpos , int dir)
    {
        List<cellInfo> cellInfoList = new List<cellInfo>();

        // �X�^�[�g�̏���ݒ肷��(�X�^�[�g�͓G)
        cellInfo start = new cellInfo();
        //start.pos = map.WorldToCell(enemy.transform.position);
        start.cost = 0;
        //start.heuristic = Vector2.Distance(enemy.transform.position, goal);
        start.sumConst = start.cost + start.heuristic;
        start.parent = new Vector3(-9999, -9999, 0);    // �X�^�[�g���̐e�̈ʒu�͂��肦�Ȃ��l�ɂ��Ă����܂�
        start.isOpen = true;

        //Debug.Log("map = " + map);
        //Debug.Log("posLeft = " + posLeft);

        cellInfoList.Add(start);

        exitFlg = false;

        goal = mapInfo.map.WorldToCell(Startpos);
        start.pos = mapInfo.map.WorldToCell(Goalpos);

        start.heuristic = Vector2.Distance(start.pos, goal);

        //�M�~�b�N�����p�ϐ�
        int Count = 0;
        List<Vector3Int> GimmickStartPos = new List<Vector3Int>() { new Vector3Int(-9999, -9999, 0) };
        List<Vector3Int> GimmickGoalPos = new List<Vector3Int>() { new Vector3Int(-9999, -9999, 0) };

        //�I�[�v�������݂�����胋�[�v
        //�����ΏۂƂ��āA�܂܂�Ă���}�X�ڂ��������A���ꂪ�I���̃}�X�ڂłȂ���Α���
        //�Ȃ��AcellInfo��while���ł�����������
        while (cellInfoList.Where(x => x.isOpen == true).Select(x => x).Count() > 0 && exitFlg == false)
        {
            //�ŏ��R�X�g�̃m�[�h��T��
            //�o�H�T���̑O�ɁA�����Ώۂ݂̂������Ƃ��A�T���ς݂̐i�s�����̐���R�X�g���A�ł����Ȃ����̂�I�o�B
            cellInfo minCell = cellInfoList.Where(x => x.isOpen == true).OrderBy(x => x.sumConst).Select(x => x).First();

            //�����Ώۂ̊J����s��
            //openSurround(minCell, map,cellInfoList , Count, GimmickStartPos, GimmickGoalPos);
            openSurround(minCell, cellInfoList,mapInfo);

            // ���S�̃m�[�h�����
            minCell.isOpen = false;
        }

        //�M�~�b�N�̐��������Ɉڂ�
        //MapAutomizer.Instance.Gimmick_Init(map,GimmickStartPos, GimmickGoalPos, dir);
    }
    /// <summary>
    /// ���ӂ̃Z�����J���܂�
    /// </summary>
    private void openSurround(cellInfo center, List<cellInfo> cellInfoList ,MapInfo mapInfo)
    {
        // �|�W�V������Vector3Int�֕ϊ�
        Vector3Int centerPos = mapInfo.map.WorldToCell(center.pos);

        //-1�`1�͈̔͂ŒT��
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // �㉺���E�̂݉Ƃ���A���A���S�͏��O
                if (((i != 0 && j == 0) || (i == 0 && j != 0)) && !(i == 0 && j == 0))
                {
                    //�i�s�����̊m��
                    //���Ȃ炸�㉺���E�̂ǂ��炩�ɐi��ł���
                    Vector3Int posInt = new Vector3Int(centerPos.x + i, centerPos.y + j, centerPos.z);

                    // ���X�g�ɑ��݂��Ȃ����T��
                    Vector3 pos = mapInfo.map.CellToWorld(posInt);
                    pos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z);
                    if (cellInfoList.Where(x => x.pos == pos).Select(x => x).Count() == 0)
                    {
                        // ���X�g�ɒǉ�
                        cellInfo cell = new cellInfo();
                        cell.pos = pos;
                        cell.cost = center.cost + 1;// ���R�X�g(���܂ŉ�����������)
                        cell.heuristic = Vector2.Distance(pos, goal);// ����R�X�g(�S�[���܂ł̋���)
                        cell.sumConst = cell.cost + cell.heuristic;//�����܂ł̎��ۂ̃R�X�g
                        cell.parent = center.pos;
                        cell.isOpen = true;
                        cellInfoList.Add(cell);

                        /*//���ȏ�A���ɘA���Ői��ł���΁A���̒��Ԃ̈ʒu�ɃM�~�b�N������
                        //�n�_�ƏI�_�����߂�
                        if(i == 0 && j == -1)
                        {
                            if(GimmickStartPos[Count] == new Vector3Int(-9999, -9999, 0))
                            {
                                GimmickStartPos[Count] = map.WorldToCell(pos);
                            }
                        }
                        //���ȊO�ɐi�񂾂�A�J�E���g��
                        else
                        {
                            if (GimmickStartPos[Count] != new Vector3Int(-9999, -9999, 0))
                            {
                                //�����r�؂ꂽ�n�_���S�[���Ƃ��ēo�^����
                                GimmickGoalPos[Count] = map.WorldToCell(pos);

                                Count++;
                                GimmickStartPos.Add(new Vector3Int(-9999, -9999, 0));
                                GimmickGoalPos.Add(new Vector3Int(-9999, -9999, 0));
                            }
                            
                        }*/

                        // �S�[���̈ʒu�ƈ�v������I��
                        if (mapInfo.map.WorldToCell(goal) == mapInfo.map.WorldToCell(pos))
                        {
                            cellInfo preCell = cell;


                            //�Б��̕������̎�����z��
                            if(mapInfo.GimmickStartPosx[0] == new Vector3Int(-9999, -9999, 0))
                            {
                                mapInfo.GimmickStartPosx[0] = mapInfo.map.WorldToCell(preCell.pos);
                            }

                            if (mapInfo.GimmickStartPosy[0] == new Vector3Int(-9999, -9999, 0))
                            {
                                mapInfo.GimmickStartPosy[0] = mapInfo.map.WorldToCell(preCell.pos);
                            }

                            while (preCell.parent != new Vector3(-9999, -9999, 0))
                            {
                                //Debug.Log(mapInfo.map.WorldToCell(preCell.pos));

                                //�}�X�̐ݒu
                                mapInfo.map.SetTile(mapInfo.map.WorldToCell(preCell.pos), replaceTile);

                                //�ݒu�����^�C���̏��ێ�
                                var prevCell = preCell;

                                //���ɐݒu����^�C���̏��
                                preCell = cellInfoList.Where(x => x.pos == preCell.parent).Select(x => x).First();

                                //�M�~�b�N��������
                                //�O�̃}�X��x�����������A�����ł��������؂�
                                if(prevCell.pos.x != preCell.pos.x)
                                {
                                    //Debug.Log("�Ȃ�����");

                                    //���݂̃}�X�ڂœo�^
                                    mapInfo.GimmickGoalPosy.Add(mapInfo.map.WorldToCell(prevCell.pos));

                                    //���̃}�X�ڂ��A�V���ȃX�^�[�g�n�_�Ƃ��ēo�^
                                    mapInfo.GimmickStartPosy.Add(mapInfo.map.WorldToCell(preCell.pos));
                                }

                                if (prevCell.pos.y != preCell.pos.y)
                                {
                                    //Debug.Log("�Ȃ�����");

                                    //���݂̃}�X�ڂœo�^
                                    mapInfo.GimmickGoalPosx.Add(mapInfo.map.WorldToCell(prevCell.pos));

                                    //���̃}�X�ڂ��A�V���ȃX�^�[�g�n�_�Ƃ��ēo�^
                                    mapInfo.GimmickStartPosx.Add(mapInfo.map.WorldToCell(preCell.pos));
                                }
                            }
                            

                            exitFlg = true;
                            return;
                        }
                    }

                }
            }
        }
    }

}