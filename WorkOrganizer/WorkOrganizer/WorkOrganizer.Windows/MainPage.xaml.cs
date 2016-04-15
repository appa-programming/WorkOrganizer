using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WorkOrganizer.NavigationObjects;

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
            /*if (App.DB != null)
                TextBoxTest.Text = "" + App.DB.Houses.Count + App.DB.Owners.Count + App.DB.WorkEvents.Count;*/
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.ToString() != "")
            {
                var Dt = (DateTime)e.Parameter;
                DatePickerSelect.Date = Dt;
                if (App.DB.IsLoaded)
                    AddWorkEventBars();
            }
        }

        private void AddWorkEventBars()
        {
            ProgressRing.Visibility = Visibility.Collapsed;
            ScrollWorkEvents.Visibility = Visibility.Visible;

            StackWorkEvents.Children.Clear();

            foreach (var we in App.DB.WorkEvents.FindAll(w => w.Time.Day == DatePickerSelect.Date.Day))
            {
                Grid MainGrid = new Grid();
                MainGrid.Name = "MainGrid" + we.Id;

                ColumnDefinition maincd0 = new ColumnDefinition();
                maincd0.Width = new GridLength(6, GridUnitType.Star);
                MainGrid.ColumnDefinitions.Add(maincd0);
                ColumnDefinition maincd1 = new ColumnDefinition();
                maincd1.Width = new GridLength(1, GridUnitType.Star);
                MainGrid.ColumnDefinitions.Add(maincd1);

                Grid LeftGrid = new Grid();

                LeftGrid.Tag = we.Id;
                LeftGrid.Height = 80;

                Style Style = new Style { TargetType = typeof(Border) };
                Style.Setters.Add(new Setter(Border.BorderBrushProperty, "Green"));
                Style.Setters.Add(new Setter(Border.BorderThicknessProperty, "1"));
                Style.Setters.Add(new Setter(Border.PaddingProperty, "2"));
                LeftGrid.Resources.Add("BorderStyle", Style);

                ColumnDefinition cd0 = new ColumnDefinition();
                cd0.Width = new GridLength(1, GridUnitType.Star);
                LeftGrid.ColumnDefinitions.Add(cd0);
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(2, GridUnitType.Star);
                LeftGrid.ColumnDefinitions.Add(cd1);
                ColumnDefinition cd2 = new ColumnDefinition();
                cd2.Width = new GridLength(4, GridUnitType.Star);
                LeftGrid.ColumnDefinitions.Add(cd2);
                ColumnDefinition cd3 = new ColumnDefinition();
                cd3.Width = new GridLength(1, GridUnitType.Star);
                LeftGrid.ColumnDefinitions.Add(cd3);

                /*<Border Style="{StaticResource borderStyle}" LeftGrid.Row="0" LeftGrid.Column="0">*/
                Border b0 = new Border();
                b0.Style = LeftGrid.Resources["BorderStyle"] as Style;

                TextBlock Time = new TextBlock();
                Time.Text = we.Time.Hour + ":" + String.Format("{0:00}", we.Time.Minute);
                Time.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Time.TextAlignment = TextAlignment.Center;
                Time.FontWeight = FontWeights.Bold;
                b0.Child = Time;

                b0.SetValue(Grid.ColumnProperty, 0);
                LeftGrid.Children.Add(b0);

                Border b1 = new Border();
                b1.Style = LeftGrid.Resources["BorderStyle"] as Style;

                TextBlock Location = new TextBlock();
                Location.Text = App.DB.Houses.FirstOrDefault(h => h.IdHouse == we.IdHouse).Name;
                Location.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Location.TextAlignment = TextAlignment.Center;
                b1.Child = Location;

                b1.SetValue(Grid.ColumnProperty, 1);
                LeftGrid.Children.Add(b1);

                Border b2 = new Border();
                b2.Style = LeftGrid.Resources["BorderStyle"] as Style;
                TextBlock Note = new TextBlock();
                Note.Text = we.Note;
                Note.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Note.TextAlignment = TextAlignment.Center;
                b2.Child = Note;

                b2.SetValue(Grid.ColumnProperty, 2);
                LeftGrid.Children.Add(b2);

                Border b3 = new Border();
                b3.Style = LeftGrid.Resources["BorderStyle"] as Style;
                TextBlock Money = new TextBlock();
                int Units = we.SumUnits();
                int Cents = we.SumCents();
                Units += Cents / 100;
                Cents = Cents % 100;
                Money.Text = Units + " € " + String.Format("{0:00}", Cents);
                Money.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Money.Foreground = new SolidColorBrush(Colors.Green);
                Money.TextAlignment = TextAlignment.Right;
                b3.Child = Money;

                b3.SetValue(Grid.ColumnProperty, 3);
                LeftGrid.Children.Add(b3);

                LeftGrid.Tapped += WorkEvent_Tap;

                //------------------------------------------------

                Border b4 = new Border();
                b4.Style = LeftGrid.Resources["BorderStyle"] as Style;

                Button Delete = new Button();
                Delete.HorizontalAlignment = HorizontalAlignment.Center;
                Delete.VerticalAlignment = VerticalAlignment.Center;
                Delete.Width = 32;
                Delete.Height = 32;
                Delete.Tag = we.Id;
                string Template = "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                                        "<Image Source=\"/Assets/Minus.png\" />" +
                                    "</ControlTemplate>";
                Delete.Template = (ControlTemplate)XamlReader.Load(Template);
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets//Minus.png"));
                Delete.Content = img;
                Delete.Click += Delete_Click;

                b4.Child = Delete;

                LeftGrid.SetValue(Grid.ColumnProperty, 0);
                b4.SetValue(Grid.ColumnProperty, 1);

                MainGrid.Children.Add(LeftGrid);
                MainGrid.Children.Add(b4);

                StackWorkEvents.Children.Add(MainGrid);
            }
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var Btn = sender as Button;
            var WorkEvent = App.DB.WorkEvents.Find(w => w.Id.ToString() == Btn.Tag.ToString());
            var MsgDialog = new MessageDialog("Are you sure you want to remove this work event from your list ?");

            MsgDialog.Commands.Add(new UICommand("Yes", null, "YES"));
            MsgDialog.Commands.Add(new UICommand("No", null, "NO"));
            var op = await MsgDialog.ShowAsync();
            if ((string)op.Id == "YES")
            {
                var Msg = await App.DB.RemoveWorkEvent(WorkEvent.Id);
                if (!Msg)
                {
                    // TODO ERROR
                }
                else
                {
                    var Grid = (Grid)FindName("MainGrid" + WorkEvent.Id);
                    StackWorkEvents.Children.Remove(Grid);
                }
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

        private void Configs_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ConfigsPage));
        }
    }
}
