using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class LivingRangeAttackingAction : BaseAction
{
	private Living m_living;

	private List<Player> m_players;

	private int m_fx;

	private int m_tx;

	private string m_action;

	private bool m_removeFrost;

	private bool m_directDamage;

	public LivingRangeAttackingAction(Living living, int fx, int tx, string action, int delay, bool removeFrost, bool directDamage, List<Player> players)
		: base(delay, 1000)
	{
		m_living = living;
		m_players = players;
		m_fx = fx;
		m_tx = tx;
		m_action = action;
		m_removeFrost = removeFrost;
		m_directDamage = directDamage;
	}

	private int MakeDamage(Living p)
	{
		double baseDamage = m_living.BaseDamage;
		double num = p.BaseGuard;
		double num2 = p.Defence + p.MagicDefence;
		double num3 = m_living.Attack + m_living.MagicAttack;
		if (p.AddArmor && (p as Player).DeputyWeapon != null)
		{
			int num4 = (p as Player).DeputyWeapon.Template.Property7 + (int)Math.Pow(1.1, (p as Player).DeputyWeapon.StrengthenLevel);
			num += (double)num4;
			num2 += (double)num4;
		}
		if (m_living.IgnoreArmor)
		{
			num *= 0.6;
			num2 *= 0.6;
		}
		float currentDamagePlus = m_living.CurrentDamagePlus;
		float currentShootMinus = m_living.CurrentShootMinus;
		double num5 = 0.95 * (num - (double)(3 * m_living.Grade)) / (500.0 + num - (double)(3 * m_living.Grade));
		double num6 = 0.0;
		num6 = ((!(num2 - m_living.Lucky / 2.0 < 0.0)) ? (0.95 * (num2 - m_living.Lucky) / (600.0 + num2 - m_living.Lucky)) : 0.0);
		double num7 = baseDamage * (1.0 + num3 * 0.003) * (1.0 - (num5 + num6 - num5 * num6)) * (double)currentDamagePlus * (double)currentShootMinus + m_living.MagicAttack;
		double num8 = m_living.MagicAttack * 0.8 - p.MagicDefence * 0.7;
		double num9 = m_living.FireAttack * 0.6 - p.FireDefence * 0.3;
		double num10 = m_living.WaterAttack * 0.6 - p.WaterDefence * 0.3;
		double num11 = m_living.WindAttack * 0.6 - p.WindDefence * 0.3;
		double num12 = m_living.LandAttack * 0.6 - p.LandDefence * 0.3;
		double num13 = num9 + num10 + num11 + num12;
		num7 += num8;
		num7 += num13;
		if (!m_directDamage)
		{
			Rectangle directDemageRect = p.GetDirectDemageRect();
			double num14 = Math.Sqrt((directDemageRect.X - m_living.X) * (directDemageRect.X - m_living.X) + (directDemageRect.Y - m_living.Y) * (directDemageRect.Y - m_living.Y));
			num7 *= 1.0 - num14 / (double)Math.Abs(m_tx - m_fx) / 4.0;
		}
		if (num7 < 0.0)
		{
			return 1;
		}
		return (int)num7;
	}

	private int MakeCriticalDamage(Living p, int baseDamage)
	{
		double lucky = m_living.Lucky;
		Random random = new Random();
		if (15000.0 * lucky / (40000.0 - lucky) > (double)random.Next(100000))
		{
			int num = p.ReduceCritFisrtGem + p.ReduceCritSecondGem;
			int num2 = (int)(0.25 * (double)baseDamage);
			return num2 * (100 - num) / 100;
		}
		return 0;
	}

	protected override void ExecuteImp(BaseGame game, long tick)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, m_living.Id);
		gSPacketIn.Parameter1 = m_living.Id;
		gSPacketIn.WriteByte(61);
		List<Living> list = game.Map.FindPlayers(m_fx, m_tx, m_players);
		int num = list.Count;
		foreach (Living item in list)
		{
			if (m_living.IsFriendly(item))
			{
				num--;
			}
		}
		gSPacketIn.WriteInt(num);
		m_living.SyncAtTime = false;
		try
		{
			foreach (Living item2 in list)
			{
				item2.SyncAtTime = true;
				if (m_living.IsFriendly(item2))
				{
					continue;
				}
				int val = 0;
				item2.IsHide = false;
				game.SendGameUpdateHideState(item2);
				if (m_removeFrost)
				{
					item2.IsFrost = false;
					game.SendGameUpdateFrozenState(item2);
				}
				int damageAmount = MakeDamage(item2);
				int criticalAmount = MakeCriticalDamage(item2, damageAmount);
				int val2 = 0;
				if (item2 is Player)
				{
					item2.OnTakedDamage(item2, ref damageAmount, ref criticalAmount);
				}
				if (item2.TakeDamage(m_living, ref damageAmount, ref criticalAmount, "范围攻击"))
				{
					val2 = damageAmount + criticalAmount;
					if (item2 is Player)
					{
						Player player = item2 as Player;
						val = player.Dander;
					}
				}
				gSPacketIn.WriteInt(item2.Id);
				gSPacketIn.WriteInt(val2);
				gSPacketIn.WriteInt(item2.Blood);
				gSPacketIn.WriteInt(val);
				gSPacketIn.WriteInt(1);
			}
			game.SendToAll(gSPacketIn);
			Finish(tick);
		}
		finally
		{
			m_living.SyncAtTime = true;
			foreach (Living item3 in list)
			{
				item3.SyncAtTime = true;
			}
		}
	}
}
