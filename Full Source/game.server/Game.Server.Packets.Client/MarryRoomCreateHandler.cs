using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(241, "礼堂创建")]
public class MarryRoomCreateHandler : IPacketHandler
{
	protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (!client.Player.PlayerCharacter.IsMarried)
		{
			return 1;
		}
		if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
		{
			return 1;
		}
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		if (client.Player.CurrentRoom != null)
		{
			client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
		}
		if (client.Player.CurrentMarryRoom != null)
		{
			client.Player.CurrentMarryRoom.RemovePlayer(client.Player);
		}
		MarryRoomInfo marryRoomInfo = new MarryRoomInfo
		{
			Name = packet.ReadString(),
			Pwd = packet.ReadString(),
			MapIndex = packet.ReadInt(),
			AvailTime = packet.ReadInt(),
			MaxCount = packet.ReadInt(),
			GuestInvite = packet.ReadBoolean(),
			RoomIntroduction = packet.ReadString(),
			ServerID = GameServer.Instance.Configuration.ServerID,
			IsHymeneal = false,
			MerryActivity = packet.ReadBoolean(),
			SeniorType = packet.ReadInt()
		};
		string[] array = GameProperties.PRICE_MARRY_ROOM.Split(',');
		if (marryRoomInfo.SeniorType > 0)
		{
			array = GameProperties.SENIOR_MARRY_MONEY.Split(',');
		}
		if (array.Length < 3)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("MarryRoomCreateMoney node in configuration file is wrong");
			}
			return 1;
		}
		int num = marryRoomInfo.SeniorType switch
		{
			1 => int.Parse(array[0]), 
			2 => int.Parse(array[1]), 
			3 => int.Parse(array[2]), 
			4 => 900, 
			_ => 700, 
		};
		if (client.Player.MoneyDirect(num))
		{
			MarryRoom marryRoom = MarryRoomMgr.CreateMarryRoom(client.Player, marryRoomInfo);
			if (marryRoom != null)
			{
				GSPacketIn packet2 = client.Player.Out.SendMarryRoomInfo(client.Player, marryRoom);
				client.Player.Out.SendMarryRoomLogin(client.Player, result: true, 0);
				marryRoom.SendToScenePlayer(packet2);
				CountBussiness.InsertSystemPayCount(client.Player.PlayerCharacter.ID, num, 0, 0, 0);
			}
			return 0;
		}
		return 1;
	}
}
