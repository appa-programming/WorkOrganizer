﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WorkOrganizer.Specs;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrganizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SummaryPage : Page
    {
        public Owner CurrentOwner { get; private set; }
        public DateTime CurrentDateTime { get; private set; }

        public SummaryPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.ToString() != "")
            {
                var List = (List<WorkEvent>)e.Parameter;
                //List = List.OrderBy(we => we.IdHouse).ThenBy(we => we.Time).ToList();
                List = List.OrderBy(we => we.Time).ToList();
                //var Text = MakeTextBasedOnHouses(List);
                var Text = MakeTextBasedOnDate(List);
                TextBoxSummary.Text = Text;

                if (CurrentOwner != null && CurrentOwner.Email != "")
                {
                    ButtonSendEmail.Visibility = Visibility.Visible;
                }
            }
        }

        private string MakeTextBasedOnDate(List<WorkEvent> list)
        {
            if (list.Count == 0)
            {
                return "There were no services made for this owner.";
            }

            Dictionary<int, House> Houses = new Dictionary<int, House>();
            for (int i = 0; i < list.Count; i++)
            {
                if (!Houses.ContainsKey(list[i].IdHouse))
                    Houses[list[i].IdHouse] = App.DB.Houses.FirstOrDefault(h => h.IdHouse == list[i].IdHouse);
            }
            var Owner = App.DB.GetOwnerOfHouse(App.DB.Houses.FirstOrDefault(h => h.IdHouse == list[0].IdHouse));
            CurrentOwner = Owner;
            CurrentDateTime = list[0].Time;
            StringBuilder Resp = new StringBuilder();
            Resp.Append("" + Owner.Name + ":\n");

            int SumMoneyUnits = 0;
            int SumMoneyCents = 0;

            for (int i = 0; i < list.Count; i++)
            {
                House House = Houses[list[i].IdHouse];
                
                Resp.Append(list[i].Time.ToString("dd/MM/yyyy HH:mm") + " at " + House.Name + " ->");
                string Prefix = "";
                int Count = 0;
                if (list[i].CheckInMoneyUnits > 0 || list[i].CheckInMoneyCents > 0)
                {
                    Resp.Append(Prefix + " CheckIn: " + FormatToMoney(list[i].CheckInMoneyUnits, list[i].CheckInMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (list[i].CleaningMoneyUnits > 0 || list[i].CleaningMoneyCents > 0)
                {
                    Resp.Append(Prefix + " Cleaning: " + FormatToMoney(list[i].CleaningMoneyUnits, list[i].CleaningMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (list[i].LaundryMoneyUnits > 0 || list[i].LaundryMoneyCents > 0)
                {
                    Resp.Append(Prefix + " Laundry: " + FormatToMoney(list[i].LaundryMoneyUnits, list[i].LaundryMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (list[i].StairsMoneyUnits > 0 || list[i].StairsMoneyCents > 0)
                {
                    Resp.Append(Prefix + " Stairs: " + FormatToMoney(list[i].LaundryMoneyUnits, list[i].LaundryMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (list[i].ConstructionCleaningMoneyUnits > 0 || list[i].ConstructionCleaningMoneyCents > 0)
                {
                    Resp.Append(Prefix + " Construction Cleaning: " + FormatToMoney(list[i].ConstructionCleaningMoneyUnits, list[i].ConstructionCleaningMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (Count > 1)
                {
                    SumMoneyUnits = list[i].SumUnits();
                    SumMoneyCents = list[i].SumCents();
                    SumMoneyUnits += SumMoneyCents / 100;
                    SumMoneyCents = SumMoneyCents % 100;
                    Resp.Append(" -> Total: " + FormatToMoney(SumMoneyUnits, SumMoneyCents, '€'));
                }
                if (list[i].Note.Trim() != "")
                    Resp.Append("\nNote: " + list[i].Note);
                Resp.Append("\n");
            }
            SumMoneyUnits = list.Sum(we => we.SumUnits());
            SumMoneyCents = list.Sum(we => we.SumCents());
            SumMoneyUnits += SumMoneyCents / 100;
            SumMoneyCents = SumMoneyCents % 100;
            Resp.Append("\nTotal: " + FormatToMoney(SumMoneyUnits, SumMoneyCents, '€'));

            return Resp.ToString();
        }

        private string MakeTextBasedOnHouses(List<WorkEvent> list)
        {
            var Owner = App.DB.GetOwnerOfHouse(App.DB.Houses.FirstOrDefault(h => h.IdHouse == list[0].IdHouse));
            StringBuilder Resp = new StringBuilder();
            Resp.Append("" + Owner.Name + ",\n");

            int SumMoneyUnits = 0;
            int SumMoneyCents = 0;

            Resp.Append("Houses:\n");
            bool IsFirstTimeHouse = true;
            string HouseId = "";
            for (int i = 0; i < list.Count; i++)
            {
                if (HouseId != list[i].IdHouse.ToString())
                {
                    IsFirstTimeHouse = true;
                    HouseId = list[i].IdHouse.ToString();
                }
                if (IsFirstTimeHouse)
                {
                    IsFirstTimeHouse = false;
                    var House = App.DB.Houses.FirstOrDefault(h => h.IdHouse == list[i].IdHouse);
                    Resp.Append(House.Name + " ");
                }
                Resp.Append(list[i].Time.ToString("dd/MM/yyyy HH:mm") + " ->");
                string Prefix = "";
                int Count = 0;
                if (list[i].CheckInMoneyUnits > 0 || list[i].CheckInMoneyCents > 0)
                {
                    Resp.Append(Prefix + " CheckIn: " + FormatToMoney(list[i].CheckInMoneyUnits, list[i].CheckInMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (list[i].CleaningMoneyUnits > 0 || list[i].CleaningMoneyCents > 0)
                {
                    Resp.Append(Prefix + " Cleaning: " + FormatToMoney(list[i].CleaningMoneyUnits, list[i].CleaningMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (list[i].LaundryMoneyUnits > 0 || list[i].LaundryMoneyCents > 0)
                {
                    Resp.Append(Prefix + " Laundry: " + FormatToMoney(list[i].LaundryMoneyUnits, list[i].LaundryMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (list[i].StairsMoneyUnits > 0 || list[i].StairsMoneyCents > 0)
                {
                    Resp.Append(Prefix + " Stairs: " + FormatToMoney(list[i].LaundryMoneyUnits, list[i].LaundryMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (list[i].ConstructionCleaningMoneyUnits > 0 || list[i].ConstructionCleaningMoneyCents > 0)
                {
                    Resp.Append(Prefix + " Construction Cleaning: " + FormatToMoney(list[i].ConstructionCleaningMoneyUnits, list[i].ConstructionCleaningMoneyCents, '€'));
                    Prefix = ",";
                    Count++;
                }
                if (Count > 1)
                {
                    SumMoneyUnits = list[i].SumUnits();
                    SumMoneyCents = list[i].SumCents();
                    SumMoneyUnits += SumMoneyCents / 100;
                    SumMoneyCents = SumMoneyCents % 100;
                    Resp.Append(" -> Total: " + FormatToMoney(SumMoneyUnits, SumMoneyCents, '€'));
                }
                if (list[i].Note.Trim() != "")
                    Resp.Append("\nNote: " + list[i].Note);
                Resp.Append("\n");
            }
            SumMoneyUnits = list.Sum(we => we.SumUnits());
            SumMoneyCents = list.Sum(we => we.SumCents());
            SumMoneyUnits += SumMoneyCents / 100;
            SumMoneyCents = SumMoneyCents % 100;
            Resp.Append("\nTotal: " + FormatToMoney(SumMoneyUnits, SumMoneyCents, '€'));
            return Resp.ToString();
        }

        private string FormatToMoney(int moneyUnits, int moneyCents, char delimiter)
        {
            string Cents = moneyCents.ToString();
            if (Cents.Length == 1)
                Cents = "0" + Cents;
            else if (Cents.Length == 0)
                Cents = "00";
            return moneyUnits.ToString() + delimiter + Cents;
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void ButtonSendEmail_Click(object sender, RoutedEventArgs e)
        {
            string subject = "[Accounting] " + CurrentDateTime.ToString("MM/yyyy");
            string body = MyUrlEncode(TextBoxSummary.Text);
            var mailto = new Uri("mailto:?to=" + CurrentOwner.Email + "&subject=" + subject + "&body=" + body);
            await Windows.System.Launcher.LaunchUriAsync(mailto);
        }
        public static string MyUrlEncode(string value)
        {
            // Temporarily replace spaces with the literal -SPACE-
            string url = value.Replace(" ", "-SPACE-");
            url = WebUtility.UrlEncode(url);

            // Some servers have issues with ( and ), but UrlEncode doesn't 
            // affect them, so we include those in the encoding as well.
            return url.Replace("-SPACE-", "%20").Replace("(", "%28").Replace(")", "%29");
        }
    }
}
