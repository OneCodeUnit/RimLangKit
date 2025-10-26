using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TextExporter
{
    public class RimDef
    {
        public string? DefName { get; set; }
        public string? DefClass { get; set; }
        public string? DefFile { get; set; }

        public BaseDef? RimTag { get; set; }
    }
}
