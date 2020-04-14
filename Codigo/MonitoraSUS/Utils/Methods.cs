using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonitoraSUS.Utils
{
    public class Methods
    {
        public static string RemoveSpecialsCaracts(string poluatedString) => Regex.Replace(poluatedString, "[^0-9a-zA-Z]+", "");
    }
}
