using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdTerrorBloomNpcS : ABrain
{
	private const int AddBloom = 1600;

	private static readonly string[] AntChat = new string[5]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalBloomNpcS.msg1"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalBloomNpcS.msg2"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalBloomNpcS.msg3"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalBloomNpcS.msg4"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.ThirdNormalBloomNpcS.msg5")
	};

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		base.Body.PlayMovie("", 0, 1000);
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.Random.Next(0, 5) == 0)
		{
			m_body.Say(AntChat[base.Game.Random.Next(0, AntChat.Length)], 0, 1000);
		}
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 100 && allFightPlayer.X < base.Body.X + 100)
			{
				num++;
			}
		}
		base.Body.PlayMovie("renew", 0, 0);
		foreach (Player allFightPlayer2 in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer2.IsLiving && allFightPlayer2.X > base.Body.X - 100 && allFightPlayer2.X < base.Body.X + 100)
			{
				allFightPlayer2.AddBlood(1600 / num);
			}
		}
	}
}
