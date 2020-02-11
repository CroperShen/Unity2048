using System;
using System.Collections.Generic;
using System.Collections;
using MyEvent;
using UnityEngine;
using GlobalSettings;



class Playground : MonoBehaviour
{   
    public GameObject cell_prototypes = null;
    public GameObject grid_bg_prototypes = null;

    private bool game_running = true;
    private bool win_flag;
    private bool win_flag_checked;
    Cell[,] grid;

    public void Clear()
    {
        for (int i = 0; i < grid.GetLength(0); ++i)
        {
            for (int j = 0; j < grid.GetLength(1); ++j)
            {
                if (grid[i, j] == null)
                {
                    continue;
                }
                grid[i, j].level = -1;
                grid[i, j].PlayAnimation("hide");
                grid[i, j] = null;
            }
        }
    }

    public bool CoordValid(int x,int y)
    {
        return y >= 0 && y < grid.GetLength(0) && x >= 0 && x < grid.GetLength(1);
    }
    private void OnGameOperate(EVENT_BASE e)
    {
        var evnt = e.ToDeliever<EVENT_GAME_OPERATE>();
        if (DoGameOperator(evnt.direct))
        {
            if (JudgeWin())
            {
                EVENT_GAME_FINISHED e2 = new EVENT_GAME_FINISHED();
                e2.data.iswin = true;
                EventManager.dispatch_event(e2);
            }
            CreateCellRandom();
            if (JudgeFailure())
            {
                EVENT_GAME_FINISHED e2 = new EVENT_GAME_FINISHED();
                e2.data.iswin = false;
                EventManager.dispatch_event(e2);
            }
        }
    }

    //判断是否胜利
    private bool JudgeWin()
    {
        if (win_flag && !win_flag_checked)
        {
            win_flag_checked = true;
            return true;
        }
        return false;
    }

    //判断是否无路可走
    private bool JudgeFailure()
    {
        if (grid[0, 0] == null)
        {
            return false;
        }
        for (int i = 0; i < grid.GetLength(0); ++i)
        {
            for (int j = 0; j < grid.GetLength(1); ++j)
            {
                int x = j + 1;
                int y = i;
                if (CoordValid(x, y) &&  (grid[y,x]==null || grid[y,x].level==grid[i,j].level)) 
                {
                    return false;
                }

                x = j;
                y = i+1;
                if (CoordValid(x, y) && (grid[y, x] == null || grid[y, x].level == grid[i, j].level))
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void Awake()
    {
        grid = new Cell[4, 4];
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 4; ++j)
            {
                var bg = Instantiate(grid_bg_prototypes, transform).transform;
                bg.localPosition = new Vector3((i - 1.5f) * 2.5f, (j - 1.5f) * 2.5f, -0.1f);
            }
        }
        EventManager.add_listener<EVENT_GAME_OPERATE>(OnGameOperate);
        EventManager.add_listener<EVENT_GAME_RESTART>(OnGameRestart);
    }

    private void OnGameRestart()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        if (game_running)
        {
            game_running = false;
            win_flag = false;
            win_flag_checked = false;
            Clear();
            yield return new WaitForSeconds(0.5f);
            CreateCellRandom();
            CreateCellRandom();
            game_running = true;
        }

    }
    private Cell CreateCellAtPos(int i,int x=0,int y=0)
    {
        Cell ret = Instantiate(cell_prototypes,transform).GetComponent<Cell>();

        ret.Initialize();
        ret.level = i;
        ret.logical_X = x;
        ret.logical_Y = y;
        grid[y,x]= ret;
        ret.gameObject.SetActive(true);
        return ret;
    }

    private Cell CreateCellRandom()
    {
        List<Vector2> empty_grid = new List<Vector2>();
        for (int i = 0; i < grid.GetLength(0); ++i)
        {
            for (int j = 0; j < grid.GetLength(1); ++j)
            {
                if (grid[i, j] == null)
                {
                    empty_grid.Add(new Vector2(j, i));
                }
            }
        }
        if (empty_grid.Count == 0)
        {
            return null;
        }
        int n = UnityEngine.Random.Range(0, empty_grid.Count);
        int level= UnityEngine.Random.Range(1,3);
        return CreateCellAtPos(level,(int)empty_grid[n].x,(int)empty_grid[n].y);
    }

    static readonly int[,] d_arr = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } }; //sb C#居然不能定义常量数组

    //进行游戏操作的核心函数
    bool DoGameOperator(int d)
    {
        bool new_update = false;
        List<int> combine_list = new List<int>();
        for (int i = 0; i < grid.GetLength(0); ++i)
        {
            for (int j = 0; j < grid.GetLength(1); ++j)
            {
                int y = i;
                int x = j;
                if (d < 2)
                {
                    y = grid.GetLength(0) - 1 - i;
                    x = grid.GetLength(1) - 1 - j;
                }
                if (grid[y, x] == null)
                {
                    continue;
                }

                Cell c = grid[y, x];
                int nx = x+ d_arr[d, 0];
                int ny = y+ d_arr[d, 1];
                while (ny >= 0 && ny < grid.GetLength(0) && nx >= 0 && nx < grid.GetLength(1) && grid[ny, nx] == null)
                {
                    nx += d_arr[d, 0];
                    ny += d_arr[d, 1];
                }
                if (ny >= 0 && ny < grid.GetLength(0) && nx >= 0 && nx < grid.GetLength(1) && grid[ny, nx].level == grid[y, x].level) {
                    grid[ny, nx].level += 1;
                    combine_list.Add(grid[ny, nx].level);
                    if (grid[ny, nx].level >= 11)
                    {
                        win_flag = true;
                    }
                    grid[y, x].level = -1;
                    grid[y, x].logical_X = nx;
                    grid[y, x].logical_Y = ny;
                    grid[y, x] = null;
                    new_update = true;
                }
                else
                {
                    nx -= d_arr[d, 0];
                    ny -= d_arr[d, 1];
                    if (nx != x || ny != y)
                    {
                        grid[y, x].logical_Y = ny;
                        grid[y, x].logical_X = nx;
                        grid[ny, nx] = grid[y, x];
                        grid[y, x] = null;

                        new_update = true;
                    }

                }
                c.PlayAnimation("move");
            }
        }
        if (new_update)
        {
            EVENT_GAME_OPERATOR_DONE e = new EVENT_GAME_OPERATOR_DONE();
            e.lst = combine_list;
            EventManager.dispatch_event(e);
        }
        return new_update;
    }

    private void OnGUI()
    {
#pragma warning disable CS0162
        if (GlobalSetting.DebugMode)
        {
            return;
        }
        if (GUILayout.Button("创造"))
        {
           CreateCellRandom();
        }
        if (GUILayout.Button("胜利"))
        {
            win_flag = true;
            win_flag_checked = true;
            EVENT_GAME_FINISHED e = new EVENT_GAME_FINISHED();
            e.data.iswin = true;
            EventManager.dispatch_event(e);
        }
        if (GUILayout.Button("失败"))
        {
            EVENT_GAME_FINISHED e = new EVENT_GAME_FINISHED();
            e.data.iswin = false;
            EventManager.dispatch_event(e);
        }
    }
#pragma warning restore CS0162
}
