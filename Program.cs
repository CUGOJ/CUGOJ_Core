
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
    if (!CUGOJ.CUGOJ_Tools.Tools.DBTools.InitSqlite<CUGOJ.CUGOJ_Core.Dao.DB.CoreContext>())
    {
        throw new Exception("初始化数据库失败");
    }
    var properties = CUGOJ.CUGOJ_Core.Service.ServiceManager.LoadProperties();
    if (properties != null)
    {
        await CUGOJ.CUGOJ_Tools.RPC.RPCService.StartCoreService<CUGOJ.CUGOJ_Core.CoreServiceHandler>(
            properties.Env,
            properties.ServiceID,
            properties.ServiceIP,
            null,
            properties.TraceAddress,
            properties.LogAddress,
            properties.MysqlAddress,
            properties.RedisAddress,
            properties.Neo4jAddress,
            CUGOJ.CUGOJ_Core.Service.ServiceManager.RegisterSelf
        );
    }
    else
    {
        await CUGOJ.CUGOJ_Tools.RPC.RPCService.StartCoreService<CUGOJ.CUGOJ_Core.CoreServiceHandler>(env, null, ip, port, traceAddress, logAddress, mysqlAddress, redisAddress, neo4jAddress, CUGOJ.CUGOJ_Core.Service.ServiceManager.RegisterSelf);
    }
}
catch (Exception e)
{
    Console.WriteLine("Catch exception," + e.Message);
    CUGOJ.CUGOJ_Tools.Log.Logger.Error("Catch exception,{Message}", e.Message);
}