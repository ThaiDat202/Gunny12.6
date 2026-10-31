using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_TeamRecord
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int teamID { get; set; }

	public bool isWin { get; set; }

	public int active { get; set; }

	public DateTime date { get; set; }

	public string teamName { get; set; }

	public string teamZone { get; set; }

	public int teamKill { get; set; }

	public int teamSurvival { get; set; }

	public string teamMemberInfo { get; set; }

	public string enemyName { get; set; }

	public string enemyZone { get; set; }

	public int enemyKill { get; set; }

	public int enemySurvival { get; set; }

	public string enemyMemberInfo { get; set; }
}
