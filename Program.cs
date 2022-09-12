
try
{
    var parsedArgs = CUGOJ.CUGOJ_Tools.Tools.ParamsTool.ParseArgs(args, CUGOJ.RPC.Gen.Base.ServiceTypeEnum.Core);
    string env = "prod";
    if (parsedArgs.ContainsKey("env"))
        env = parsedArgs["env"];
    string? ip = null;
    if (parsedArgs.ContainsKey("localhost"))
        ip = "127.0.0.1";
    string? traceAddress = null;
    if (parsedArgs.ContainsKey("trace"))
        traceAddress = parsedArgs["trace"];
    string? logAddress = null;
    if (parsedArgs.ContainsKey("log"))
        logAddress = parsedArgs["log"];
    int? port = null;
    if (parsedArgs.ContainsKey("port"))
        port = Convert.ToInt32(parsedArgs["port"]);
    string? mysqlAddress = null;
    if (parsedArgs.ContainsKey("mysql"))
        mysqlAddress = parsedArgs["mysql"];
    string? redisAddress = null;
    if (parsedArgs.ContainsKey("redis"))
        redisAddress = parsedArgs["redis"];
    string? neo4jAddress = null;

    // CUGOJ.CUGOJ_Core.Service.ServiceManager.PingCycle();
    await CUGOJ.CUGOJ_Tools.RPC.RPCService.StartCoreService<CUGOJ.CUGOJ_Core.CoreServiceHandler>(env, ip, port, traceAddress, logAddress, mysqlAddress, redisAddress, neo4jAddress, CUGOJ.CUGOJ_Core.Service.ServiceManager.PingCycle);
}
catch (Exception e)
{
    Console.WriteLine("Catch exception," + e.Message);
    CUGOJ.CUGOJ_Tools.Log.Logger.Error("Catch exception,{Message}", e.Message);
}