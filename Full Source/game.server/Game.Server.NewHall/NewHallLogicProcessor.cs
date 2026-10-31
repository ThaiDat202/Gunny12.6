using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.NewHall.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.NewHall;

[NewHallProcessorAtribute(40, "礼堂逻辑")]
public class NewHallLogicProcessor : AbstractNewHallProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private NewHallHandleMgr _commandMgr;

	public NewHallLogicProcessor()
	{
		_commandMgr = new NewHallHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		NewHallPackageType code = (NewHallPackageType)packet.ReadByte();
		try
		{
			INewHallCommandHadler newHallCommandHadler = _commandMgr.LoadCommandHandler((int)code);
			if (newHallCommandHadler != null)
			{
				newHallCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", ex.ToString(), player.Client.TcpEndpoint));
		}
	}
}
