using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeTemple.Handle;

[HomeTempleHandleAttbute(1)]
public class Immolation : IHomeTempleCommandHadler
{
	private Random rand = new Random();

	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		if (player.Actives.Info == null)
		{
			return 0;
		}
		if (num == 11176 || num == 11177)
		{
			ItemInfo itemByTemplateID = player.PropBag.GetItemByTemplateID(11176);
			if (itemByTemplateID == null)
			{
				itemByTemplateID = player.PropBag.GetItemByTemplateID(11177);
			}
			if (itemByTemplateID == null)
			{
				player.SendMessage(LanguageMgr.GetTranslation("HomeTemple.Handle.Immolation.ItemNotEnough"));
				return 0;
			}
			int num2 = 1;
			int currentLevel = player.Actives.Info.CurrentLevel;
			int homeTempleExp = player.Actives.Info.HomeTempleExp;
			int num3 = 0;
			if (HomeTempPracticeMgr.MaxLevel() == currentLevel)
			{
				player.SendMessage(LanguageMgr.GetTranslation("HomeTemple.Handle.Immolation.MaxLevel"));
				return 0;
			}
			int property = itemByTemplateID.Template.Property2;
			HomeTempPracticeInfo homeTempPracticeInfo = HomeTempPracticeMgr.FindHomeTempPractice(currentLevel + 1);
			if (homeTempPracticeInfo == null)
			{
				player.SendMessage(LanguageMgr.GetTranslation("HomeTemple.Handle.Immolation.ErrorData"));
				return 0;
			}
			if (flag)
			{
				int num4 = homeTempPracticeInfo.Exp - homeTempleExp;
				num2 = ((num4 < property) ? 1 : (num4 / property)) + 1;
			}
			if (num2 > itemByTemplateID.Count)
			{
				num2 = itemByTemplateID.Count;
			}
			if (num2 <= 0)
			{
				player.SendMessage(LanguageMgr.GetTranslation("HomeTemple.Handle.Immolation.ItemNotEnough"));
				return 0;
			}
			num3 = property * num2;
			int val = 0;
			int val2 = 0;
			int num5 = rand.Next(10000);
			if (num5 < 50)
			{
				val = 1;
				num3 = num3 * 200 / 100;
			}
			else if (num5 < 100)
			{
				val2 = 1;
				num3 = num3 * 150 / 100;
			}
			homeTempleExp = player.Actives.Info.HomeTempleExp + num3;
			bool flag2 = false;
			if (homeTempleExp >= homeTempPracticeInfo.Exp)
			{
				player.Actives.Info.CurrentLevel++;
				player.EquipBag.UpdatePlayerProperties();
				flag2 = true;
			}
			player.Actives.Info.HomeTempleExp = homeTempleExp;
			bool flag3 = false;
			if (player.Actives.Info.CurrentLevel > player.Actives.Info.CurrentSelectIndex * 10)
			{
				player.Actives.Info.CurrentSelectIndex++;
				flag3 = true;
			}
			if (flag3)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(297);
				gSPacketIn.WriteByte(0);
				gSPacketIn.WriteInt(player.Actives.Info.CurrentSelectIndex);
				gSPacketIn.WriteInt(player.Actives.Info.CurrentLevel);
				gSPacketIn.WriteInt(player.Actives.Info.HomeTempleExp);
				if (GameProperties.VERSION >= 11000)
				{
					packet.WriteInt(player.Actives.Info.promoteLevel);
				}
				player.SendTCP(gSPacketIn);
			}
			else
			{
				GSPacketIn gSPacketIn2 = new GSPacketIn(297);
				gSPacketIn2.WriteByte(1);
				gSPacketIn2.WriteInt(player.Actives.Info.CurrentLevel);
				gSPacketIn2.WriteInt(player.Actives.Info.HomeTempleExp);
				gSPacketIn2.WriteInt(val);
				gSPacketIn2.WriteInt(val2);
				player.SendTCP(gSPacketIn2);
			}
			if (flag2)
			{
				player.OnGodTempleImmolation(player.Actives.Info.CurrentSelectIndex, player.Actives.Info.CurrentLevel);
			}
			player.PropBag.RemoveTemplate(itemByTemplateID.TemplateID, num2);
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("HomeTemple.Handle.Immolation.ItemNotEnough"));
		}
		return 1;
	}
}
