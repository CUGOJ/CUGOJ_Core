namespace CUGOJ.CUGOJ_Core.Service.OJ;

public static class OJProblemService
{
    public class OJProblemServiceProcessor
    {
        public virtual async Task<List<ProblemStruct>?> GetProblemList(long cursor, long limit)
        {
            try
            {
                using var client = await RPCClientManager.GetBaseClient();
                if (client == null)
                {
                    return null;
                }
                var req = new GetProblemListRequest(cursor, limit);
                req.Base = RPCTools.NewBase();
                var res = await client.GetProblemList(req);
                return res.ProblemList;
            }
            catch (Exception e)
            {
                Logger.Error("获取题目列表失败,cursor={0},limit={1},Exception={2}", cursor, limit, e);
                return null;
            }
        }
    }
    public static OJProblemServiceProcessor Processor { get; } = TraceFactory.CreateTracableObject<OJProblemServiceProcessor>(false, false);

}