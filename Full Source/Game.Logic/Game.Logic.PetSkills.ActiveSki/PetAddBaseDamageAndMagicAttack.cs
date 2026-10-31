using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddBaseDamageAndMagicAttackWind : BasePetEffect
{
	private int m_added;

	public PetAddBaseDamageAndMagicAttackWind(int skillId, string elementID)
		: base(ePetEffectType.PetAddBaseDamageAndMagicAttackWind, elementID)
	{
		if (skillId == 599)
		{
			m_added = 30;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamageAndMagicAttackWind) is PetAddBaseDamageAndMagicAttackWind)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += player_PlayerShoot;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= player_PlayerShoot;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_AfterPlayerShooted(Player player)
	{
		player.MagicAttack = player.OldMagicAttack;
		player.BaseDamage = player.OldBaseDamage;
	}

	private void player_PlayerShoot(Player player)
	{
		float wind = player.Game.Wind;
		int direction = player.Direction;
		float num = wind * (float)m_added / 100f * (float)direction;
		if (num <= 0f)
		{
			num = 0f;
		}
		player.MagicAttack *= 1f + num;
		player.BaseDamage *= 1f + num;
	}
}
