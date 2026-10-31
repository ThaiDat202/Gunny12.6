using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class FiveNormalFirstBoss : ABrain
{
	private int m_attackTurn = 0;

	private SimpleNpc m_friendNpc;

	protected Player m_targer;

	private List<PhysicalObj> phyObjects = new List<PhysicalObj>();

	private int m_state = 0;

	private int m_bossId = 5101;

	private int m_npcId = 5102;

	private int m_reduceStreng = 50;

	private int m_reduceBlood = 200;

	private static string[] AttackChat = new string[1] { "" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		if (m_friendNpc == null)
		{
			SimpleNpc[] nPCLivingWithID = ((PVEGame)base.Game).GetNPCLivingWithID(m_npcId);
			if (nPCLivingWithID.Length != 0)
			{
				m_friendNpc = nPCLivingWithID[0];
			}
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		SetSpecialEffect();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		m_state = 0;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_attackTurn++;
		List<Player> list = new List<Player>();
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving && allLivingPlayer.X > 1199)
			{
				list.Add(allLivingPlayer);
			}
			else if (allLivingPlayer.IsLiving && allLivingPlayer.X < 290 && allLivingPlayer.Y < 555)
			{
				list.Add(allLivingPlayer);
			}
		}
		if (list.Count > 0)
		{
			DirectKill(list);
			return;
		}
		switch (m_state)
		{
		case 0:
			BeatToA();
			break;
		case 1:
			BeatToB();
			break;
		case 2:
			BeatToC();
			break;
		case 3:
			BeatToD();
			break;
		case 4:
			if (m_friendNpc.IsLiving)
			{
				BeatToE();
			}
			else
			{
				FallFromD();
			}
			break;
		case 5:
			if (m_friendNpc.IsLiving)
			{
				CreateShootBeatE();
			}
			else
			{
				FallFromE();
			}
			break;
		}
	}

	private void DirectKill(List<Player> players)
	{
		base.Body.CurrentDamagePlus = 1000f;
		foreach (Player player in players)
		{
			base.Body.BeatDirect(player, "", 2000, 1, 1);
		}
	}

	private void BeatToA()
	{
		m_state = 1;
		base.Body.PlayMovie("beatA", 1000, 10000);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.X + 10000, "cry", 10000, directDamage: false);
		base.Body.CallFuction(CreateEffectBeatA, 10000);
	}

	private void BeatToB()
	{
		m_state = 2;
		NpcWalkEffect();
		m_targer = base.Game.FindRandomPlayer();
		if (base.Body.ShootPoint(m_targer.X, m_targer.Y, 56, 1000, 10000, 1, 1.5f, 8000))
		{
			base.Body.PlayMovie("beatB", 1000, 10000);
		}
	}

	private void BeatToC()
	{
		m_state = 3;
		NpcWalkEffect();
		base.Body.CurrentDamagePlus = 1.2f;
		base.Body.PlayMovie("beatC", 1000, 11000);
		int num = base.Game.Random.Next(50);
		if (num < 50)
		{
			base.Body.CallFuction(CreateEffectBeatSpecialC, 9000);
		}
		else
		{
			base.Body.CallFuction(CreateEffectBeatC, 9000);
		}
	}

	private void BeatToD()
	{
		m_state = 4;
		base.Body.CurrentDamagePlus = 1.5f;
		((PVEGame)base.Game).SendObjectFocus(m_friendNpc, 1, 1000, 500);
		((PVEGame)base.Game).SendLivingActionMapping(m_friendNpc, "stand", "standB");
		m_friendNpc.PlayMovie("AtoB", 2000, 0);
		m_friendNpc.PlayMovie("walkB", 6000, 0);
		m_friendNpc.Config.CanTakeDamage = true;
		((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 9000, 500);
		base.Body.PlayMovie("beatD", 10000, 10000);
		base.Body.CallFuction(CreateEffectBeatD, 18000);
	}

	private void BeatToE()
	{
		m_state = 5;
		NpcWalkEffect();
		base.Body.PlayMovie("DtoE", 1000, 8000);
		base.Body.CallFuction(CreateShootBeatE, 8000);
	}

	private void FallFromD()
	{
		m_state = 0;
		SetSpecialEffect();
		base.Body.PlayMovie("fallingA", 800, 3000);
		((PVEGame)base.Game).SendFreeFocus(1441, 681, 1, 1000, 0);
		base.Body.CallFuction(CallNewNpc, 3000);
	}

	private void FallFromE()
	{
		m_state = 0;
		SetSpecialEffect();
		base.Body.PlayMovie("fallingB", 800, 3000);
		((PVEGame)base.Game).SendFreeFocus(1441, 681, 1, 1000, 0);
		base.Body.CallFuction(CallNewNpc, 3000);
	}

	private void CallNewNpc()
	{
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.HasTurn = false;
		livingConfig.CanTakeDamage = false;
		SimpleNpc friendNpc = ((PVEGame)base.Game).CreateNpc(m_npcId, m_friendNpc.X, m_friendNpc.Y, 1, 1, livingConfig);
		base.Game.RemoveLiving(m_friendNpc.Id);
		m_friendNpc = friendNpc;
		((PVEGame)base.Game).SendLivingActionMapping(m_friendNpc, "stand", "standA");
		((PVEGame)base.Game).SendObjectFocus(m_friendNpc, 1, 2000, 0);
		m_friendNpc.PlayMovie("in", 3000, 10000);
		base.Body.CallFuction(NpcWalkEffect, 11000);
		((PVEGame)base.Game).SendObjectFocus(base.Body, 1, 14000, 500);
		base.Body.CallFuction(BeatToA, 15000);
	}

	private void NpcWalkEffect()
	{
		if (m_friendNpc.Config.CanTakeDamage)
		{
			m_friendNpc.PlayMovie("walkB", 1000, 3000);
		}
		else
		{
			m_friendNpc.PlayMovie("walkA", 1000, 3000);
		}
	}

	private void CreateShootBeatE()
	{
		SetSpecialEffect();
		base.Body.CurrentDamagePlus = 3f;
		m_targer = base.Game.FindRandomPlayer();
		if (base.Body.ShootPoint(m_targer.X - 20, m_targer.Y, 72, 1000, 10000, 1, 0.1f, 3300))
		{
			base.Body.PlayMovie("beatE", 1000, 0);
		}
	}

	private void CreateEffectBeatD()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving)
			{
				phyObjects.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.4.minigun", "", 1, 1));
				allLivingPlayer.AddEffect(new ReduceStrengthEffect(2, m_reduceStreng), 1000);
				base.Body.BeatDirect(allLivingPlayer, "", 1000, 3, 1);
			}
		}
	}

	private void CreateEffectBeatC()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving)
			{
				phyObjects.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y - 10, "", "asset.game.4.zap", "", 1, 1));
				allLivingPlayer.AddEffect(new ReduceStrengthEffect(2, m_reduceStreng), 1000);
				base.Body.BeatDirect(allLivingPlayer, "", 1000, 3, 1);
			}
		}
	}

	private void CreateEffectBeatSpecialC()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving)
			{
				phyObjects.Add(((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y - 10, "", "asset.game.4.zap2", "", 1, 1));
				allLivingPlayer.AddEffect(new ReduceStrengthEffect(2, m_reduceStreng), 1000);
				base.Body.BeatDirect(allLivingPlayer, "", 1000, 4, 1);
			}
		}
	}

	private void CreateEffectBeatA()
	{
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if (allLivingPlayer.IsLiving)
			{
				allLivingPlayer.AddEffect(new ContinueReduceBloodEffect(2, m_reduceBlood, base.Body), 500);
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

	public override void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		base.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
	}

	private void SetSpecialEffect()
	{
		switch (m_state)
		{
		case 1:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryA");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieA");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standA");
			base.Body.SetOffsetY(-75);
			base.Body.SetRect(-110, -144, 230, 149);
			base.Body.SetRelateDemagemRect(-110, -144, 230, 149);
			break;
		case 2:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryB");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieB");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
			base.Body.SetOffsetY(-150);
			base.Body.FireY = -268;
			base.Body.SetRect(-110, -268, 230, 149);
			base.Body.SetRelateDemagemRect(-110, -268, 230, 149);
			break;
		case 3:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryC");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieC");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standC");
			base.Body.SetOffsetY(-225);
			base.Body.SetRect(-110, -391, 230, 149);
			base.Body.SetRelateDemagemRect(-110, -391, 230, 149);
			break;
		case 4:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryD");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieD");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standD");
			base.Body.SetOffsetY(-300);
			base.Body.SetRect(-110, -500, 230, 149);
			base.Body.SetRelateDemagemRect(-110, -500, 230, 149);
			break;
		case 5:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cryE");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "dieE");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standE");
			base.Body.SetOffsetY(-420);
			base.Body.FireX = -120;
			base.Body.FireY = -500;
			base.Body.SetRect(-110, -630, 230, 149);
			base.Body.SetRelateDemagemRect(-110, -630, 230, 149);
			break;
		default:
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "cry", "cry");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "die", "die");
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "stand");
			base.Body.FireX = ((SimpleBoss)base.Body).NpcInfo.FireX;
			base.Body.FireY = ((SimpleBoss)base.Body).NpcInfo.FireY;
			break;
		}
	}
}
