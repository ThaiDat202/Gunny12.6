using System;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourHardWolfNpc : ABrain
{
	private Player m_targetPlayer = null;

	private int friendlyNpcId = 4205;

	private SimpleBoss friendBoss = null;

	private bool OpenEye = false;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		if (OpenEye)
		{
			base.Body.Config.IsShield = true;
		}
		else
		{
			base.Body.Config.IsShield = false;
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
		if (friendBoss == null)
		{
			SimpleBoss[] array = base.Game.FindLivingTurnBossWithID(friendlyNpcId);
			int num = 0;
			if (num < array.Length)
			{
				SimpleBoss simpleBoss = (friendBoss = array[num]);
			}
		}
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		if ((int)friendBoss.Properties1 == 0 && OpenEye)
		{
			base.Body.PlayMovie("BtoA", 1000, 4000);
			base.Body.CallFuction(RemoveTargetPlayer, 5000);
			OpenEye = false;
		}
		else if ((int)friendBoss.Properties1 == 1 && !OpenEye)
		{
			base.Body.PlayMovie("AtoB", 2000, 4000);
			base.Body.CallFuction(FindRandomPlayer, 4000);
			OpenEye = true;
		}
		else if (OpenEye && m_targetPlayer != null)
		{
			int num = base.Game.Random.Next(100);
			if (num < 50 && base.Body.Distance(m_targetPlayer.X, m_targetPlayer.Y) > 600.0)
			{
				JumpToTargetPlayer();
			}
			else
			{
				RunToTargetPlayer();
			}
		}
		else if (!OpenEye)
		{
			RandomMove();
		}
		else
		{
			Console.WriteLine("eye: " + OpenEye + " - friendBoss.Properties1: " + friendBoss.Properties1);
		}
	}

	private void RandomMove()
	{
		base.Body.ChangeDirection(-1, 1200);
		base.Body.ChangeDirection(1, 1800);
		base.Body.ChangeDirection(-1, 2300);
		int num = base.Game.Random.Next(-300, 300);
		base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walkA", 3500, 3, NothingAction);
	}

	private void NothingAction()
	{
		base.Body.ChangeDirection(1, 500);
		base.Body.ChangeDirection(-1, 1000);
		base.Body.PlayMovie("beatA", 1000, 2000, SetState);
	}

	private void FindRandomPlayer()
	{
		m_targetPlayer = base.Game.FindRandomPlayer();
		if (m_targetPlayer != null)
		{
			base.Body.ChangeDirection(m_targetPlayer, 500);
			((PVEGame)base.Game).SendPlayersPicture(m_targetPlayer, 7, state: true);
			base.Body.Say("Lo chạy đi <span class=\"red\">" + m_targetPlayer.PlayerDetail.PlayerCharacter.NickName + "</span>. Ta đến đây...", 0, 2000, 0);
			base.Body.CallFuction(SetState, 5000);
		}
	}

	private void RemoveTargetPlayer()
	{
		if (m_targetPlayer != null)
		{
			((PVEGame)base.Game).SendPlayersPicture(m_targetPlayer, 7, state: false);
			m_targetPlayer = null;
		}
		base.Body.CallFuction(SetState, 1000);
	}

	private void RunToTargetPlayer()
	{
		base.Body.ChangeDirection(m_targetPlayer, 200);
		double num = base.Body.Distance(m_targetPlayer.X, m_targetPlayer.Y);
		if (num > 600.0)
		{
			base.Body.MoveTo(m_targetPlayer.X, m_targetPlayer.Y, "walkB", 2200, 10, BeatTargetPlayer);
		}
		else
		{
			base.Body.MoveTo(m_targetPlayer.X, m_targetPlayer.Y, "walkC", 2200, 5, BeatTargetPlayer);
		}
	}

	private void JumpToTargetPlayer()
	{
		base.Body.ChangeDirection(m_targetPlayer, 200);
		base.Body.PlayMovie("jump", 2000, 0);
		((PVEGame)base.Game).SendObjectFocus(m_targetPlayer, 1, 5000, 0);
		base.Body.BoltMove(m_targetPlayer.X, m_targetPlayer.Y, 6000);
		base.Body.PlayMovie("fall", 7000, 0);
		base.Body.RangeAttacking(m_targetPlayer.X - 100, m_targetPlayer.X + 100, "cry", 8000, null);
		base.Body.CallFuction(BeatTargetPlayer, 8500);
	}

	private void BeatTargetPlayer()
	{
		base.Body.CurrentDamagePlus = 2f;
		base.Body.Beat(m_targetPlayer, "beatB", 0, 0, 0);
		base.Body.CallFuction(SetState, 4000);
	}

	public override void OnBeforeTakedDamage(Living source, ref int damage, ref int crit)
	{
		if (!OpenEye && (damage > 0 || crit > 0))
		{
			friendBoss.TakeDamage(source, ref damage, ref crit, "");
		}
		base.OnBeforeTakedDamage(source, ref damage, ref crit);
	}

	private void SetState()
	{
		if (OpenEye)
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
		}
		else
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standA");
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
