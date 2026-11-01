using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDatabase.PlayerModels;

public class Sys_Users_ElfEquip
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    public int ID { get; set; }

    public int UserID { get; set; }

    public int TemplateID { get; set; }

    public int Type { get; set; }

    public int Quality { get; set; }

    public int Level { get; set; }

    public bool IsEquip { get; set; }
}

