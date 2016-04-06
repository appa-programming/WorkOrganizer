using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WorkOrganizer.NavigationObjects;
using WorkOrganizer.Specs;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrganizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            var UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();
            if (!App.DB.IsLoaded)
                Task.Run(() => App.DB.Load()).ContinueWith(tsk => AddWorkEventBars(), UISyncContext);
            else
                AddWorkEventBars();
            /*if (App.DB != null)
                TextBoxTest.Text = "" + App.DB.Houses.Count + App.DB.Owners.Count + App.DB.WorkEvents.Count;*/
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.ToString() != "")
            {
                var Dt = (DateTime)e.Parameter;
                DatePickerSelect.Date = Dt;
            }
        }

        private void AddWorkEventBars()
        {
            ProgressRing.Visibility = Visibility.Collapsed;
            ScrollWorkEvents.Visibility = Visibility.Visible;

            StackWorkEvents.Children.Clear();

            foreach (var we in App.DB.WorkEvents.FindAll(w => w.Time.Day == DatePickerSelect.Date.Day))
            {
                Grid Grid = new Grid();

                Grid.Tag = we.Id;
                Grid.Height = 80;

                Style Style = new Style { TargetType = typeof(Border) };
                Style.Setters.Add(new Setter(Border.BorderBrushProperty, "Green"));
                Style.Setters.Add(new Setter(Border.BorderThicknessProperty, "1"));
                Style.Setters.Add(new Setter(Border.PaddingProperty, "2"));
                Grid.Resources.Add("BorderStyle", Style);

                ColumnDefinition cd0 = new ColumnDefinition();
                cd0.Width = new GridLength(1, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd0);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(2, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd1);
                ColumnDefinition cd2 = new ColumnDefinition();
                cd2.Width = new GridLength(4, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd2);
                ColumnDefinition cd3 = new ColumnDefinition();
                cd3.Width = new GridLength(1, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd3);

                /*<Border Style="{StaticResource borderStyle}" Grid.Row="0" Grid.Column="0">*/
                Border b0 = new Border();
                b0.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock Time = new TextBlock();
                Time.Text = we.Time.Hour + ":" + String.Format("{0:00}", we.Time.Minute);
                Time.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Time.TextAlignment = TextAlignment.Center;
                Time.FontWeight = FontWeights.Bold;
                b0.Child = Time;

                b0.SetValue(Grid.ColumnProperty, 0);
                Grid.Children.Add(b0);

                Border b1 = new Border();
                b1.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock Location = new TextBlock();
                Location.Text = App.DB.Houses.FirstOrDefault(h => h.IdHouse == we.IdHouse).Name;
                Location.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Location.TextAlignment = TextAlignment.Center;
                b1.Child = Location;

                b1.SetValue(Grid.ColumnProperty, 1);
                Grid.Children.Add(b1);

                Border b2 = new Border();
                b2.Style = Grid.Resources["BorderStyle"] as Style;
                TextBlock Note = new TextBlock();
                Note.Text = we.Note;
                Note.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Note.TextAlignment = TextAlignment.Center;
                b2.Child = Note;

                b2.SetValue(Grid.ColumnProperty, 2);
                Grid.Children.Add(b2);

                Border b3 = new Border();
                b3.Style = Grid.Resources["BorderStyle"] as Style;
                TextBlock Money = new TextBlock();
                Money.Text = we.MoneyUnits + " € " + String.Format("{0:00}", we.MoneyCents);
                Money.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Money.Foreground = new SolidColorBrush(Colors.Green);
                Money.TextAlignment = TextAlignment.Right;
                b3.Child = Money;

                b3.SetValue(Grid.ColumnProperty, 3);
                Grid.Children.Add(b3);

                Grid.Tapped += WorkEvent_Tap;
                StackWorkEvents.Children.Add(Grid);
            }
        }

        private void WorkEvent_Tap(object sender, RoutedEventArgs e)
        {
            var we = App.DB.WorkEvents.FirstOrDefault(w => w.Id == ((Guid)((Grid)sender).Tag));
            Frame.Navigate(typeof(WorkEventPage),
                new WorkEventPageNavigation(DatePickerSelect.Date.DateTime, we));
        }

        private void ButtonAddEvent_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WorkEventPage),
                new WorkEventPageNavigation(DatePickerSelect.Date.DateTime, null));
        }

        private async void DatePickerSelect_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            if (App.DB.WorkEventsDate.Month != DatePickerSelect.Date.DateTime.Month ||
                App.DB.WorkEventsDate.Year != DatePickerSelect.Date.DateTime.Year)
            {
                App.DB.WorkEventsDate = DatePickerSelect.Date.DateTime;
                await App.DB.LoadWorkEvents();
            }
            AddWorkEventBars();
        }

        private void SwipeToMonth_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WorkAnalyzerPage), DatePickerSelect.Date.DateTime);
        }
    }
}
