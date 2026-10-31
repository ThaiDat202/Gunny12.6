using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(196, "场景用户离开")]
public class CardResetHandler : IPacketHandler
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		int[] array = new int[4]
		{
			packet.ReadInt(),
			packet.ReadInt(),
			packet.ReadInt(),
			packet.ReadInt()
		};
		bool flag = packet.ReadBoolean();
		UsersCardInfo itemByPlace = client.Player.CardBag.GetItemByPlace(0, num2);
		if (itemByPlace == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardResetHander.Translate1"));
			return 0;
		}
		if (client.Player.PlayerCharacter.CardSoul <= 50)
		{
			return 0;
		}
		List<int> list = new List<int>();
		string message = "";
		int minValue = 1;
		int maxValue = 10;
		if (itemByPlace.CardType == 2)
		{
			minValue = 5;
			maxValue = 31;
		}
		if (itemByPlace.CardType == 1)
		{
			minValue = 15;
			maxValue = 51;
		}
		if (client.Player.CardBag.LastCard == 0 || client.Player.CardBag.LastCard != itemByPlace.TemplateID)
		{
			client.Player.CardBag.LastCard = itemByPlace.TemplateID;
			client.Player.CardBag.LastAttack = 0;
			client.Player.CardBag.LastAgility = 0;
			client.Player.CardBag.LastDefence = 0;
			client.Player.CardBag.LastLuck = 0;
		}
		switch (num)
		{
		case 0:
		{
			for (int i = 0; i < 4; i++)
			{
				int num4 = random.Next(minValue, maxValue);
				switch (i)
				{
				case 0:
					if (array[i] == 1)
					{
						num4 = client.Player.CardBag.LastAttack;
					}
					itemByPlace.Attack = num4;
					client.Player.CardBag.LastAttack = num4;
					list.Add(num4);
					break;
				case 1:
					if (array[i] == 1)
					{
						num4 = client.Player.CardBag.LastDefence;
					}
					itemByPlace.Defence = num4;
					client.Player.CardBag.LastDefence = num4;
					list.Add(num4);
					break;
				case 2:
					if (array[i] == 1)
					{
						num4 = client.Player.CardBag.LastAgility;
					}
					itemByPlace.Agility = num4;
					client.Player.CardBag.LastAgility = num4;
					list.Add(num4);
					break;
				case 3:
					if (array[i] == 1)
					{
						num4 = client.Player.CardBag.LastLuck;
					}
					itemByPlace.Luck = num4;
					client.Player.CardBag.LastLuck = num4;
					list.Add(num4);
					break;
				}
			}
			client.Player.CardBag.UpdateTempCard(itemByPlace);
			client.Player.RemoveCardSoul(50);
			client.Player.Out.SendPlayerCardReset(client.Player.PlayerCharacter, list);
			break;
		}
		case 1:
			message = LanguageMgr.GetTranslation("CardResetHander.Translate2");
			client.Player.CardBag.UpdateCard();
			if (num2 < 5)
			{
				client.Player.EquipBag.UpdatePlayerProperties();
			}
			client.Player.CardBag.SaveToDatabase();
			break;
		}
		client.Out.SendMessage(eMessageType.Normal, message);
		return 0;
	}
}
