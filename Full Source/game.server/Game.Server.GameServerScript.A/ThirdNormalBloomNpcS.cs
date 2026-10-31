using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdNormalBloomNpcS : ABrain
{
	private int m_recoverBlood = 500;

	private static string[] SayChat = new string[4] { "Cẩn thận... chúng ta phải tiêu diệt tà thần", "Không được đầu hàng !", "Tôi sẽ tiếp cho các bạn sức mạnh！", "Chúng ta sẽ đẩy lùi bộ tộc tà thần！" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		if (base.Game.TurnIndex % 2 == 0)
		{
			int num = base.Game.Random.Next(0, SayChat.Length);
			base.Body.Say(SayChat[num], 1, 500);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		base.Body.PlayMovie("renew", 1000, 3000);
		base.Body.CallFuction(RecoverBlood, 3000);
	}

	private void RecoverBlood()
	{
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 300 && allFightPlayer.X < base.Body.X + 300)
			{
				allFightPlayer.AddBlood(m_recoverBlood);
			}
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
