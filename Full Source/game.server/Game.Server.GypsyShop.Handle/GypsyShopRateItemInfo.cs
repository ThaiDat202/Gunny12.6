using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GypsyShop.Handle;

[GypsyShopHandleAttbute(4)]
public class GypsyShopRateItemInfo : IGypsyShopCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		List<MysteryShopInfo> rateMysteryShop = GypsyShopMgr.GetRateMysteryShop();
		GSPacketIn gSPacketIn = new GSPacketIn(278, player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(rateMysteryShop.Count);
		foreach (MysteryShopInfo item in rateMysteryShop)
		{
			gSPacketIn.WriteInt(item.InfoID);
		}
		player.Out.SendTCP(gSPacketIn);
		return false;
	}
}
