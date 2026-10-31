using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SixHardThirdNpc : ABrain
{
	private int m_attackTurn = 0;

	private bool m_runEffectBlock = false;

	private Player m_lastAttack;

	private Dictionary<int, int> m_countPlayerDamage = new Dictionary<int, int>();

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.MaxBeatDis = 200;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_attackTurn++;
		if (m_runEffectBlock)
		{
			m_runEffectBlock = false;
			int num = m_countPlayerDamage[m_lastAttack.PlayerDetail.PlayerCharacter.ID];
			if (num > 2)
			{
				base.Body.Say("<span class='red'>" + m_lastAttack.PlayerDetail.PlayerCharacter.NickName + "</span> nhắc nhiều vẫn lì. Ra khỏi sân ngay!!", 0, 1000);
				base.Body.PlayMovie("beat", 1000, 5000);
				m_lastAttack.SyncAtTime = true;
				m_lastAttack.Die();
			}
			else if (num == 2)
			{
				base.Body.Say("<span class='red'>" + m_lastAttack.PlayerDetail.PlayerCharacter.NickName + "</span> đánh trọng tài lần nữa ta sẽ đuổi khỏi sân.", 0, 1000, 4000);
			}
			else
			{
				base.Body.Say("<span class='red'>" + m_lastAttack.PlayerDetail.PlayerCharacter.NickName + "</span> đánh trọng tài là vi phạm quy tắc.", 0, 1000, 4000);
			}
		}
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnAfterTakedBomb()
	{
		base.OnAfterTakedBomb();
	}

	public override void OnAfterTakeDamage(Living source)
	{
		base.OnAfterTakeDamage(source);
		if (source is Player && !m_runEffectBlock)
		{
			m_lastAttack = source as Player;
			int iD = (source as Player).PlayerDetail.PlayerCharacter.ID;
			if (m_countPlayerDamage.ContainsKey(iD))
			{
				m_countPlayerDamage[iD]++;
			}
			else
			{
				m_countPlayerDamage.Add(iD, 1);
			}
			(base.Game as PVEGame).SendLivingActionMapping(base.Body, "stand", "stand");
			(base.Body as SimpleBoss).Delay = base.Game.GetLowDelayTurn() - 1;
			m_runEffectBlock = true;
			base.Body.Config.CompleteStep = false;
		}
	}
}
