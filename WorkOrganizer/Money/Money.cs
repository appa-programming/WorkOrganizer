using System;
using static MoneyLib.MoneyCalculator;

namespace MoneyLib
{
    public class Money
    {
        public int MoneyUnits { get; private set; }
        public int MoneyCents { get; private set; }
        public string Currency { get; private set; }

        public Money(int mu, int mc, string c)
        {
            string Aux = GenerateMoneyString(mu, mc, MoneyFormat.UU_MM_CC, c);
            Tuple<int, int> Tuple = MoneyCalculator.GetTupleAmmount(Aux, c.ToCharArray()[0]);
            MoneyUnits = Tuple.Item1;
            MoneyCents = Tuple.Item2;
            Currency = c;
        }

        public Money(string money)
        {
            Char CurrencyAux = '€';
            Tuple<int, int> t = GetTupleAmmount(money, CurrencyAux);
            MoneyUnits = t.Item1;
            MoneyCents = t.Item2;
            Currency = CurrencyAux.ToString();
        }
    }
}
