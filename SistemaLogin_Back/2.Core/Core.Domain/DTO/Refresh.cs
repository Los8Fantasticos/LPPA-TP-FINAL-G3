using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO
{
    public class Refresh
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string token { get; set; }
        public DateTime expires { get; set; }

    }
}
