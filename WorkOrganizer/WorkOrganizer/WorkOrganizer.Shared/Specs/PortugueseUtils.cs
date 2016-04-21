using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrganizer.Specs
{
    static class PortugueseUtils
    {
        public static string GetMonthName(DateTime dt)
        {
            return GetMonthName(dt.Month);
        }

        public static string GetMonthName(int m)
        {
            if      (m ==  1) return "Janeiro";
            else if (m ==  2) return "Fevereiro";
            else if (m ==  3) return "Março";
            else if (m ==  4) return "Abril";
            else if (m ==  5) return "Maio";
            else if (m ==  6) return "Junho";
            else if (m ==  7) return "Julho";
            else if (m ==  8) return "Agosto";
            else if (m ==  9) return "Setembro";
            else if (m == 10) return "Outubro";
            else if (m == 11) return "Novembro";
            else if (m == 12) return "Dezembro";
            else              return "Asneira";
        }

    }
}
