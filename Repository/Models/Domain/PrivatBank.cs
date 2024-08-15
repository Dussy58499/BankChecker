using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Models.Domain
{
    public class PrivatBank
    {
        public string Ccy { get; set; }
        public string Base_Ccy { get; set; }
        public decimal Buy { get; set; }
        public decimal Sale { get; set; }
    }
}
