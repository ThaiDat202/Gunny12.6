using System;
using System.Linq;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(76)]
public class DDPLayStart : IActiveSystemCommandHadler
{
	public static Random random = new Random();

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(76);
		int num = 0;
		if (Player.PropBag.GetItemCount(201310) <= 0)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("DDPLayStart.NotEnoughtCoin"));
		}
		else
		{
			Player.PropBag.RemoveTemplate(201310, 1);
			Player.Actives.Info.DDPlayPoint += 20;
			num = random.Next(100);
			int[] source = new int[7] { 1, 2, 3, 4, 5, 20, 50 };
			if (source.Contains(num))
			{
				int money = 10000 * num;
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					Player.SendMoneyMailToUser(pb, LanguageMgr.GetTranslation("DDPLayStart.XCoin", num), LanguageMgr.GetTranslation("DDPLayStart.Title", num), money, eMailType.BuyItem);
				}
				if (source.Contains(num) && num == 50)
				{
					string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation6", Player.PlayerCharacter.NickName);
					GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation);
					GameServer.Instance.LoginServer.SendPacket(packet2);
				}
				if (source.Contains(num) && num == 20)
				{
					string translation2 = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation7", Player.PlayerCharacter.NickName);
					GSPacketIn packet3 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation2);
					GameServer.Instance.LoginServer.SendPacket(packet3);
				}
			}
			else
			{
				num = 0;
			}
		}
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteInt(Player.Actives.Info.DDPlayPoint);
		Player.Out.SendTCP(gSPacketIn);
		return true;
	}
}
