using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotRiftPeek.Models
{
    public class HelpResponse
    {
        public Dictionary<string, string> events;
        public Dictionary<string, string> functions;
        public Dictionary<string, string> types;
    }
}
