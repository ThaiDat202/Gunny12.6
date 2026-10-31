using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMpOnTakeDamage_Passive : BasePetEffect
{
	private int m_added;

	private int m_value;

	public PetAddMpOnTakeDamage_Passive(string elementID)
		: base(ePetEffectType.PetAddMpOnTakeDamage_Passive, elementID)
	{
		if (!(elementID == "1748"))
		{
			if (elementID == "1815")
			{
				m_added = 20;
				m_value = 25;
			}
		}
		else
		{
			m_added = 20;
			m_value = 20;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMpOnTakeDamage_Passive) is PetAddMpOnTakeDamage_Passive)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageamount, ref int criticalamount)
	{
		if (living != source && damageamount + criticalamount > m_value)
		{
			living.BeforeTakeDamage -= player_BeforeTakeDamage;
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
			(living as Player).AddPetMP(m_added);
			living.BeginNextTurn += player_BeginNextTurn;
		}
	}

	private void player_BeginNextTurn(Living living)
	{
		living.BeforeTakeDamage += player_BeforeTakeDamage;
		living.BeginNextTurn -= player_BeginNextTurn;
	}
}
