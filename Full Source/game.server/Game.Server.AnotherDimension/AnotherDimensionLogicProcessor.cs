using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.AnotherDimension.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.AnotherDimension;

[AnotherDimensionProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class AnotherDimensionLogicProcessor : AbstractAnotherDimensionProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private AnotherDimensionHandleMgr _commandMgr;

	public AnotherDimensionLogicProcessor()
	{
		_commandMgr = new AnotherDimensionHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		AnotherDimensionPackageType anotherDimensionPackageType = (AnotherDimensionPackageType)packet.ReadInt();
		try
		{
			IAnotherDimensionCommandHadler anotherDimensionCommandHadler = _commandMgr.LoadCommandHandler((int)anotherDimensionPackageType);
			if (anotherDimensionCommandHadler != null)
			{
				anotherDimensionCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("AnotherDimensionPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), anotherDimensionPackageType));
		}
	}
}
