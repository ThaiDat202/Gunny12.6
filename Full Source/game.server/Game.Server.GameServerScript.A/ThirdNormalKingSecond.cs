using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdNormalKingSecond : ABrain
{
	private int m_attackTurn = 0;

	private int npcID = 3106;

	private SimpleBoss friendBoss;

	private int BloodRecover = 2000;

	private List<PhysicalObj> phyStarList = new List<PhysicalObj>();

	private static string[] AllAttackChat = new string[4] { "Kiếm thần!!! Kiếm thần!!", "Hãy đỡ tuyệt chiêu kiếm thần!!", "Kiếm thần!! Trợ giúp!!", "Kiếm thần đây. Hãy đỡ!!!!" };

	private static string[] CallChat = new string[4] { "Vệ binh! <br/> bảo vệ! ! ", "Dũng sĩ bộ lạc đâu thể hiện đi!", "Đệ tử của ta đâu mau ra đây!!!", "Lính đâu ra giết bọn chúng." };

	private static string[] ShootCallChat = new string[4] { "Chạy à? Chạy nữa đi!!!", "Một phát chết 3 con gián.", "Trẻ trâu mà láo à?", "Lũ gà!!! Chết này." };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		if (friendBoss != null && friendBoss.IsLiving)
		{
			return;
		}
		List<Living> list = base.Game.FindAllTurnBossLiving();
		foreach (Living item in list)
		{
			if (item != base.Body)
			{
				friendBoss = item as SimpleBoss;
				break;
			}
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 300 && allFightPlayer.X < base.Body.X + 300)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 300, base.Body.X + 300);
		}
		else if (friendBoss != null && !friendBoss.IsLiving)
		{
			base.Body.PlayMovie("castA", 800, 0);
			base.Body.Say("Anh trai ơi đừng đầu hàng.<br/>Em sẽ giúp anh hồi sinh!!!", 0, 1000);
			((PVEGame)base.Game).SendObjectFocus(friendBoss, 1, 3000, 1000);
			base.Body.CallFuction(ReviveFriendBoss, 5000);
		}
		else if (friendBoss != null && friendBoss.Blood <= friendBoss.NpcInfo.Blood / 10)
		{
			base.Body.PlayMovie("castA", 800, 0);
			base.Body.Say("Các ngươi làm sao đủ khả năng đánh bại anh ta!!!", 0, 1000);
			((PVEGame)base.Game).SendObjectFocus(friendBoss, 1, 3500, 5000);
			base.Body.CallFuction(RecoverFriendBlood, 4000);
		}
		else
		{
			JumpBeat();
		}
	}

	public void RecoverFriendBlood()
	{
		friendBoss.AddBlood(BloodRecover);
	}

	public void ReviveFriendBoss()
	{
		SimpleBoss simpleBoss = null;
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.ReduceBloodStart = 2;
		base.Game.RemoveLiving(friendBoss.Id);
		simpleBoss = ((PVEGame)base.Game).CreateBoss(friendBoss.NpcInfo.ID, friendBoss.X, friendBoss.Y, friendBoss.Direction, 1, "raise", livingConfig);
		simpleBoss.SetRelateDemagemRect(simpleBoss.NpcInfo.X, simpleBoss.NpcInfo.Y, simpleBoss.NpcInfo.Width, simpleBoss.NpcInfo.Height);
		simpleBoss.Say("Ta đã hồi sinh!!! Các ngươi phải chết.", 0, 3000);
		friendBoss = simpleBoss;
	}

	private void JumpBeat()
	{
		if (base.Body.Y >= 310 && base.Body.Y <= 420)
		{
			base.Body.FallFromTo(base.Body.X, 477, "", 1000, 0, 25, GoInBeat);
		}
		else if (base.Body.Y >= 450 && base.Body.Y <= 540)
		{
			base.Body.PlayMovie("walk", 100, 1000);
			base.Body.FallFromTo(base.Body.X, 623, "", 1000, 0, 25, GoInBeat);
		}
		else if (base.Body.Y >= 580 && base.Body.Y <= 668)
		{
			base.Body.JumpTo(base.Body.X, 361, "walk", 1000, 1, 20, GoInBeat);
		}
	}

	private void GoInBeat()
	{
		switch (m_attackTurn)
		{
		case 0:
			ShootBeat();
			break;
		case 1:
			CreateAllAttack();
			break;
		case 2:
			CallbBoss();
			m_attackTurn = 0;
			break;
		}
		m_attackTurn++;
	}

	private void ShootBeat()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.PlayMovie("stand", 0, 0);
		base.Body.CurrentDamagePlus = 1.8f;
		int num = base.Game.Random.Next(0, ShootCallChat.Length);
		base.Body.Say(ShootCallChat[num], 0, 1000);
		if (player != null && base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 3, 2f, 2300))
		{
			base.Body.PlayMovie("beatA", 1500, 0);
		}
	}

	private void CallbBoss()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 0, 1000);
		base.Body.PlayMovie("castA", 1000, 0);
		base.Body.CallFuction(CreateChild, 4000);
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 1369, 242, 0, 1, -1);
	}

	private void CreateAllAttack()
	{
		base.Body.CurrentDamagePlus = 1.5f;
		int num = base.Game.Random.Next(0, AllAttackChat.Length);
		base.Body.Say(AllAttackChat[num], 0, 1000);
		base.Body.PlayMovie("castA", 1000, 0);
		Player obj = base.Game.FindRandomPlayer();
		((PVEGame)base.Game).SendObjectFocus(obj, 2, 3000, 1000);
		base.Body.CallFuction(CreateStarMovie, 4000);
	}

	public void CreateStarMovie()
	{
		Player[] allPlayers = base.Game.GetAllPlayers();
		foreach (Player player in allPlayers)
		{
			if (player.IsLiving)
			{
				PhysicalObj item = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "", "game.crazytank.assetmap.Dici", "", 1, 0);
				phyStarList.Add(item);
			}
		}
		base.Body.RangeAttacking(-10000, 10000, "cry", 700, null);
		base.Body.CallFuction(RemoveMovie, 1500);
	}

	public void RemoveMovie()
	{
		foreach (PhysicalObj phyStar in phyStarList)
		{
			base.Game.RemovePhysicalObj(phyStar, sendToClient: true);
		}
		phyStarList = new List<PhysicalObj>();
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("call", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
