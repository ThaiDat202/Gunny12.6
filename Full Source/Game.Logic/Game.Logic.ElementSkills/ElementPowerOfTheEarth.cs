using System;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills;

public class ElementPowerOfTheEarth : BasePetEffect
{
	public ElementPowerOfTheEarth()
		: base(ePetEffectType.ElementPowerOfTheEarth, "")
	{
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.OnMakeDamageEvent += player_OnMakeDamageEvent;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.OnMakeDamageEvent -= player_OnMakeDamageEvent;
	}

	private void player_OnMakeDamageEvent(Living target, Living source, ref int damageamount, ref int criticalamount)
	{
		if (source.Y < target.Y)
		{
			source.Game.AddAction(new LivingSayAction(source, "大地之力", 9, 0, 1000));
			int num = Math.Abs(target.Y - source.Y);
			damageamount += (int)(source.LandAttack * 0.8) * num / 100;
		}
	}

	public override bool Start(Living living)
	{
		return base.Start(living);
	}
}
