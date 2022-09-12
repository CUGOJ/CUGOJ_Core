using CUGOJ.RPC.Gen.Services.Core;
using CUGOJ.RPC.Gen.Base;
using CUGOJ.CUGOJ_Tools.RPC;
using CUGOJ.CUGOJ_Tools.Context;
using CUGOJ.CUGOJ_Tools.Log;
namespace CUGOJ.CUGOJ_Core;

public class CoreServiceHandler : CUGOJ.RPC.Gen.Services.Core.CoreService.IAsync
{
    public CoreServiceHandler()
    {
        // Service.ServiceManager.PingCycle();
    }
    public virtual Task<PingResponse> Ping(PingRequest req, CancellationToken token)
    {
        var resp = new PingResponse(CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili());
        resp.BaseResp = RPCTools.SuccessBaseResp();
        return Task.FromResult(resp);
    }

    public virtual async Task<RegisterServiceResponse> RegisterService(RegisterServiceRequest req, CancellationToken token)
    {
        Logger.Info("RegisterService,req = {Req}", req);
        var connectionInfo = RPCRegisterInfo.NewRPCRegisterInfoByConnectionString(req.ConnectionString);
        var serviceBaseInfo = new ServiceBaseInfo()
        {
            ServiceID = System.Guid.NewGuid().ToString(),
            ServiceIP = Context.ClientIP,
            ServicePort = req.Port.ToString(),
            ServiceType = connectionInfo.ServiceType,
            RegisterTime = CUGOJ.CUGOJ_Tools.Tools.CommonTools.Unix(),
            Env = Context.ServiceBaseInfo.Env,
            TraceAddress = Context.ServiceBaseInfo.TraceAddress,
            LogAddress = Context.ServiceBaseInfo.LogAddress,
            MysqlAddress = Context.ServiceBaseInfo.MysqlAddress,
            RedisAddress = Context.ServiceBaseInfo.RedisAddress,
            Neo4jAddress = Context.ServiceBaseInfo.Neo4jAddress
        };
        serviceBaseInfo = await Service.ServiceManager.RegisterService(serviceBaseInfo, connectionInfo.Token);
        if (serviceBaseInfo == null)
        {
            var resp = new RegisterServiceResponse(new ServiceBaseInfo());
            resp.BaseResp = RPCTools.ErrorBaseResp(new Exception("错误的Token"));
            return resp;
        }
        else
        {
            var resp = new RegisterServiceResponse(serviceBaseInfo);
            resp.BaseResp = RPCTools.SuccessBaseResp();
            return resp;
        }
    }

    public virtual async Task<DiscoverServiceResponse> DiscoverService(DiscoverServiceRequest req, CancellationToken token)
    {
        var resp = new DiscoverServiceResponse((await Service.ServiceManager.DiscoverServices(req.ServiceType)).ToList());
        resp.BaseResp = RPCTools.SuccessBaseResp();
        return resp;
    }

    public Task<GetProblemListResponse> GetProblemList(GetProblemListRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<GetProblemResponse> GetProblem(GetProblemRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<GetContestListResponse> GetContestList(GetContestListRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<GetContestDetailResponse> GetContestDetail(GetContestDetailRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SaveProblemInfoResponse> SaveProblemInfo(SaveProblemInfoRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SaveContestInfoResponse> SaveContestInfo(SaveContestInfoRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<GetUserDetailResponse> GetUserDetail(GetUserDetailRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SaveUserDetailResponse> SaveUserDetail(SaveUserDetailRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResp> ChangePassword(ChangePasswordRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<LogupResponse> Logup(LogupRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SubmitProblemResponse> SubmitProblem(SubmitProblemRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<GetContestSubmissionListResponse> GetContestSubmissionList(GetContestSubmissionListRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<GetProblemSubmissionListResponse> GetProblemSubmissionList(GetProblemSubmissionListRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<GetSubmissionDetailResponse> GetSubmissionDetail(GetSubmissionDetailRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}