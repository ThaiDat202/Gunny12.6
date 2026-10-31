using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Rooms;

namespace Game.Server.GameRoom.Handle;

[GameRoomHandleAttbute(2)]
public class SetupChange : IGameRoomCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.CurrentRoom != null && Player == Player.CurrentRoom.Host && !Player.CurrentRoom.IsPlaying)
		{
			int num = packet.ReadInt();
			eRoomType eRoomType = (eRoomType)packet.ReadByte();
			bool flag = packet.ReadBoolean();
			string pic = "";
			string password = packet.ReadString();
			string roomname = packet.ReadString();
			byte timeMode = packet.ReadByte();
			byte b = packet.ReadByte();
			int levelLimits = packet.ReadInt();
			bool isCrosszone = packet.ReadBoolean();
			int num2 = packet.ReadInt();
			packet.ReadInt();
			packet.ReadBoolean();
			if (Player.Client.Server.Configuration.DebugMode)
			{
				Console.WriteLine("MapID:{0} roomType: {1} hardLevel: {2}=>{5} isOpenBoss: {3} mapId2 {4}", num, eRoomType, (eHardLevel)b, flag, num2, b);
			}
			int bossId = 1;
			if (num == 0 && eRoomType == eRoomType.Lanbyrinth)
			{
				num = 401;
				bossId = Player.PlayerCharacter.LabyrinthInfo.currentFinishLevel;
			}
			if (eRoomType == eRoomType.ActivityDungeon)
			{
				string format = "{0}" + LanguageMgr.GetTranslation("GameRoomRoomSetupChange.Msg3");
				BaseRoom currentRoom = Player.CurrentRoom;
				List<GamePlayer> players = currentRoom.GetPlayers();
				int num3 = 0;
				string text = "";
				foreach (GamePlayer item in players)
				{
					if (item.Actives.Info.activityTanabataNum > 0)
					{
						text = ((item.PlayerCharacter.ID != Player.PlayerCharacter.ID) ? (text + item.PlayerCharacter.NickName) : (text + LanguageMgr.GetTranslation("GameRoomRoomSetupChange.Msg4")));
						text += ",";
						num3++;
					}
				}
				if (num3 > 0)
				{
					text = text.Substring(0, text.Length - 1);
					Player.SendMessage(string.Format(format, text));
					return false;
				}
			}
			if (eRoomType == eRoomType.FarmBoss)
			{
				if (!Player.Farm.CanFight())
				{
					Player.CurrentRoom.RemovePlayerUnsafe(Player);
					return false;
				}
				Player.Farm.FarmPoultryFight(ref bossId);
			}
			RoomMgr.UpdateRoomGameType(Player.CurrentRoom, eRoomType, timeMode, (eHardLevel)b, levelLimits, num, password, roomname, isCrosszone, flag, pic, bossId);
		}
		return true;
	}
}
