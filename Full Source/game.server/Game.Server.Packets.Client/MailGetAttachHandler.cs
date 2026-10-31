using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Emblem;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(113, "获取邮件到背包")]
public class MailGetAttachHandler : IPacketHandler
{
	private void checkmaildel(PlayerBussiness db, int mailID, MailInfo[] mailAll, string itemid)
	{
		foreach (MailInfo mailInfo in mailAll)
		{
			if (mailInfo.ID != mailID)
			{
				bool flag = false;
				if (mailInfo.Annex1 == itemid)
				{
					mailInfo.Annex1 = "";
					flag = true;
				}
				if (mailInfo.Annex2 == itemid)
				{
					mailInfo.Annex2 = "";
					flag = true;
				}
				if (mailInfo.Annex3 == itemid)
				{
					mailInfo.Annex3 = "";
					flag = true;
				}
				if (mailInfo.Annex4 == itemid)
				{
					mailInfo.Annex4 = "";
					flag = true;
				}
				if (mailInfo.Annex5 == itemid)
				{
					mailInfo.Annex5 = "";
					flag = true;
				}
				if (flag)
				{
					db.UpdateMail(mailInfo);
				}
			}
		}
	}

	public bool GetAnnex(string value, GamePlayer player, ref string msg, ref bool result, ref eMessageType eMsg, ref eBageType ebag)
	{
		int itemID = int.Parse(value);
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			ItemInfo userItemSingle = playerBussiness.GetUserItemSingle(itemID);
			if (userItemSingle != null && player.AddTemplate(userItemSingle))
			{
				eMsg = eMessageType.Normal;
				if (userItemSingle.BagType > -1)
				{
					ebag = (eBageType)userItemSingle.BagType;
				}
				else
				{
					ebag = userItemSingle.Template.BagType;
				}
				return true;
			}
		}
		return false;
	}

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		List<MailToBagInfo> list = new List<MailToBagInfo>();
		GSPacketIn gSPacketIn = new GSPacketIn(113, client.Player.PlayerCharacter.ID);
		eMessageType eMsg = eMessageType.Normal;
		string msg = "";
		byte b = packet.ReadByte();
		int num = packet.ReadInt();
		for (int i = 0; i < num; i++)
		{
			MailToBagInfo mailToBagInfo = new MailToBagInfo();
			mailToBagInfo.List = new List<int>();
			int num2 = (mailToBagInfo.ID = packet.ReadInt());
			List<int> list2 = new List<int>();
			List<string> list3 = new List<string>();
			int value = 0;
			int value2 = 0;
			int value3 = 0;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
				int num3 = 0;
			}
			else if (client.Player.LastGetMailID == num2)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MailGetAttachHandler.Translate1"));
				int num3 = 0;
			}
			else
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					MailInfo mailSingle = playerBussiness.GetMailSingle(client.Player.PlayerCharacter.ID, num2);
					if (mailSingle.Money > 0 && client.Player.PlayerCharacter.Grade < GameProperties.LimitGardeSendMailMomey)
					{
						client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MailGetAttachHandler.Translate2") + GameProperties.LimitGardeSendMailMomey);
						gSPacketIn.WriteInt(0);
						client.Out.SendTCP(gSPacketIn);
						return 0;
					}
					client.Player.LastGetMailID = num2;
					MailInfo[] mailByUserID = playerBussiness.GetMailByUserID(client.Player.PlayerCharacter.ID);
					if (mailSingle != null)
					{
						bool result = true;
						int money = mailSingle.Money;
						if (mailSingle.Type > 100 && !client.Player.ActiveMoneyEnable(money))
						{
							return 0;
						}
						GamePlayer playerById = WorldMgr.GetPlayerById(mailSingle.ReceiverID);
						if (!mailSingle.IsRead)
						{
							mailSingle.IsRead = true;
							mailSingle.ValidDate = 72;
							mailSingle.SendTime = DateTime.Now;
						}
						if (result && (b == 0 || b == 1) && !string.IsNullOrEmpty(mailSingle.Annex1))
						{
							list2.Add(1);
							list3.Add(mailSingle.Annex1);
							mailSingle.Annex1 = null;
						}
						if (result && (b == 0 || b == 2) && !string.IsNullOrEmpty(mailSingle.Annex2))
						{
							list2.Add(2);
							list3.Add(mailSingle.Annex2);
							mailSingle.Annex2 = null;
						}
						if (result && (b == 0 || b == 3) && !string.IsNullOrEmpty(mailSingle.Annex3))
						{
							list2.Add(3);
							list3.Add(mailSingle.Annex3);
							mailSingle.Annex3 = null;
						}
						if (result && (b == 0 || b == 4) && !string.IsNullOrEmpty(mailSingle.Annex4))
						{
							list2.Add(4);
							list3.Add(mailSingle.Annex4);
							mailSingle.Annex4 = null;
						}
						if (result && (b == 0 || b == 5) && !string.IsNullOrEmpty(mailSingle.Annex5))
						{
							list2.Add(5);
							list3.Add(mailSingle.Annex5);
							mailSingle.Annex5 = null;
						}
						if ((b == 0 || b == 6) && mailSingle.Gold > 0)
						{
							list2.Add(6);
							value2 = mailSingle.Gold;
							mailSingle.Gold = 0;
						}
						if ((b == 0 || b == 7) && mailSingle.Type < 100 && mailSingle.Money > 0)
						{
							list2.Add(7);
							value = mailSingle.Money;
							mailSingle.Money = 0;
						}
						if (mailSingle.Type > 100 && mailSingle.GiftToken > 0)
						{
							list2.Add(8);
							value3 = mailSingle.GiftToken;
							mailSingle.GiftToken = 0;
						}
						if (mailSingle.Type > 100 && mailSingle.Money > 0)
						{
							mailSingle.Money = 0;
							msg = LanguageMgr.GetTranslation("MailGetAttachHandler.Deduct") + (string.IsNullOrEmpty(msg) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success") : msg);
						}
						if (playerBussiness.UpdateMail(mailSingle))
						{
							if (mailSingle.Type > 100 && money > 0)
							{
								client.Out.SendMailResponse(mailSingle.SenderID, eMailRespose.Receiver);
								client.Out.SendMailResponse(mailSingle.ReceiverID, eMailRespose.Send);
							}
							playerById.AddMoney(value);
							playerById.AddGold(value2);
							playerById.AddGiftToken(value3);
							List<string> list4 = new List<string>();
							List<eBageType> list5 = new List<eBageType>();
							foreach (string item in list3)
							{
								if (list4.Contains(item))
								{
									continue;
								}
								ItemInfo userItemSingle = playerBussiness.GetUserItemSingle(int.Parse(item));
								if (userItemSingle.Template.CategoryID == 83)
								{
									EmblemMgr.AddItem(client.Player, userItemSingle.Template.TemplateID);
									continue;
								}
								if (userItemSingle.Template.CategoryID == 82)
								{
									EmblemMgr.AddScrollItem(client.Player, userItemSingle.Template.TemplateID, userItemSingle.Count);
									continue;
								}
								eBageType ebag = eBageType.EquipBag;
								GetAnnex(item, client.Player, ref msg, ref result, ref eMsg, ref ebag);
								if (!list5.Contains(ebag))
								{
									list5.Add(ebag);
								}
								checkmaildel(playerBussiness, num2, mailByUserID, item);
								list4.Add(item);
							}
							foreach (eBageType item2 in list5)
							{
								client.Player.GetInventory(item2).SaveToDatabase();
							}
						}
					}
					else
					{
						client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MailGetAttachHandler.Falied"));
					}
					client.Player.LastGetMailID = -1;
				}
				int num3 = 0;
			}
			mailToBagInfo.List = list2;
			list.Add(mailToBagInfo);
		}
		gSPacketIn.WriteInt(list.Count);
		for (int j = 0; j < list.Count; j++)
		{
			gSPacketIn.WriteInt(list[j].ID);
			gSPacketIn.WriteInt(list[j].List.Count);
			foreach (int item3 in list[j].List)
			{
				gSPacketIn.WriteInt(item3);
			}
		}
		client.Out.SendTCP(gSPacketIn);
		client.Out.SendMessage(eMsg, string.IsNullOrEmpty(msg) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success") : msg);
		return 0;
	}
}
