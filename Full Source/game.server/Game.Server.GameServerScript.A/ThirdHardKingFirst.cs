using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdHardKingFirst : ABrain
{
	private int m_attackTurn = 0;

	private List<PhysicalObj> starDamageGlobal = new List<PhysicalObj>();

	private SimpleBoss friendBoss;

	private int npcID = 3206;

	private int fearId = 3210;

	private int lockId = 3211;

	private int BloodRecover = 5000;

	private static string[] AllAttackChat = new string[3] { "Trận động đất, bản thân mình! ! <br/> bạn vui lòng Ay giúp đỡ", "Hạ vũ khí xuống!", "Xem nếu bạn có thể đủ khả năng, một số ít!！" };

	private static string[] CallChat = new string[4] { "Vệ binh! <br/> bảo vệ! ! ", "Dũng sĩ bộ lạc đâu thể hiện đi!", "Đệ tử của ta đâu mau ra đây!!!", "Lính đâu ra giết bọn chúng." };

	private static string[] FearCallChat = new string[3] { "Có cảm thấy bủn rủn không?", "Ngươi không đủ sức để hạ ta đâu.", "Thấy mệt mỏi chưa?" };

	private static string[] LockCallChat = new string[2] { "Sao mà bắn ta được?", "Giỏi thì thử bắn nữa đi." };

	private static string[] ShootCallChat = new string[2] { "Sao mà bắn ta được?", "Giỏi thì thử bắn nữa đi." };

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
			base.Body.Say("Em trai ơi đừng đầu hàng.<br/>Anh sẽ giúp em hồi sinh!!!", 0, 1000);
			((PVEGame)base.Game).SendObjectFocus(friendBoss, 1, 3000, 1000);
			base.Body.CallFuction(ReviveFriendBoss, 5000);
		}
		else if (friendBoss != null && friendBoss.Blood <= friendBoss.NpcInfo.Blood / 10)
		{
			base.Body.PlayMovie("castA", 800, 0);
			base.Body.Say("Các ngươi làm sao đủ khả năng đánh bại em ta!!!", 0, 1000);
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

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("call", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void JumpBeat()
	{
		if (base.Body.Y >= 310 && base.Body.Y <= 420)
		{
			base.Body.FallFromTo(base.Body.X, 483, "", 1000, 0, 25, GoInBeat);
		}
		else if (base.Body.Y >= 450 && base.Body.Y <= 540)
		{
			base.Body.PlayMovie("walk", 100, 1000);
			base.Body.FallFromTo(base.Body.X, 623, "", 1000, 0, 25, GoInBeat);
		}
		else if (base.Body.Y >= 580 && base.Body.Y <= 668)
		{
			base.Body.JumpTo(base.Body.X, 360, "walk", 1000, 1, 20, GoInBeat);
		}
	}

	private void GoInBeat()
	{
		switch (m_attackTurn)
		{
		case 0:
			if (!IsFearBuffLiving())
			{
				CreateFearBuff();
			}
			else
			{
				ShootBeat();
			}
			m_attackTurn++;
			break;
		case 1:
			if (!IsChildNPCLiving())
			{
				CallbBoss();
			}
			else
			{
				ShootBeat();
			}
			m_attackTurn++;
			break;
		case 2:
			ShootBeat();
			m_attackTurn++;
			break;
		case 3:
			if (!IsLockBuffLiving())
			{
				CreateLockBuff();
			}
			else
			{
				ShootBeat();
			}
			m_attackTurn = 0;
			break;
		}
	}

	private bool IsFearBuffLiving()
	{
		SimpleNpc[] array = base.Game.FindAllNpc();
		SimpleNpc[] array2 = array;
		foreach (SimpleNpc simpleNpc in array2)
		{
			if (simpleNpc.IsLiving && simpleNpc.NpcInfo.ID == fearId)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsChildNPCLiving()
	{
		SimpleNpc[] array = base.Game.FindAllNpc();
		SimpleNpc[] array2 = array;
		foreach (SimpleNpc simpleNpc in array2)
		{
			if (simpleNpc.IsLiving && simpleNpc.NpcInfo.ID == npcID)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsLockBuffLiving()
	{
		SimpleNpc[] array = base.Game.FindAllNpc();
		SimpleNpc[] array2 = array;
		foreach (SimpleNpc simpleNpc in array2)
		{
			if (simpleNpc.IsLiving && simpleNpc.NpcInfo.ID == lockId)
			{
				return true;
			}
		}
		return false;
	}

	private void CreateFearBuff()
	{
		int num = base.Game.Random.Next(0, FearCallChat.Length);
		base.Body.Say(FearCallChat[num], 0, 2000);
		base.Body.PlayMovie("call", 2000, 0);
		base.Body.CallFuction(CreateFearNPC, 5000);
	}

	public void CreateFearNPC()
	{
		((SimpleBoss)base.Body).CreateChild(fearId, 1079, 631, 0, 3, -1);
	}

	private void CreateLockBuff()
	{
		int num = base.Game.Random.Next(0, LockCallChat.Length);
		base.Body.Say(LockCallChat[num], 0, 2000);
		base.Body.PlayMovie("call", 2000, 0);
		base.Body.CallFuction(CreateLockNPC, 5000);
	}

	public void CreateLockNPC()
	{
		((SimpleBoss)base.Body).CreateChild(lockId, 567, 631, 0, 3, 1);
	}

	private void CallbBoss()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 0, 2300);
		base.Body.PlayMovie("castA", 2500, 0);
		base.Body.CallFuction(CreateChild, 5000);
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 217, 242, 0, 3, 1);
	}

	private void ShootBeat()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.PlayMovie("stand", 0, 0);
		base.Body.CurrentDamagePlus = 1.8f;
		if (player != null && base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 3, 2f, 2300))
		{
			base.Body.PlayMovie("beatA", 1500, 0);
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
