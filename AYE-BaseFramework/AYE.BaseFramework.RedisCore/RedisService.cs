using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.RedisCore;

public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public string GetString(string key)
    {
        var db = _connectionMultiplexer.GetDatabase();
        return db.StringGet(key);
    }

    public void SetString(string key, string value)
    {
        var db = _connectionMultiplexer.GetDatabase();
        db.StringSet(key, value);
    }

    public void HashSet(string key, string hashField, string value)
    {
        var db = _connectionMultiplexer.GetDatabase();
        db.HashSet(key, hashField, value);
    }

    public string HashGet(string key, string hashField)
    {
        var db = _connectionMultiplexer.GetDatabase();
        return db.HashGet(key, hashField);
    }

    public void ListRightPush(string key, string value)
    {
        var db = _connectionMultiplexer.GetDatabase();
        db.ListRightPush(key, value);
    }

    public string ListLeftPop(string key)
    {
        var db = _connectionMultiplexer.GetDatabase();
        return db.ListLeftPop(key);
    }

    public void SetAdd(string key, string value)
    {
        var db = _connectionMultiplexer.GetDatabase();
        db.SetAdd(key, value);
    }

    public bool SetContains(string key, string value)
    {
        var db = _connectionMultiplexer.GetDatabase();
        return db.SetContains(key, value);
    }

    public void SortedSetAdd(string key, string value, double score)
    {
        var db = _connectionMultiplexer.GetDatabase();
        db.SortedSetAdd(key, value, score);
    }

    public string[] SortedSetRangeByRank(string key, long start = 0, long stop = -1)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var result = db.SortedSetRangeByRank(key, start, stop);
        return Array.ConvertAll(result, item => (string)item);
    }
}