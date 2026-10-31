using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_ListBox
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }

	public int UserID { get; set; }

	public int BoxId { get; set; }

	public int Count { get; set; }

	public int state { get; set; }

	public int boxIndex { get; set; }

	public int subtractMin { get; set; }

	public int min { get; set; }

	public int max { get; set; }

	public int subtractMax { get; set; }

	public int needmoney { get; set; }

	public int currentIndex { get; set; }

	public DateTime OpenDate { get; set; }
}
