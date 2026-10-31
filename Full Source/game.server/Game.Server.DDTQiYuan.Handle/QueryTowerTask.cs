using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DDTQiYuan.Handle;

[QiYuanHandleAttbute(35)]
public class QueryTowerTask : IQiYuanCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		packet.ClientID = player.PlayerId;
		packet.WriteHeader();
		GameServer.Instance.LoginServer.SendPacket(packet);
		return 1;
	}
}
