using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GuildBattle.Action;

public class SendAwardAction : IGuildBattleAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private long m_tick;

	private bool m_isFinished;

	public SendAwardAction(int delay)
	{
		m_isFinished = false;
		m_tick += TickHelper.GetTickCount() + delay;
	}

	public void Execute(GuildBattleMgr game, long tick)
	{
		if (game.State == GuildBattleState.CLOSE && !game.IsSendAward)
		{
			game.IsSendAward = true;
			List<ItemInfo> list = null;
			GuildBattleConsortiaInfo[] array = (from a in game.GetAllConsortia()
				orderby a.Score descending
				select a).ToArray();
			UserGuildBattleInfo[] source = game.GetAllUser().ToArray();
			int num = 1;
			int num2 = 1;
			GuildBattleConsortiaInfo[] array2 = array;
			foreach (GuildBattleConsortiaInfo cor in array2)
			{
				UserGuildBattleInfo[] array3 = (from a in source
					where a.ConsortiaID == cor.ConsortiaID
					orderby a.Score descending
					select a).ToArray();
				num2 = 1;
				UserGuildBattleInfo[] array4 = array3;
				foreach (UserGuildBattleInfo userGuildBattleInfo in array4)
				{
					switch (num)
					{
					}
					if (list == null || list.Count > 0)
					{
					}
					if (num2 <= 20)
					{
						list = EventAwardMgr.GetEventAwardByType(eEventType.GUILD_BATTLE_PERSON_TOP);
						if (list != null && list.Count > 0)
						{
							WorldEventMgr.SendItemsToMails(list, userGuildBattleInfo.UserID, userGuildBattleInfo.NickName, GameServer.Instance.Configuration.AreaId, LanguageMgr.GetTranslation("GameServer.GuildBattle.MailAwardPerson.Title"), LanguageMgr.GetTranslation("GameServer.GuildBattle.MailAwardPerson.Content"));
						}
					}
					num2++;
				}
				num++;
			}
			game.SaveCurrentRankToDatabase(clear: true);
		}
		m_isFinished = true;
	}

	public bool IsFinished(long tick)
	{
		return m_isFinished;
	}
}
