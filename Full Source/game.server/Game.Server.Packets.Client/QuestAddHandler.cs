using System.Collections.Generic;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(176, "添加任务")]
public class QuestAddHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int num2 = packet.ReadInt();
			QuestInfo singleQuest = QuestMgr.GetSingleQuest(num2);
			if (singleQuest != null && singleQuest.QuestID != 10)
			{
				client.Player.QuestInventory.AddQuest(singleQuest, out var _);
			}
			if (num2 < 5300 || num2 > 5350)
			{
				continue;
			}
			List<ItemInfo> strengthenItems = client.Player.GetStrengthenItems();
			foreach (ItemInfo item in strengthenItems)
			{
				if (item != null)
				{
					client.Player.OnItemStrengthen(item.Template.CategoryID, item.StrengthenLevel);
				}
			}
		}
		return 0;
	}
}
