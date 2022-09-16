using CUGOJ.RPC.Gen.Base;
namespace CUGOJ.CUGOJ_Core.Service;

public class ServiceBaseInfoExtern : ServiceBaseInfo
{
    public long ConnectCount { get; private set; } = 0;
    public DateTime LastTick { get; private set; } = DateTime.UnixEpoch;

    public void OnPing(long startTime)
    {
        if (startTime - (LastTick - DateTime.UnixEpoch).TotalSeconds > 10)
        {
            if (ConnectCount == 0)
            {
                Logger.Info("服务{ServiceName}:{ServiceID}已连接,ping={ping}", ServiceType.ToString(), ServiceID, CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili() - startTime);
            }
            else
            {
                Logger.Info("服务{ServiceName}:{ServiceID}已重连,ping={ping}", ServiceType.ToString(), ServiceID, CUGOJ.CUGOJ_Tools.Tools.CommonTools.UnixMili() - startTime);
            }
            ConnectCount++;
        }
    }
}