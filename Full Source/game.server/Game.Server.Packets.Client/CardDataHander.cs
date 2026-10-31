using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.CardNewSystem;
using Game.Server.GameUtils;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(216, "防沉迷系统开关")]
internal class CardDataHander : IPacketHandler
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static Random Random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		PlayerInfo playerCharacter = client.Player.PlayerCharacter;
		int num2 = 0;
		CardInventory cardBag = client.Player.CardBag;
		List<ItemInfo> list = new List<ItemInfo>();
		string text = "";
		switch (num)
		{
		case 0:
		{
			int num5 = packet.ReadInt();
			int num6 = packet.ReadInt();
			UsersCardInfo itemAt = cardBag.GetItemAt(num5);
			UsersCardInfo itemAt2 = cardBag.GetItemAt(num6);
			if (itemAt == null)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardDataHander.Msg1"));
				return 0;
			}
			if (itemAt2 != null && num6 >= 5)
			{
				return 0;
			}
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(itemAt.TemplateID);
			if (itemTemplateInfo.Property8 == 0 && num6 == 0)
			{
				return 0;
			}
			if (itemTemplateInfo.Property8 == 1 && num6 > 0 && num6 <= 4)
			{
				return 0;
			}
			if (num5 == num6)
			{
				cardBag.MoveCard(num5, num6);
				client.Player.EquipBag.UpdatePlayerProperties();
				break;
			}
			if (cardBag.FindEquipCard(itemAt.TemplateID))
			{
				text = LanguageMgr.GetTranslation("CardDataHander.Msg2");
			}
			else
			{
				cardBag.MoveCard(num5, num6);
				client.Player.EquipBag.UpdatePlayerProperties();
				text = LanguageMgr.GetTranslation("CardDataHander.Msg3");
			}
			client.Out.SendMessage(eMessageType.Normal, text);
			break;
		}
		case 1:
		case 4:
		{
			int slot = packet.ReadInt();
			num2 = packet.ReadInt();
			ItemInfo itemAt3 = client.Player.EquipBag.GetItemAt(slot);
			if (num == 4)
			{
				itemAt3 = client.Player.PropBag.GetItemAt(slot);
			}
			if (itemAt3 == null)
			{
				return 0;
			}
			if (itemAt3.Count < num2 || num2 < 1)
			{
				return 0;
			}
			if (!itemAt3.IsCard())
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardDataHander.Msg4", itemAt3.Template.Name));
				return 0;
			}
			bool flag2 = false;
			int num7 = 0;
			int num8 = 0;
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			for (int j = 0; j < num2; j++)
			{
				if (itemAt3.Template.Property5 != 0)
				{
					flag2 = TakeCard(client, itemAt3.Template.Property5);
				}
				else if (ItemBoxMgr.CreateItemBox(itemAt3.TemplateID, list, ref specialValue))
				{
					int index = Random.Next(list.Count);
					ItemInfo itemInfo = list[index];
					flag2 = TakeCard(client, itemInfo.Template.Property5);
				}
				if (flag2)
				{
					num7++;
				}
				else
				{
					num8 = ((itemAt3.TemplateID != 11917) ? ((itemAt3.TemplateID != 20150) ? (num8 + Random.Next(5, 25)) : (num8 + Random.Next(500))) : 500);
				}
			}
			client.Player.RemoveTemplate(itemAt3.TemplateID, num2);
			client.Player.AddCardSoul(num8);
			CardNewSystemMgr.Out.SendCardSoul(client.Player);
			client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, isSoul: true, num8);
			string translation = LanguageMgr.GetTranslation("CardDataHander.Msg5", num8, num7);
			client.Out.SendMessage(eMessageType.Normal, translation);
			break;
		}
		case 5:
		{
			int value = 50;
			if (!client.Player.MoneyDirect(value))
			{
				break;
			}
			if (client.Player.PlayerCharacter.GetSoulCount > 0)
			{
				int num9 = 0;
				num9 = 50;
				int num10 = Random.Next(1000);
				if (num10 < 100)
				{
					num9 = 120;
				}
				client.Player.AddCardSoul(num9);
				CardNewSystemMgr.Out.SendCardSoul(client.Player);
				client.Player.PlayerCharacter.GetSoulCount--;
				client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, isSoul: true, num9);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardDataHander.Msg6", num9));
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardDataHander.Msg7"));
			}
			break;
		}
		case 2:
		{
			num2 = packet.ReadInt();
			List<UsersCardInfo> cards = cardBag.GetCards(5, cardBag.Capalility);
			if (num2 != cards.Count)
			{
				return 0;
			}
			bool flag = false;
			cardBag.BeginChanges();
			try
			{
				UsersCardInfo[] rawSpaces = cardBag.GetRawSpaces();
				cardBag.Clear();
				for (int i = 0; i < num2; i++)
				{
					int num3 = packet.ReadInt();
					int num4 = packet.ReadInt();
					UsersCardInfo usersCardInfo = rawSpaces[num3];
					if (usersCardInfo != null && !cardBag.AddCardTo(usersCardInfo, num4))
					{
						throw new Exception($"move card Bag error: old place:{num3} new place:{num4}");
					}
				}
				flag = true;
			}
			catch (Exception ex)
			{
				log.ErrorFormat("Arrage bag errror,user id:{0}   msg:{1}", client.Player.PlayerId, ex.Message);
			}
			finally
			{
				if (!flag)
				{
					cardBag.Clear();
					foreach (UsersCardInfo item in cards)
					{
						cardBag.AddCardTo(item, item.Place);
					}
				}
				cardBag.CommitChanges();
			}
			break;
		}
		}
		return 0;
	}

	private bool TakeCard(GameClient client, int templateId)
	{
		bool result = false;
		int place = client.Player.CardBag.FindFirstEmptySlot(5);
		int num = -1;
		UsersCardInfo usersCardInfo = null;
		CardTemplateInfo card = CardMgr.GetCard(templateId);
		if (card != null)
		{
			num = client.Player.CardBag.FindPlaceByTamplateId(5, templateId);
			if (num == -1)
			{
				usersCardInfo = new UsersCardInfo();
				usersCardInfo.CardType = card.CardType;
				usersCardInfo.UserID = client.Player.PlayerCharacter.ID;
				usersCardInfo.Place = place;
				usersCardInfo.TemplateID = card.CardID;
				usersCardInfo.isFirstGet = true;
				usersCardInfo.Attack = 0;
				usersCardInfo.Agility = 0;
				usersCardInfo.Defence = 0;
				usersCardInfo.Luck = 0;
				usersCardInfo.Damage = 0;
				usersCardInfo.Guard = 0;
			}
			else
			{
				usersCardInfo = client.Player.CardBag.GetItemAt(num);
				if (canUpdate(usersCardInfo.CardType, card.CardType))
				{
					usersCardInfo.isFirstGet = true;
				}
				else
				{
					usersCardInfo = null;
				}
			}
			if (usersCardInfo != null)
			{
				result = ((num != -1) ? client.Player.CardBag.UpdateCardType(usersCardInfo.TemplateID, card.CardType) : client.Player.CardBag.AddCardTo(usersCardInfo, place));
				client.Out.SendGetCard(client.Player.PlayerCharacter, usersCardInfo);
				if (usersCardInfo.CardType == 44 || usersCardInfo.CardType == 11)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(card.CardID);
					string translation = LanguageMgr.GetTranslation("CardDataHandler.Notice", client.Player.PlayerCharacter.NickName, itemTemplateInfo.Name, cardQuality(card.CardType));
					GSPacketIn packet = WorldMgr.SendSysNotice(translation);
					GameServer.Instance.LoginServer.SendPacket(packet);
				}
			}
		}
		return result;
	}

	private string cardQuality(int Type)
	{
		string translation;
		switch (Type)
		{
		case 2:
			return LanguageMgr.GetTranslation("CardDataHander.Msg8");
		case 1:
			return LanguageMgr.GetTranslation("CardDataHander.Msg9");
		default:
			translation = LanguageMgr.GetTranslation("CardDataHander.Msg11");
			break;
		case 4:
			translation = LanguageMgr.GetTranslation("CardDataHander.Msg10");
			break;
		}
		return translation;
	}

	private bool canUpdate(int oldtype, int newType)
	{
		if (newType == 2 && (oldtype == 0 || oldtype == 3))
		{
			return true;
		}
		if (newType == 1 && (oldtype == 2 || oldtype == 0 || oldtype == 3))
		{
			return true;
		}
		if (newType == 4 && (oldtype == 1 || oldtype == 2 || oldtype == 0 || oldtype == 3))
		{
			return true;
		}
		return false;
	}
}
