using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle;

[GodCardRaiseHandleAttbute(20)]
public class GodCardInfoAttribute : IGodCardRaiseCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(20);
		gSPacketIn.WriteInt(player.Actives.ListCards.Count);
		foreach (GodCardUser listCard in player.Actives.ListCards)
		{
			gSPacketIn.WriteInt(listCard.CardId);
			gSPacketIn.WriteInt(listCard.Count);
		}
		gSPacketIn.WriteInt(player.Actives.Info.cardScore);
		gSPacketIn.WriteInt(player.Actives.Info.cardChipCount);
		gSPacketIn.WriteInt(player.Actives.Info.cardFreeCount);
		gSPacketIn.WriteInt(player.Actives.ListAwards.Count);
		foreach (int listAward in player.Actives.ListAwards)
		{
			gSPacketIn.WriteInt(listAward);
		}
		gSPacketIn.WriteInt(player.Actives.ListExchanges.Count);
		foreach (GodCardGroupUser listExchange in player.Actives.ListExchanges)
		{
			gSPacketIn.WriteInt(listExchange.GroupId);
			gSPacketIn.WriteInt(listExchange.Count);
		}
		if (GameProperties.VERSION >= 8800)
		{
			gSPacketIn.WriteDateTime(DateTime.Now);
		}
		player.Out.SendTCP(gSPacketIn);
		return 1;
	}
}
