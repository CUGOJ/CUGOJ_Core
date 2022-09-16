using CUGOJ.RPC.Gen.Services.Core;
using CUGOJ.RPC.Gen.Base;
using CUGOJ.CUGOJ_Tools.RPC;
using CUGOJ.CUGOJ_Tools.Context;
using CUGOJ.CUGOJ_Tools.Log;
using CUGOJ.CUGOJ_Core.Service;
namespace CUGOJ.CUGOJ_Core;

public class CoreServiceHandler : CUGOJ.RPC.Gen.Services.Core.CoreService.IAsync
{
    public CoreServiceHandler()
    {
        // Service.ServiceManager.PingCycle();
    }
    public virtual async Task<PingResponse> Ping(PingRequest req, CancellationToken token)
    {
        try
        {
            await Task.Run(() => { ServiceManager.OnPing(req.Timestamp); });
        }
        catch (Exception) { }
        var resp = new PingResponse(CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili());
        resp.BaseResp = RPCTools.SuccessBaseResp();
        return resp;
    }

    public virtual async Task<RegisterServiceResponse> RegisterService(RegisterServiceRequest req, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            Logger.Info("RegisterService,req = {Req}", req);
            var connectionInfo = RPCRegisterInfo.NewRPCRegisterInfoByConnectionString(req.ConnectionString);
            var serviceBaseInfo = Service.ServiceManager.RegisterService(connectionInfo, req.Port);
            if (serviceBaseInfo == null)
            {
                var resp = new RegisterServiceResponse(new ServiceBaseInfo());
                resp.BaseResp = RPCTools.ErrorBaseResp(new Exception("连接失败"));
                return resp;
            }
            else
            {
                var resp = new RegisterServiceResponse(serviceBaseInfo);
                resp.BaseResp = RPCTools.SuccessBaseResp();
                return resp;
            }
        });
    }

    public virtual async Task<DiscoverServiceResponse> DiscoverService(DiscoverServiceRequest req, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var resp = new DiscoverServiceResponse((Service.ServiceManager.DiscoverServices(req.ServiceType)).ToList());
            resp.BaseResp = RPCTools.SuccessBaseResp();
            return resp;
        });
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

    public async Task<GetAllServicesResponse> GetAllServices(GetAllServicesRequest req, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var resp = new GetAllServicesResponse();
            resp.Services = ServiceManager.GetAllServices();
            resp.BaseResp = RPCTools.SuccessBaseResp();
            return resp;
        });
    }

    public async Task<SetupServiceResponse> SetupService(SetupServiceRequest req, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var resp = new SetupServiceResponse();
            if (ServiceManager.SetupService(req.ServiceID, req.SetupServiceType, req.SetupValue))
            {
                resp.BaseResp = RPCTools.SuccessBaseResp();
            }
            else
                resp.BaseResp = RPCTools.ErrorBaseResp(new Exception("配置失败"));
            return resp;
        });
    }
}