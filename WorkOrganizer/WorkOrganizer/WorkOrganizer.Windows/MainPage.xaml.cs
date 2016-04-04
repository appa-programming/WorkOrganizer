using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            List<WorkEvent> ListAux = new List<WorkEvent>();
            ListAux.Add(new WorkEvent() { Id = 10 });
            //TextBoxTest.Text = (await IOHandler.WriteJsonAsync("file123.xml", ListAux)).ToString();
            TextBoxTest.Text = "" + App.DB.Houses.Count() + App.DB.Owners.Count();
        }

        private async void DatePickerSelect_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            TextBoxTest.Text = "";
            var Obj = (DatePicker)sender;
            if (Obj.Date.Day == 10)
            {
                List<WorkEvent> Events = (await IOHandler.ReadJsonAsync<WorkEvent>("file123.xml")).ToList();
                for (int i = 0; i < Events.Count(); i++)
                {
                    TextBoxTest.Text += Events[i].Id;
                    if (i < Events.Count - 1)
                        TextBoxTest.Text += "\n";
                }
            }
        }

        private void SwipeToMonth_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WorkAnalyzerPage), DatePickerSelect.Date.DateTime);
        }
    }
}
