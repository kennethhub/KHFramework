/**
 * @date 2016-05-01
 * @author kenneth huang<kennethhuang@live.cn> 
 * 
 * 功 能： N/A
 * 描 述： 定义缓存接口
 * 
 * Ver    变更日期              负责人  变更内容
 * ────────────────────────────────
 * V0.01  2016-05-01 09:00:00   黄鑫    初版
 *
 * Copyright (c) Kenneth Huang www.kennethhuang.cn
 * All rights reserved.
 *┌───────────────────────────────┐
 *│　此技术信息未经本人书面同意禁止向第三方披露。                │
 *│　版权所有：Kenneth Huang　　　     　　　          　　　　　│
 *└───────────────────────────────┘
 */
using System;

namespace KH.Cache
{
    public interface ICache
    {
        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <returns></returns>
        T GetCache<T>(string cacheKey) where T : class;

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        /// <param name="isNullHander">为空处理程序</param>
        /// <returns></returns>
        T GetCache<T>(string cacheKey, Func<T> isNullHander) where T : class;

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="value">对象数据</param>
        /// <param name="cacheKey">键</param>
        void WriteCache<T>(T value, string cacheKey) where T : class;

        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="value">对象数据</param>
        /// <param name="cacheKey">键</param>
        /// <param name="expireTime">到期时间</param>
        void WriteCache<T>(T value, string cacheKey, DateTime expireTime) where T : class;

        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        void RemoveCache(string cacheKey);

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        void RemoveCache();
    }
}
