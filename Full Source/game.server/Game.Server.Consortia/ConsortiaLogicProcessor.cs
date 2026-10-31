using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.Consortia.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.Consortia;

[ConsortiaProcessorAtribute(99, "礼堂逻辑")]
public class ConsortiaLogicProcessor : AbstractConsortiaProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private ConsortiaHandleMgr _commandMgr;

	public ConsortiaLogicProcessor()
	{
		_commandMgr = new ConsortiaHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		ConsortiaPackageType consortiaPackageType = (ConsortiaPackageType)packet.ReadInt();
		try
		{
			IConsortiaCommandHadler consortiaCommandHadler = _commandMgr.LoadCommandHandler((int)consortiaPackageType);
			if (consortiaCommandHadler != null)
			{
				consortiaCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception)
		{
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("ConsortiaLogicProcessor PackageType {0} not found!", consortiaPackageType);
			Console.WriteLine("_______________END_______________");
		}
	}
}
