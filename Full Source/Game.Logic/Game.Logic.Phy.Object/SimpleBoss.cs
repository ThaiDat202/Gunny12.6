using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class SimpleBoss : TurnedLiving
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private NpcInfo m_npcInfo;

	private ABrain m_ai;

	private List<SimpleNpc> m_child = new List<SimpleNpc>();

	private List<SimpleBoss> m_boss = new List<SimpleBoss>();

	private Dictionary<Player, int> m_mostHateful;

	public int TotalCure;

	public NpcInfo NpcInfo => m_npcInfo;

	public List<SimpleNpc> Child => m_child;

	public int CurrentLivingNpcNum
	{
		get
		{
			int num = 0;
			foreach (SimpleNpc item in Child)
			{
				if (!item.IsLiving)
				{
					num++;
				}
			}
			return Child.Count - num;
		}
	}

	public List<SimpleBoss> Boss => m_boss;

	public int CurrentLivingBossNum
	{
		get
		{
			int num = 0;
			foreach (SimpleBoss item in Boss)
			{
				if (!item.IsLiving)
				{
					num++;
				}
			}
			return Boss.Count - num;
		}
	}

	public SimpleBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type, string actions)
		: base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
	{
		switch (type)
		{
		case 0:
			base.Type = eLivingType.SimpleBoss;
			break;
		case 1:
			base.Type = eLivingType.ClearEnemy;
			break;
		default:
			base.Type = (eLivingType)type;
			break;
		}
		base.ActionStr = actions;
		m_mostHateful = new Dictionary<Player, int>();
		m_npcInfo = npcInfo;
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
			log.ErrorFormat("SimpleBoss Created error:{0}", arg);
		}
	}

	public override void Reset()
	{
		if (base.Config.IsWorldBoss)
		{
			m_maxBlood = int.MaxValue;
		}
		else
		{
			m_maxBlood = m_npcInfo.Blood;
		}
		BaseDamage = m_npcInfo.BaseDamage;
		BaseGuard = m_npcInfo.BaseGuard;
		Attack = m_npcInfo.Attack;
		Defence = m_npcInfo.Defence;
		Agility = m_npcInfo.Agility;
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
		m_delay = GetTurnDelay();
		ForbidAidBlood = false;
		base.CanAddBlood = true;
		SetRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
		SetRelateDemagemRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
		base.Reset();
	}

	private int GetTurnDelay()
	{
		double num = 1600.0;
		double num2 = Attack / num;
		double num3 = Agility / num;
		double num4 = Agility / (10.0 + num3);
		double num5 = Attack / (10.0 + num2);
		return (int)(num - 1200.0 * num4 / (num4 + 1200.0) + num5 / 10.0);
	}

	public override void Die()
	{
		base.Die();
	}

	public override void Die(int delay)
	{
		base.Die(delay);
	}

	public void RemoveAllChild()
	{
		foreach (SimpleNpc item in Child)
		{
			if (item.IsLiving)
			{
				item.Die();
			}
		}
		m_child = new List<SimpleNpc>();
	}

	public SimpleNpc FindFrostChild()
	{
		SimpleNpc result = null;
		foreach (SimpleNpc item in Child)
		{
			if (item.IsLiving && item.IsFrost)
			{
				result = item;
				break;
			}
		}
		return result;
	}

	public List<SimpleNpc> FindChildLiving(int npcId)
	{
		List<SimpleNpc> list = new List<SimpleNpc>();
		foreach (SimpleNpc item in m_child)
		{
			if (item != null && item.IsLiving && item.NpcInfo.ID == npcId)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		bool flag = false;
		flag = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
		if (source is Player)
		{
			Player key = source as Player;
			int num = damageAmount + criticalAmount;
			if (m_mostHateful.ContainsKey(key))
			{
				m_mostHateful[key] += num;
			}
			else
			{
				m_mostHateful.Add(key, num);
			}
		}
		return flag;
	}

	public Player FindMostHatefulPlayer()
	{
		if (m_mostHateful.Count > 0)
		{
			KeyValuePair<Player, int> keyValuePair = m_mostHateful.ElementAt(0);
			foreach (KeyValuePair<Player, int> item in m_mostHateful)
			{
				if (keyValuePair.Value < item.Value)
				{
					keyValuePair = item;
				}
			}
			return keyValuePair.Key;
		}
		return null;
	}

	public void TowardsToPlayer(int playerX, int delay)
	{
		if (playerX > X)
		{
			ChangeDirection(1, delay);
		}
		else
		{
			ChangeDirection(-1, delay);
		}
	}

	public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type)
	{
		CreateChild(id, brithPoint, maxCount, maxCountForOnce, type, 0);
	}

	public void CreateChild(int id, int x, int y, int disToSecond, int maxCount, int direction)
	{
		if (CurrentLivingNpcNum < maxCount)
		{
			if (maxCount - CurrentLivingNpcNum >= 2)
			{
				Child.Add(((PVEGame)base.Game).CreateNpc(id, x + disToSecond, y, 1, direction));
				Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, 1, direction));
			}
			else if (maxCount - CurrentLivingNpcNum == 1)
			{
				Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, 1, direction));
			}
		}
	}

	public SimpleNpc CreateChild(int id, int x, int y, bool showBlood, LivingConfig config)
	{
		return CreateChild(id, x, y, 1, -1, showBlood, config);
	}

	public SimpleNpc CreateChild(int id, int x, int y, int dir, bool showBlood, LivingConfig config)
	{
		return CreateChild(id, x, y, 1, dir, showBlood, config);
	}

	public SimpleNpc CreateChild(int id, int x, int y, int type, int dir, bool showBlood, LivingConfig config)
	{
		SimpleNpc simpleNpc = ((PVEGame)base.Game).CreateNpc(id, x, y, type, dir, config);
		Child.Add(simpleNpc);
		if (!showBlood)
		{
			base.Game.SendLivingShowBlood(simpleNpc, 0);
		}
		return simpleNpc;
	}

	public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type, int direction)
	{
		int num = 0;
		int num2 = base.Game.Random.Next(0, maxCountForOnce);
		for (int i = 0; i < num2; i++)
		{
			num = base.Game.Random.Next(0, brithPoint.Length);
			CreateChild(id, brithPoint[num].X, brithPoint[num].Y, 4, maxCount, direction);
		}
	}

	public SimpleNpc CreateBoss(int id, int x, int y, int direction, int type)
	{
		SimpleNpc simpleNpc = ((PVEGame)base.Game).CreateNpc(id, x, y, type, direction);
		Child.Add(simpleNpc);
		return simpleNpc;
	}

	public void CreateBoss(int id, int x, int y, int direction, int disToSecond, int maxCount, string action)
	{
		CreateBoss(id, x, y, direction, 1, disToSecond, maxCount, action);
	}

	public void CreateBoss(int id, int x, int y, int direction, int type, int disToSecond, int maxCount, string action)
	{
		if (CurrentLivingBossNum < maxCount)
		{
			if (maxCount - CurrentLivingNpcNum >= 2)
			{
				Boss.Add(((PVEGame)base.Game).CreateBoss(id, x + disToSecond, y, direction, type, action));
				Boss.Add(((PVEGame)base.Game).CreateBoss(id, x, y, direction, type, action));
			}
			else if (maxCount - CurrentLivingBossNum == 1)
			{
				Boss.Add(((PVEGame)base.Game).CreateBoss(id, x, y, direction, type, action));
			}
		}
	}

	public void RandomSay(string[] msg, int type, int delay, int finishTime)
	{
		string text = null;
		int num = base.Game.Random.Next(0, 2);
		if (num == 1)
		{
			int num2 = base.Game.Random.Next(0, msg.Count());
			text = msg[num2];
			m_game.AddAction(new LivingSayAction(this, text, type, delay, finishTime));
		}
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
			log.ErrorFormat("SimpleBoss BeginNewTurn error:{0}", arg);
		}
	}

	public override void PrepareSelfTurn()
	{
		base.PrepareSelfTurn();
		DefaultDelay = m_delay;
		AddDelay(m_npcInfo.Delay);
		try
		{
			m_ai.OnBeginSelfTurn();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleBoss BeginSelfTurn error:{0}", arg);
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
			log.ErrorFormat("SimpleBoss StartAttacking error:{0}", arg);
		}
		if (base.IsAttacking)
		{
			StopAttacking();
		}
	}

	public override void StopAttacking()
	{
		base.StopAttacking();
		try
		{
			m_ai.OnStopAttacking();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleBoss StopAttacking error:{0}", arg);
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
			log.ErrorFormat("SimpleBoss BeforeTakeDamage error:{0}", arg);
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
			log.ErrorFormat("SimpleBoss OnAfterTakedBomb error:{0}", arg);
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
			log.ErrorFormat("SimpleBoss OnAfterTakedFrozen error:{0}", arg);
		}
		base.OnAfterTakedFrozen();
	}

	public List<SimpleNpc> FindChildLivings()
	{
		List<SimpleNpc> list = new List<SimpleNpc>();
		lock (m_child)
		{
			foreach (SimpleNpc item in m_child)
			{
				if (item != null && item.IsLiving)
				{
					list.Add(item);
				}
			}
		}
		return list;
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
			log.ErrorFormat("SimpleBoss Dispose error:{0}", arg);
		}
	}
}
