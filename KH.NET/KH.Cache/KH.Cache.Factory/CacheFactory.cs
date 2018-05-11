/**
 * @date 2016-05-01
 * @author kenneth huang<kennethhuang@live.cn> 
 * 
 * 功 能： N/A
 * 描 述： 缓存工厂
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
using KH.Common.Config;

namespace KH.Cache.Factory
{
    public class CacheFactory<T> where T : class, new()
    {
        /// <summary>
        /// 定义通用的Repository
        /// </summary>
        /// <returns></returns>
        public static ICache Cache()
        {
            //修改为支持Redis
            var cacheType = Config.GetValue("CacheType");
            switch (cacheType)
            {
                case "Redis":
                    return new Redis.Cache();
                case "WebCache":
                    return new Cache();
                default:
                    return new Cache();
            }
        }

        /// <summary>
        /// 获取缓存值，为空时设置缓存并返回值
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="cacheHandle">设置缓存值的委托方法</param>
        /// <param name="expireHours">过期时效，单位：小时 默认8小时</param>
        /// <returns></returns>
        public static T GetorNullSet(string key, Func<T> cacheHandle, int expireHours = 8)
        {
            var result = Cache().GetCache<T>(key);
            if (result != null) return result;

            result = cacheHandle();
            Cache().WriteCache(result, key, DateTime.Now.AddHours(expireHours));
            return result;
        }


        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        /// <param name="key">缓存键值</param>
        public static void Remove(string key)
        {
            Cache().RemoveCache(key);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void Remove()
        {
            Cache().RemoveCache();
        }
    }
}
