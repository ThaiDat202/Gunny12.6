using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.GuardCoreEffects;

public class BaseGuardCoreEffect : AbstractGuardCoreEffect
{
	public BaseGuardCoreEffect(eGuardCoreEffectType type, GuardCoreTemplateInfo info)
		: base(type, info)
	{
	}

	public override bool Start(Living living)
	{
		if (living is Player)
		{
			return base.Start(living);
		}
		return false;
	}

	public sealed override void OnAttached(Living living)
	{
		if (living is Player)
		{
			OnAttachedToPlayer(living as Player);
		}
	}

	public sealed override void OnRemoved(Living living)
	{
		if (living is Player)
		{
			OnRemovedFromPlayer(living as Player);
		}
	}

	protected virtual void OnAttachedToPlayer(Player player)
	{
	}

	protected virtual void OnRemovedFromPlayer(Player player)
	{
	}
}
