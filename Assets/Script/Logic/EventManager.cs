


using System;
using System.Collections.Generic;
using MyExtent;
using UnityEngine;

/// <summary>
/// 基本的事件系统，目的是让各个系统直接解耦合
/// </summary>
namespace MyEvent
{



    static class EventManager
    {
        public delegate void call_back_param(EVENT_BASE e);
        public delegate void call_back_void();
        private static Dictionary<Type, List<call_back_param>> map_param = new Dictionary<Type, List<call_back_param>>();
        private static Dictionary<Type, List<call_back_void>> map_void = new Dictionary<Type, List<call_back_void>>();
       
        /// <summary>
        /// 派发一个事件
        /// </summary>
        /// <param name="evnt"></param>
        public static void dispatch_event(EVENT_BASE evnt)
        {
            foreach (var f in map_param.GetOrNew(evnt.GetType()))
            {
                f(evnt);
            }
            foreach (var f in map_void.GetOrNew(evnt.GetType()))
            {
                f();
            }
        }


        /// <summary>
        /// 注册事件调用
        /// </summary>
        /// <typeparam name="T">事件种类</typeparam>
        /// <param name="func">调用函数</param>
        public static void add_listener<T>(call_back_param func)
         where T : EVENT_BASE
        {
            var f = map_param.GetOrNew(typeof(T));
            if (!f.Contains(func))
            {
                f.Add(func);
                map_param.Set(typeof(T), f);
            }
        }

        /// <summary>
        /// 注册事件调用
        /// </summary>
        /// <typeparam name="T">事件种类</typeparam>
        /// <param name="func">调用函数</param>
        public static void add_listener<T>(call_back_void func)
         where T : EVENT_BASE
        {
            var f = map_void.GetOrNew(typeof(T));
            if (!f.Contains(func))
            {
                f.Add(func);
                map_void.Set(typeof(T), f);
            }
        }

        /// <summary>
        /// 移除事件调用
        /// </summary>
        /// <typeparam name="T">事件种类</typeparam>
        /// <param name="func">调用函数</param>
        public static void remove_listener<T>(call_back_param func)
         where T : EVENT_BASE
        {
            var f = map_param.GetOrNew(typeof(T));
            if (f.Contains(func))
            {
                f.Remove(func);
                map_param.Set(typeof(T), f);
            }
        }

        /// <summary>
        /// 移除事件调用
        /// </summary>
        /// <typeparam name="T">事件种类</typeparam>
        /// <param name="func">调用函数</param>
        public static void remove_listener<T>(call_back_void func)
         where T : EVENT_BASE
        {
            var f = map_void.
                
                
                GetOrNew(typeof(T));
            if (f.Contains(func))
            {
                f.Remove(func);
                map_void.Set(typeof(T), f);
            }
        }

    }
    abstract class EVENT_BASE { 

        /// <summary>
        /// 转化为具体事件，添加了不为null的检测。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToDeliever<T>()
            where T:EVENT_BASE
        {
            T ret = (T)this;
            Debug.Assert(ret != null);
            return ret;
        }

    } //事件基类
    class EVENT_GAME_FINISHED : EVENT_BASE{
        public GamePlayInfo data;
        public int step;
        public EVENT_GAME_FINISHED()
        {
            data = new GamePlayInfo();
            step = 0;
        }
    }  //游戏胜利

    class EVENT_GAME_RESTART : EVENT_BASE {}//重新开始游戏
    class EVENT_BUTTON_CLICK : EVENT_BASE //按钮按下
    {
        public string button_label;
    }

    class EVENT_MOUSE_DRAG : EVENT_BASE  //鼠标拖动(全局)
    {
        public Vector2 offset;
    }

    class EVENT_GAME_OPERATE : EVENT_BASE  //发送游戏操作
    {
        public int direct;
    }

    class EVENT_GAME_OPERATOR_DONE : EVENT_BASE //完成游戏操作
    {
        public List<int> lst;
    }

    class EVENT_NEW_HISCORE_RECORDED : EVENT_BASE
    {
        public GamePlayInfo data;
    }

    class EVENT_HISCORE_DATA_LOADED : EVENT_BASE
    {
        public string lastname;
    }

    class EVENT_HISCORE_SCORELINE_CHANGED : EVENT_BASE
    {
        public GamePlayInfo last_hiscore;
    }
}

