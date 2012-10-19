using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Eking.News
{
    public static class Extensions
    {
    }

    public static class Translator
    {
        public static string T(string key)
        {
            var trans = LocalizedText_vi.ResourceManager.GetString(key);
            if (trans == null)
                return key;
            return trans;
        }
    }
}