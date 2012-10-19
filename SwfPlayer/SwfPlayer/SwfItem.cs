using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwfPlayer
{
    public class SwfItem
    {
        public string Title { get; set; }

        public string FilePath { get; set; }

        public SwfCategory Category { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
