using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBaseDamageAfterKilled_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_coldDown = 0;

	private int m_value = 0;

	private string m_elementID;

	public PetAddBaseDamageAfterKilled_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddBaseDamageAfterKilled_Passive, elementID)
	{
		m_coldDown = 0;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_elementID = elementID;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamageAfterKilled_Passive) is PetAddBaseDamageAfterKilled_Passive petAddBaseDamageAfterKilled_Passive)
		{
			petAddBaseDamageAfterKilled_Passive.m_probability = ((m_probability > petAddBaseDamageAfterKilled_Passive.m_probability) ? m_probability : petAddBaseDamageAfterKilled_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.AfterKilledByLiving += player_AfterKilledByLiving;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		int num = 0;
		string elementID = m_elementID;
		string text = elementID;
		if (!(text == "1355"))
		{
			if (text == "1357")
			{
				num = 6;
				m_value = 60;
			}
		}
		else
		{
			num = 4;
			m_value = 40;
		}
		if (m_coldDown < num)
		{
			living.BaseDamage += m_value;
			IsTrigger = true;
			m_coldDown++;
		}
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_value /= 2;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (IsTrigger)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			IsTrigger = false;
		}
	}
}
