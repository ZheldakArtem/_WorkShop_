using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class MapId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int idUi { get; set; }
        public int idAzure { get; set; }
    }
}
