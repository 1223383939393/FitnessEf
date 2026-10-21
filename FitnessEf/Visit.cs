using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessEf
{
    public class Visit
    {
        public int VisitId { get; set; }
        public int ClientId { get; set; }
        public DateOnly VisitDate { get; set; }
        public string? Note { get; set; }

        // Ссылка на объект клиента, которому принадлежит посещение
        public Client Client { get; set; } = null!;
    }
}
