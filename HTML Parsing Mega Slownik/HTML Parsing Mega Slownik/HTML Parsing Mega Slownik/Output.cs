using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTML_Parsing_Mega_Slownik
{
    struct Output
    {
        public string Example { get; set; }
        public string Meaning { get; set; }
        public string Category { get; set; }
        public List<string> Synonyms { get; set; }
        public List<string> LookAt { get; set; }
    }
}
