using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(5)]
public class CardModProperty : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		packet.ReadBoolean();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		int num4 = packet.ReadInt();
		int num5 = packet.ReadInt();
		int num6 = ((num == 1) ? 50 : 2000);
		ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();
		if (Player.PlayerCharacter.CardSoul < num6)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardNew.CardModProperty.NoCardSoul"));
			return 0;
		}
		List<int> list = new List<int>();
		List<int> source = new List<int> { num2, num3, num4, num5 };
		int num7 = source.Count((int p) => p == 1);
		if (num7 > 0)
		{
			Player.RemoveMoney(Convert.ToInt32(GameProperties.CardLockAttrMoney.Split('|')[num7 - 1]));
		}
		Sys_Users_CardInfo sys_Users_CardInfo = new Sys_Users_CardInfo();
		Player.CardBag.LastAttack = Player.PlayerCharacter.CardInfo.Attack;
		Player.CardBag.LastAgility = Player.PlayerCharacter.CardInfo.Agility;
		Player.CardBag.LastDefence = Player.PlayerCharacter.CardInfo.Defence;
		Player.CardBag.LastLuck = Player.PlayerCharacter.CardInfo.Lucky;
		for (int i = 0; i < 4; i++)
		{
			int num8 = threadSafeRandom.Next((num == 0) ? 1 : 10, 240);
			switch (i)
			{
			case 0:
				if (num2 == 1)
				{
					num8 = Player.CardBag.LastAttack;
				}
				sys_Users_CardInfo.Attack = num8;
				Player.CardBag.LastAttack = num8;
				list.Add(num8);
				break;
			case 1:
				if (num3 == 1)
				{
					num8 = Player.CardBag.LastDefence;
				}
				sys_Users_CardInfo.Defence = num8;
				Player.CardBag.LastDefence = num8;
				list.Add(num8);
				break;
			case 2:
				if (num4 == 1)
				{
					num8 = Player.CardBag.LastAgility;
				}
				sys_Users_CardInfo.Agility = num8;
				Player.CardBag.LastAgility = num8;
				list.Add(num8);
				break;
			case 3:
				if (num5 == 1)
				{
					num8 = Player.CardBag.LastLuck;
				}
				sys_Users_CardInfo.Lucky = num8;
				Player.CardBag.LastLuck = num8;
				list.Add(num8);
				break;
			}
		}
		Player.CardBag.UpdateTempCardNews(sys_Users_CardInfo);
		Player.RemoveCardSoul(num6);
		Player.UpdateCardInfo();
		CardNewSystemMgr.Out.SendCardSoul(Player);
		CardNewSystemMgr.Out.SendCardReset(Player, list);
		return 0;
	}
}
