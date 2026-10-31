using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.MoneyTree.Handle;

[MoneyTreeHandleAttbute(3)]
public class SendRedPackage : IMoneyTreeCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		List<int> list = new List<int>();
		for (int i = 0; i < num; i++)
		{
			list.Add(packet.ReadInt());
		}
		if (Player.Home.Data == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("SpeedUp.GetFail"));
		}
		else if (list.Count > Player.Home.Data.NumRedPkgRemain)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("SendRedPackage.Fail"));
		}
		else
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(11963);
			if (itemTemplateInfo == null)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("SpeedUp.GetFail"));
			}
			else
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
				itemInfo.IsBinds = true;
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				foreach (int item in list)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(item);
					PlayerInfo playerInfo = null;
					playerInfo = ((playerById == null) ? playerBussiness.GetUserSingleByUserID(item) : playerById.PlayerCharacter);
					if (playerInfo != null)
					{
						if (itemInfo.ItemID == 0)
						{
							playerBussiness.AddGoods(itemInfo);
						}
						WorldEventMgr.SendItemToMail(itemInfo, playerInfo.ID, playerInfo.NickName, Player.ZoneId, LanguageMgr.GetTranslation("SendRedPackage.SendGift"), Player.PlayerCharacter.NickName);
					}
				}
			}
			Player.Home.Data.NumRedPkgRemain = Player.Home.Data.NumRedPkgRemain - list.Count;
			GSPacketIn gSPacketIn = new GSPacketIn(296);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(Player.Home.Data.NumRedPkgRemain);
			Player.SendTCP(gSPacketIn);
			Player.SendMessage(LanguageMgr.GetTranslation("SendRedPackage.Success"));
		}
		return false;
	}
}
