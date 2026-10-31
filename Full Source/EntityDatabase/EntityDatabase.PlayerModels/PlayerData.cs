using System.Data.Entity;

namespace EntityDatabase.PlayerModels;

public class PlayerData : DbContext
{
	public virtual DbSet<Sys_Users_CardActive> Sys_Users_CardActive { get; set; }

	public virtual DbSet<Sys_Users_CardInfo> Sys_Users_CardInfo { get; set; }

	public virtual DbSet<Sys_Users_Elf> Sys_Users_Elf { get; set; }

	public virtual DbSet<Sys_Users_SkillBooks> Sys_Users_SkillBooks { get; set; }

	public virtual DbSet<Sys_Users_Emblem> Sys_Users_Emblem { get; set; }

	public virtual DbSet<Sys_Users_Scroll> Sys_Users_Scroll { get; set; }

	public virtual DbSet<Sys_Users_SecondPro> Sys_Users_SecondPro { get; set; }

	public virtual DbSet<Sys_Users_ElementPro> Sys_Users_ElementPro { get; set; }

	public virtual DbSet<Sys_Users_Honor> Sys_Users_Honor { get; set; }

	public virtual DbSet<Sys_Users_HonorRecord> Sys_Users_HonorRecord { get; set; }

	public virtual DbSet<Sys_Users_GourdExp> Sys_Users_GourdExp { get; set; }

	public virtual DbSet<Sys_Users_DevilTurn> Sys_Users_DevilTurn { get; set; }

	public virtual DbSet<Sys_Users_DevilTurn_Rank> Sys_Users_DevilTurn_Rank { get; set; }

	public virtual DbSet<Sys_Users_ListBox> Sys_Users_ListBox { get; set; }

	public virtual DbSet<Sys_Users_Team> Sys_Users_Team { get; set; }

	public virtual DbSet<Sys_Users_Team_User> Sys_Users_Team_User { get; set; }

	public virtual DbSet<Sys_Users_TeamActive> Sys_Users_TeamActive { get; set; }

	public virtual DbSet<Sys_Users_TeamDayActive> Sys_Users_TeamDayActive { get; set; }

	public virtual DbSet<Sys_Users_TeamInviteList> Sys_Users_TeamInviteList { get; set; }

	public virtual DbSet<Sys_Users_TeamRecord> Sys_Users_TeamRecord { get; set; }

	public virtual DbSet<Sys_Users_TeamSelfActive> Sys_Users_TeamSelfActive { get; set; }

	public virtual DbSet<Sys_Users_Sworn> Sys_Users_Sworn { get; set; }

	public virtual DbSet<Sys_Users_Labyrinth> Sys_Users_Labyrinth { get; set; }

	public virtual DbSet<User_GmActivityCondition> User_GmActivityCondition { get; set; }

	public virtual DbSet<User_GmActivityReward> User_GmActivityReward { get; set; }

	public virtual DbSet<Sys_Users_FireWorks> Sys_Users_FireWorks { get; set; }

	public virtual DbSet<Sys_Users_FireWorks_Data> Sys_Users_FireWorks_Data { get; set; }

	public virtual DbSet<Sys_Users_Academy> Sys_Users_Academy { get; set; }

	public virtual DbSet<Sys_Users_Butterfly> Sys_Users_Butterfly { get; set; }

	public PlayerData()
		: base("name=PlayerData")
	{
	}

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
	}
}
