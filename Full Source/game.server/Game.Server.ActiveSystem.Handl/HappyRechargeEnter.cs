using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(179)]
internal class HappyRechargeEnter : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(179);
		gSPacketIn.WriteInt(15);
		gSPacketIn.WriteInt(Player.GetItemCount(400100));
		gSPacketIn.WriteInt(Player.HappyRechargeData.LotteryTicket);
		gSPacketIn.WriteDateTime(DateTime.Parse(GameProperties.HappyRechargeBeginDate));
		gSPacketIn.WriteDateTime(DateTime.Parse(GameProperties.HappyRechargeEndDate));
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			HappyRechargeData happyRechargeData = produceBussiness.HappyRechargePrize();
			gSPacketIn.WriteInt(happyRechargeData.TemplateID);
			gSPacketIn.WriteInt(happyRechargeData.Count);
			gSPacketIn.WriteInt(happyRechargeData.ValiDate);
			gSPacketIn.WriteString("0,0,0,0");
			gSPacketIn.WriteInt(1);
		}
		List<HappyRechargeReward> happyRechargeRewards = ActiveSystemMgr.HappyRechargeRewards;
		gSPacketIn.WriteInt(happyRechargeRewards.Count);
		foreach (HappyRechargeReward item in happyRechargeRewards)
		{
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteInt(item.ValiDate);
			gSPacketIn.WriteString("0,0,0,0");
			gSPacketIn.WriteInt(item.ID);
			gSPacketIn.WriteInt(1);
		}
		using (new ProduceBussiness())
		{
			List<HappyRechargeTicketReward> happyTicketRewards = ActiveSystemMgr.HappyTicketRewards;
			gSPacketIn.WriteInt(happyTicketRewards.Count);
			foreach (HappyRechargeTicketReward item2 in happyTicketRewards)
			{
				gSPacketIn.WriteInt(item2.TemplateID);
				gSPacketIn.WriteInt(item2.Count);
				gSPacketIn.WriteInt(item2.ValiDate);
				gSPacketIn.WriteString("0,0,0,0");
				gSPacketIn.WriteString(item2.TicketPrice);
				gSPacketIn.WriteInt(1);
			}
		}
		using (ProduceBussiness produceBussiness3 = new ProduceBussiness())
		{
			HappyRechargeDataPrize[] happyRechargeData2 = produceBussiness3.GetHappyRechargeData();
			gSPacketIn.WriteInt(happyRechargeData2.Length);
			for (int i = 0; i < happyRechargeData2.Length; i++)
			{
				gSPacketIn.WriteInt(happyRechargeData2[i].PrizeType);
				gSPacketIn.WriteInt(happyRechargeData2[i].Count);
				gSPacketIn.WriteString(happyRechargeData2[i].NickName);
			}
		}
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
