using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.SanXiao.Handle;
using log4net;

namespace Game.Server.SanXiao;

[SanXiaoProcessorAtribute(40, "礼堂逻辑")]
public class SanXiaoLogicProcessor : AbstractSanXiaoProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private SanXiaoHandleMgr _commandMgr;

	public SanXiaoLogicProcessor()
	{
		_commandMgr = new SanXiaoHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		SanXiaoPackageType sanXiaoPackageType = (SanXiaoPackageType)packet.ReadByte();
		try
		{
			ISanXiaoCommandHadler sanXiaoCommandHadler = _commandMgr.LoadCommandHandler((int)sanXiaoPackageType);
			if (sanXiaoCommandHadler != null)
			{
				sanXiaoCommandHadler.CommandHandler(player, packet);
				if (player.Client.Server.Configuration.DebugMode)
				{
					Console.WriteLine(sanXiaoCommandHadler);
				}
			}
			else
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("SanXiaoLogicProcessor SanXiaoPackageType.{0} not found!", sanXiaoPackageType);
				Console.WriteLine("_______________END_______________");
			}
		}
		catch (Exception ex)
		{
			log.Error(string.Format("SanXiaoLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), sanXiaoPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}
