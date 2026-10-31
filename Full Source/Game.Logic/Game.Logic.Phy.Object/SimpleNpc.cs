using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class SimpleNpc : Living
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private NpcInfo m_npcInfo;

	private ABrain m_ai;

	public int TotalCure;

	public NpcInfo NpcInfo => m_npcInfo;

	public SimpleNpc(int id, BaseGame game, NpcInfo npcInfo, int type, int direction, string actions)
		: base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
	{
		if (type == 0)
		{
			base.Type = eLivingType.SimpleNpc;
		}
		else
		{
			base.Type = eLivingType.SimpleNpc1;
		}
		m_npcInfo = npcInfo;
		base.ActionStr = actions;
		m_ai = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
		if (m_ai == null)
		{
			log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
			m_ai = SimpleBrain.Simple;
		}
		m_ai.Game = m_game;
		m_ai.Body = this;
		try
		{
			m_ai.OnCreated();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc Created error:{0}", arg);
		}
	}

	public override void Reset()
	{
		Agility = m_npcInfo.Agility;
		Attack = m_npcInfo.Attack;
		BaseDamage = m_npcInfo.BaseDamage;
		BaseGuard = m_npcInfo.BaseGuard;
		Lucky = m_npcInfo.Lucky;
		MagicAttack = m_npcInfo.MagicAttack;
		MagicDefence = m_npcInfo.MagicDefence;
		FireAttack = m_npcInfo.FireAttack;
		FireDefence = m_npcInfo.FireDefence;
		WaterAttack = m_npcInfo.WaterAttack;
		WaterDefence = m_npcInfo.WaterDefence;
		LandAttack = m_npcInfo.LandAttack;
		LandDefence = m_npcInfo.LandDefence;
		WindAttack = m_npcInfo.WindAttack;
		WindDefence = m_npcInfo.WindDefence;
		base.FireX = NpcInfo.FireX;
		base.FireY = NpcInfo.FireY;
		Grade = m_npcInfo.Level;
		Experience = m_npcInfo.Experience;
		TotalCure = 0;
		SetRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
		SetRelateDemagemRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
		base.Reset();
	}

	public void OnDie()
	{
		try
		{
			m_ai.OnDie();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc OnDie error {0}", arg);
		}
	}

	public void GetDropItemInfo()
	{
		if (!(m_game.CurrentLiving is Player))
		{
			return;
		}
		Player player = m_game.CurrentLiving as Player;
		List<ItemInfo> info = null;
		DropInventory.NPCDrop(m_npcInfo.DropId, ref info);
		if (info == null)
		{
			return;
		}
		foreach (ItemInfo item in info)
		{
			if (item != null && item.Template.CategoryID != 10)
			{
				player.PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eGameView.dungeonTypeGet);
			}
		}
	}

	public override void Die()
	{
		GetDropItemInfo();
		base.Die();
	}

	public override void Die(int delay)
	{
		GetDropItemInfo();
		base.Die(delay);
	}

	public override void PrepareNewTurn()
	{
		base.PrepareNewTurn();
		try
		{
			m_ai.OnBeginNewTurn();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc BeginNewTurn error:{0}", arg);
		}
	}

	public override void StartAttacking()
	{
		base.StartAttacking();
		try
		{
			m_ai.OnStartAttacking();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc StartAttacking error:{0}", arg);
		}
	}

	public override void BeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		try
		{
			m_ai.OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc BeforeTakeDamage error:{0}", arg);
		}
		base.BeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
	}

	public override void OnAfterTakedBomb()
	{
		try
		{
			m_ai.OnAfterTakedBomb();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc OnAfterTakedBomb error:{0}", arg);
		}
	}

	public override void OnAfterTakedFrozen()
	{
		try
		{
			m_ai.OnAfterTakedFrozen();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc OnAfterTakedFrozen error:{0}", arg);
		}
		base.OnAfterTakedFrozen();
	}

	public override void Dispose()
	{
		base.Dispose();
		try
		{
			m_ai.Dispose();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc Dispose error:{0}", arg);
		}
	}
}
