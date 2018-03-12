using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VipcoMaintenance.Models.Maintenances
{
    public class Branch:BaseModel
    {
        [Key]
        public int BranchId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        //Fk
        //Item
        public virtual ICollection<Item> Items { get; set; }
        // RequireMaintenance
        public virtual ICollection<RequireMaintenance> RequireMaintenances { get; set; }
    }
}
