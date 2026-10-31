using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.RingStation.Handle;

[RingStationHandleAttbute(8)]
public class RingStationGetReward : IRingStationCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		UserRingStationInfo singleRingStationInfos = RingStationMgr.GetSingleRingStationInfos(Player.PlayerId);
		GSPacketIn gSPacketIn = new GSPacketIn(404, Player.PlayerId);
		gSPacketIn.WriteByte(6);
		if (singleRingStationInfos != null && singleRingStationInfos.ReardEnable)
		{
			singleRingStationInfos.ReardEnable = false;
			RingStationMgr.UpdateRingStationInfo(singleRingStationInfos);
			int num = RingStationMgr.ConfigInfo.AwardNumByRank(singleRingStationInfos.Rank);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(singleRingStationInfos.Rank);
			gSPacketIn.WriteInt(num);
			Player.AddLeagueMoney(num);
		}
		else
		{
			gSPacketIn.WriteBoolean(val: false);
		}
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
