using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.BuffHorseRace;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(282, "场景用户离开")]
public class HorseRaceHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		switch (packet.ReadInt())
		{
		case 1:
		{
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			int num4 = packet.ReadInt();
			int num5 = packet.ReadInt();
			if (client.Player.PlayerCharacter.ID != num3 || client.Player.CurrentHorseRaceInfo == null || client.Player.CurrentHorseRaceRoom == null)
			{
				break;
			}
			GamePlayer gamePlayer = null;
			BuffHorseRaceInfo buffHorseRaceInfo = null;
			AbstractHorseRaceBuffer abstractHorseRaceBuffer = null;
			GamePlayer[] array = null;
			string buffName = LanguageMgr.GetTranslation("GameServer.HorseGame.Msg.All");
			if (num4 != 0)
			{
				gamePlayer = client.Player.CurrentHorseRaceRoom.GetPlayerByID(num4);
			}
			switch (num2)
			{
			case 1:
				if (num4 == 0 || num4 == num3)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoSelectPlayer"));
					return 0;
				}
				if (gamePlayer == null)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoHavePlayer"));
					return 0;
				}
				buffHorseRaceInfo = new BuffHorseRaceInfo(1, 3);
				abstractHorseRaceBuffer = gamePlayer.CurrentHorseRaceInfo.BuffList.CreateBuffer(buffHorseRaceInfo, 0);
				if (abstractHorseRaceBuffer != null && client.Player.CurrentHorseRaceInfo.RemoveBuff(num2))
				{
					abstractHorseRaceBuffer.Start(gamePlayer.CurrentHorseRaceInfo, client.Player.CurrentHorseRaceInfo);
					buffName = gamePlayer.PlayerCharacter.NickName;
				}
				break;
			case 5:
				if (num4 == 0 || num4 == num3)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoSelectPlayer"));
					return 0;
				}
				if (gamePlayer == null)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoHavePlayer"));
					return 0;
				}
				buffHorseRaceInfo = new BuffHorseRaceInfo(5, 10);
				abstractHorseRaceBuffer = gamePlayer.CurrentHorseRaceInfo.BuffList.CreateBuffer(buffHorseRaceInfo, gamePlayer.CurrentHorseRaceInfo.Speed);
				if (abstractHorseRaceBuffer != null && client.Player.CurrentHorseRaceInfo.RemoveBuff(num2))
				{
					abstractHorseRaceBuffer.Start(client.Player.CurrentHorseRaceInfo, client.Player.CurrentHorseRaceInfo);
					buffName = gamePlayer.PlayerCharacter.NickName;
				}
				break;
			case 7:
			{
				buffHorseRaceInfo = new BuffHorseRaceInfo(7, 2);
				abstractHorseRaceBuffer = client.Player.CurrentHorseRaceInfo.BuffList.CreateBuffer(buffHorseRaceInfo, 0);
				if (abstractHorseRaceBuffer == null || !client.Player.CurrentHorseRaceInfo.RemoveBuff(num2))
				{
					break;
				}
				array = client.Player.CurrentHorseRaceRoom.GetPlayersUnSafe();
				GamePlayer[] array2 = array;
				foreach (GamePlayer gamePlayer2 in array2)
				{
					if (gamePlayer2 != client.Player)
					{
						abstractHorseRaceBuffer = client.Player.CurrentHorseRaceInfo.BuffList.CreateBuffer(buffHorseRaceInfo, 0);
						abstractHorseRaceBuffer.Start(gamePlayer2.CurrentHorseRaceInfo, client.Player.CurrentHorseRaceInfo);
					}
				}
				break;
			}
			case 3:
				if (num4 == 0 || num4 == num3)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoSelectPlayer"));
					return 0;
				}
				if (gamePlayer == null)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoHavePlayer"));
					return 0;
				}
				buffHorseRaceInfo = new BuffHorseRaceInfo(3, 5);
				abstractHorseRaceBuffer = client.Player.CurrentHorseRaceInfo.BuffList.CreateBuffer(buffHorseRaceInfo, 0);
				if (abstractHorseRaceBuffer != null && client.Player.CurrentHorseRaceInfo.RemoveBuff(num2))
				{
					abstractHorseRaceBuffer.Start(client.Player.CurrentHorseRaceInfo, client.Player.CurrentHorseRaceInfo);
					abstractHorseRaceBuffer = client.Player.CurrentHorseRaceInfo.BuffList.CreateBuffer(buffHorseRaceInfo, 0);
					abstractHorseRaceBuffer.Start(gamePlayer.CurrentHorseRaceInfo, client.Player.CurrentHorseRaceInfo);
					buffName = gamePlayer.PlayerCharacter.NickName;
				}
				break;
			case 2:
				if (num4 == 0 || num4 == num3)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoSelectPlayer"));
					return 0;
				}
				if (gamePlayer == null)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoHavePlayer"));
					return 0;
				}
				buffHorseRaceInfo = new BuffHorseRaceInfo(2, 5);
				abstractHorseRaceBuffer = client.Player.CurrentHorseRaceInfo.BuffList.CreateBuffer(buffHorseRaceInfo, 0);
				if (abstractHorseRaceBuffer != null && client.Player.CurrentHorseRaceInfo.RemoveBuff(num2))
				{
					abstractHorseRaceBuffer.Start(gamePlayer.CurrentHorseRaceInfo, client.Player.CurrentHorseRaceInfo);
					buffName = gamePlayer.PlayerCharacter.NickName;
				}
				break;
			case 6:
				buffHorseRaceInfo = new BuffHorseRaceInfo(6, 5);
				abstractHorseRaceBuffer = client.Player.CurrentHorseRaceInfo.BuffList.CreateBuffer(buffHorseRaceInfo, 0);
				if (abstractHorseRaceBuffer != null && client.Player.CurrentHorseRaceInfo.RemoveBuff(num2))
				{
					abstractHorseRaceBuffer.Start(client.Player.CurrentHorseRaceInfo, client.Player.CurrentHorseRaceInfo);
					buffName = client.Player.PlayerCharacter.NickName;
				}
				break;
			}
			client.Player.CurrentHorseRaceRoom.SendShowMsg(client.Player.PlayerCharacter.NickName, buffName, LanguageMgr.GetTranslation(NameBuffer(num2)));
			client.Player.CurrentHorseRaceRoom.SendUpdateBuffItem(client.Player, successpingzhang: false);
			break;
		}
		case 2:
		{
			if (client.Player.CurrentHorseRaceInfo == null)
			{
				return 0;
			}
			int horseGameUsePapawMoney = GameProperties.HorseGameUsePapawMoney;
			if (client.Player.MoneyDirect(horseGameUsePapawMoney))
			{
				client.Player.CurrentHorseRaceInfo.BuffList.StopAll();
				client.Player.CurrentHorseRaceRoom.SendUpdateBuffItem(client.Player, successpingzhang: true);
			}
			break;
		}
		case 3:
			if (client.Player.CurrentHorseRaceInfo != null)
			{
				client.Player.CurrentHorseRaceInfo.FinishTime = DateTime.Now;
			}
			break;
		case 4:
		{
			int horseGameCostMoneyCount = GameProperties.HorseGameCostMoneyCount;
			if (client.Player.MoneyDirect(horseGameCostMoneyCount))
			{
				client.Player.AddHorseRaceTimes(1);
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.BuyTimesSucess"));
			}
			break;
		}
		}
		return 0;
	}

	private string NameBuffer(int type)
	{
		return type switch
		{
			1 => "GameServer.HorseGame.Msg.BuffName1", 
			2 => "GameServer.HorseGame.Msg.BuffName2", 
			3 => "GameServer.HorseGame.Msg.BuffName3", 
			5 => "GameServer.HorseGame.Msg.BuffName5", 
			6 => "GameServer.HorseGame.Msg.BuffName6", 
			7 => "GameServer.HorseGame.Msg.BuffName7", 
			8 => "GameServer.HorseGame.Msg.BuffName8", 
			_ => "Unknown", 
		};
	}
}
