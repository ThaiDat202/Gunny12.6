using System.Collections.Generic;
using System.Linq;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.CardNewSystem;

public class CardNewSystemPacketsOut
{
	private const int DefaultCardType = 5;

	public void SendCardInfo(GamePlayer player)
	{
		List<UsersCardInfo> list = (from p in player.CardBag.GetCards()
			where p.Place > 4
			select p).ToList();
		GSPacketIn gSPacketIn = new GSPacketIn(648);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.CardLevel);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Point);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Armor);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Crits);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Trick);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Viole);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Attack);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Defence);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Agility);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.Lucky);
		gSPacketIn.WriteInt(list.Count);
		foreach (UsersCardInfo item in list)
		{
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(5 - item.CardType);
		}
		gSPacketIn.WriteInt(player.PlayerCharacter.ActiveCards.Count);
		foreach (Sys_Users_CardActive activeCard in player.PlayerCharacter.ActiveCards)
		{
			gSPacketIn.WriteInt(activeCard.MainID);
			gSPacketIn.WriteBoolean(activeCard.IsActive);
		}
		gSPacketIn.WriteInt(player.PlayerCharacter.CardInfo.SuitLevel);
		gSPacketIn.WriteString(player.PlayerCharacter.CardInfo.CardSuitStr);
		player.Out.SendTCP(gSPacketIn);
	}

	public void SendCardReset(GamePlayer player, List<int> points)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(648, player.PlayerId);
		gSPacketIn.WriteByte(5);
		foreach (int point in points)
		{
			gSPacketIn.WriteInt(point);
		}
		player.Out.SendTCP(gSPacketIn);
	}

	public void SendCardSoul(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(648, player.PlayerId);
		gSPacketIn.WriteByte(10);
		gSPacketIn.WriteInt(player.PlayerCharacter.CardSoul);
		player.Out.SendTCP(gSPacketIn);
	}
}
