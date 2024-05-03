using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class LogModel
    {
        public string log { get; set; }
        public LogLevel LogLevel { get; set; }
        public DateTime createDate { get; set; }
    }
}
