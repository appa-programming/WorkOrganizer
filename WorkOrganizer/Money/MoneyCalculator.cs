using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoneyLib
{
    public static class MoneyCalculator
    {
        public enum MoneyFormat
        {
            UU_MM_CC,
            UU_DOT_CC_MM,
            UU_COMA_CC_MM
        }

        public static string GenerateMoneyString(int units, int cents, string format, string moneyUnit)
        {
            if (format.ToUpper() == "UU_MM_CC")
                return GenerateMoneyString(units, cents, MoneyFormat.UU_MM_CC, moneyUnit);
            else if (format.ToUpper() == "UU_DOT_CC_MM")
                return GenerateMoneyString(units, cents, MoneyFormat.UU_DOT_CC_MM, moneyUnit);
            else if (format.ToUpper() == "UU_COMA_CC_MM")
                return GenerateMoneyString(units, cents, MoneyFormat.UU_COMA_CC_MM, moneyUnit);
            else
                throw new Exception("MoneyLib Lib does not recognize the format " + format + ".");
        }
        public static string GenerateMoneyString(int units, int cents, MoneyFormat mf, string moneyUnit)
        {
            string TreatedUnits = (units + cents / 100).ToString();
            string TreatedCents = (cents % 100).ToString();
            if (TreatedCents.Length == 1)
                TreatedCents = "0" + TreatedCents;

            if (TreatedCents == "00")
                return TreatedUnits + moneyUnit;
            else if (mf == MoneyFormat.UU_MM_CC)
                return TreatedUnits + moneyUnit + TreatedCents;
            else if (mf == MoneyFormat.UU_DOT_CC_MM)
                return TreatedUnits + "." + TreatedCents + moneyUnit;
            else if (mf == MoneyFormat.UU_COMA_CC_MM)
                return TreatedUnits + "," + TreatedCents + moneyUnit;
            else
                throw new Exception("MoneyLib Lib can't handle Black Magic.");
        }

        public static bool IsItTheSameAmmount(string ammount1, string ammount2)
        {
            Tuple<int, int> TupleAmmounts1 = GetTupleAmmount(ammount1, '€');
            Tuple<int, int> TupleAmmounts2 = GetTupleAmmount(ammount2, '€');
            return (TupleAmmounts1.Item1 == TupleAmmounts2.Item1 &&
                        TupleAmmounts1.Item2 == TupleAmmounts2.Item2);
        }

        internal static Tuple<int, int> GetTupleAmmount(string ammount, char Currency)
        {
            string[] CurrencySplit = ammount.Split(Currency);
            if ((CurrencySplit.Length == 1 || CurrencySplit[1].Trim() == "") &&
                CurrencySplit[0].IndexOf(",") == -1 &&
                CurrencySplit[0].IndexOf(".") == -1)
            {
                return new Tuple<int, int> (int.Parse(CurrencySplit[0]), 0);
            }
            else if ((CurrencySplit.Length == 1 || CurrencySplit[1].Trim() == "") &&
                (CurrencySplit[0].IndexOf(",") > -1 ||
                CurrencySplit[0].IndexOf(".") > -1))
            {
                char Delimeter = '.';
                if (CurrencySplit[0].IndexOf(",") > -1)
                    Delimeter = ',';
                string[] DelimeterSplit = CurrencySplit[0].Split(Delimeter);
                int Decimal = int.Parse(DelimeterSplit[1]);
                if (DelimeterSplit[1].Length == 1)
                    Decimal = int.Parse(DelimeterSplit[1]) * 10;
                return new Tuple<int, int>(int.Parse(DelimeterSplit[0]), Decimal);
            }
            else
            {
                int Decimal = int.Parse(CurrencySplit[1]);
                if (CurrencySplit[1].Length == 1)
                    Decimal = int.Parse(CurrencySplit[1]) * 10;
                return new Tuple<int, int>(int.Parse(CurrencySplit[0]), int.Parse(CurrencySplit[1]));
            }
        }

        public static Money Multiply(Money m1, Money m2)
        {
            int M1_100 = m1.MoneyUnits * 100 + m1.MoneyCents;
            int M2_100 = m2.MoneyUnits * 100 + m2.MoneyCents;
            return new Money(
                                (M1_100 * M2_100) / 10000,
                                ((M1_100 * M2_100) % 10000) /100,
                                "€"
                            );
        }
    }
}
