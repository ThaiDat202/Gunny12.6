using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class LivingDelayPetEffectAction : BaseAction
{
	private AbstractPetEffect m_effect;

	private Living m_living;

	public LivingDelayPetEffectAction(Living living, AbstractPetEffect effect, int delay)
		: base(delay)
	{
		m_effect = effect;
		m_living = living;
	}

	protected override void ExecuteImp(BaseGame game, long tick)
	{
		m_effect.Start(m_living);
		Finish(tick);
	}
}
