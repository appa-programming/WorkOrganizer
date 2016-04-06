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
using WorkOrganizer.Specs;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrganizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HousePage : Page
    {
        public House HouseOnEdit { get; private set; }
        public bool IsEdit { get; private set; }
        public HousePage()
        {
            this.InitializeComponent();
            ComboOwners.ItemsSource = App.DB.Owners;
            HouseOnEdit = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HouseOnEdit = (House)e.Parameter;
            IsEdit = (HouseOnEdit != null);
            if (IsEdit)
            {
                ButtonCreateOrEditHouse.Content = "Edit House";
                TextBoxName.Text = HouseOnEdit.Name.ToString();
                ComboOwners.SelectedValue = HouseOnEdit.IdOwner;
            }
            else
                ButtonCreateOrEditHouse.Content = "Create House";
        }
        private async void ButtonCreateOrEditHouse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ComboOwners.SelectedIndex != -1)
                {
                    DatabaseMessage msg = null;
                    House h = new House(TextBoxName.Text, ((int)ComboOwners.SelectedValue));
                    if (IsEdit)
                        msg = await App.DB.EditHouse(HouseOnEdit.IdHouse, h);
                    else
                        msg = await App.DB.AddHouse(h);
                    if (msg.State == DatabaseMessageState.OK)
                    {
                        TextError.Text = "";
                        TextError.Visibility = Visibility.Collapsed;
                        Frame.GoBack();
                    }
                    else
                    {
                        TextError.Text = msg.Error;
                        TextError.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    TextError.Text = "You have to select an owner";
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

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
