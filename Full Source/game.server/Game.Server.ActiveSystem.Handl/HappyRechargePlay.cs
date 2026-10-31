using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(176)]
internal class HappyRechargePlay : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(176);
		bool isLog = false;
		HappyRechargeReward happyRechargeReward = null;
		int num = ThreadSafeRandom.NextStatic(0, 12);
		if (!Player.RemoveTemplate(400100, 1))
		{
			Player.SendMessage("转盘抽奖券数量不足.");
			return false;
		}
		if (num == 8)
		{
			Player.HappyRechargeData.LotteryCount++;
			IPacketLib @out = Player.Out;
			HappyRechargePrizeType happyRechargePrizeType = (HappyRechargePrizeType)num;
			@out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("HappyRecharge." + happyRechargePrizeType));
			num = ThreadSafeRandom.NextStatic(0, 7);
		}
		if (num == 9)
		{
			Player.HappyRechargeData.LotteryTicket++;
		}
		if (num == 10)
		{
			isLog = true;
		}
		if (num == 11)
		{
			isLog = true;
		}
		if (num == 12)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			for (int i = 0; i < allPlayers.Length; i++)
			{
				allPlayers[i].Out.SendMessage(eMessageType.ChatNormal, string.Format(LanguageMgr.GetTranslation("HappyRecharge.JackPot", Player.PlayerCharacter.NickName)));
			}
			isLog = true;
		}
		if (num < 8)
		{
			using (new ProduceBussiness())
			{
				happyRechargeReward = ActiveSystemMgr.HappyRechargeRewards[num];
			}
		}
		string text = "Default";
		if (num > 7)
		{
			HappyRechargePrizeType happyRechargePrizeType = (HappyRechargePrizeType)num;
			text = happyRechargePrizeType.ToString();
		}
		Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("恭喜获得: " + text));
		gSPacketIn.WriteInt((num < 8) ? happyRechargeReward.TemplateID : num);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteInt((num >= 7) ? 1 : happyRechargeReward.Count);
		gSPacketIn.WriteInt(Player.GetItemCount(400100));
		gSPacketIn.WriteInt(Player.HappyRechargeData.LotteryTicket);
		Player.SendTCP(gSPacketIn);
		HappyRecharegeUpdate(happyRechargeReward, Player, isLog, num);
		return true;
	}

	private void HappyRecharegeUpdate(HappyRechargeReward happyItem, GamePlayer player, bool isLog, int type)
	{
		if (happyItem != null)
		{
			using (new ProduceBussiness())
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(happyItem.TemplateID);
				if (itemTemplateInfo != null)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
					itemInfo.IsBinds = true;
					itemInfo.ValidDate = happyItem.ValiDate;
					itemInfo.Count = happyItem.Count;
					itemInfo.AttackCompose = 0;
					itemInfo.DefendCompose = 0;
					itemInfo.AgilityCompose = 0;
					itemInfo.LuckCompose = 0;
					player.AddTemplate(itemInfo);
				}
			}
		}
		using ProduceBussiness produceBussiness2 = new ProduceBussiness();
		HappyRechargeData happyRechargeData = produceBussiness2.HappyRechargePrize();
		happyRechargeData.Count += 20;
		if (happyRechargeData.Count >= 10000)
		{
			happyRechargeData.Count = 10000;
		}
		int num = 0;
		if (type == 10)
		{
			num = happyRechargeData.Count / 100 * 5;
		}
		if (type == 11)
		{
			num = happyRechargeData.Count / 100 * 10;
		}
		if (type == 12)
		{
			num = happyRechargeData.Count;
		}
		if (type > 9)
		{
			happyRechargeData.Count -= num;
			produceBussiness2.UpdateHappyRechargeDataPrize(type, num, player.PlayerCharacter.NickName);
		}
		produceBussiness2.UpdateHappyRechargePrize((happyRechargeData.Count == 0) ? 1000 : happyRechargeData.Count);
		ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(happyRechargeData.TemplateID);
		if (itemTemplateInfo2 != null)
		{
			ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(itemTemplateInfo2, 1, 102);
			itemInfo2.IsBinds = true;
			itemInfo2.ValidDate = happyRechargeData.ValiDate;
			itemInfo2.Count = num;
			itemInfo2.AttackCompose = 0;
			itemInfo2.DefendCompose = 0;
			itemInfo2.AgilityCompose = 0;
			itemInfo2.LuckCompose = 0;
			player.AddTemplate(itemInfo2);
		}
		GSPacketIn gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(180);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(happyRechargeData.Count);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteString(player.PlayerCharacter.NickName);
		gSPacketIn.WriteInt(num);
		player.SendTCP(gSPacketIn);
		gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(180);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteInt(player.GetItemCount(400100));
	}
}
