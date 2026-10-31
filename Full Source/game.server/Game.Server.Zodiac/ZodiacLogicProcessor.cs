using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Zodiac.Handle;
using log4net;

namespace Game.Server.Zodiac;

[ZodiacProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class ZodiacLogicProcessor : AbstractZodiacProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private ZodiacHandleMgr _commandMgr;

	public ZodiacLogicProcessor()
	{
		_commandMgr = new ZodiacHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		ZodiacPackageType zodiacPackageType = (ZodiacPackageType)packet.ReadByte();
		try
		{
			IZodiacCommandHadler zodiacCommandHadler = _commandMgr.LoadCommandHandler((int)zodiacPackageType);
			if (zodiacCommandHadler != null)
			{
				zodiacCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("ZodiacPackageType: {1}, OnGameData is Error: {0}", ex.ToString(), zodiacPackageType));
		}
	}
}
