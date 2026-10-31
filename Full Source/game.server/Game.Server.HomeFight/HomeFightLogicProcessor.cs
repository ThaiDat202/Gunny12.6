using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.HomeFight.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.HomeFight;

[HomeFightProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class HomeFightLogicProcessor : AbstractHomeFightProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private HomeFightHandleMgr _commandMgr;

	public HomeFightLogicProcessor()
	{
		_commandMgr = new HomeFightHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		HomeFightPackageType homeFightPackageType = (HomeFightPackageType)packet.ReadByte();
		try
		{
			IHomeFightCommandHadler homeFightCommandHadler = _commandMgr.LoadCommandHandler((int)homeFightPackageType);
			if (homeFightCommandHadler != null)
			{
				homeFightCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("HomeFightPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), homeFightPackageType));
		}
	}
}
