using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.MoneyTree.Handle;
using Game.Server.Packets;
using log4net;

namespace Game.Server.MoneyTree;

[MoneyTreeProcessorAtribute(byte.MaxValue, "礼堂逻辑")]
public class MoneyTreeLogicProcessor : AbstractMoneyTreeProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private MoneyTreeHandleMgr _commandMgr;

	public MoneyTreeLogicProcessor()
	{
		_commandMgr = new MoneyTreeHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		MoneyTreePackageType moneyTreePackageType = (MoneyTreePackageType)packet.ReadByte();
		try
		{
			IMoneyTreeCommandHadler moneyTreeCommandHadler = _commandMgr.LoadCommandHandler((int)moneyTreePackageType);
			if (moneyTreeCommandHadler != null)
			{
				moneyTreeCommandHadler.CommandHandler(player, packet);
				return;
			}
			Console.WriteLine("______________ERROR______________");
			Console.WriteLine("LoadCommandHandler not found!");
			Console.WriteLine("_______________END_______________");
		}
		catch (Exception ex)
		{
			log.Error(string.Format("MoneyTreePackageType: {1}, OnGameData is Error: {0}", ex.ToString(), moneyTreePackageType));
		}
	}
}
