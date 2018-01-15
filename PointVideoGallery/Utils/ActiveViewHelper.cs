using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PointVideoGallery.Utils
{
    public class ActiveViewHelper
    {
        public static string ActiveClass(string routeName, string activeName)
        {
            if (routeName.Equals(activeName, StringComparison.OrdinalIgnoreCase))
                return "active";
            return String.Empty;
        }
    }
}