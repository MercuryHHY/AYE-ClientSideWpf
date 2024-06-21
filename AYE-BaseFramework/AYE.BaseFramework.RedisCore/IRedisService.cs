using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.RedisCore;

public interface IRedisService
{
    string GetString(string key);
    void SetString(string key, string value);

    // 哈希操作
    void HashSet(string key, string hashField, string value);
    string HashGet(string key, string hashField);

    // 列表操作
    void ListRightPush(string key, string value);
    string ListLeftPop(string key);

    // 集合操作
    void SetAdd(string key, string value);
    bool SetContains(string key, string value);

    // 有序集合操作
    void SortedSetAdd(string key, string value, double score);
    string[] SortedSetRangeByRank(string key, long start = 0, long stop = -1);
}
