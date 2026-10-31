using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Pet.Handle;
using log4net;

namespace Game.Server.Pet;

[PetProcessorAtribute(40, "礼堂逻辑")]
public class PetLogicProcessor : AbstractPetProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private PetHandleMgr _commandMgr;

	public PetLogicProcessor()
	{
		_commandMgr = new PetHandleMgr();
	}

	public override void OnGameData(GamePlayer player, GSPacketIn packet)
	{
		PetPackageType petPackageType = (PetPackageType)packet.ReadByte();
		try
		{
			IPetCommandHadler petCommandHadler = _commandMgr.LoadCommandHandler((int)petPackageType);
			if (petCommandHadler != null)
			{
				if (player.Client.Server.Configuration.DebugMode)
				{
					Console.WriteLine(petCommandHadler);
				}
				petCommandHadler.CommandHandler(player, packet);
			}
			else
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("LoadCommandHandler not found!");
				Console.WriteLine("_______________END_______________");
			}
		}
		catch (Exception ex)
		{
			log.Error(string.Format("PetLogicProcessor PackageType {2} :{1}, OnGameData is Error: {0}", ex.ToString(), player.Client.TcpEndpoint, petPackageType));
		}
	}
}
