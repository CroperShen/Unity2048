using System.Collections.Generic;
using System;
using UnityEngine;
namespace MyExtent
{
    //对字典的扩展
    public static class DictExtent
    {
        //为字典储存默认值
        private class DefaultValue<K, V>
        {
            public static Dictionary<Dictionary<K, V>, V> map = new Dictionary<Dictionary<K, V>, V>();
        }


        //从字典获取一个值，如果没有使用默认值，
        public static V GetOrDefault<K, V>(this Dictionary<K, V> d, K key)
        {
            if (d.ContainsKey(key))
            {
                return d[key];
            }
            if (DefaultValue<K, V>.map.ContainsKey(d))
            {
                return DefaultValue<K, V>.map[d];
            }
            return default(V);
        }

        //从字典获取一个值，如果没有就new一个
        public static V GetOrNew<K, V>(this Dictionary<K, V> d, K key)
            where V:new()
        {
            if (d.ContainsKey(key))
            {
                return d[key];
            }
            return new V();
        }

        //为字典设置值，如果没有就新建
        public static void Set<K, V>(this Dictionary<K, V> d, K key, V value)
        {
            if (d.ContainsKey(key))
            {
                d[key] = value;
            }
            else
            {
                d.Add(key, value);
            }

        }

        public static void SetDefaultValue<K,V>(this Dictionary<K,V> d,V value)
        {
            DefaultValue<K, V>.map.Set(d, value);
        }
    }

    public static class Extend_Class
    {
        public static void SetInRange<T>(this ref T t,T lowbound,T highbound)
            where T:struct,IComparable
        {
            if (t.CompareTo(lowbound) < 0)
            {
                t = lowbound;
            }
            if (t.CompareTo(highbound) > 0)
            {
                t = highbound;
            }
        }

        public static void SetMagnitude(this ref Vector2 v,float newlength)
        {
            var l = v.magnitude;
            if (l == 0)
            {
                v.x = newlength;
                return;
            }
            l = newlength / l;
            v.x *= l;
            v.y *= l;
        }
    }
}
