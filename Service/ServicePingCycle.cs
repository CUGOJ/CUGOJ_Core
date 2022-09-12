using Thrift;
using Thrift.Transport;
using Thrift.Transport.Client;
using Thrift.Protocol;
using CUGOJ.RPC.Gen.Base;
using CUGOJ.CUGOJ_Tools.Tools;
using CUGOJ.CUGOJ_Tools.RPC;
namespace CUGOJ.CUGOJ_Core.Service;

public static partial class ServiceManager
{
    public class PingExcuter
    {
        public virtual long ExcutePing(TBaseClient client)
        {
            var method = client.GetType().GetMethod("Ping");
            if (method == null)
                throw new Exception("错误的Client");
            var pingReq = new PingRequest(CommonTools.UnixMili());
            pingReq.Base = RPCTools.NewBase();
            dynamic? pingResp = method.Invoke(client, new object?[] { pingReq, null });
            if (pingResp == null)
                return -1;
            if (pingResp is Task<PingResponse>)
            {
                pingResp.Wait();
                pingResp = pingResp.Result;
            }
            if (pingResp is PingResponse)
            {
                return ((PingResponse)pingResp).Timestamp;
            }
            return -1;
        }
    }
    private static PingExcuter _excuter = CUGOJ_Tools.Trace.TraceFactory.CreateTracableObject<PingExcuter>(false, false);
    private static Dictionary<string, TBaseClient> _clients = new();
    private static TBaseClient? GetClient(RPC.Gen.Base.ServiceBaseInfo service)
    {
        if (_clients.ContainsKey(service.ServiceID))
        {
            return _clients[service.ServiceID];
        }
        TTransport transport = new TSocketTransport(service.ServiceIP, Convert.ToInt32(service.ServicePort), new TConfiguration(), 1000);
        TProtocol protocol = new TBinaryProtocol(transport);
        TBaseClient? client = null;
        switch (service.ServiceType)
        {
            case RPC.Gen.Base.ServiceTypeEnum.Core:
                client = new CUGOJ.RPC.Gen.Services.Core.CoreService.Client(protocol);
                break;
            case RPC.Gen.Base.ServiceTypeEnum.Authentication:
                client = new CUGOJ.RPC.Gen.Services.Authentication.AuthenticationService.Client(protocol);
                break;
        }
        if (client != null) _clients[service.ServiceID] = client;
        return client;
    }

    private static object _servicePingLock = new();
    private static int _servicePingCount = 0;
    private static void ServicePingCycle()
    {
        if (_servicePingCount != 0) return;
        lock (_servicePingLock)
        {
            List<string> errorServiceList = new List<string>();
            _servicePingCount++;
            foreach (var service in _services)
            {
                try
                {
                    var client = GetClient(service);
                    if (client == null)
                        throw new Exception();
                    long ping = _excuter.ExcutePing(client);
                    if (ping == -1)
                        throw new Exception("Ping未响应");
                }
                catch (Exception e)
                {
                    Logger.Warn("服务连接丢失,service = {Service},exception message = {Message}", System.Text.Json.JsonSerializer.Serialize(service), e.Message);
                    Logger.Warn("服务已移除");
                    errorServiceList.Add(service.ServiceID);
                    _clients.Remove(service.ServiceID);
                }
            }
            _servicePingCount--;
            _services.RemoveAll((service) => errorServiceList.Contains(service.ServiceID));
        }
    }
}