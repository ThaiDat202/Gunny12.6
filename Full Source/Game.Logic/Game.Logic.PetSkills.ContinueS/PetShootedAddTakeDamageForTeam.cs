using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetShootedAddTakeDamageForTeamEquip : AbstractPetEffect
{
	private int m_count;

	public PetShootedAddTakeDamageForTeamEquip(int count, string elementID)
		: base(ePetEffectType.PetShootedAddTakeDamageForTeamEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShootedAddTakeDamageForTeamEquip) is PetShootedAddTakeDamageForTeamEquip petShootedAddTakeDamageForTeamEquip)
		{
			petShootedAddTakeDamageForTeamEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		(living as Player).PlayerShoot += player_PlayerShoot;
	}

	public override void OnRemoved(Living living)
	{
		(living as Player).PlayerShoot -= player_PlayerShoot;
	}

	private void player_PlayerShoot(Player living)
	{
		foreach (Player allTeamPlayer in living.Game.GetAllTeamPlayers(living))
		{
			if (allTeamPlayer != living)
			{
				allTeamPlayer.AddPetEffect(new PetAddTakeDamageEquip(2, base.Info.ID.ToString()), 0);
			}
		}
		m_count--;
		if (m_count < 0)
		{
			Stop();
		}
	}
}
