using System;
using System.Reflection;
using Bussiness.Protocol;
using Game.Base.Packets;
using Game.Server.DDTQiYuan.Handle;
using Game.Server.GameObjects;
using log4net;

namespace Game.Server.DDTQiYuan;

[QiYuanProcessorAtribute(40, "礼堂逻辑")]
public class QiYuanLogicProcessor : AbstractQiYuanProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private QiYuanHandleMgr _commandMgr;

	public QiYuanLogicProcessor()
	{
		_commandMgr = new QiYuanHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		QiYuanPackageType qiYuanPackageType = (QiYuanPackageType)packet.ReadByte();
		try
		{
			IQiYuanCommandHadler qiYuanCommandHadler = _commandMgr.LoadCommandHandler((int)qiYuanPackageType);
			if (qiYuanCommandHadler != null)
			{
				qiYuanCommandHadler.CommandHandler(player, packet);
				if (player.Client.Server.Configuration.DebugMode)
				{
					Console.WriteLine(qiYuanCommandHadler);
				}
			}
			else
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("QiYuanLogicProcessor QiYuanPackageType.{0} not found!", qiYuanPackageType);
				Console.WriteLine("_______________END_______________");
			}
		}
		catch (Exception ex)
		{
			log.Error(string.Format("QiYuanLogicProcessor PackageType:{1}, OnGameData is Error: {0}", ex.ToString(), qiYuanPackageType));
			log.Error($"IP: {player.Client.TcpEndpoint}");
		}
	}
}
