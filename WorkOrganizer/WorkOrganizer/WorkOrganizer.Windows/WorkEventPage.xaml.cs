using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
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
    public sealed partial class WorkEventPage : Page
    {
        public WorkEvent WorkEventOnEdit { get; private set; }
        public bool IsEdit { get; private set; }

        public WorkEventPage()
        {
            this.InitializeComponent();
            ComboHouses.ItemsSource = App.DB.Houses;
            WorkEventOnEdit = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var nav = (WorkEventPageNavigation)e.Parameter;
            DatePicker.Date = nav.Date;
            WorkEventOnEdit = nav.WorkEvent;
            IsEdit = (WorkEventOnEdit != null);
            if (IsEdit)
            {
                ButtonCreateOrEditEvent.Content = "Edit Event";
                DatePicker.Date = WorkEventOnEdit.Time;
                TimeSpan ts = new TimeSpan(WorkEventOnEdit.Time.Hour, WorkEventOnEdit.Time.Minute, 0);
                TimePicker.Time = ts;
                ComboHouses.SelectedValue = WorkEventOnEdit.IdHouse;
                TextBoxNotes.Text = WorkEventOnEdit.Note;
                TextBoxMoneyUnits.Text = WorkEventOnEdit.MoneyUnits.ToString();
                TextBoxMoneyCents.Text = WorkEventOnEdit.MoneyCents.ToString();
            }
            else
                ButtonCreateOrEditEvent.Content = "Create Event";
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
                    WorkEvent we = new WorkEvent(Time,
                                                int.Parse(ComboHouses.SelectedValue.ToString()),
                                                TextBoxNotes.Text,
                                                int.Parse(TextBoxMoneyUnits.Text),
                                                int.Parse(TextBoxMoneyCents.Text));
                    if (IsEdit)
                        App.DB.EditWorkEvent(WorkEventOnEdit.Id, we);
                    else
                        await App.DB.SaveSpecificWorkEvent(we);

                    TextError.Text = "";
                    TextError.Visibility = Visibility.Collapsed;
                    Frame.GoBack();
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

        private string GetDataValidation()
        {
            int MoneyUnits = 0;
            int MoneyCents = 0;
            if (DatePicker.Date.DateTime <= DateTime.Today)
                return "You can't sent events for the past";
            else if (!int.TryParse(TextBoxMoneyUnits.Text, out MoneyUnits) ||
                !int.TryParse(TextBoxMoneyCents.Text, out MoneyCents))
                return "That is not a number";
            else if (MoneyUnits < 0 || MoneyCents < 0)
                return "Money must be 0 or a positive number";
            else if (MoneyCents >= 100)
                return "Cents can't be over 100";
            else if (ComboHouses.SelectedIndex == -1)
                return "You have to select a house";
            else
                return "OK";
        }
    }
}
