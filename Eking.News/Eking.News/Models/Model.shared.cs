using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eking.News.Models
{
    public partial class Group
    {
        public override string ToString()
        {
            return string.Format("{0}:{1}", Id, Name);
        }
    }

    public partial class Entry
    {
        public override string ToString()
        {
            return string.Format("{0}:{1}", Id, Title);
        }
    }
}