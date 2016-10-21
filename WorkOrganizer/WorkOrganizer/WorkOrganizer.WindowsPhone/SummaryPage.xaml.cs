using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WorkOrganizer.Specs;
using static MoneyLib.MoneyCalculator;

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
        private List<WorkEvent> WorkEvents { get; set; }

        public SummaryPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.ToString() != "")
            {
                var List = (List<WorkEvent>)e.Parameter;
                WorkEvents = List;

                CurrentOwner = App.DB.GetOwnerOfHouse(App.DB.Houses.FirstOrDefault(h => h.IdHouse == WorkEvents[0].IdHouse));
                ComboType.SelectedIndex = int.Parse(CurrentOwner.DefaultEmailType.Substring(4)) - 1;

                if (CurrentOwner != null && CurrentOwner.Email != "")
                {
                    ButtonSendEmail.Visibility = Visibility.Visible;
                }
            }
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

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkEvents != null && WorkEvents.Count > 0)
            {
                var Type = "" + ((ComboBoxItem)(sender as ComboBox).SelectedItem).Name;
                var Handler = EmailHandlerFactory.CreateNewHandler(Type);
                TextBoxSummary.Text = Handler.MakeText(WorkEvents, App.DB, CurrentOwner);
            }
        }
    }
}
