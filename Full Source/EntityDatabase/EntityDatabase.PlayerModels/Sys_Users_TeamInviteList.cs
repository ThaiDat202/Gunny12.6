using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_TeamInviteList
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserId { get; set; }

	public int teamID { get; set; }

	public string name { get; set; }

	public DateTime date { get; set; }

	public int VipLv { get; set; }
}
