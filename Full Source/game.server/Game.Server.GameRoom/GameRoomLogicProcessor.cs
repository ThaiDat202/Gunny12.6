using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameRoom.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.GameRoom;

[GameRoomProcessorAtribute(40, "礼堂逻辑")]
public class GameRoomLogicProcessor : AbstractGameRoomProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private GameRoomHandleMgr _commandMgr;

	public GameRoomLogicProcessor()
	{
		_commandMgr = new GameRoomHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		GameRoomPackageType gameRoomPackageType = (GameRoomPackageType)packet.ReadInt();
		try
		{
			IGameRoomCommandHadler gameRoomCommandHadler = _commandMgr.LoadCommandHandler((int)gameRoomPackageType);
			if (gameRoomCommandHadler != null)
			{
				gameRoomCommandHadler.CommandHandler(player, packet);
				if (player.Client.Server.Configuration.DebugMode)
				{
					Console.WriteLine("GameRoomLogicProcessor PackageType {0}", gameRoomPackageType);
				}
			}
			else
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("GameRoomLogicProcessor PackageType {0} not found!", gameRoomPackageType);
				Console.WriteLine("_______________END_______________");
			}
		}
		catch (Exception ex)
		{
			log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", ex.ToString(), player.Client.TcpEndpoint));
		}
	}
}
