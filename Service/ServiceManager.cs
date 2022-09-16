using System.Threading;
using System;
using System.Data.Entity;
using CUGOJ.RPC.Gen.Base;
using CUGOJ.CUGOJ_Tools.Log;
using CUGOJ.CUGOJ_Tools.RPC;
using CUGOJ.CUGOJ_Core.Dao.DB;
using CUGOJ.RPC.Gen.Services.Core;
namespace CUGOJ.CUGOJ_Core.Service;

public static partial class ServiceManager
{
    public static async Task<RPCRegisterInfo> RegisterNewService(ServiceTypeEnum serviceType)
    {
        using var context = new CoreContext();
        try
        {
            ServiceInfo serviceInfo = new()
            {
                ServiceID = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString(),
                Env = Context.ServiceBaseInfo.Env,
                TraceAddress = Context.ServiceBaseInfo.TraceAddress,
                LogAddress = Context.ServiceBaseInfo.LogAddress,
                ServiceType = serviceType,
            };
            context.ServiceInfos.Add(serviceInfo);
            await context.SaveChangesAsync();
            return new()
            {
                ServiceID = serviceInfo.ServiceID,
                CoreIP = Context.ServiceBaseInfo.ServiceIP,
                CorePort = int.Parse(Context.ServiceBaseInfo.ServicePort),
                Token = serviceInfo.Token,
            };
        }
        catch (Exception e)
        {
            Logger.Error("[严重错误]注册新服务出错,未能连接到Sqlite数据库,Error = {error}", e);
            throw new Exception("未能连接到Sqlite数据库");
        }

    }

    public static void OnPing(long startTime)
    {
        using var context = new CoreContext();
        try
        {
            if (Context.ClientIP == null)
            {
                Logger.Warn("收到了未知IP的服务连接请求,ServiceID={ServiceID}", Context.ServiceID);
                return;
            }
            var serviceInfo = (
                from s in context.ServiceInfos
                where s.ServiceID == Context.ServiceID
                select s
            ).FirstOrDefault();
            if (serviceInfo == null)
            {
                Logger.Warn("收到了未注册的服务连接请求,ServiceID={ServiceID}", Context.ServiceID);
                return;
            }
            if (serviceInfo.ServiceIP != Context.ClientIP)
            {
                Logger.Warn("收到了已注册服务不一致IP的请求,ServiceID={ServiceID},注册时的IP={IP},本次请求的IP={CurIP}", Context.ServiceID, serviceInfo.ServiceIP, Context.ClientIP);
                return;
            }
            if ((CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili() - serviceInfo.LastTickTime) > 7000)
            {
                Logger.Info("服务已重连,ServiceID={ServiceID},服务地址 {IP}:{Port}", Context.ServiceID, Context.ClientIP, serviceInfo.ServicePort);
            }
            serviceInfo.LastTickTime = CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili();
            serviceInfo.ConnectCount++;
            context.Update(serviceInfo);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            Logger.Error("[严重错误]注册新服务出错,未能连接到Sqlite数据库,Error = {error}", e);
            throw new Exception("未能连接到Sqlite数据库");
        }
    }

    public static ServiceBaseInfo? RegisterService(RPCRegisterInfo registerInfo, int port)
    {
        using var context = new CoreContext();
        try
        {
            if (Context.ClientIP == null)
            {
                Logger.Warn("收到了未知IP的服务连接请求,ServiceID={ServiceID}", Context.ServiceID);
                return null;
            }
            var serviceInfo = (
                from s in context.ServiceInfos
                where s.ServiceID == Context.ServiceID
                select s
            ).FirstOrDefault();
            if (serviceInfo == null)
            {
                Logger.Warn("收到了未注册的服务连接请求,ServiceID={ServiceID}", Context.ServiceID);
                return null;
            }
            if (serviceInfo.ConnectCount == 0)
            {
                serviceInfo.ServiceIP = Context.ClientIP;
                serviceInfo.ServicePort = port.ToString();
                serviceInfo.LastTickTime = CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili();
                serviceInfo.ConnectCount = 1;
                serviceInfo.RegisterTime = CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili();
                context.Update(serviceInfo);
                context.SaveChanges();
                Logger.Info("服务已连接,ServiceID={ServiceID},服务地址 {IP}:{Port}", Context.ServiceID, Context.ClientIP, port);
            }
            else
            {
                serviceInfo.ServicePort = port.ToString();
                serviceInfo.LastTickTime = CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili();
                serviceInfo.ConnectCount++;
                context.Update(serviceInfo);
                context.SaveChanges();
                Logger.Info("服务已重连,ServiceID={ServiceID},服务地址 {IP}:{Port}", Context.ServiceID, Context.ClientIP, port);
            }
            return serviceInfo.ToServiceBaseInfo();
        }
        catch (Exception e)
        {
            Logger.Error("[严重错误]注册新服务出错,未能连接到Sqlite数据库,Error = {error}", e);
            throw new Exception("未能连接到Sqlite数据库");
        }
    }

    public static IEnumerable<ServiceBaseInfo> DiscoverServices(ServiceTypeEnum serviceType)
    {
        using var context = new CoreContext();
        try
        {
            return (from s in context.ServiceInfos
                    where s.ServiceType == serviceType
                    && (CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili() - s.LastTickTime) <= 7000
                    select s.ToServiceBaseInfo()).ToList();
        }
        catch (Exception e)
        {
            Logger.Error("[严重错误]注册新服务出错,未能连接到Sqlite数据库,Error = {error}", e);
            throw new Exception("未能连接到Sqlite数据库");
        }
    }
    public record CoreProperties
    {
        public string ServiceID { get; set; } = string.Empty;
        public string Env { get; set; } = string.Empty;
        public string ServiceIP { get; set; } = string.Empty;
        public string? LogAddress { get; set; } = string.Empty;
        public string? TraceAddress { get; set; } = string.Empty;
        public string? MysqlAddress { get; set; } = string.Empty;
        public string? RedisAddress { get; set; } = string.Empty;
        public string? RabbitMQAddress { get; set; } = string.Empty;
        public string? Neo4jAddress { get; set; } = string.Empty;
    }

    public static CoreProperties? LoadProperties()
    {
        try
        {
            var context = new CoreContext();
            var serviceInfo = (from s in context.ServiceInfos where s.Id == 1 select s).FirstOrDefault();
            if (serviceInfo == null)
                return null;
            return new CoreProperties()
            {
                ServiceID = serviceInfo.ServiceID,
                Env = serviceInfo.Env,
                ServiceIP = serviceInfo.ServiceIP,
                LogAddress = serviceInfo.LogAddress,
                TraceAddress = serviceInfo.TraceAddress,
                MysqlAddress = serviceInfo.MysqlAddress,
                RedisAddress = serviceInfo.RedisAddress,
                RabbitMQAddress = serviceInfo.RabbitMQAddress,
                Neo4jAddress = serviceInfo.Neo4jAddress
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("未能连接到Sqlite数据库,Error = {error}", e);
            return null;
        }
    }

    public static void RegisterSelf()
    {
        using var context = new CoreContext();
        try
        {
            var serviceInfo = (
               from s in context.ServiceInfos
               where s.ServiceID == Context.ServiceBaseInfo.ServiceID
               select s
           ).FirstOrDefault();
            if (serviceInfo == null || serviceInfo.ConnectCount == 0)
            {
                serviceInfo = new ServiceInfo();
                serviceInfo.Id = 1;
                serviceInfo.ServiceID = Context.ServiceBaseInfo.ServiceID;
                serviceInfo.ServiceIP = Context.ServiceBaseInfo.ServiceIP;
                serviceInfo.ServicePort = Context.ServiceBaseInfo.ServicePort;
                serviceInfo.LastTickTime = CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili();
                serviceInfo.ConnectCount = 1;
                serviceInfo.RegisterTime = CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili();
                serviceInfo.ServiceType = ServiceTypeEnum.Core;
                serviceInfo.Env = Context.ServiceBaseInfo.Env;
                serviceInfo.TraceAddress = Context.ServiceBaseInfo.TraceAddress;
                serviceInfo.LogAddress = Context.ServiceBaseInfo.LogAddress;
                serviceInfo.MysqlAddress = Context.ServiceBaseInfo.MysqlAddress;
                serviceInfo.RedisAddress = Context.ServiceBaseInfo.RedisAddress;
                serviceInfo.RabbitMQAddress = Context.ServiceBaseInfo.RabbitMQAddress;
                serviceInfo.Neo4jAddress = Context.ServiceBaseInfo.Neo4jAddress;
                context.Add(serviceInfo);
                context.SaveChanges();
                Logger.Info("Core服务已注册:{Service}", serviceInfo);
            }
            else
            {
                serviceInfo.ServicePort = Context.ServiceBaseInfo.ServicePort;
                serviceInfo.LastTickTime = CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili();
                serviceInfo.ConnectCount++;
                context.Update(serviceInfo);
                context.SaveChanges();
                Logger.Info("Core服务信息已更新");
            }
        }
        catch (Exception e)
        {
            Logger.Error("[严重错误]注册新服务出错,未能连接到Sqlite数据库,Error = {error}", e);
            throw new Exception("未能连接到Sqlite数据库");
        }
    }

    public static List<ServiceBaseInfo> GetAllServices()
    {
        try
        {
            using var context = new CoreContext();
            return (from s in context.ServiceInfos select s.ToServiceBaseInfo()).ToList();
        }
        catch (Exception e)
        {
            Logger.Error("获取服务信息失败:{Error}", e);
            return new List<ServiceBaseInfo>();
        }
    }

    public static bool SetupService(string ServiceID, SetupServiceTypeEnum setupServiceType, string value)
    {
        try
        {
            using var context = new CoreContext();
            var service = (from s in context.ServiceInfos where s.ServiceID == ServiceID select s).FirstOrDefault();
            if (service == null)
            {
                Logger.Warn("修改未注册的服务配置,ServiceID={ServiceID},{Type}={Value}", ServiceID, setupServiceType, value);
                return false;
            }
            switch (setupServiceType)
            {
                case SetupServiceTypeEnum.Log:
                    service.LogAddress = value;
                    break;
                case SetupServiceTypeEnum.Trace:
                    service.TraceAddress = value;
                    break;
                case SetupServiceTypeEnum.Mysql:
                    service.MysqlAddress = value;
                    break;
                case SetupServiceTypeEnum.Redis:
                    service.RedisAddress = value;
                    break;
                case SetupServiceTypeEnum.RabbitMQ:
                    service.RabbitMQAddress = value;
                    break;
                case SetupServiceTypeEnum.Neo4j:
                    service.Neo4jAddress = value;
                    break;
            }
            context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Logger.Error("配置服务时出现问题:{Error}", e);
            return false;
        }
    }
}