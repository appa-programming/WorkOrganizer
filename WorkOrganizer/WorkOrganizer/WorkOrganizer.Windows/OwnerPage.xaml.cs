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
    public sealed partial class OwnerPage : Page
    {
        public Owner OwnerOnEdit { get; private set; }
        public bool IsEdit { get; private set; }
        public OwnerPage()
        {
            this.InitializeComponent();
            OwnerOnEdit = null;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            OwnerOnEdit = (Owner)e.Parameter;
            IsEdit = (OwnerOnEdit != null);
            if (IsEdit)
            {
                ButtonCreateOrEditOwner.Content = "Edit Owner";
                TextBoxName.Text = OwnerOnEdit.Name.ToString();
                TextBoxEmail.Text = OwnerOnEdit.Email.ToString();
            }
            else
                ButtonCreateOrEditOwner.Content = "Create Owner";
        }

        private async void ButtonCreateOrEditOwner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextBoxName.Text.ToLower() != "me" &&
                    TextBoxName.Text.ToLower() != "vanessa barroso" &&
                    TextBoxName.Text.ToLower() != "vanessabarroso" &&
                    TextBoxName.Text.ToLower() != "eu")
                {
                    DatabaseMessage msg = null;
                    Owner o = new Owner(TextBoxName.Text, TextBoxEmail.Text);
                    if (IsEdit)
                        msg = await App.DB.EditOwner(OwnerOnEdit.IdOwner, o);
                    else
                        msg = await App.DB.AddOwner(o);
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
                    TextError.Text = "YES! I knew one day you would get rich :) Good Luck";
                    TextError.Inlines.Add(new Run { Text = " Sweetie", Foreground = new SolidColorBrush(Colors.HotPink) });
                    TextError.Inlines.Add(new Run { Text = ". PS - The program won't let you use your name, I suggest you use" });
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
