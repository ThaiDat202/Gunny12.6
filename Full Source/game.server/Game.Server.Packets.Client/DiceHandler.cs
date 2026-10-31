using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(134, "场景用户离开")]
public class DiceHandler : IPacketHandler
{
	private Random Random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		int iD = client.Player.PlayerCharacter.ID;
		switch (b)
		{
		case 10:
			client.Player.Dice.ReceiveData();
			client.Player.Out.SendDiceReceiveData(client.Player.Dice);
			break;
		case 11:
		{
			if (client.Player.Dice.Data.LuckIntegral >= client.Player.Dice.IntegralPoint[client.Player.Dice.MAX_LEVEL - 1])
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("DiceHandler.Msg1"));
				return 0;
			}
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			int index;
			int value;
			switch (num)
			{
			case 1:
				index = Random.Next(2, 13);
				value = client.Player.Dice.doubleDicePrice;
				break;
			case 2:
				index = Random.Next(4, 7);
				value = client.Player.Dice.bigDicePrice;
				break;
			case 3:
				index = Random.Next(1, 4);
				value = client.Player.Dice.smallDicePrice;
				break;
			default:
				index = Random.Next(1, 7);
				value = client.Player.Dice.commonDicePrice;
				break;
			}
			if (client.Player.Dice.Data.FreeCount > 0)
			{
				client.Player.Dice.Data.FreeCount--;
				receiveResult(client.Player, index);
			}
			else if (client.Player.MoneyDirect(value))
			{
				receiveResult(client.Player, index);
			}
			break;
		}
		case 12:
		{
			int refreshPrice = client.Player.Dice.refreshPrice;
			if (client.Player.MoneyDirect(refreshPrice))
			{
				client.Player.Dice.CreateDiceAward();
				client.Player.Out.SendDiceReceiveData(client.Player.Dice);
			}
			break;
		}
		}
		return 0;
	}

	private void receiveResult(GamePlayer player, int index)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(134);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(player.Dice.Data.CurrentPosition);
		gSPacketIn.WriteInt(index);
		player.Dice.Data.CurrentPosition += index;
		if (player.Dice.Data.CurrentPosition > 18)
		{
			player.Dice.Data.CurrentPosition -= 19;
		}
		EventAwardInfo eventAwardInfo = player.Dice.RewardItem[player.Dice.Data.CurrentPosition];
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(eventAwardInfo.TemplateID), eventAwardInfo.Count, 103);
		itemInfo.IsBinds = eventAwardInfo.IsBinds;
		if (!player.AddTemplate(itemInfo, LanguageMgr.GetTranslation("DiceHandler.Msg2")))
		{
			player.SendItemToMail(itemInfo, itemInfo.Template.Name.ToString(), LanguageMgr.GetTranslation("DiceHandler.Msg3"), eMailType.OpenUpArk);
		}
		player.Dice.RewardName = itemInfo.Template.Name;
		int num = Random.Next(2, 13);
		player.Dice.Data.LuckIntegral += num;
		int luckIntegralLevel = player.Dice.Data.LuckIntegralLevel;
		if (player.Dice.Data.LuckIntegral >= player.Dice.IntegralPoint[luckIntegralLevel + 1])
		{
			player.Dice.Data.LuckIntegralLevel++;
			player.PlayerCharacter.luckyNum++;
			player.Dice.GetLevelAward();
		}
		if (player.Dice.Data.LuckIntegralLevel > 3)
		{
			player.Dice.Data.LuckIntegralLevel = 3;
			player.Dice.Data.LuckIntegral = player.Dice.IntegralPoint[player.Dice.MAX_LEVEL - 1];
		}
		gSPacketIn.WriteInt(player.Dice.Data.LuckIntegral);
		gSPacketIn.WriteInt(player.Dice.Data.LuckIntegralLevel);
		gSPacketIn.WriteInt(player.Dice.Data.FreeCount);
		gSPacketIn.WriteString(player.Dice.RewardName);
		player.Out.SendTCP(gSPacketIn);
	}
}
