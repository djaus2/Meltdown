using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazMail.Data
{
    public class AppSettings
    {
        public string ClientHost { get; set; }
		public string fromName { get; set; }
		public string fromEmail { get; set; }
		public string fromPassword { get; set; }
		public string ClientPortStr { get; set; }
    }
}
