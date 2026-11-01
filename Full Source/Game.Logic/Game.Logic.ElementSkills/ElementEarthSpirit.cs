using System;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills;

public class ElementEarthSpirit : AbstractPetEffect
{
	public ElementEarthSpirit()
		: base(ePetEffectType.ElementEarthSpirit, "")
	{
	}

	public override void OnAttached(Living player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	public override void OnRemoved(Living player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		living.Game.AddAction(new LivingSayAction(living, "Tinh Hoa Của Đất", 9, 0, 1000));
		int num = Math.Abs(living.X - source.X);
		damageAmount -= (int)(living.LandDefence * 0.6) * num / 100;
	}

	public override bool Start(Living living)
	{
		return base.Start(living);
	}
}
