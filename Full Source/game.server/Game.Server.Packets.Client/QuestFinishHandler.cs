using System;
using Game.Base.Packets;
using Game.Server.Quests;

namespace Game.Server.Packets.Client;

[PacketHandler(179, "任务完成")]
public class QuestFinishHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int selectedItem = packet.ReadInt();
		if (DateTime.Compare(client.Player.LastDrillUpTime.AddSeconds(1.0), DateTime.Now) > 0)
		{
			return 0;
		}
		client.Player.LastDrillUpTime = DateTime.Now;
		BaseQuest baseQuest = client.Player.QuestInventory.FindQuest(num);
		bool flag = false;
		if (baseQuest != null)
		{
			flag = client.Player.QuestInventory.Finish(baseQuest, selectedItem);
		}
		if (flag)
		{
			client.Player.OnQuestFinish(baseQuest.Data, baseQuest.Info);
			GSPacketIn gSPacketIn = new GSPacketIn(179, client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteInt(num);
			client.Out.SendTCP(gSPacketIn);
		}
		return 1;
	}
}
