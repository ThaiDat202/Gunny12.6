using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdNormalKingThird : ABrain
{
	private int attackingTurn = 1;

	private int npcID = 3103;

	private int npcID2 = 3112;

	private int npcID1 = 3113;

	private int maxRespawnNPC = 10;

	private int coldDownBuffBlood = 0;

	public List<SimpleNpc> orchins = new List<SimpleNpc>();

	private static string[] AllAttackChat = new string[1] { "Tiếng gầm của mảnh hổ !!..." };

	private static string[] ShootChat = new string[4] { "Nhìn có giống cái gì không?", "Ta ném!! Tá ném!!!!", "Giỏi thì né nữa đi.", "Bách phát bách trúng" };

	private static string[] CallChat = new string[1] { "Vũ điệu<br/>săn bắn...." };

	private static string[] CallBuffChat = new string[3] { "Sức mạnh bộ tộc<br/>Hãy giúp ta!!!", "Ta muốn có nhiều sức mạnh!!!!", "Tiếp sức cho ta tiêu diệt kẻ thù..." };

	private static string[] CallExplodeChat = new string[3] { "Có đỡ được không?", "Bộ lạc hãy giúp ta", "Tiếp sức!!! Tiếp sức!!!!" };

	private static string[] AngryChat = new string[3] { "Dám chọc giận ta à?", "Ta né, ta né!!!", "Đồ khốn nạn. Dám đánh ta!!" };

	private static string[] KillAttackChat = new string[1] { "Muốn xem lợi hại của cổ họng của ta!" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		if (coldDownBuffBlood > 0)
		{
			coldDownBuffBlood--;
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		bool flag = false;
		int num = 0;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 250 && allFightPlayer.X < base.Body.X + 250)
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
			KillAttack(base.Body.X - 250, base.Body.X + 250);
			return;
		}
		if (attackingTurn == 1)
		{
			HalfAttack();
		}
		else if (attackingTurn == 2)
		{
			PersonalAttack();
		}
		else if (attackingTurn == 3)
		{
			List<SimpleNpc> list = (base.Body as SimpleBoss).FindChildLiving(npcID1);
			if (list.Count <= 0)
			{
				Summon();
			}
			else
			{
				List<SimpleNpc> list2 = (base.Body as SimpleBoss).FindChildLiving(npcID2);
				if (list.Count <= 0)
				{
					SummonExplode();
				}
				else
				{
					PersonalAttack();
				}
			}
		}
		else if (attackingTurn == 4)
		{
			PersonalAttackDame();
		}
		else if (attackingTurn == 5)
		{
			int num3 = base.Game.Random.Next(0, 10);
			if (num3 > 5)
			{
				SummonExplode();
			}
			else
			{
				PersonalAttack();
			}
		}
		else
		{
			if ((base.Body as SimpleBoss).FindChildLiving(npcID).Count <= 0)
			{
				SummonNpc();
			}
			else
			{
				PersonalAttackDame();
			}
			attackingTurn = 0;
		}
		attackingTurn++;
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
		base.Game.RemoveLiving(npcID);
	}

	public void HalfAttack()
	{
		base.Body.CurrentDamagePlus = 1.5f;
		int num = base.Game.Random.Next(0, AllAttackChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatC", 2500, 0);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 3500, null);
	}

	private void PersonalAttackDame()
	{
		Player[] allPlayers = base.Game.GetAllPlayers();
		int num = 200;
		int num2 = base.Game.Random.Next(0, ShootChat.Length);
		base.Body.Say(ShootChat[num2], 1, num);
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			if (player.IsLiving)
			{
				num += 200;
				base.Body.ChangeDirection(base.Body.FindDirection(player), num);
				num += 2000;
				if (base.Body.ShootPoint(player.X, player.Y, 55, 1000, 10000, 1, 1.5f, num))
				{
					base.Body.PlayMovie("beatB", num - 1000, 0);
				}
			}
		}
	}

	private void PersonalAttack()
	{
		int x = base.Game.Random.Next(700, 800);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 4, NextAttack);
	}

	private void NextAttack()
	{
		Player[] allPlayers = base.Game.GetAllPlayers();
		int num = 200;
		int num2 = base.Game.Random.Next(0, ShootChat.Length);
		base.Body.Say(ShootChat[num2], 1, num);
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			if (player.IsLiving)
			{
				num += 200;
				base.Body.ChangeDirection(base.Body.FindDirection(player), num);
				num += 2000;
				if (base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 1, 1.5f, num))
				{
					base.Body.PlayMovie("beatA", num - 1000, 0);
				}
			}
		}
	}

	public void Summon()
	{
		coldDownBuffBlood = 5;
		int num = base.Game.Random.Next(0, CallBuffChat.Length);
		base.Body.Say(CallBuffChat[num], 1, 0);
		base.Body.PlayMovie("callA", 100, 0);
		base.Body.CallFuction(CreateBuffBlood, 2500);
	}

	public void SummonNpc()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 1, 0);
		base.Body.PlayMovie("callB", 100, 0);
		base.Body.CallFuction(CreateChild, 4500);
	}

	public void SummonExplode()
	{
		int num = base.Game.Random.Next(0, CallExplodeChat.Length);
		base.Body.Say(CallExplodeChat[num], 1, 0);
		base.Body.PlayMovie("callA", 100, 0);
		base.Body.CallFuction(CreateExplode, 2500);
	}

	public void KillAttack(int fx, int mx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatC", 2500, 0);
		base.Body.RangeAttacking(fx, mx, "cry", 3300, null);
	}

	public void CreateChild()
	{
		int num = 1000;
		for (int i = 0; i < maxRespawnNPC; i++)
		{
			((SimpleBoss)base.Body).CreateChild(npcID, num, 555, -1, maxRespawnNPC, -1);
			num -= 50;
		}
	}

	public void CreateBuffBlood()
	{
		((SimpleBoss)base.Body).CreateBoss(npcID1, 568, 554, -1, 0);
		((SimpleBoss)base.Body).CreateBoss(npcID1, 1093, 556, 1, 0);
	}

	public void CreateExplode()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			((SimpleBoss)base.Body).CreateBoss(npcID2, player.X, player.Y, player.Direction, 0);
		}
	}
}
