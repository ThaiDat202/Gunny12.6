using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.DragonBoat.Handle;

[DragonBoatHandleAttbute(2)]
public class DragonBoatBuildDecorate : IDragonBoatCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(100);
		ActiveSystemInfo info = Player.Actives.Info;
		byte b = packet.ReadByte();
		int num = packet.ReadInt();
		if (num <= 0)
		{
			return false;
		}
		CommunalActiveInfo communalActiveInfo = CommunalActiveMgr.FindCommunalActive(1);
		if (communalActiveInfo != null && communalActiveInfo.LimitGrade > Player.PlayerCharacter.Grade)
		{
			Player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DragonBoatHandler.Msg2", communalActiveInfo.LimitGrade));
			return false;
		}
		if (DragonBoatMgr.periodType() == 2)
		{
			Player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DragonBoatHandler.Msg3"));
			return false;
		}
		if (info.useableScore >= int.MaxValue)
		{
			Player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DragonBoatHandler.Msg4"));
			return false;
		}
		int num2 = DragonBoatMgr.DayMaxScore();
		if (info.dayScore < num2)
		{
			int templateId = DragonBoatMgr.DRAGONBOAT_CHIP;
			if (DragonBoatMgr.OpenType() == 4)
			{
				templateId = DragonBoatMgr.KINGSTATUE_CHIP;
			}
			ItemInfo itemByTemplateID = Player.GetItemByTemplateID(templateId);
			string[] array = DragonBoatMgr.AddPropertyByMoney().Split(',');
			int num3 = int.Parse(array[1]);
			int num4 = int.Parse(array[0].Split(':')[0]);
			int num5 = num3 * num;
			bool flag = false;
			byte b2 = b;
			byte b3 = b2;
			if (b3 == 1)
			{
				if (itemByTemplateID == null)
				{
					Player.SendMessage(LanguageMgr.GetTranslation("DragonBoatHandler.Msg5", itemByTemplateID.Template.Name));
				}
				else
				{
					if (itemByTemplateID.Count < num)
					{
						num = itemByTemplateID.Count;
					}
					array = DragonBoatMgr.AddPropertyByProp().Split(',');
					num3 = int.Parse(array[1]);
					num4 = int.Parse(array[0].Split(':')[0]);
					num5 = num3 * num;
					if (info.dayScore + num5 > num2)
					{
						num = (num2 - info.dayScore) / num3;
					}
					int count = num * num4;
					flag = Player.RemoveTemplate(templateId, count);
				}
			}
			else
			{
				if (info.dayScore + num5 > num2)
				{
					num = (num2 - info.dayScore) / num3;
				}
				int value = num * num4;
				flag = Player.ActiveMoneyEnable(value);
			}
			if (flag)
			{
				num5 = num3 * num;
				info.totalScore += num5;
				info.useableScore += num5;
				info.dayScore += num5;
				Player.SendMessage(LanguageMgr.GetTranslation("DragonBoatHandler.Msg6"));
				DragonBoatMgr.UpdateLocalBoatExp(info);
			}
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(info.useableScore);
			gSPacketIn.WriteInt(info.totalScore);
			Player.SendTCP(gSPacketIn);
			GSPacketIn gSPacketIn2 = new GSPacketIn(100);
			gSPacketIn2.WriteByte(3);
			gSPacketIn2.WriteInt(DragonBoatMgr.boatCompleteExp());
			Player.SendTCP(gSPacketIn2);
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("DragonBoatHandler.MaxDayScore"));
		}
		return true;
	}
}
