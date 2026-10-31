using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class ShowPic : BasePetEffect
{
	private int count = 0;

	public ShowPic(string elementID)
		: base(ePetEffectType.ShowPic, elementID)
	{
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.ShowPic) is ShowPic)
		{
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
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (living.Game is PVPGame && count == 0)
		{
			count = 1;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		}
	}
}
