using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddMPOnGameStart_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value = 0;

	private int m_count = 3;

	public PetAddMPOnGameStart_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddMPOnGameStart_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPOnGameStart_Passive) is PetAddMPOnGameStart_Passive petAddMPOnGameStart_Passive)
		{
			petAddMPOnGameStart_Passive.m_probability = ((m_probability > petAddMPOnGameStart_Passive.m_probability) ? m_probability : petAddMPOnGameStart_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginNextTurn += player_BeginNextTurn;
	}

	private void player_BeginNextTurn(Living living)
	{
		if (m_value == 0)
		{
			m_value = 50;
			(living as Player).AddPetMP(m_value);
			m_count = 3;
		}
		m_count--;
		if (m_count < 0)
		{
			if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMPOnGameStart_Passive) is PetAddMPOnGameStart_Passive petAddMPOnGameStart_Passive)
			{
				petAddMPOnGameStart_Passive.Stop();
			}
			(living as Player).RemovePetMP(50);
		}
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginNextTurn -= player_BeginNextTurn;
	}
}
