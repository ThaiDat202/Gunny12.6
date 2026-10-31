using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;

namespace Game.Server.GameRoom.Handle;

[GameRoomHandleAttbute(0)]
public class Create : IGameRoomCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		byte timeType = packet.ReadByte();
		string name = packet.ReadString();
		string password = packet.ReadString();
		if (b == 15)
		{
			if (Player.PlayerCharacter.LabyrinthInfo.nowContinueCount <= 0)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("GameRoomCreate.Msg1"));
				return false;
			}
			Player.PlayerCharacter.LabyrinthInfo.isPlayGame = true;
		}
		if (b == 14)
		{
			if (!RoomMgr.WorldBossRoom.worldOpen)
			{
				Player.CurrentRoom.RemovePlayerUnsafe(Player);
				return false;
			}
			double timeDelay = GetTimeDelay(Player.FightPower);
			int num = DateTime.Compare(Player.LastEnterWorldBoss.AddSeconds(timeDelay), DateTime.Now);
			if (num > 0)
			{
				Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GameRoomCreate.Msg2", num));
				return false;
			}
			Player.LastEnterWorldBoss = DateTime.Now;
			Player.WorldbossBood = RoomMgr.WorldBossRoom.Blood;
			BufferList.CreatePayBuffer(400, 500000, 1)?.Start(Player);
			BufferList.CreatePayBuffer(406, 80000, 1)?.Start(Player);
		}
		RoomMgr.CreateRoom(Player, name, password, (eRoomType)b, timeType);
		return true;
	}

	public double GetTimeDelay(int fightPower)
	{
		if (fightPower < 100000000)
		{
			return 5.0;
		}
		if (fightPower > 100000000)
		{
			return 60.0;
		}
		return 5.0;
	}
}
