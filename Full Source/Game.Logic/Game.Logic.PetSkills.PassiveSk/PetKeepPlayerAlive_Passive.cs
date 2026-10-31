using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetKeepPlayerAlive_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	private string m_elementID;

	public PetKeepPlayerAlive_Passive(int value, int probability, string elementID)
		: base(ePetEffectType.PetKeepPlayerAlive_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_value = value;
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetKeepPlayerAlive_Passive) is PetKeepPlayerAlive_Passive petKeepPlayerAlive_Passive)
		{
			petKeepPlayerAlive_Passive.m_probability = ((m_probability > petKeepPlayerAlive_Passive.m_probability) ? m_probability : petKeepPlayerAlive_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKilledByLiving += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (living == target || living.Blood > 0)
		{
			return;
		}
		if (living.KeepLifeCount < m_value)
		{
			if (m_elementID == "1626")
			{
				IsTrigger = true;
				living.EffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(living.MaxBlood * 15 / 100);
				living.Game.SendGameUpdateHealth(living, 0, living.Blood);
				living.SyncAtTime = false;
				living.Game.SendEquipEffect(living, "触发<觉·功夫专精>免疫本次致命伤害,恢复15%最大生命值血量");
				living.KeepLifeCount++;
			}
			else if (m_elementID == "1778")
			{
				IsTrigger = true;
				living.EffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(living.MaxBlood * 15 / 100);
				living.Game.SendGameUpdateHealth(living, 0, living.Blood);
				living.SyncAtTime = false;
				living.Game.SendEquipEffect(living, "触发<觉·天赋>免疫本次致命伤害,恢复15%最大生命值血量");
				living.KeepLifeCount++;
			}
			else if (m_elementID == "1513")
			{
				IsTrigger = true;
				living.EffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(living.MaxBlood * 8 / 100);
				(living as Player).AddDander(100);
				living.Game.SendGameUpdateHealth(living, 0, living.Blood);
				living.SyncAtTime = false;
				living.Game.SendEquipEffect(living, "触发<觉·三命金毫>免疫本次致命伤害,恢复8%最大生命值血量并增加50点怒气值");
				living.KeepLifeCount++;
			}
			else if (m_elementID == "3492")
			{
				IsTrigger = true;
				living.EffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(living.MaxBlood * 10 / 100);
				living.Game.SendGameUpdateHealth(living, 0, living.Blood);
				living.SyncAtTime = false;
				living.Game.SendEquipEffect(living, "触发<觉·冰骸铠甲>免疫本次致命伤害,恢复10%最大生命值血量");
				living.KeepLifeCount++;
			}
			else
			{
				IsTrigger = true;
				living.EffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(living.MaxBlood * 5 / 100);
				living.Game.SendGameUpdateHealth(living, 0, living.Blood);
				living.SyncAtTime = false;
				living.Game.SendEquipEffect(living, "触发<三命金毫>免疫本次致命伤害,恢复5%最大生命值血量");
				living.KeepLifeCount++;
				living.KeepLife = true;
			}
		}
		else
		{
			living.KeepLife = false;
			Stop();
		}
	}
}
