/**
 * @date 2016-05-01
 * @author kenneth huang<kennethhuang@live.cn> 
 * 
 * 功 能： N/A
 * 描 述： Redis缓存帮助文件
 * 
 * Ver    变更日期              负责人  变更内容
 * ────────────────────────────────
 * V0.01  2016-05-01 10:00:00   黄鑫    初版
 *
 * Copyright (c) Kenneth Huang www.kennethhuang.cn
 * All rights reserved.
 *┌───────────────────────────────┐
 *│　此技术信息未经本人书面同意禁止向第三方披露。                │
 *│　版权所有：Kenneth Huang　　　     　　　          　　　　　│
 *└───────────────────────────────┘
 */
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;

namespace KH.Cache.Redis
{
    public class RedisCache
    {
        #region 连接信息

        /// <summary>
        /// redis配置文件信息
        /// </summary>
        private static readonly RedisConfigInfo redisConfigInfo = RedisConfigInfo.GetConfig();
        private static PooledRedisClientManager prcm;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        static RedisCache()
        {
            CreateManager();
        }
        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private static void CreateManager()
        {
            var writeServerList = SplitString(redisConfigInfo.WriteServerList, ",");
            var readServerList = SplitString(redisConfigInfo.ReadServerList, ",");

            prcm = new PooledRedisClientManager(readServerList, writeServerList,
                             new RedisClientManagerConfig
                             {
                                 MaxWritePoolSize = redisConfigInfo.MaxWritePoolSize,
                                 MaxReadPoolSize = redisConfigInfo.MaxReadPoolSize,
                                 AutoStart = redisConfigInfo.AutoStart,
                             });
        }
        private static IEnumerable<string> SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }

        #endregion

        #region Item

        /// <summary>
        /// 设置单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool Set<T>(string key, T t)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.Set(key, t);
            }
        }

        /// <summary>
        /// 设置单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool Set<T>(string key, T t, TimeSpan timeSpan)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.Set(key, t, timeSpan);
            }
        }
        /// <summary>
        /// 设置单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool Set<T>(string key, T t, DateTime dateTime)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.Set(key, t, dateTime);
            }
        }

        /// <summary>
        /// 获取单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class
        {
            using (var redis = prcm.GetClient())
            {
                return redis.Get<T>(key);
            }
        }

        /// <summary>
        /// 移除单体
        /// </summary>
        /// <param name="key"></param>
        public static bool Remove(string key)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.Remove(key);
            }
        }
        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public static void RemoveAll()
        {
            using (var redis = prcm.GetClient())
            {
                redis.FlushAll();
            }
        }

        #endregion

        #region List

        public static void List_Add<T>(string key, T t)
        {
            using (var redis = prcm.GetClient())
            {

                var redisTypedClient = redis.As<T>();
                redisTypedClient.AddItemToList(redisTypedClient.Lists[key], t);
            }
        }



        public static bool List_Remove<T>(string key, T t)
        {
            using (var redis = prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.RemoveItemFromList(redisTypedClient.Lists[key], t) > 0;
            }
        }
        public static void List_RemoveAll<T>(string key)
        {
            using (var redis = prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.Lists[key].RemoveAll();
            }
        }

        public static long List_Count(string key)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.GetListCount(key);
            }
        }

        public static List<T> List_GetRange<T>(string key, int start, int count)
        {
            using (var redis = prcm.GetClient())
            {
                var c = redis.As<T>();
                return c.Lists[key].GetRange(start, start + count - 1);
            }
        }


        public static List<T> List_GetList<T>(string key)
        {
            using (var redis = prcm.GetClient())
            {
                var c = redis.As<T>();
                return c.Lists[key].GetRange(0, c.Lists[key].Count);
            }
        }

        public static List<T> List_GetList<T>(string key, int pageIndex, int pageSize)
        {
            var start = pageSize * (pageIndex - 1);
            return List_GetRange<T>(key, start, pageSize);
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void List_SetExpire(string key, DateTime datetime)
        {
            using (var redis = prcm.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }

        #endregion

        #region Set

        public static void Set_Add<T>(string key, T t)
        {
            using (var redis = prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.Sets[key].Add(t);
            }
        }
        public static bool Set_Contains<T>(string key, T t)
        {
            using (var redis = prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.Sets[key].Contains(t);
            }
        }
        public static bool Set_Remove<T>(string key, T t)
        {
            using (var redis = prcm.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.Sets[key].Remove(t);
            }
        }

        #endregion

        #region Hash

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Exist(string key, string dataKey)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.HashContainsEntry(key, dataKey);
            }
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool Hash_Set<T>(string key, string dataKey, T t)
        {
            using (var redis = prcm.GetClient())
            {
                var value = ServiceStack.Text.JsonSerializer.SerializeToString(t);
                return redis.SetEntryInHash(key, dataKey, value);
            }
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Remove(string key, string dataKey)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.RemoveEntryFromHash(key, dataKey);
            }
        }

        /// <summary>
        /// 移除整个hash
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Hash_Remove(string key)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.Remove(key);
            }
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static T Hash_Get<T>(string key, string dataKey)
        {
            using (var redis = prcm.GetClient())
            {
                var value = redis.GetValueFromHash(key, dataKey);
                return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
            }
        }

        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> Hash_GetAll<T>(string key)
        {
            using (var redis = prcm.GetClient())
            {
                var list = redis.GetHashValues(key);
                if (list == null || list.Count <= 0) return null;

                return list.Select(ServiceStack.Text.JsonSerializer.DeserializeFromString<T>).ToList();
                //return list.Select(item => ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item)).ToList();
            }
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void Hash_SetExpire(string key, DateTime datetime)
        {
            using (var redis = prcm.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }

        #endregion

        #region SortedSet

        /// <summary>
        ///  添加数据到 SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="score"></param>
        public static bool SortedSet_Add<T>(string key, T t, double score)
        {
            using (var redis = prcm.GetClient())
            {
                var value = ServiceStack.Text.JsonSerializer.SerializeToString(t);
                return redis.AddItemToSortedSet(key, value, score);
            }
        }

        /// <summary>
        /// 移除数据从SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool SortedSet_Remove<T>(string key, T t)
        {
            using (var redis = prcm.GetClient())
            {
                var value = ServiceStack.Text.JsonSerializer.SerializeToString(t);
                return redis.RemoveItemFromSortedSet(key, value);
            }
        }

        /// <summary>
        /// 修剪SortedSet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="size">保留的条数</param>
        /// <returns></returns>
        public static long SortedSet_Trim(string key, int size)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.RemoveRangeFromSortedSet(key, size, 9999999);
            }
        }

        /// <summary>
        /// 获取SortedSet的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long SortedSet_Count(string key)
        {
            using (var redis = prcm.GetClient())
            {
                return redis.GetSortedSetCount(key);
            }
        }

        /// <summary>
        /// 获取SortedSet的分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetList<T>(string key, int pageIndex, int pageSize)
        {
            using (var redis = prcm.GetClient())
            {
                var list = redis.GetRangeFromSortedSet(key, (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                if (list == null || list.Count <= 0) return null;

                return list.Select(ServiceStack.Text.JsonSerializer.DeserializeFromString<T>).ToList();
            }
        }

        /// <summary>
        /// 获取SortedSet的全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetListALL<T>(string key)
        {
            using (var redis = prcm.GetClient())
            {
                var list = redis.GetRangeFromSortedSet(key, 0, 9999999);
                if (list == null || list.Count <= 0) return null;
                return list.Select(ServiceStack.Text.JsonSerializer.DeserializeFromString<T>).ToList();
            }
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void SortedSet_SetExpire(string key, DateTime datetime)
        {
            using (var redis = prcm.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }

        #endregion
    }
}
