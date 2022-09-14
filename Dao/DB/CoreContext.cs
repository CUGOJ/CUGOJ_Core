using System.Collections.Generic;
using System.Diagnostics;
using CUGOJ.RPC.Gen.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CUGOJ.CUGOJ_Core.Dao.DB;

public class CoreContext : DbContext
{
    public DbSet<ServiceInfo> ServiceInfos { get; set; } = null!;
    public DbSet<MiddleWareInfo> MiddleWareInfos { get; set; } = null!;
    public string DbPath { get; }
    public CoreContext()
    {
        DbPath = "./data/serviceManager.db";
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
    public static bool InitDB()
    {
        using var context = new CoreContext();
        if (!File.Exists(context.DbPath))
        {
            Console.WriteLine("正在初始化服务管理数据库");
            return context.Database.EnsureCreated();
        }
        return true;
    }
}

[Index(nameof(Type))]
public class MiddleWareInfo
{
    public enum MiddleWareType
    {
        Mysql,
        Redis,
        Neo4j,
        RabbitMQ,
    }

    public long Id { get; set; }
    public MiddleWareType Type { get; set; }
    public string Address { get; set; } = string.Empty;
    public long LastCheckTime { get; set; } = 0;
}

[Index(nameof(ServiceID))]
[Index(nameof(ServiceType))]
public class ServiceInfo
{
    public long Id { get; set; }
    public string ServiceID { get; set; } = string.Empty;
    public string ServiceIP { get; set; } = string.Empty;
    public string ServicePort { get; set; } = string.Empty;
    public ServiceTypeEnum ServiceType { get; set; } = ServiceTypeEnum.Core;
    public long RegisterTime { get; set; } = 0;
    public string Env { get; set; } = string.Empty;
    public string? TraceAddress { get; set; } = null;
    public string? LogAddress { get; set; } = null;
    public string? MysqlAddress { get; set; } = null;
    public string? RedisAddress { get; set; } = null;
    public string? RabbitMQAddress { get; set; } = null;
    public string? Neo4jAddress { get; set; } = null;
    public long LastTickTime { get; set; } = 0;
    public string? ConnectionString { get; set; } = null;
    public string? Token { get; set; } = null;
    public long ConnectCount { get; set; } = 0;
    public CUGOJ.RPC.Gen.Base.ServiceBaseInfo ToServiceBaseInfo()
    {
        return new ServiceBaseInfo
        {
            ServiceID = ServiceID,
            ServiceIP = ServiceIP,
            ServicePort = ServicePort,
            ServiceType = ServiceType,
            RegisterTime = RegisterTime,
            Env = Env,
            TraceAddress = TraceAddress,
            LogAddress = LogAddress,
            MysqlAddress = MysqlAddress,
            RedisAddress = RedisAddress,
            RabbitMQAddress = RabbitMQAddress,
            Neo4jAddress = Neo4jAddress,
        };
    }
}
