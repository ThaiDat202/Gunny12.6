using System;
using System.Linq;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Packets.Client;

[PacketHandler(341, "招财神")]
public class GodOfWealthHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		switch (packet.ReadByte())
		{
		case 2:
			onPay(client.Player);
			break;
		case 3:
			onInfo(client.Player);
			break;
		}
		return 1;
	}

	private void onInfo(GamePlayer player)
	{
		ServerData serverData = new ServerData();
		GSPacketIn gSPacketIn = new GSPacketIn(341);
		gSPacketIn.WriteByte(3);
		if (player.Actives.Info.godLevel > 6)
		{
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteDateTime(DateTime.Now);
		}
		else
		{
			gSPacketIn.WriteInt(serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).NeedMoney);
			gSPacketIn.WriteInt(serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).AwardMin);
			gSPacketIn.WriteInt(serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).AwardMax);
			gSPacketIn.WriteDateTime(DateTime.Now.AddDays(1.0));
		}
		player.SendTCP(gSPacketIn);
	}

	private void onPay(GamePlayer player)
	{
		ServerData serverData = new ServerData();
		player.RemoveMoney(serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).NeedMoney);
		int num = new Random().Next(serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).AwardMin, serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).AwardMax);
		player.AddGiftToken(num);
		player.Actives.Info.godLevel++;
		GSPacketIn gSPacketIn = new GSPacketIn(341);
		gSPacketIn.WriteByte(2);
		if (player.Actives.Info.godLevel > 6)
		{
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
		}
		else
		{
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).NeedMoney);
			gSPacketIn.WriteInt(serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).AwardMin);
			gSPacketIn.WriteInt(serverData.God_Of_Wealth_Reward.FirstOrDefault((God_Of_Wealth_Reward i) => i.ID == player.Actives.Info.godLevel).AwardMax);
		}
		player.SendTCP(gSPacketIn);
	}
}
