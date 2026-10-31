using System.Runtime.CompilerServices;
using Game.Server;
using Game.Server.Packets;

namespace Game.Service.GMService;

public class Service : IService
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	public void ShowMessage(int type, string msg)
	{
		GameClient[] allClients = GameServer.Instance.GetAllClients();
		GameClient[] array = allClients;
		foreach (GameClient gameClient in array)
		{
			if (gameClient.Out != null)
			{
				gameClient.Out.SendMessage((eMessageType)type, msg);
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public Service()
	{
	}
}
