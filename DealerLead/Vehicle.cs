using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DealerLead
{
    public class Vehicle
    {
        [Key]
        [Column("VehicleId")]
        public int Id { get; set; }

        [ForeignKey("ModelId")]
        [Column("ModelId")]
        [Display(Name = "Model")]
        public int SupportedModelId { get; set; }
        [Display(Name = "Model")]
        public SupportedModel SupportedModel { get; set; }

        [Column("MSRP")]
        public decimal MSRP { get; set; }

        [Column("StockNumber")]
        [Display(Name = "Stock Number")]
        public string StockNumber { get; set; }

        [Column("Color")]
        public string Color { get; set; }

        [ForeignKey("DealershipId")]
        [Column("DealershipId")]
        [Display(Name = "Dealership")]
        public int DealershipId { get; set; }
        public Dealership Dealership { get; set; }

        [Display(Name = "Sell Date")]
        public DateTime? SellDate { get; set; }

        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreateDate { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? ModifyDate { get; set; }

        public ICollection<Lead> Leads { get; set; }
    }
}
