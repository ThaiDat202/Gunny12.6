using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_FireWorks
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int SendMoney { get; set; }

	public int SendCount { get; set; }

	public int SendType { get; set; }

	public string SendMessage { get; set; }

	public DateTime SendDate { get; set; }
}
