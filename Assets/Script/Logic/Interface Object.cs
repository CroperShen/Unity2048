using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using MyExtent;
using Debug= System.Diagnostics.Debug;

/// <summary>
/// 能播放简单动画的类
/// </summary>
/// <typeparam name="T"></typeparam>
abstract class AnimationObject<T> : MonoBehaviour
    where T:AnimationObject<T>
{
    private const BindingFlags BF_SelectAll = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private struct AnmInfo
    {
        public MethodInfo act_func; //动画播放时执行函数
        public MethodInfo end_func; //动画播放完毕执行函数
        public float length;  //默认播放时间
    }
    private static Dictionary<string, AnmInfo> anm_map = new Dictionary<string, AnmInfo>();
    static AnimationObject()
    {
        var v = new AnmInfo();
        v.length = 0.2f;
        anm_map.SetDefaultValue(v);
    }
    protected static void SetAnmLength(string anm_name, float length)
    {
        var tmp = anm_map.GetOrDefault(anm_name);
        tmp.length = length;
        anm_map.Set(anm_name, tmp);
    }
    protected static void SetAnmActFunc(string anm_name,string func_name)
    {

        MethodInfo func_info = (new StackTrace()).GetFrame(1).GetMethod().DeclaringType.GetMethod(func_name, BF_SelectAll);
        Debug.Assert(func_info != null);
        var tmp = anm_map.GetOrDefault(anm_name);
        tmp.act_func = func_info;
        anm_map.Set(anm_name, tmp);
    }

    protected static void SetAnmEndFunc(string anm_name, string func_name)
    {
        MethodInfo func_info = (new StackTrace()).GetFrame(1).GetMethod().DeclaringType.GetMethod(func_name, BF_SelectAll);
        Debug.Assert(func_info != null);
        var tmp = anm_map.GetOrDefault(anm_name);
        tmp.end_func = func_info;
        anm_map.Set(anm_name, tmp);
    }

    private AnmInfo anm_playing_info;
    private IEnumerator anm_playing_iem = null;
    protected float frame_length_factor
    {
        private set;
        get;
    }
    private void FinishAnimation()
    {
        if (anm_playing_iem != null)
        {
            anm_playing_iem = null;
            anm_playing_info.end_func?.Invoke(this,null);
        }
    }
    internal void PlayAnimation(string anm_name, float t = -1)
    {
        if (!anm_map.ContainsKey(anm_name))
        {
            return;
        }

        FinishAnimation();
        anm_playing_info = anm_map[anm_name];
        anm_playing_info.length = t > 0 ? t : anm_playing_info.length;
        anm_playing_iem = (IEnumerator)anm_playing_info.act_func?.Invoke(this,null);
        if (anm_playing_iem == null)
        {
            anm_playing_info.end_func?.Invoke(this,null);
        }
    }
    protected void UpdateAnimation(float delta_time=-1)
    {
        if (anm_playing_iem == null)
        {
            return;
        }
        delta_time = delta_time > 0 ? delta_time : Time.deltaTime;
        frame_length_factor = delta_time / anm_playing_info.length;
        frame_length_factor = frame_length_factor > 1 ? 1 : frame_length_factor;
        anm_playing_info.length -= delta_time;
        anm_playing_iem.MoveNext();

        if (anm_playing_info.length <= 0)
        {
            anm_playing_iem = null;
            anm_playing_info.end_func?.Invoke(this,null);
        }
    }
    private void Update()
    {
        UpdateAnimation(Time.deltaTime);
    }
}
abstract class InterfaceObject<T> : AnimationObject<InterfaceObject<T>>
{


    private Vector2 _scale_2d= new Vector2(1.0f, 1.0f);
    private Vector2 _localscale_2d;
    public Vector2 scale_2d
    {
        get
        {
            return _scale_2d;
        }
        set
        {
            _scale_2d = value;
            var tmp = _localscale_2d;
            tmp.Scale(_scale_2d);
            transform.localScale = new Vector3(tmp.x, tmp.y, 1);
        }
    }

    static InterfaceObject()
    {
        SetAnmActFunc("show", "AnmShowAct");
        SetAnmActFunc("hide", "AnmHideAct");
    }
    public void Initialize()
    {
        _localscale_2d = new Vector2(transform.localScale.x, transform.localScale.y);
        scale_2d = new Vector2(0, 0);
    }

    public IEnumerator AnmShowAct()
    {
        scale_2d = new Vector2(0f, 0f);
        yield return null;
        while (true)
        {
            scale_2d += (new Vector2(1, 1) - scale_2d) * frame_length_factor;
            yield return null;
        }
    }
    
    protected IEnumerator AnmHideAct()
    {
        yield return null;
        while (true)
        {
            scale_2d = scale_2d* (1-frame_length_factor);
            yield return null;
        }
    }
}

/// <summary>
/// 一局游戏的信息，用于统计以及排行榜
/// </summary>
struct GamePlayInfo
{
    public string name;   //玩家名称
    public int score;     //分数
    public bool iswin;    //是否已经胜利

    public int CompareTo(GamePlayInfo o)
    {
        if (iswin ^ o.iswin)
        {
            return iswin ? 1 : -1;
        }
        return score.CompareTo(o.score);
    }
}
