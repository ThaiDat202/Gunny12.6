using System;
using System.Threading;
using Bussiness;
using Game.Base;
using Game.Server.Packets;

namespace Game.Server.Commands.Admin;

[Cmd("&shutdown", ePrivLevel.Admin, "停止服务器的业务,并退出服务器进程", new string[] { "eg: /shutdown      在5分钟后停止服务器", "   /shutdown n     在n分钟后停止服务器" })]
public class ShutDownCommand : AbstractCommandHandler, ICommandHandler
{
	private BaseClient m_client;

	private int m_count;

	private Timer m_timer;

	public bool OnCommand(BaseClient client, string[] args)
	{
		if (client != null)
		{
			m_client = client;
			m_count = 5;
			if (args.Length > 1)
			{
				int.TryParse(args[1], out m_count);
			}
			m_count = ((m_count > 0) ? m_count : 0);
			m_timer = new Timer(ShutDownCallBack, null, 0, 60000);
		}
		else
		{
			DisplayMessage(null, "Server is shutdowning,{0} mins left!", m_count);
		}
		return true;
	}

	private void ShutDownCallBack(object state)
	{
		if (m_count > 0)
		{
			m_client.DisplayMessage($"Server will shutdown after {m_count} mins!");
			GameClient[] allClients = GameServer.Instance.GetAllClients();
			GameClient[] array = allClients;
			foreach (GameClient gameClient in array)
			{
				if (gameClient.Out != null)
				{
					gameClient.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1"), m_count, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2")));
				}
			}
			m_count--;
		}
		else
		{
			m_timer.Dispose();
			GameServer.Instance.Stop();
			m_client.DisplayMessage("Server has stopped!");
			GameServer.KeepRunning = false;
			Environment.Exit(0);
		}
	}
}
