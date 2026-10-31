using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.DDTQiYuan.Handle;

[QiYuanHandleAttbute(38)]
public class OffterTimes : IQiYuanCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool flag2 = false;
		num = ((num <= 0) ? 1 : 10);
		int itemCount = player.PropBag.GetItemCount(12543);
		if (itemCount >= num)
		{
			flag2 = player.PropBag.RemoveTemplate(12543, num);
		}
		else
		{
			string[] array = GameProperties.DdtLuckWorshipMoney.Split('|');
			int num2 = 0;
			switch (num)
			{
			case 1:
				num2 = int.Parse(array[0]);
				break;
			case 10:
				num2 = int.Parse(array[1]);
				break;
			}
			if (num2 > 0 && player.PlayerCharacter.Money >= num2)
			{
				player.RemoveMoney(num2);
				flag2 = true;
			}
		}
		if (flag2)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			for (int i = 0; i < num; i++)
			{
				QiYuanAwardInfo[] random = QiYuanAwardMgr.GetRandom(eQiYuanAwardType.REWARD_BOX);
				if (random != null && random.Length != 0)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(random[0].TemplateID), random[0].Count, 103);
					itemInfo.IsBinds = random[0].IsBind;
					itemInfo.ValidDate = random[0].VaildDate;
					list.Add(itemInfo);
				}
			}
			packet.ClientID = player.PlayerId;
			packet.WriteHeader();
			GSPacketIn gSPacketIn = new GSPacketIn(329);
			gSPacketIn.WriteByte(38);
			gSPacketIn.WriteInt(list.Count);
			packet.WriteInt(list.Count);
			foreach (ItemInfo item in list)
			{
				player.AddTemplate(item);
				gSPacketIn.WriteInt(item.TemplateID);
				gSPacketIn.WriteInt(item.Count);
				gSPacketIn.WriteBoolean(item.IsBinds);
				gSPacketIn.WriteInt(item.ValidDate);
				gSPacketIn.WriteInt(item.StrengthenLevel);
				gSPacketIn.WriteInt(item.AttackCompose);
				gSPacketIn.WriteInt(item.DefendCompose);
				gSPacketIn.WriteInt(item.AgilityCompose);
				gSPacketIn.WriteInt(item.LuckCompose);
				gSPacketIn.WriteInt(item.MagicAttack);
				gSPacketIn.WriteInt(item.MagicDefence);
				packet.WriteInt(item.TemplateID);
				packet.WriteInt(item.Count);
			}
			player.SendTCP(gSPacketIn);
			GameServer.Instance.LoginServer.SendPacket(packet);
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("QiYuan.CenterServer.OffterTimesNoMoney"));
		}
		return 1;
	}
}
