using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Server.GameServerScript.AI.Messions;

namespace Game.Server.GameServerScript.AI.NPC;

public class FourTerrorShortNpc : ABrain
{
	private string[] NormalSay = new string[5] { "Thằng khốn nạn nào quăng mình ra đây thế này?", "Đây là đâu? Sao nóng thế?", "Tại sao mình lại ở đây nhỉ?", "Chuyện gì đang xảy ra vậy?", "Thật không thể hiểu nổi!" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (base.Game.Random.Next(100) < 50)
		{
			int num = base.Game.Random.Next(0, NormalSay.Length);
			base.Body.Say(NormalSay[num], 1, 0);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		SimpleBoss helper = (((PVEGame)base.Game).MissionAI as DCT4301).Helper;
		int num = base.Game.Random.Next(100, 200);
		base.Body.MoveTo(helper.X + num, helper.Y, "walk", 1500, 3);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
