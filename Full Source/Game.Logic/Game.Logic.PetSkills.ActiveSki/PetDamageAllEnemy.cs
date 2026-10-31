using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetDamageAllEnemy : BasePetEffect
{
	private int m_probability;

	private int m_currentId;

	private int m_value;

	private string m_elementID;

	private bool m_percent;

	public PetDamageAllEnemy(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetDamageAllEnemy, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		if (!(elementID == "1161"))
		{
			if (elementID == "1162")
			{
				m_value = 5000;
			}
		}
		else
		{
			m_value = 3000;
		}
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDamageAllEnemy) is PetDamageAllEnemy petDamageAllEnemy)
		{
			petDamageAllEnemy.m_probability = ((m_probability > petDamageAllEnemy.m_probability) ? m_probability : petDamageAllEnemy.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill != m_currentId || !(player.Game is PVPGame))
		{
			return;
		}
		int num = 0;
		string elementID = m_elementID;
		string text = elementID;
		if (!(text == "2668"))
		{
			if (text == "2669")
			{
				num = (int)(player.BaseDamage * 150.0 / 100.0 + player.BaseGuard * 300.0 / 100.0);
				m_percent = true;
			}
		}
		else
		{
			num = (int)(player.BaseDamage * 120.0 / 100.0 + player.BaseGuard * 200.0 / 100.0);
			m_percent = true;
		}
		List<Player> allEnemyPlayers = player.Game.GetAllEnemyPlayers(player);
		foreach (Player item in allEnemyPlayers)
		{
			item.Game.sendShowPicSkil(item, base.ElementInfo, isActive: true);
			item.SyncAtTime = true;
			if (m_elementID == "3217")
			{
				int num2 = MakeDamage(player, item);
				num2 = num2 * 500 / 100;
				item.AddBlood(-num2, 1);
			}
			else if (m_elementID == "3227")
			{
				int num3 = MakeDamage(player, item);
				num3 = num3 * 650 / 100;
				item.AddBlood(-num3, 1);
			}
			else if (m_percent)
			{
				item.AddBlood(-num, 1);
			}
			else
			{
				item.AddBlood(-m_value, 1);
			}
			item.SyncAtTime = false;
			if (m_elementID == "3217")
			{
				double num4 = (double)Math.Round((decimal)item.Blood / (decimal)item.MaxBlood, 3) * 100.0;
				if (num4 <= 10.0)
				{
					item.Blood = 0;
					item.Game.SendGameUpdateHealth(item, 1, int.MaxValue);
					Thread.Sleep(200);
					item.Die();
					player?.PlayerDetail.OnKillingLiving(player.Game, 2, item.Id, item.IsLiving, m_value);
				}
			}
			else if (m_elementID == "3227")
			{
				double num5 = (double)Math.Round((decimal)item.Blood / (decimal)item.MaxBlood, 3) * 100.0;
				if (num5 <= 15.0)
				{
					item.Blood = 0;
					item.Game.SendGameUpdateHealth(item, 1, int.MaxValue);
					Thread.Sleep(200);
					item.Die();
					player?.PlayerDetail.OnKillingLiving(player.Game, 2, item.Id, item.IsLiving, m_value);
				}
			}
			if (item.Blood <= 0)
			{
				item.Die();
				player?.PlayerDetail.OnKillingLiving(player.Game, 2, item.Id, item.IsLiving, m_value);
			}
		}
	}

	private int MakeDamage(Player player, Living target)
	{
		double num = player.BaseDamage;
		double baseGuard = target.BaseGuard;
		baseGuard += target.BaseGuard * (10000.0 * player.SunderArmor / (player.SunderArmor + 2500.0) * 0.75 / 100.0) / 100.0;
		baseGuard -= target.BaseGuard * (10000.0 * target.AvoidInjury / (target.AvoidInjury + 2500.0) * 0.75 / 100.0) / 100.0;
		double num2 = target.Defence;
		double attack = player.Attack;
		if (target.AddArmor && (target as Player).DeputyWeapon != null)
		{
			int num3 = (int)target.getHertAddition((target as Player).DeputyWeapon);
			baseGuard += (double)num3;
			num2 += (double)num3;
		}
		if (player.IgnoreArmor)
		{
			baseGuard *= 0.6;
			num2 *= 0.6;
		}
		if (player.AddDamage > 0)
		{
			num += num / 100.0 * (double)player.AddDamage;
		}
		if (player.IgnoreGuard > 0 && !player.IgnoreArmor)
		{
			num2 -= num2 / 100.0 * (double)player.IgnoreGuard;
			baseGuard -= baseGuard / 100.0 * (double)player.IgnoreGuard;
		}
		float currentDamagePlus = player.CurrentDamagePlus;
		float currentShootMinus = player.CurrentShootMinus;
		currentShootMinus += player.CurrentShootMinus * (float)(10000.0 * player.KillPow / (player.KillPow + 2500.0) * 0.75 / 100.0) / 100f;
		currentShootMinus -= player.CurrentShootMinus * (float)(10000.0 * target.WillFight / (target.WillFight + 2500.0) * 0.75 / 100.0) / 100f;
		double num4 = 0.95 * (baseGuard - (double)(3 * player.Grade)) / (500.0 + baseGuard - (double)(3 * player.Grade));
		double num5 = ((num2 - player.Lucky >= 0.0) ? (0.95 * (num2 - player.Lucky) / (600.0 + num2 - player.Lucky)) : 0.0);
		double num6 = (double)player.FightBuffers.WorldBossAddDamage * (1.0 - (baseGuard / 200.0 + num2 * 0.003));
		double num7 = (num6 + num * (1.0 + attack * 0.001) * (1.0 - (num4 + num5 - num4 * num5))) * (double)currentDamagePlus * (double)currentShootMinus;
		Rectangle directDemageRect = target.GetDirectDemageRect();
		double num8 = Math.Sqrt((directDemageRect.X - player.X) * (directDemageRect.X - player.X) + (directDemageRect.Y - player.Y) * (directDemageRect.Y - player.Y));
		double num9 = player.MagicAttack * 0.8 - target.MagicDefence * 0.7;
		if (num9 <= 0.0)
		{
			num9 = 0.0;
		}
		double num10 = player.FireAttack * 0.6 - target.FireDefence * 0.3;
		double num11 = player.WaterAttack * 0.6 - target.WaterDefence * 0.3;
		double num12 = player.WindAttack * 0.6 - target.WindDefence * 0.3;
		double num13 = player.LandAttack * 0.6 - target.LandDefence * 0.3;
		double num14 = num10 + num11 + num12 + num13;
		if (num14 <= 0.0)
		{
			num14 = 0.0;
		}
		num7 += num9;
		num7 += num14;
		if (target.KeepLife)
		{
			return 0;
		}
		if (num7 <= 0.0)
		{
			return 1;
		}
		return (int)num7;
	}
}
