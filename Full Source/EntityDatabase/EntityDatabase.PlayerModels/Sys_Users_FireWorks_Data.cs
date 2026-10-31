using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_FireWorks_Data
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int FireWorksID { get; set; }

	public bool IsGet { get; set; }

	public int GetMoney { get; set; }

	public DateTime GetDate { get; set; }
}
