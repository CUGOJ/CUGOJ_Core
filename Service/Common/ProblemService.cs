namespace CUGOJ.CUGOJ_Core.Service.Common;

public static class ProblemService
{
    public class ProblemServiceProcessor
    {
        public virtual async Task<ProblemStruct?> GetProblemDetail(long problemID)
        {
            try
            {
                using var client = await RPCClientManager.GetBaseClient();
                if (client == null)
                {
                    return null;
                }
                var req = new MulGetProblemInfoRequest(new List<long> { problemID });
                req.Base = RPCTools.NewBase();
                var res = await client.MulGetProblemInfo(req);
                return res.ProblemList.FirstOrDefault();
            }
            catch (Exception e)
            {
                Logger.Error("获取题目详情失败,ProblemID={0},Exception={1}", problemID, e);
                return null;
            }
        }
    }
    public static ProblemServiceProcessor Processor { get; } = TraceFactory.CreateTracableObject<ProblemServiceProcessor>(false, false);

}