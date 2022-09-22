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

    public async Task<GetProblemResponse> GetProblem(GetProblemRequest req, CancellationToken cancellationToken = default)
    {
        var problemStruct = await Service.Common.ProblemService.Processor.GetProblemDetail(req.ProblemId);
        var resp = new GetProblemResponse(problemStruct);
        resp.BaseResp = RPCTools.SuccessBaseResp();
        return resp;
    }


    public Task<GetContestDetailResponse> GetContestDetail(GetContestDetailRequest req, CancellationToken cancellationToken = default)
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

    public Task<RestartResponse> Restart(RestartRequest req, CancellationToken cancellationToken = default)
    {
        CUGOJ.CUGOJ_Tools.RPC.RPCService.RestartService();
        var resp = new RestartResponse();
        resp.BaseResp = RPCTools.SuccessBaseResp();
        return Task.FromResult(resp);
    }

    public async Task<AddServiceResponse> AddService(AddServiceRequest req, CancellationToken cancellationToken = default)
    {
        var registerInfo = await ServiceManager.RegisterNewService(req.ServiceType);
        if (registerInfo == null)
        {
            var resp = new AddServiceResponse();
            resp.BaseResp = RPCTools.ErrorBaseResp(new Exception("添加服务失败"));
            return resp;
        }
        else
        {
            var resp = new AddServiceResponse(registerInfo.ToString());
            resp.BaseResp = RPCTools.SuccessBaseResp();
            return resp;
        }
    }

    public Task<GetUnRegisteredServicesResponse> GetUnRegisteredServices(GetUnRegisteredServicesRequest req, CancellationToken cancellationToken = default)
    {
        var services = ServiceManager.GetUnRegisteredServices();
        var connectionString = (from x in services select ServiceManager.GetConnectionStringByServiceID(x.ServiceID)).ToList();
        var resp = new GetUnRegisteredServicesResponse(services, connectionString);
        resp.BaseResp = RPCTools.SuccessBaseResp();
        return Task.FromResult(resp);
    }

    public Task<GetConnectionStringByServiceIDResponse> GetConnectionStringByServiceID(GetConnectionStringByServiceIDRequest req, CancellationToken cancellationToken = default)
    {
        var resp = new GetConnectionStringByServiceIDResponse(ServiceManager.GetConnectionStringByServiceID(req.ServiceID));
        if (resp.ConnectionString == "未知服务")
            resp.BaseResp = RPCTools.ErrorBaseResp(new Exception("未知服务"));
        else
            resp.BaseResp = RPCTools.SuccessBaseResp();
        return Task.FromResult(resp);
    }

    public async Task<OJGetProblemListResponse> OJGetProblemList(OJGetProblemListRequest req, CancellationToken cancellationToken = default)
    {
        var problemList = await Service.OJ.OJProblemService.Processor.GetProblemList(req.Cursor, req.Limit);
        var resp = new OJGetProblemListResponse();
        if (problemList == null)
        {
            throw new Exception("获取题目列表失败");
        }
        resp.ProblemList = problemList;
        resp.ProblemStatus = new();
        resp.BaseResp = RPCTools.SuccessBaseResp();
        return resp;
    }

    public Task<RPC.Gen.Services.Core.GetContestListResponse> GetContestList(RPC.Gen.Services.Core.GetContestListRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RPC.Gen.Services.Core.SaveProblemInfoResponse> SaveProblemInfo(RPC.Gen.Services.Core.SaveProblemInfoRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RPC.Gen.Services.Core.SaveContestInfoResponse> SaveContestInfo(RPC.Gen.Services.Core.SaveContestInfoRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RPC.Gen.Services.Core.GetSubmissionDetailResponse> GetSubmissionDetail(RPC.Gen.Services.Core.GetSubmissionDetailRequest req, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}