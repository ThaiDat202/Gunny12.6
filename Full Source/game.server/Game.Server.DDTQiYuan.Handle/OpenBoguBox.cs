using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.DDTQiYuan.Handle;

[QiYuanHandleAttbute(34)]
public class OpenBoguBox : IQiYuanCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		if (packet.DataLeft <= 0)
		{
			packet.ClientID = player.PlayerId;
			packet.WriteHeader();
			GameServer.Instance.LoginServer.SendPacket(packet);
		}
		else
		{
			bool flag2 = false;
			int itemCount = player.PropBag.GetItemCount(12544);
			if (itemCount > 0)
			{
				flag2 = player.PropBag.RemoveTemplate(12544, 1);
			}
			else
			{
				int ddtLuckOpenBoxMoney = GameProperties.DdtLuckOpenBoxMoney;
				flag2 = player.RemoveMoney(ddtLuckOpenBoxMoney) > 0;
			}
			if (flag2)
			{
				QiYuanAwardInfo[] random = QiYuanAwardMgr.GetRandom(eQiYuanAwardType.TREASURE_BOX);
				if (random != null && random.Length != 0)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(random[0].TemplateID), random[0].Count, 103);
					itemInfo.IsBinds = random[0].IsBind;
					itemInfo.ValidDate = random[0].VaildDate;
					player.AddTemplate(itemInfo);
					GSPacketIn gSPacketIn = new GSPacketIn(329);
					gSPacketIn.WriteByte(34);
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(itemInfo.TemplateID);
					gSPacketIn.WriteInt(itemInfo.Count);
					gSPacketIn.WriteBoolean(itemInfo.IsBinds);
					gSPacketIn.WriteInt(itemInfo.ValidDate);
					gSPacketIn.WriteInt(itemInfo.StrengthenLevel);
					gSPacketIn.WriteInt(itemInfo.AttackCompose);
					gSPacketIn.WriteInt(itemInfo.DefendCompose);
					gSPacketIn.WriteInt(itemInfo.AgilityCompose);
					gSPacketIn.WriteInt(itemInfo.LuckCompose);
					gSPacketIn.WriteInt(itemInfo.MagicAttack);
					gSPacketIn.WriteInt(itemInfo.MagicDefence);
					player.SendTCP(gSPacketIn);
				}
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("QiYuan.CenterServer.OffterTimesNoMoney"));
			}
		}
		return 1;
	}
}
