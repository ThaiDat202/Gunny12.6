using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetShowFireofResentmentEquip : BasePetEffect
{
	private int m_count;

	public PetShowFireofResentmentEquip(int count, string elementID)
		: base(ePetEffectType.PetShowFireofResentmentEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShowFireofResentmentEquip) is PetShowFireofResentmentEquip petShowFireofResentmentEquip)
		{
			petShowFireofResentmentEquip.m_count += m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		m_count--;
		if (base.ElementInfo.Pic != -1)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		}
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
		else
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
		}
	}
}
