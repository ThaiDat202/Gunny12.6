using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Labyrinth.Handle;

[LabyrinthHandle(10)]
internal class RequestRank : ILabyrinthCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int val = packet.ReadInt();
		int val2 = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(131);
		gSPacketIn.WriteByte(10);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(val2);
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			WarriorFamRankInfo[] allWarriorFamRank = playerBussiness.GetAllWarriorFamRank();
			gSPacketIn.WriteInt(allWarriorFamRank.Length);
			WarriorFamRankInfo[] array = allWarriorFamRank;
			foreach (WarriorFamRankInfo warriorFamRankInfo in array)
			{
				gSPacketIn.WriteInt(warriorFamRankInfo.PlayerRank);
				gSPacketIn.WriteInt(1001);
				gSPacketIn.WriteString("弹弹堂");
				gSPacketIn.WriteInt(warriorFamRankInfo.UserID);
				gSPacketIn.WriteString(warriorFamRankInfo.PlayerName);
				gSPacketIn.WriteInt(warriorFamRankInfo.FamLevel);
				gSPacketIn.WriteInt(warriorFamRankInfo.Turn);
			}
			Player.SendTCP(gSPacketIn);
		}
		return 0;
	}
}
