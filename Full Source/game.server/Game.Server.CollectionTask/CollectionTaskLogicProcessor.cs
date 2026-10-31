using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.CollectionTask.Handle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;

namespace Game.Server.CollectionTask;

[CollectionTaskProcessorAtribute(40, "礼堂逻辑")]
public class CollectionTaskLogicProcessor : AbstractCollectionTaskProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private CollectionTaskHandleMgr _commandMgr;

	public CollectionTaskLogicProcessor()
	{
		_commandMgr = new CollectionTaskHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		CollectionTaskPackageType code = (CollectionTaskPackageType)packet.ReadByte();
		try
		{
			ICollectionTaskCommandHadler collectionTaskCommandHadler = _commandMgr.LoadCommandHandler((int)code);
			if (collectionTaskCommandHadler != null)
			{
				collectionTaskCommandHadler.CommandHandler(player, packet);
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
