using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(177)]
internal class HappyRechargeExchange : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int itemId = packet.ReadInt();
		using (new ProduceBussiness())
		{
			Console.WriteLine(itemId);
			HappyRechargeTicketReward happyRechargeTicketReward = ActiveSystemMgr.HappyTicketRewards.Find((HappyRechargeTicketReward t) => t.TemplateID == itemId);
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(itemId);
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
				itemInfo.IsBinds = true;
				itemInfo.ValidDate = happyRechargeTicketReward.ValiDate;
				itemInfo.Count = happyRechargeTicketReward.Count;
				itemInfo.AttackCompose = 0;
				itemInfo.DefendCompose = 0;
				itemInfo.AgilityCompose = 0;
				itemInfo.LuckCompose = 0;
				Player.AddTemplate(itemInfo);
			}
		}
		Player.HappyRechargeData.LotteryTicket -= num;
		GSPacketIn gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(177);
		gSPacketIn.WriteInt(Player.HappyRechargeData.LotteryTicket);
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
