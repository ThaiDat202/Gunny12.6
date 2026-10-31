using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EntityDatabase.ServerModels;

public class ServerData : DbContext
{
	public virtual DbSet<TS_CardBooklet> TS_CardBooklet { get; set; }

	public virtual DbSet<TS_CardMain> TS_CardMain { get; set; }

	public virtual DbSet<TS_CardSuit> TS_CardSuit { get; set; }

	public virtual DbSet<TS_UpgradeTemplate> TS_UpgradeTemplate { get; set; }

	public virtual DbSet<TS_NecklaceCasting> TS_NecklaceCasting { get; set; }

	public virtual DbSet<StrengThenExp> StrengThenExp { get; set; }

	public virtual DbSet<TS_Emblem> TS_Emblem { get; set; }

	public virtual DbSet<TS_Scroll> TS_Scroll { get; set; }

	public virtual DbSet<Elf_SkillBook> Elf_SkillBook { get; set; }

	public virtual DbSet<Elf_Template_Info> Elf_Template_Info { get; set; }

	public virtual DbSet<TS_HonorSystem_Template> TS_HonorSystem_Template { get; set; }

	public virtual DbSet<DevilTreasItemList> DevilTreasItemList { get; set; }

	public virtual DbSet<DevilTreasPointsList> DevilTreasPointsList { get; set; }

	public virtual DbSet<DevilTreasRankRewardList> DevilTreasRankRewardList { get; set; }

	public virtual DbSet<DevilTreasSarahToBoxList> DevilTreasSarahToBoxList { get; set; }

	public virtual DbSet<DevilTurnOpenBox> DevilTurnOpenBox { get; set; }

	public virtual DbSet<God_Of_Wealth_Reward> God_Of_Wealth_Reward { get; set; }

	public virtual DbSet<BattleTeamActiveTemplateList> BattleTeamActiveTemplateList { get; set; }

	public virtual DbSet<BattleTeamLevelList> BattleTeamLevelList { get; set; }

	public virtual DbSet<BattleTeamSegmentList> BattleTeamSegmentList { get; set; }

	public virtual DbSet<BattleTeamShopItemList> BattleTeamShopItemList { get; set; }

	public virtual DbSet<BattleTeamSeasonList> BattleTeamSeasonList { get; set; }

	public virtual DbSet<Pet_Special_Skills> Pet_Special_Skills { get; set; }

	public virtual DbSet<Labyrinth_Rewards> Labyrinth_Rewards { get; set; }

	public virtual DbSet<GM_Activity> GM_Activity { get; set; }

	public virtual DbSet<GM_Gift> GM_Gift { get; set; }

	public virtual DbSet<GM_Active_Reward> GM_Active_Reward { get; set; }

	public virtual DbSet<GM_Active_Condition> GM_Active_Condition { get; set; }

	public virtual DbSet<Consortia_Task_Conditions> Consortia_Task_Conditions { get; set; }

	public virtual DbSet<TS_Butterfly> TS_Butterfly { get; set; }

	public virtual DbSet<TS_ButterflyTask> TS_ButterflyTask { get; set; }

	public virtual DbSet<TS_HomeTempAdvance_Template> TS_HomeTempAdvance_Template { get; set; }

	public ServerData()
		: base("name=ServerData")
	{
	}

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
	}
}
