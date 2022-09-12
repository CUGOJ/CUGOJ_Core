using CUGOJ.RPC.Gen.Base;
using CUGOJ.CUGOJ_Tools.Log;
namespace CUGOJ.CUGOJ_Core.Service;

public static partial class ServiceManager
{
    private class ConnectToken
    {
        public string Token = System.Guid.NewGuid().ToString();
        public DateTime Timeout = DateTime.Now.AddMinutes(10);

        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj is ConnectToken)
            {
                return ((ConnectToken)obj).Token == Token;
            }
            return base.Equals(obj);
        }
    }
    private static List<ServiceBaseInfo> _services = new();
    private static Dictionary<string, int> _misstime = new();
    private static Dictionary<string, ServiceBaseInfo> _servicesDict = new();
    private static HashSet<ConnectToken> _tokens = new();
    private static object _lockServiceManager = new();
    public static string GetConnectionString(ServiceTypeEnum serviceType)
    {
        lock (_lockServiceManager)
        {
            _tokens.RemoveWhere(x => x.Timeout < DateTime.Now);
            var token = new ConnectToken();
            _tokens.Add(token);
            var connectionInfo = new CUGOJ.CUGOJ_Tools.RPC.RPCRegisterInfo()
            {
                CoreIP = Context.ServiceBaseInfo.ServiceIP,
                CorePort = Convert.ToInt32(Context.ServiceBaseInfo.ServicePort),
                Token = token.Token,
                ServiceType = serviceType
            };
            return connectionInfo.ToString();
        }
    }
    public static async Task<ServiceBaseInfo?> RegisterService(ServiceBaseInfo serviceInfo, string token)
    {
        return await Task.Run(() =>
        {
            lock (_lockServiceManager)
            {
                if (_servicesDict.ContainsKey(token))
                {
                    return _servicesDict[token];
                }
                if (!_tokens.Contains(new() { Token = token }))
                {
                    return null;
                }
                _services.Add(serviceInfo);
                _servicesDict[token] = serviceInfo;
                _tokens.RemoveWhere(x => x.Token == token);
                return serviceInfo;
            }
        }
        );
    }

    public static async Task<IEnumerable<ServiceBaseInfo>> DiscoverServices(ServiceTypeEnum serviceType)
    {
        return await Task.Run(() =>
        {
            lock (_lockServiceManager)
            {
                return from x in _services
                       where x.ServiceType == serviceType
                       select x;
            }
        });
    }
    private static object _pingCycleLock = new();
    private static int _pingCycleCount = 0;
    public static void PingCycle()
    {
        if (_pingCycleCount != 0) return;
        lock (_pingCycleLock)
        {
            if (_pingCycleCount != 0) return;
            _pingCycleCount++;
            System.Threading.Timer timer = new Timer((state) =>
                {
                    var str = GetConnectionString(ServiceTypeEnum.Authentication);
                    Logger.Info(str);
                    Console.WriteLine(str);
                }, null, 5000, 10000);
            System.Threading.Timer pingTimer = new Timer((state) =>
            {
                ServicePingCycle();
            }, null, 0, 1000);
        }
    }
}