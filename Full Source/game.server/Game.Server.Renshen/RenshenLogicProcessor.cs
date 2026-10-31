using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Renshen.Handle;
using log4net;

namespace Game.Server.Renshen;

[RenshenProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class RenshenLogicProcessor : AbstractRenshenProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private RenshenHandleMgr _commandMgr;

	public RenshenLogicProcessor()
	{
		_commandMgr = new RenshenHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		RenshenPackageType renshenPackageType = (RenshenPackageType)packet.ReadByte();
		try
		{
			IRenshenCommandHadler renshenCommandHadler = _commandMgr.LoadCommandHandler((int)renshenPackageType);
			if (renshenCommandHadler != null)
			{
				renshenCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("RenshenPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), renshenPackageType));
		}
	}
}
