using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkOrganizer.Specs;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrganizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WorkAnalyzerPage : Page
    {
        bool IsFullyInitialized;
        List<WorkEvent> WorkEventsInTheHousesThisMonth;

        public WorkAnalyzerPage()
        {
            IsFullyInitialized = false;
            this.InitializeComponent();
            IsFullyInitialized = true;
            var UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();
            if (!App.DB.IsLoaded)
                Task.Run(() => App.DB.Load()).ContinueWith(tsk => RemakeList(), UISyncContext);
            else
                RemakeList();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var Dt = (DateTime)e.Parameter;
            DatePickerMonthYear.Date = Dt;
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), DatePickerMonthYear.Date.Date);
        }

        private async void DatePickerMonthYear_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            if (IsFullyInitialized)
            {
                int day = App.DB.WorkEventsDate.Day;
                App.DB.WorkEventsDate = new DateTime(DatePickerMonthYear.Date.Year,
                                                    DatePickerMonthYear.Date.Month,
                                                    day);

                ProgressRing.Visibility = Visibility.Visible;
                ScrollSummary.Visibility = Visibility.Collapsed;

                await App.DB.LoadWorkEvents();
                RemakeList();
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (IsFullyInitialized)
                RemakeList();
        }

        private void RemakeList()
        {
            ProgressRing.Visibility = Visibility.Collapsed;
            ScrollSummary.Visibility = Visibility.Visible;

            StackSummary.Children.Clear();

            if (RadioButtonOwner.IsChecked == true)
            {
                MakeOwnerList();
            }
            else if (RadioButtonHouse.IsChecked == true)
            {
                MakeHouseList();
            }
            else
            {
                // Never happens, trust me I'm an engineer :P
                throw (new Exception());
            }
        }

        private void MakeOwnerList()
        {
            List<Tuple<Owner, int, int>> DataNeededForPresentation = new List<Tuple<Owner, int, int>>();
            foreach (var owner in App.DB.Owners)
            {
                List<House> OwnersHouses = App.DB.GetOwnersHouses(owner.IdOwner);
                WorkEventsInTheHousesThisMonth = App.DB.GetWorkEventsInTheHousesThisMonth(OwnersHouses);

                if (WorkEventsInTheHousesThisMonth.Count == 0 && owner.IsInvisible)
                {
                    continue;
                }
                DataNeededForPresentation.Add(PrepareOwnerDataForPresentation(owner, WorkEventsInTheHousesThisMonth));
            }
            DataNeededForPresentation = DataNeededForPresentation.OrderByDescending(d => d.Item2).ThenByDescending(d => d.Item3).ToList();
            foreach (var tuple in DataNeededForPresentation)
            {
                Grid Grid = new Grid();

                Grid.Tag = tuple.Item1.IdOwner;
                Grid.Height = 80;

                Style Style = new Style { TargetType = typeof(Border) };
                Style.Setters.Add(new Setter(Border.BorderBrushProperty, "Green"));
                Style.Setters.Add(new Setter(Border.BorderThicknessProperty, "1"));
                Style.Setters.Add(new Setter(Border.PaddingProperty, "2"));
                Grid.Resources.Add("BorderStyle", Style);

                ColumnDefinition cd0 = new ColumnDefinition();
                cd0.Width = new GridLength(6, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd0);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd1);

                Border b0 = new Border();
                b0.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock Name = new TextBlock();
                Name.Text = tuple.Item1.Name;
                Name.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Name.TextAlignment = TextAlignment.Center;
                Name.FontWeight = FontWeights.Bold;
                b0.Child = Name;

                b0.SetValue(Grid.ColumnProperty, 0);
                Grid.Children.Add(b0);

                Border b1 = new Border();
                b1.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock Money = new TextBlock();
                Money.Text = tuple.Item2 + " € " + String.Format("{0:00}", tuple.Item3);
                Money.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Money.TextAlignment = TextAlignment.Right;
                b1.Child = Money;

                b1.SetValue(Grid.ColumnProperty, 1);
                Grid.Children.Add(b1);

                StackSummary.Children.Add(Grid);
            }
        }
        private Tuple<Owner, int, int> PrepareOwnerDataForPresentation(Owner owner, List<WorkEvent> workEventsInTheHousesThisMonth)
        {
            if (workEventsInTheHousesThisMonth == null || workEventsInTheHousesThisMonth.Count == 0)
                return new Tuple<Owner, int, int>(owner, 0, 0);
            else
            {
                int SumMoneyUnits = workEventsInTheHousesThisMonth.Sum(we => we.MoneyUnits);
                int SumMoneyCents = workEventsInTheHousesThisMonth.Sum(we => we.MoneyCents);
                SumMoneyUnits += SumMoneyCents / 100;
                SumMoneyCents = SumMoneyCents % 100;
                return new Tuple<Owner, int, int>(owner, SumMoneyUnits, SumMoneyCents);
            }
        }

        private void MakeHouseList()
        {
            List<Tuple<House, string, int, int>> DataNeededForPresentation = new List<Tuple<House, string, int, int>>();
            foreach (var house in App.DB.Houses)
            {
                List<WorkEvent> WorkEventsInTheHouseThisMonth = App.DB.GetWorkEventsInTheHouseThisMonth(house).ToList();

                if (WorkEventsInTheHouseThisMonth.Count == 0 && house.IsInvisible)
                {
                    continue;
                }

                DataNeededForPresentation.Add(PrepareHouseDataForPresentation(house, WorkEventsInTheHouseThisMonth));
            }
            DataNeededForPresentation = DataNeededForPresentation.OrderByDescending(d => d.Item3).ThenByDescending(d => d.Item4).ToList();
            foreach (var tuple in DataNeededForPresentation)
            {
                Grid Grid = new Grid();

                Grid.Tag = tuple.Item1.IdOwner;
                Grid.Height = 80;

                Style Style = new Style { TargetType = typeof(Border) };
                Style.Setters.Add(new Setter(Border.BorderBrushProperty, "Green"));
                Style.Setters.Add(new Setter(Border.BorderThicknessProperty, "1"));
                Style.Setters.Add(new Setter(Border.PaddingProperty, "2"));
                Grid.Resources.Add("BorderStyle", Style);

                ColumnDefinition cd0 = new ColumnDefinition();
                cd0.Width = new GridLength(4, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd0);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(2, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd1);
                ColumnDefinition cd2 = new ColumnDefinition();
                cd2.Width = new GridLength(1, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd2);

                Border b0 = new Border();
                b0.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock HouseName = new TextBlock();
                HouseName.Text = tuple.Item1.Name;
                HouseName.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                HouseName.TextAlignment = TextAlignment.Center;
                HouseName.FontWeight = FontWeights.Bold;
                b0.Child = HouseName;

                b0.SetValue(Grid.ColumnProperty, 0);
                Grid.Children.Add(b0);

                Border b1 = new Border();
                b1.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock OwnerName = new TextBlock();
                OwnerName.Text = tuple.Item2;
                OwnerName.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                OwnerName.TextAlignment = TextAlignment.Center;
                b1.Child = OwnerName;

                b1.SetValue(Grid.ColumnProperty, 1);
                Grid.Children.Add(b1);

                Border b2 = new Border();
                b2.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock Money = new TextBlock();
                Money.Text = tuple.Item3 + " € " + String.Format("{0:00}", tuple.Item4);
                Money.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Money.TextAlignment = TextAlignment.Right;
                b2.Child = Money;

                b2.SetValue(Grid.ColumnProperty, 2);
                Grid.Children.Add(b2);

                StackSummary.Children.Add(Grid);
            }
        }

        private Tuple<House, string, int, int> PrepareHouseDataForPresentation(House house, List<WorkEvent> workEventsInTheHouseThisMonth)
        {
            if (workEventsInTheHouseThisMonth == null || workEventsInTheHouseThisMonth.Count == 0)
                return new Tuple<House, string, int, int>(house, App.DB.GetOwnerOfHouse(house).Name, 0, 0);
            else
            {
                int SumMoneyUnits = workEventsInTheHouseThisMonth.Sum(we => we.MoneyUnits);
                int SumMoneyCents = workEventsInTheHouseThisMonth.Sum(we => we.MoneyCents);
                SumMoneyUnits += SumMoneyCents / 100;
                SumMoneyCents = SumMoneyCents % 100;
                return new Tuple<House, string, int, int>(house, App.DB.GetOwnerOfHouse(house).Name, SumMoneyUnits, SumMoneyCents);
            }
        }

        private void SwipeToManagement_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HouseManagementPage));
        }
    }
}
