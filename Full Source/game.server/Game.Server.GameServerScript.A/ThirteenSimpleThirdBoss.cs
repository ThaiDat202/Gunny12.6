using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirteenSimpleThirdBoss : ABrain
{
	private int m_attackTurn = 0;

	private SimpleBoss m_friendNpc;

	protected Player m_targer;

	private List<Player> m_listtarget;

	private List<PhysicalObj> phyObjects = new List<PhysicalObj>();

	private PhysicalObj phyBig1000;

	private PhysicalObj phyIronFire;

	private int m_friendBoss = 13006;

	private int m_guardDamage = 40;

	private int m_bloodHeal = 5000;

	private string[] Chat_CallFriendAttacking = new string[4] { "Đè chết mọe nó đi cho ta.", "Đè chết choa nó đi cho ta.", "Hấp diêm nó cho ta.", "Đè xịt shit bọn nó cho ta." };

	private string[] Chat_CallChoiceTarget = new string[3] { "Ồ, đúng rồi đó. Chính là ngươi", "Người tiếp theo sẽ là ngươi", "Ngươi. Chính ngươi đấy!!" };

	private string[] Chat_CallFriendFucking = new string[5] { "Mi bị cái gì thế?", "Đồ ngu học chả được tích sự gì cả.", "Ngươi muốn bị ta lột da hả?", "Con cờ hó kia bóp zái ta hả?", "Ngươi đang giúp bọn chúng hay giúp ta vậy?" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		if (m_friendNpc == null)
		{
			SimpleBoss[] array = ((PVEGame)base.Game).FindLivingTurnBossWithID(m_friendBoss);
			if (array.Length != 0)
			{
				m_friendNpc = array[0];
			}
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (phyObjects == null || phyObjects.Count <= 0)
		{
			return;
		}
		foreach (PhysicalObj phyObject in phyObjects)
		{
			base.Game.RemovePhysicalObj(phyObject, sendToClient: true);
		}
		phyObjects.Clear();
	}

	public override void OnCreated()
	{
		base.OnCreated();
		base.Body.Properties1 = 0;
		base.Body.Config.CancelGuard = false;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		switch (m_attackTurn)
		{
		case 0:
			FirstAttack();
			break;
		case 1:
			AttackToTarget();
			break;
		case 2:
			HealBloodOrWalkup();
			break;
		case 3:
			NextWalkAttack();
			break;
		case 4:
			m_attackTurn = -1;
			break;
		}
		m_attackTurn++;
	}

	private void FirstAttack()
	{
		if ((int)base.Body.Properties1 != 0)
		{
			HealBloodOrWalkup();
		}
		else
		{
			FindRandomTarget();
		}
	}

	private void NextWalkAttack()
	{
		base.Body.CurrentDamagePlus = 1.8f;
		if (m_targer == null)
		{
			return;
		}
		m_listtarget = new List<Player>();
		base.Body.ChangeDirection(m_targer, 1000);
		if (base.Body.FindDirection(m_targer) == 1)
		{
			foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
			{
				if (allLivingPlayer.X >= base.Body.X && allLivingPlayer.X <= base.Game.Map.Bound.Width)
				{
					m_listtarget.Add(allLivingPlayer);
				}
			}
			base.Body.MoveTo(base.Game.Map.Bound.Width, base.Body.Y, "walk", 2000, 10, CallDamageListPlayers);
			return;
		}
		foreach (Player allLivingPlayer2 in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer2.X >= 0 && allLivingPlayer2.X <= base.Body.X)
			{
				m_listtarget.Add(allLivingPlayer2);
			}
		}
		base.Body.MoveTo(0, base.Body.Y, "walk", 2000, 10, CallDamageListPlayers);
	}

	private void CallDamageListPlayers()
	{
		if (m_listtarget.Count > 0)
		{
			foreach (Player item in m_listtarget)
			{
				base.Body.BeatDirect(item, "", 1000, 1, 1);
			}
		}
		SimpleNpc[] array = base.Game.FindAllNpcLiving();
		if (array.Length != 0)
		{
			SimpleNpc[] array2 = array;
			foreach (SimpleNpc simpleNpc in array2)
			{
				if (simpleNpc.Properties1 != null)
				{
					Player player = base.Game.FindPlayer((int)simpleNpc.Properties1);
					if (player != null && player.IsLiving)
					{
						player.SetVisible(state: true);
						player.BlockTurn = false;
					}
				}
				simpleNpc.PlayMovie("die", 500, 0);
				simpleNpc.Die(2000);
			}
		}
		base.Body.CallFuction(RandomJumpBack, 2500);
	}

	private void RandomJumpBack()
	{
		int x = base.Game.Random.Next(100, base.Game.Map.Bound.Width - 100);
		base.Body.PlayMovie("jump", 1000, 0);
		base.Body.BoltMove(x, base.Body.Y, 3000);
		(base.Game as PVEGame).SendFreeFocus(x, base.Body.Y, 1, 2000, 0);
		base.Body.PlayMovie("fall", 3000, 2000);
	}

	private void HealBloodOrWalkup()
	{
		if ((int)base.Body.Properties1 == 0)
		{
			base.Body.PlayMovie("beatA", 1000, 4000);
			base.Body.CallFuction(HealBlood, 3000);
		}
		else
		{
			(base.Game as PVEGame).SendLivingActionMapping(base.Body, "stand", "stand");
			(base.Game as PVEGame).SendLivingActionMapping(base.Body, "cry", "cry");
			base.Body.Properties1 = 0;
			base.Body.Config.CancelGuard = false;
			base.Body.PlayMovie("stand", 500, 0);
			base.Body.ChangeDirection(m_friendNpc, 1000);
			int num = base.Game.Random.Next(Chat_CallFriendFucking.Length);
			base.Body.Say(Chat_CallFriendFucking[num], 2000, 3000);
		}
		base.Body.CallFuction(FindNextTarget, 5000);
	}

	private void FindNextTarget()
	{
		m_targer = base.Game.FindRandomPlayer();
		base.Body.ChangeDirection(m_targer, 500);
		int num = base.Game.Random.Next(Chat_CallChoiceTarget.Length);
		base.Body.Say(Chat_CallChoiceTarget[num], 1000, 0);
		base.Body.CallFuction(SetTargetPlayer, 2000);
	}

	private void HealBlood()
	{
		base.Body.AddBlood(m_bloodHeal);
	}

	private void AttackToTarget()
	{
		base.Body.CurrentDamagePlus = 2.5f;
		base.Body.ChangeDirection(m_targer, 1000);
		base.Body.PlayMovie("jump", 2000, 1600);
		base.Body.BoltMove(m_targer.X, m_targer.Y, 3700);
		base.Body.PlayMovie("fallB", 4000, 2000);
		base.Body.BeatDirect(m_targer, "", 5000, 1, 1);
		base.Body.CallFuction(RemoveEffectPlayer, 6000);
	}

	private void RemoveEffectPlayer()
	{
		if (m_targer != null)
		{
			if (m_targer.EffectList.GetOfType(eEffectType.AddTargetEffect) is AddTargetEffect addTargetEffect)
			{
				addTargetEffect.Stop();
			}
			return;
		}
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.EffectList.GetOfType(eEffectType.AddTargetEffect) is AddTargetEffect addTargetEffect2)
			{
				addTargetEffect2.Stop();
			}
		}
	}

	private void FindRandomTarget()
	{
		m_targer = base.Game.FindRandomPlayer();
		int num = base.Game.Random.Next(Chat_CallFriendAttacking.Length);
		base.Body.Say(Chat_CallFriendAttacking[num], 1000, 0);
		base.Body.CallFuction(SetTargetPlayer, 3000);
	}

	private void SetTargetPlayer()
	{
		if (m_targer != null)
		{
			m_targer.AddEffect(new AddTargetEffect(), 0);
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
}
