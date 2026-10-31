using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.MagicHouse.Handle;

[MagicHouseHandleAttbute(4)]
public class MagicHouseChargeBox : IMagicHouseCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		int magicRoomBoxNeedMoney = GameProperties.MagicRoomBoxNeedMoney;
		int num = packet.ReadInt();
		int chargeGetCount = Player.MagicHouse.Info.chargeGetCount;
		if (num > 0 && Player.MagicHouse.Info.chargeGetCount < Player.MagicHouse.CHARGEBOX_MAXCOUNT)
		{
			if (chargeGetCount + num > Player.MagicHouse.CHARGEBOX_MAXCOUNT)
			{
				num = Player.MagicHouse.CHARGEBOX_MAXCOUNT - chargeGetCount;
			}
			if (Player.MoneyDirect(magicRoomBoxNeedMoney * num))
			{
				Player.MagicHouse.Info.chargeGetTime = DateTime.Now;
				Player.MagicHouse.Info.chargeGetCount += num;
				ItemInfo[] award = Player.MagicHouse.GetAward(num, eEventType.MAGIC_HOUSE_PAY);
				GSPacketIn gSPacketIn = new GSPacketIn(84, Player.PlayerId);
				gSPacketIn.WriteInt(4);
				gSPacketIn.WriteInt(5);
				gSPacketIn.WriteInt(Player.MagicHouse.Info.chargeGetCount);
				gSPacketIn.WriteDateTime(Player.MagicHouse.Info.chargeGetTime);
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
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseChargeBox.Empty"));
		}
		return false;
	}
}
