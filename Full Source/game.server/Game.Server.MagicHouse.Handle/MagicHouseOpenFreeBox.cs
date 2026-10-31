using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.MagicHouse.Handle;

[MagicHouseHandleAttbute(3)]
public class MagicHouseOpenFreeBox : IMagicHouseCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (num > 0 && Player.MagicHouse.Info.freeGetCount < Player.MagicHouse.FREEBOX_MAXCOUNT)
		{
			Player.MagicHouse.Info.freeGetTime = DateTime.Now;
			ItemInfo[] award = Player.MagicHouse.GetAward(num, eEventType.MAGIC_HOUSE_FREE);
			Player.MagicHouse.Info.freeGetCount = num;
			GSPacketIn gSPacketIn = new GSPacketIn(84, Player.PlayerId);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteInt(Player.MagicHouse.Info.freeGetCount);
			gSPacketIn.WriteDateTime(Player.MagicHouse.Info.freeGetTime);
			gSPacketIn.WriteInt(award.Length);
			ItemInfo[] array = award;
			foreach (ItemInfo itemInfo in array)
			{
				gSPacketIn.WriteInt(itemInfo.TemplateID);
				gSPacketIn.WriteInt(itemInfo.Count);
				gSPacketIn.WriteInt(itemInfo.ValidDate);
				gSPacketIn.WriteBoolean(itemInfo.IsBinds);
				gSPacketIn.WriteInt(itemInfo.StrengthenLevel);
				gSPacketIn.WriteInt(itemInfo.AttackCompose);
				gSPacketIn.WriteInt(itemInfo.DefendCompose);
				gSPacketIn.WriteInt(itemInfo.AgilityCompose);
				gSPacketIn.WriteInt(itemInfo.LuckCompose);
				Player.AddTemplate(itemInfo);
			}
			Player.SendTCP(gSPacketIn);
			Player.MagicHouse.UpdateMessage();
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseOpenFreeBox.Empty"));
		}
		return false;
	}
}
