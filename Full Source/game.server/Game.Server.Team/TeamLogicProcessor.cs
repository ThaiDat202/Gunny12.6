using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Team.Handle;
using log4net;

namespace Game.Server.Team;

public class TeamLogicProcessor : AbstractTeamProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private TeamHandleMgr _commandMgr;

	public TeamLogicProcessor()
	{
		_commandMgr = new TeamHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		eTeamPackageType eTeamPackageType = (eTeamPackageType)packet.ReadByte();
		try
		{
			ITeamCommandHandler teamCommandHandler = _commandMgr.LoadCommandHandler((int)eTeamPackageType);
			if (teamCommandHandler != null)
			{
				teamCommandHandler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("TeamLogicProcessor eTeamPackageType.{0} not found!", eTeamPackageType);
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("TeamLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), eTeamPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}
