using System;
using System.Reflection;
using Bussiness;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GuildBattle.Action;

public class SendMailNoticeAction : IGuildBattleAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_tick;

	private bool m_isFinished;

	public SendMailNoticeAction(int delay)
	{
		m_isFinished = false;
		m_tick += TickHelper.GetTickCount() + delay;
	}

	public void Execute(GuildBattleMgr game, long tick)
	{
		if (game.State == GuildBattleState.CHECKING && !game.IsSendMail)
		{
			game.IsSendMail = true;
			try
			{
				ConsortiaInfo[] tOPConsortiaDayOnline = game.GetTOPConsortiaDayOnline();
				if (tOPConsortiaDayOnline != null && tOPConsortiaDayOnline.Length != 0)
				{
					string text = game.InstallConsortiaList(tOPConsortiaDayOnline);
					using PlayerBussiness playerBussiness = new PlayerBussiness();
					ConsortiaInfo[] array = tOPConsortiaDayOnline;
					foreach (ConsortiaInfo consortiaInfo in array)
					{
						ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(consortiaInfo.ConsortiaID);
						ConsortiaUserInfo[] array2 = allMemberByConsortia;
						foreach (ConsortiaUserInfo consortiaUserInfo in array2)
						{
							if (consortiaUserInfo.LastDate.AddMonths(1) >= DateTime.Now)
							{
							}
						}
					}
				}
			}
			catch (Exception message)
			{
				log.Error(message);
			}
		}
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}
