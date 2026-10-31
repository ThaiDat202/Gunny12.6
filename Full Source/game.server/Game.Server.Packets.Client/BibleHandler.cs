using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(652, "弹王宝典")]
public class BibleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(652);
		gSPacketIn.WriteInt(client.Player.PlayerCharacter.CardInfo.CardLevel);
		gSPacketIn.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.manualLevel);
		gSPacketIn.WriteInt(0);
		client.Out.SendTCP(gSPacketIn);
		client.Player.EquipBag.UpdatePlayerProperties();
		return 0;
	}
}
