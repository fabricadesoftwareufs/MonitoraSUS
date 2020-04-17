using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace MonitoraSUS.Utils
{
    public class Methods
    {
        public static string RemoveSpecialsCaracts(string poluatedString) => Regex.Replace(poluatedString, "[^0-9a-zA-Z]+", "");

        public static string ReturnRole(int userType)
        {
            switch (userType)
            {
                case 0: return "USUARIO";
                case 1: return "AGENTE";
                case 2: return "COORDENADOR";
                case 3: return "SECRETARIO";
                case 4: return "ADM";
                default: return "UNDEFINED";
            }
        }
    }
}
