using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class RobotManager
{
	public class Robot
	{
		public string Name { get; set; } = "Robot";


		public int Level { get; set; } = 20;


		public bool Sex { get; set; }

		public int UserType { get; set; } = 1;


		public int VIPLevel { get; set; }

		public string VIPExpireDay { get; set; } = new DateTime(2018, 1, 1).ToString();


		public List<Equip> Equips { get; set; } = new List<Equip>
		{
			new Equip()
		};

	}

	public class Equip
	{
		public int ID { get; set; } = 7001;


		public int Strength { get; set; } = 0;


		public int Compose { get; set; } = 0;

	}

	public class RobotRoom
	{
		public int PlayerCount { get; set; }

		public int MaxPlayerCount { get; set; }

		public string RoomName { get; set; } = "Robot Room Name";


		public int RoomType { get; set; }
	}

	private static List<GamePlayer> robotPlayerList = new List<GamePlayer>();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static int CurRobotID = 0;

	private static Dictionary<int, RobotGamePlayer> RobotGamePlayers = new Dictionary<int, RobotGamePlayer>();

	public static bool Start()
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			for (int i = 1; i <= 65; i++)
			{
				PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(i);
				if (userSingleByUserID != null)
				{
					GamePlayer item = new GamePlayer(-i, null, null, userSingleByUserID);
					robotPlayerList.Add(item);
				}
			}
		}
		try
		{
			foreach (GamePlayer robotPlayer in robotPlayerList)
			{
				try
				{
					RobotGamePlayer robotGamePlayer = new RobotGamePlayer(CurRobotID, new PlayerInfo
					{
						ID = CurRobotID,
						GP = robotPlayer.PlayerCharacter.GP,
						Grade = robotPlayer.Level,
						NickName = robotPlayer.PlayerCharacter.NickName,
						Texp = new TexpInfo(),
						Sex = robotPlayer.PlayerCharacter.Sex,
						VIPLevel = robotPlayer.PlayerCharacter.VIPLevel,
						VIPExpireDay = robotPlayer.PlayerCharacter.VIPExpireDay,
						typeVIP = (byte)((robotPlayer.PlayerCharacter.typeVIP != 0) ? 2u : 0u)
					});
					for (int j = 0; j < 31; j++)
					{
						ItemInfo itemAt = robotPlayer.EquipBag.GetItemAt(j);
						if (itemAt != null)
						{
							robotGamePlayer.Equip(itemAt.TemplateID, itemAt.StrengthenLevel, itemAt.AgilityCompose, itemAt.DefendCompose, itemAt.AgilityCompose, itemAt.LuckCompose);
						}
					}
					WorldMgr.AddPlayer(CurRobotID, robotGamePlayer);
					WorldMgr.NewHallRooms.PlayerInfo(robotGamePlayer);
					RoomMgr.WaitingRoom.AddPlayer(robotGamePlayer);
					WorldMgr.NewHallRooms.AddPlayerInfo(robotGamePlayer.PlayerCharacter.ID);
					RobotGamePlayers.Add(CurRobotID--, robotGamePlayer);
				}
				catch (Exception exception)
				{
					log.Error("Error when load robot, robot name: " + robotPlayer.PlayerCharacter.NickName, exception);
				}
			}
		}
		catch (Exception exception2)
		{
			log.Error("Robot Init", exception2);
			return false;
		}
		return true;
	}
}
