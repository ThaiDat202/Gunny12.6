using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourNormalCattleBoss : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_effectAttack = null;

	private PhysicalObj m_powerUpEffect = null;

	private int m_totalNpc = 3;

	private int npcId = 4107;

	private float m_perPowerUp = 0.2f;

	private int m_reduceStreng = 50;

	private bool IsFear = false;

	private Player target = null;

	private float lastpowDamage = 0f;

	private float m_currentPowDamage = 0f;

	private string[] AllAtackPlayerSay = new string[4] { "Các ngươi đỡ được không?", "Chết chắc rồi bọn ngu si!", "Sao mà đỡ lại được đây?", "Sao? Sao? Chết đi!!!!!" };

	private string[] SpecialAttackSay = new string[4] { "Xem ngươi chạy đâu cho thoát", "Dám đánh nén ta à?", "Để xem các ngươi chạy đâu", "Đánh nén ta là điều không thể tha thứ" };

	private string[] FearSay = new string[3] { "Mệt rồi!", "Ta cảm thấy mệt quá!", "Mệt quá! Nghỉ tí đã." };

	private string[] KillAttackSay = new string[4] { "Để ta tiễn ngươi!", "Chết chắc rồi cưng à!", "Ngươi không chết mới là lạ.", "Ta sẽ ban tặng ngươi cái chết êm ái" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		ClearEffect();
		base.Body.CurrentShootMinus = 1f;
		if (!IsFear)
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
		IsFear = false;
		base.Body.CurrentDamagePlus = 1f;
		m_currentPowDamage = 1f;
	}

	public override void OnStartAttacking()
	{
		if (!IsFear)
		{
			bool flag = false;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 200 && allFightPlayer.X < base.Body.X + 200)
				{
					flag = true;
				}
			}
			if (flag)
			{
				KillAttack(base.Body.X - 200, base.Body.X + 200);
				return;
			}
		}
		if (m_attackTurn == 0)
		{
			if (!IsFear)
			{
				base.Body.CallFuction(PowerUpEffect, 2000);
				if (((SimpleBoss)base.Body).CurrentLivingNpcNum <= 0)
				{
					base.Body.Say("Lửa địa ngục đâu, ra mau.", 0, 5000);
					base.Body.CallFuction(CallNpc, 7000);
				}
				else
				{
					base.Body.CallFuction(AttackAllPlayer, 4000);
				}
			}
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			if (!IsFear)
			{
				base.Body.CallFuction(PowerUpEffect, 2000);
				base.Body.CallFuction(AttackPerson, 4000);
				m_attackTurn++;
			}
			else
			{
				m_attackTurn = 3;
			}
		}
		else if (m_attackTurn == 2)
		{
			if (!IsFear)
			{
				base.Body.CallFuction(PowerUpEffect, 2000);
				base.Body.CallFuction(AttackAllPlayer, 4000);
			}
			m_attackTurn++;
		}
		else
		{
			if (m_attackTurn != 3)
			{
				return;
			}
			if (!IsFear)
			{
				base.Body.CallFuction(PowerUpEffect, 2000);
				if (((SimpleBoss)base.Body).CurrentLivingNpcNum <= 0)
				{
					IsFear = true;
					base.Body.Say("Hơ hơ... Ta bị ốm à??", 0, 2200);
					base.Body.CallFuction(ChangeAtoB, 4000);
				}
				else
				{
					base.Body.CallFuction(JumpAndAttack, 4000);
				}
			}
			else
			{
				IsFear = false;
				m_currentPowDamage = 1f;
				base.Body.CallFuction(ChangeBtoA, 2000);
			}
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		lastpowDamage = base.Body.CurrentDamagePlus;
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.ChangeDirection(base.Game.FindlivingbyDir(base.Body), 100);
		((SimpleBoss)base.Body).RandomSay(KillAttackSay, 0, 2000, 0);
		base.Body.PlayMovie("beatC", 2000, 0);
		base.Body.PlayMovie("beatE", 5000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 7000, null);
		base.Body.CallFuction(SetState, 8000);
	}

	private void AttackPerson()
	{
		target = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		if (target != null)
		{
			base.Body.ChangeDirection(target, 100);
			base.Body.Say("Tên ranh kia hãy đỡ này!!!", 0, 1000);
			base.Body.PlayMovie("beatA", 1200, 0);
			((PVEGame)base.Game).SendObjectFocus(target, 1, 3200, 0);
			base.Body.CallFuction(CreateAttackEffect, 4000);
			if (base.Body.FindDirection(target) == -1)
			{
				base.Body.RangeAttacking(target.X - 50, base.Body.X, "cry", 4800, null);
			}
			else
			{
				base.Body.RangeAttacking(base.Body.X, target.X + 50, "cry", 4800, null);
			}
			base.Body.CallFuction(SetState, 6000);
		}
	}

	private void AttackAllPlayer()
	{
		((SimpleBoss)base.Body).RandomSay(AllAtackPlayerSay, 0, 1000, 0);
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 4100, null);
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			allLivingPlayer.AddEffect(new ReduceStrengthEffect(1, m_reduceStreng), 4200);
		}
		base.Body.CallFuction(SetState, 5000);
	}

	private void ChangeAtoB()
	{
		base.Body.PlayMovie("beatD", 1000, 0);
		((SimpleBoss)base.Body).RandomSay(FearSay, 0, 4100, 0);
		base.Body.PlayMovie("AtoB", 4000, 0);
		base.Body.CallFuction(SetState, 7000);
	}

	private void ChangeBtoA()
	{
		base.Body.PlayMovie("AtoB", 1000, 0);
		((SimpleBoss)base.Body).RandomSay(SpecialAttackSay, 0, 2200, 0);
		base.Body.CallFuction(JumpAndAttack, 2000);
	}

	private void JumpAndAttack()
	{
		target = base.Game.FindRandomPlayer();
		if (target != null)
		{
			base.Body.PlayMovie("jump", 500, 0);
			((PVEGame)base.Game).SendObjectFocus(target, 1, 2000, 0);
			base.Body.BoltMove(target.X, target.Y, 2500);
			base.Body.PlayMovie("fall", 2600, 0);
			base.Body.RangeAttacking(target.X - 100, target.X + 100, "cry", 3000, null);
			base.Body.CallFuction(SetState, 4000);
		}
	}

	private void PowerUpEffect()
	{
		m_currentPowDamage += m_perPowerUp;
		base.Body.CurrentDamagePlus = m_currentPowDamage;
		m_powerUpEffect = ((PVEGame)base.Game).Createlayer(base.Body.X, base.Body.Y - 60, "", "game.crazytank.assetmap.Buff_powup", "", 1, 0);
	}

	private void CreateAttackEffect()
	{
		if (target != null)
		{
			m_effectAttack = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "", "asset.game.4.blade", "", 1, 0);
		}
	}

	private void ClearEffect()
	{
		if (m_powerUpEffect != null)
		{
			base.Game.RemovePhysicalObj(m_powerUpEffect, sendToClient: true);
		}
		if (m_effectAttack != null)
		{
			base.Game.RemovePhysicalObj(m_effectAttack, sendToClient: true);
		}
	}

	private void CallNpc()
	{
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.IsFly = true;
		for (int i = 0; i < m_totalNpc; i++)
		{
			int x = base.Game.Random.Next(350, 1300);
			int y = base.Game.Random.Next(100, 700);
			((SimpleBoss)base.Body).CreateChild(npcId, x, y, showBlood: true, livingConfig);
		}
	}

	private void RemoveAllNpc()
	{
		((SimpleBoss)base.Body).RemoveAllChild();
	}

	private void SetState()
	{
		if (IsFear)
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
