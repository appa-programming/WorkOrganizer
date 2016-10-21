using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkOrganizer.NavigationObjects;
using WorkOrganizer.Specs;
using static MoneyLib.MoneyCalculator;
using MoneyLib;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrganizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WorkEventPage : Page
    {
        public WorkEvent WorkEventOnEdit { get; private set; }
        public bool IsEdit { get; private set; }

        public ObservableCollection<House> ComboDataHouses { get; set; }
        public ObservableCollection<ComboData> ComboDataCheckIn { get; set; }
        public ObservableCollection<ComboData> ComboDataStairs { get; set; }
        public ObservableCollection<ComboData> ComboDataCleaning { get; set; }
        public ObservableCollection<ComboData> ComboDataConstructionCleaning { get; set; }
        public string LaundryPerKg { get; set; }

        public WorkEventPage()
        {
            this.InitializeComponent();
            ComboDataHouses = new ObservableCollection<House>( App.DB.ActiveHouses );
            ComboHouses.ItemsSource = ComboDataHouses;

            var Configs = App.DB.Configs[0];
            ComboDataCheckIn = GetDataForCombo(Configs.CheckInValues);
            ComboCheckIn.ItemsSource = ComboDataCheckIn;
            ComboCheckIn.SelectedIndex = 0;

            ComboDataStairs = GetDataForCombo(Configs.Stairs);
            ComboStairs.ItemsSource = ComboDataStairs;
            ComboStairs.SelectedIndex = 0;

            ComboDataCleaning = GetDataForCombo(Configs.Cleaning);
            ComboCleaning.ItemsSource = ComboDataCleaning;
            ComboCleaning.SelectedIndex = 0;

            ComboDataConstructionCleaning = GetDataForCombo(Configs.ConstructionCleaning);
            ComboConstructionCleaning.ItemsSource = ComboDataConstructionCleaning;
            ComboConstructionCleaning.SelectedIndex = 0;

            LaundryPerKg = "0";

            WorkEventOnEdit = null;
        }

        private ObservableCollection<ComboData> GetDataForCombo(List<string> values)
        {
            ObservableCollection<ComboData> ListData = new ObservableCollection<ComboData>();
            for (int i = 0; i < values.Count; i++)
            {
                ListData.Add(new ComboData { IndexValue = i, Value = values[i] });
            }
            return ListData;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var nav = (WorkEventPageNavigation)e.Parameter;
            DatePicker.Date = nav.Date;
            WorkEventOnEdit = nav.WorkEvent;
            IsEdit = (WorkEventOnEdit != null);
            if (IsEdit)
            {
                var Configs = App.DB.Configs[0];
                ButtonCreateOrEditEvent.Content = "Edit Event";
                DatePicker.Date = WorkEventOnEdit.Time;
                TimeSpan ts = new TimeSpan(WorkEventOnEdit.Time.Hour, WorkEventOnEdit.Time.Minute, 0);
                TimePicker.Time = ts;
                House WorkEventHouse = null;

                bool HasItem = false;
                foreach (House item in ComboHouses.Items)
                {
                    if (item.IdHouse == WorkEventOnEdit.IdHouse)
                    {
                        HasItem = true;
                        WorkEventHouse = App.DB.Houses.First(h => h.IdHouse == WorkEventOnEdit.IdHouse);
                    }
                }
                if (!HasItem)
                {
                    WorkEventHouse = App.DB.Houses.First(h => h.IdHouse == WorkEventOnEdit.IdHouse);
                    ComboDataHouses.Add(WorkEventHouse);
                }

                ComboHouses.SelectedValue = WorkEventOnEdit.IdHouse;
                
                TextBoxNotes.Text = WorkEventOnEdit.Note;
                
                FillCombo(ComboDataCheckIn, ComboCheckIn, Configs.CheckInValues, WorkEventOnEdit.CheckInMoneyUnits, WorkEventOnEdit.CheckInMoneyCents);
                FillCombo(ComboDataCleaning, ComboCleaning, Configs.Cleaning, WorkEventOnEdit.CleaningMoneyUnits, WorkEventOnEdit.CleaningMoneyCents);
                FillCombo(ComboDataStairs, ComboStairs, Configs.Stairs, WorkEventOnEdit.StairsMoneyUnits, WorkEventOnEdit.StairsMoneyCents);
                FillCombo(ComboDataConstructionCleaning, ComboConstructionCleaning, Configs.ConstructionCleaning, WorkEventOnEdit.ConstructionCleaningMoneyUnits, WorkEventOnEdit.ConstructionCleaningMoneyCents);

                TextBoxLaundry.Text = WorkEventOnEdit.LaundryKgs.ToString();
                TextBoxMoneyUnits.Text = WorkEventOnEdit.LaundryMoneyUnits.ToString();
                TextBoxMoneyCents.Text = WorkEventOnEdit.LaundryMoneyCents.ToString();

                LaundryPerKg = WorkEventOnEdit.LaundryEuroPerKilo;
                if (LaundryPerKg != LoadLaundryPrice(WorkEventHouse))
                {
                    ButtonUpdateLaundry.Visibility = Visibility.Visible;
                }
            }
            else
                ButtonCreateOrEditEvent.Content = "Create Event";
        }

        private string LoadLaundryPrice(House workEventHouse)
        {
            var Owner = App.DB.GetOwnerOfHouse(workEventHouse);
            return Owner.Laundry;
        }

        private void FillCombo(ObservableCollection<ComboData> comboData, ComboBox comboBox,
                                            List<string> existingOptions, int units, int cents)
        {
            string value = GenerateMoneyString(units, cents, MoneyFormat.UU_MM_CC, "€");
            if (existingOptions.FirstOrDefault(v => IsItTheSameAmmount(v, value)) != null)
                comboBox.SelectedValue = existingOptions.FirstOrDefault(v => IsItTheSameAmmount(v, value));
            else
            {
                comboData.Add(new ComboData() { IndexValue = existingOptions.Count, Value = value });
                comboBox.SelectedIndex = existingOptions.Count;
            }
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void ButtonCreateOrEditEvent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Response = GetDataValidation();
                if (Response == "OK")
                {
                    DateTime Time = new DateTime(DatePicker.Date.DateTime.Year,
                                                DatePicker.Date.DateTime.Month,
                                                DatePicker.Date.DateTime.Day,
                                                TimePicker.Time.Hours,
                                                TimePicker.Time.Minutes, 0);
                    Tuple<int, int> LaundryMoney = CalculateLaundryMoney(LaundryPerKg, '€', TextBoxLaundry.Text, ',');
                    Money CheckInMoney = new Money(ComboCheckIn.SelectedValue.ToString());
                    Money StairsMoney = new Money(ComboStairs.SelectedValue.ToString());
                    Money CleaningMoney = new Money(ComboCleaning.SelectedValue.ToString());
                    Money ConstructionCleaningMoney = new Money(ComboConstructionCleaning.SelectedValue.ToString());
                    WorkEvent we = new WorkEvent(Time,
                                                int.Parse(ComboHouses.SelectedValue.ToString()),
                                                TextBoxNotes.Text,
                                                CheckInMoney.MoneyUnits,
                                                CheckInMoney.MoneyCents,
                                                StairsMoney.MoneyUnits,
                                                StairsMoney.MoneyCents,
                                                CleaningMoney.MoneyUnits,
                                                CleaningMoney.MoneyCents,
                                                ConstructionCleaningMoney.MoneyUnits,
                                                ConstructionCleaningMoney.MoneyCents,
                                                LaundryMoney.Item1,
                                                LaundryMoney.Item2,
                                                LaundryPerKg,
                                                GetDecimal(TextBoxLaundry.Text));
                    if (IsEdit)
                        App.DB.EditWorkEvent(WorkEventOnEdit.Id, we);
                    else
                        await App.DB.SaveSpecificWorkEvent(we);

                    TextError.Text = "";
                    TextError.Visibility = Visibility.Collapsed;

                    Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
                    Frame.Navigate(typeof(MainPage), DatePicker.Date.DateTime);
                }
                else
                {
                    TextError.Text = Response;
                    TextError.Inlines.Add(new Run { Text = " Sweetie", Foreground = new SolidColorBrush(Colors.HotPink) });
                    TextError.Inlines.Add(new Run { Text = "." });
                    TextError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private Tuple<int, int> CalculateLaundryMoney(string value1, char delimiter1,
                                                        string value2, char delimiter2)
        {
            Money M1 = new Money(value1);
            Money M2 = new Money(value2);
            Money Result = Multiply(M1, M2);
            return new Tuple<int, int>(Result.MoneyUnits, Result.MoneyCents);
        }

        private string GetDataValidation()
        {
            DateTime DtAux = DatePicker.Date.DateTime;
            DtAux = DtAux.Date + TimePicker.Time;
            /*if (DtAux <= DateTime.Today)
                return "You can't sent events for the past";*/
            if (GetDecimal(TextBoxLaundry.Text) == "ERROR")
                return "That is not a number";
            else if (ComboHouses.SelectedIndex == -1)
                return "You have to select a house";
            else
                return "OK";
        }

        private string GetDecimal(string text)
        {
            string Content = text;
            string Pattern = @"(\d+)";
            Match Result1 = Regex.Match(Content, Pattern);
            Pattern = @"((\d+)\.(\d+))";
            Match Result2 = Regex.Match(Content, Pattern);
            Pattern = @"((\d+)\,(\d+))";
            Match Result3 = Regex.Match(Content, Pattern);
            Pattern = @"(\.(\d+))";
            Match Result4 = Regex.Match(Content, Pattern);
            Pattern = @"(\,(\d+))";
            Match Result5 = Regex.Match(Content, Pattern);
            Pattern = @"((\d+)\.)";
            Match Result6 = Regex.Match(Content, Pattern);
            Pattern = @"((\d+)\,)";
            Match Result7 = Regex.Match(Content, Pattern);
            if (Result2.Success || Result3.Success || Result4.Success ||
                Result5.Success || Result6.Success || Result7.Success)
            {
                if (Result2.Success || Result4.Success || Result6.Success)
                    Content = Content.Replace('.', ',');
                Content = TreatContent(Content);
                /*if (Content == "ERROR")
                {
                }*/
            }
            else if (Content == "")
            {
                Content = "0";
            }
            else if (!Result1.Success)
            {
                Content = "ERROR";
            }
            return Content;
        }
        private string TreatContent(string content)
        {
            string Resp = content;
            string[] Strs = Resp.Split(',');
            if (Strs[0].Length == 0)
            {
                Resp = "0" + Resp;
            }
            if (int.Parse(Strs[1]) == 0)
            {
                Resp = Strs[0];
            }
            return Resp;
        }

        private void TextBoxLaundry_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (GetDecimal(tb.Text) != "ERROR")
            {
                var t = CalculateLaundryMoney(LaundryPerKg, '€', GetDecimal(tb.Text), ',');
                TextBoxMoneyUnits.Text = t.Item1.ToString();
                TextBoxMoneyCents.Text = t.Item2.ToString();
            }
        }

        private void ButtonUpdateLaundry_Click(object sender, RoutedEventArgs e)
        {
            LaundryPerKg = LoadLaundryPrice((House)ComboHouses.SelectedItem);
            var t = CalculateLaundryMoney(LaundryPerKg, '€', GetDecimal(TextBoxLaundry.Text), ',');
            TextBoxMoneyUnits.Text = t.Item1.ToString();
            TextBoxMoneyCents.Text = t.Item2.ToString();
            ButtonUpdateLaundry.Visibility = Visibility.Collapsed;
        }

        private void ComboHouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxLaundry.IsEnabled = true;
            var House = (sender as ComboBox).SelectedItem as House;
            LaundryPerKg = App.DB.GetOwnerOfHouse(House).Laundry;

            if (TextBoxLaundry.Text != "")
            {
                if (GetDecimal(TextBoxLaundry.Text) != "ERROR")
                {
                    var t = CalculateLaundryMoney(LaundryPerKg, '€', GetDecimal(TextBoxLaundry.Text), ',');
                    TextBoxMoneyUnits.Text = t.Item1.ToString();
                    TextBoxMoneyCents.Text = t.Item2.ToString();
                }
            }
        }
    }

    public class ComboData
    {
        public int IndexValue { get; set; }
        public string Value { get; set; }
    }
}
