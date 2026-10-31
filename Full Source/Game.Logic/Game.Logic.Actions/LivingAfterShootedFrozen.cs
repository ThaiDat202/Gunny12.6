using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class LivingAfterShootedFrozen : BaseAction
{
	private Living m_liv;

	public LivingAfterShootedFrozen(Living living, int delay)
		: base(delay, 0)
	{
		m_liv = living;
	}

	protected override void ExecuteImp(BaseGame game, long tick)
	{
		m_liv.OnAfterTakedFrozen();
		Finish(tick);
	}
}
