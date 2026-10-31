using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class LivingRangeAttackingNPCAction : BaseAction
{
	private Living m_living;

	private List<Living> m_livings;

	private string m_action;

	public LivingRangeAttackingNPCAction(Living living, string action, int delay, List<Living> livings)
		: base(delay, 1000)
	{
		m_living = living;
		m_livings = livings;
		m_action = action;
	}

	private int MakeDamage(Living p)
	{
		double baseDamage = m_living.BaseDamage;
		double baseGuard = p.BaseGuard;
		double defence = p.Defence;
		double attack = m_living.Attack;
		float currentDamagePlus = m_living.CurrentDamagePlus;
		float currentShootMinus = m_living.CurrentShootMinus;
		double num = 0.95 * (baseGuard - (double)(3 * m_living.Grade)) / (500.0 + baseGuard - (double)(3 * m_living.Grade));
		double num2 = 0.0;
		num2 = ((!(defence - m_living.Lucky < 0.0)) ? (0.95 * (defence - m_living.Lucky) / (600.0 + defence - m_living.Lucky)) : 0.0);
		double num3 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num + num2 - num * num2)) * (double)currentDamagePlus * (double)currentShootMinus;
		double num4 = m_living.MagicAttack * 0.8 - p.MagicDefence * 0.7;
		double num5 = m_living.FireAttack * 0.6 - p.FireDefence * 0.3;
		double num6 = m_living.WaterAttack * 0.6 - p.WaterDefence * 0.3;
		double num7 = m_living.WindAttack * 0.6 - p.WindDefence * 0.3;
		double num8 = m_living.LandAttack * 0.6 - p.LandDefence * 0.3;
		double num9 = num5 + num6 + num7 + num8;
		num3 += num4;
		num3 += num9;
		Rectangle directDemageRect = p.GetDirectDemageRect();
		double num10 = Math.Sqrt((directDemageRect.X - m_living.X) * (directDemageRect.X - m_living.X) + (directDemageRect.Y - m_living.Y) * (directDemageRect.Y - m_living.Y));
		if (num3 < 0.0)
		{
			return 1;
		}
		return (int)num3;
	}

	protected override void ExecuteImp(BaseGame game, long tick)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, m_living.Id);
		gSPacketIn.Parameter1 = m_living.Id;
		gSPacketIn.WriteByte(61);
		int count = m_livings.Count;
		gSPacketIn.WriteInt(count);
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc && m_action != "movie")
			{
				game.SendLivingPlayMovie(living, m_action);
			}
			int damageAmount = MakeDamage(living);
			int criticalAmount = 0;
			int val = 0;
			if (living is Player)
			{
				living.OnTakedDamage(living, ref damageAmount, ref criticalAmount);
			}
			if (living.TakeDamage(m_living, ref damageAmount, ref criticalAmount, "范围攻击"))
			{
				val = damageAmount + criticalAmount;
			}
			gSPacketIn.WriteInt(living.Id);
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteInt(living.Blood);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(1);
		}
		game.SendToAll(gSPacketIn);
		Finish(tick);
	}
}
