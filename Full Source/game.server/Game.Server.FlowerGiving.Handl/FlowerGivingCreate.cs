using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.FlowerGiving.Handle;

[FlowerGivingHandleAttbute(7)]
public class FlowerGivingCreate : IFlowerGivingCommandHadler
{
	protected FlowerGivingLogicProcessor m_FlowerGivingProcessor = new FlowerGivingLogicProcessor();

	private FlowerGivingProcessor m_FlowerGivingHandler;

	public FlowerGivingProcessor FlowerGivingHandler => m_FlowerGivingHandler;

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		m_FlowerGivingHandler = new FlowerGivingProcessor(m_FlowerGivingProcessor);
		return true;
	}
}
