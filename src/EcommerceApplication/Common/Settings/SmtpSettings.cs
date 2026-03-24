using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Common.Settings
{
    public class SmtpSettings
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; } = 587;
        public string From { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool EnableSsl { get; set; } = true;

    }
}
