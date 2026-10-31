using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.DDTQiYuan.Handle;

[QiYuanHandleAttbute(33)]
public class RankAward : IQiYuanCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(33);
		List<QiYuanAwardInfo> list = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.BELIEF_AWARD);
		gSPacketIn.WriteInt(list.Count);
		foreach (QiYuanAwardInfo item in list)
		{
			gSPacketIn.WriteInt(item.Rank);
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteBoolean(item.IsBind);
			gSPacketIn.WriteInt(item.VaildDate);
		}
		list = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.AREA_RANK_AWARD);
		gSPacketIn.WriteInt(list.Count);
		foreach (QiYuanAwardInfo item2 in list)
		{
			gSPacketIn.WriteInt(item2.Rank);
			gSPacketIn.WriteInt(item2.TemplateID);
			gSPacketIn.WriteInt(item2.Count);
			gSPacketIn.WriteBoolean(item2.IsBind);
			gSPacketIn.WriteInt(item2.VaildDate);
		}
		list = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.ALL_AREA_RANK_AWARD);
		gSPacketIn.WriteInt(list.Count);
		foreach (QiYuanAwardInfo item3 in list)
		{
			gSPacketIn.WriteInt(item3.Rank);
			gSPacketIn.WriteInt(item3.TemplateID);
			gSPacketIn.WriteInt(item3.Count);
			gSPacketIn.WriteBoolean(item3.IsBind);
			gSPacketIn.WriteInt(item3.VaildDate);
		}
		gSPacketIn.WriteInt(GameProperties.QiYuanJoinLeastOfferTimes);
		gSPacketIn.WriteInt(GameProperties.QiYuanRankLeastOfferTimes);
		QiYuanAwardInfo qiYuanAwardInfo = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.JOIN_REWARD, 1);
		gSPacketIn.WriteInt(qiYuanAwardInfo.TemplateID);
		gSPacketIn.WriteInt(qiYuanAwardInfo.Count);
		gSPacketIn.WriteBoolean(qiYuanAwardInfo.IsBind);
		gSPacketIn.WriteInt(qiYuanAwardInfo.VaildDate);
		qiYuanAwardInfo = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.JOIN_PROBABILITY_REWARD, 1);
		gSPacketIn.WriteInt(qiYuanAwardInfo.TemplateID);
		gSPacketIn.WriteInt(qiYuanAwardInfo.Count);
		gSPacketIn.WriteBoolean(qiYuanAwardInfo.IsBind);
		gSPacketIn.WriteInt(qiYuanAwardInfo.VaildDate);
		gSPacketIn.WriteInt(qiYuanAwardInfo.Probability);
		gSPacketIn.WriteInt(GameProperties.QiYuanOfferTimesPerBaoZhu);
		gSPacketIn.WriteInt(GameProperties.QiYuanOfferTimesPerTreasureBox);
		gSPacketIn.WriteString(QiYuanAwardMgr.QiYuanRewardBoxTemplateIds);
		gSPacketIn.WriteString(QiYuanAwardMgr.QiYuanTreasureBoxTemplateIds);
		player.SendTCP(gSPacketIn);
		return 1;
	}
}
