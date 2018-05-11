/**
 * @date 2016-05-01
 * @author kenneth huang<kennethhuang@live.cn> 
 * 
 * 功 能： N/A
 * 描 述： 缓存操作
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

namespace KH.Cache.Redis
{
    public class Cache : ICache
    {
        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <returns></returns>
        public T GetCache<T>(string cacheKey) where T : class
        {
            return RedisCache.Get<T>(cacheKey);
        }
        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <param name="isNullHander">为空处理程序</param>
        /// <returns></returns>
        public T GetCache<T>(string cacheKey, Func<T> isNullHander) where T : class
        {
            var result = RedisCache.Get<T>(cacheKey);
            return result ?? isNullHander();
        }
        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="value">对象数据</param>
        /// <param name="cacheKey">键</param>
        public void WriteCache<T>(T value, string cacheKey) where T : class
        {
            //RedisCache.Set(cacheKey, value);
            //配置成与webcache相同时间
            WriteCache(value, cacheKey, DateTime.Now.AddMinutes(10));
        }
        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="value">对象数据</param>
        /// <param name="cacheKey">键</param>
        /// <param name="expireTime">到期时间</param>
        public void WriteCache<T>(T value, string cacheKey, DateTime expireTime) where T : class
        {
            RedisCache.Set(cacheKey, value, expireTime);
        }
        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        public void RemoveCache(string cacheKey)
        {
            RedisCache.Remove(cacheKey);
        }
        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public void RemoveCache()
        {
            RedisCache.RemoveAll();
        }
    }
}
