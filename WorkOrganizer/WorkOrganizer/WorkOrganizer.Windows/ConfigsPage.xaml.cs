using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class ConfigsPage : Page
    {
        public string CurrentLaundryEuroPerKg { get; private set; }

        public ConfigsPage()
        {
            this.InitializeComponent();
            LoadConfigs(App.DB.Configs);
        }

        private void LoadConfigs(List<Config> configs)
        {
            for (int i = 1; i < configs[0].CheckInValues.Count; i++)
                AddPrice((StackPanel)this.FindName("StackInnerType1"), "1", configs[0].CheckInValues[i]);
            for (int i = 1; i < configs[0].Stairs.Count; i++)
                AddPrice((StackPanel)this.FindName("StackInnerType2"), "2", configs[0].CheckInValues[i]);
            for (int i = 1; i < configs[0].Cleaning.Count; i++)
                AddPrice((StackPanel)this.FindName("StackInnerType3"), "3", configs[0].Cleaning[i]);
            for (int i = 1; i < configs[0].ConstructionCleaning.Count; i++)
                AddPrice((StackPanel)this.FindName("StackInnerType4"), "4", configs[0].ConstructionCleaning[i]);
            
            TextBoxType5.Text = configs[0].Laundry;
            CurrentLaundryEuroPerKg = configs[0].Laundry;
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ButtonSave.IsEnabled = false;
            List<string> CheckInValues          = new List<string> { "0€00" };
            List<string> Stairs                 = new List<string> { "0€00" };
            List<string> Cleaning               = new List<string> { "0€00" };
            List<string> ConstructionCleaning   = new List<string> { "0€00" };
            string Laundry = TextBoxType5.Text;

            foreach (TextBox tb in StackInnerType1.Children)
                CheckInValues.Add(tb.Text);
            foreach (TextBox tb in StackInnerType2.Children)
                Stairs.Add(tb.Text);
            foreach (TextBox tb in StackInnerType3.Children)
                Cleaning.Add(tb.Text);
            foreach (TextBox tb in StackInnerType4.Children)
                ConstructionCleaning.Add(tb.Text);

            Config Conf = new Config(CheckInValues,
                                        Stairs,
                                        Cleaning,
                                        ConstructionCleaning,
                                        Laundry);
            App.DB.SetConfigs(Conf);
            await App.DB.SaveConfigs();
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            ButtonSave.IsEnabled = true;
            string Tag = ((Button)sender).Tag.ToString();
            var SP = (StackPanel)this.FindName("StackInnerType" + Tag);

            if (SP.Children.Count == 5)
            {
                var btnAdd = (Button)this.FindName("ButtonType" + Tag + "Add");
                btnAdd.Visibility = Visibility.Visible;
            }
            if (SP.Children.Count > 0)
                SP.Children.RemoveAt(SP.Children.Count - 1);
            if (SP.Children.Count == 0)
            {
                var tbZero = (TextBox)this.FindName("TextBoxType" + Tag + "Zero");
                tbZero.Margin = new Thickness(42, 0, 0, 0);
                var btnRemove = (Button)this.FindName("ButtonType" + Tag + "Remove");
                btnRemove.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ButtonSave.IsEnabled = true;
            string Tag = ((Button)sender).Tag.ToString();
            var SP = (StackPanel)this.FindName("StackInnerType" + Tag);
            AddPrice(SP, Tag, "");
        }

        private void AddPrice(StackPanel sP, string tag, string text)
        {
            if (sP.Children.Count == 0)
            {
                var tbZero = (TextBox)this.FindName("TextBoxType" + tag + "Zero");
                tbZero.Margin = new Thickness(0, 0, 0, 0);
                var btnRemove = (Button)this.FindName("ButtonType" + tag + "Remove");
                btnRemove.Visibility = Visibility.Visible;
            }
            if (sP.Children.Count < 5)
            {
                TextBox tb = new TextBox();
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.IsReadOnly = false;
                tb.PlaceholderText = "Fill Me";
                tb.Margin = new Thickness(10, 0, 0, 0);
                tb.Text = text;
                tb.LostFocus += tb_LostFocus;
                tb.KeyUp += tb_KeyUp;
                sP.Children.Add(tb);
            }
            if (sP.Children.Count == 5)
            {
                var btnAdd = (Button)this.FindName("ButtonType" + tag + "Add");
                btnAdd.Visibility = Visibility.Collapsed;
            }
        }

        private void tb_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                if (e.Key == VirtualKey.Enter)
                {
                    //tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    ButtonSave.Focus(FocusState.Programmatic);
                }
            }
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            string Content = tb.Text;
            string SaveContent = Content;
            string Pattern = @"(\d+)";
            Match result1 = Regex.Match(Content, Pattern);
            Pattern = @"((\d+)\.(\d*))";
            Match result2 = Regex.Match(Content, Pattern);
            Pattern = @"((\d+)\,(\d*))";
            Match result3 = Regex.Match(Content, Pattern);
            Pattern = @"((\d*)\.(\d+))";
            Match result4 = Regex.Match(Content, Pattern);
            Pattern = @"((\d*)\,(\d+))";
            Match result5 = Regex.Match(Content, Pattern);
            Pattern = @"((\d+)€(\d+))";
            Match result6 = Regex.Match(Content, Pattern);
            Pattern = @"(€(\d+))";
            Match result7 = Regex.Match(Content, Pattern);
            Pattern = @"((\d+)€)";
            Match result8 = Regex.Match(Content, Pattern);
            if (result2.Success || result3.Success || result4.Success || result5.Success)
            {
                var CharSplitter = '.';
                if (result3.Success || result5.Success)
                    CharSplitter = ',';
                Content = Content.Replace(CharSplitter, '€');
                Content = TreatContent(Content);
                if (Content == "ERROR")
                {
                    //TODO Error
                }
            }
            else if (result6.Success || result7.Success || result8.Success)
            {
                Content = TreatContent(Content);
            }
            else if (result1.Success)
            {
                Content += "€00";
            }
            else
            {
                //TODO Error
                Content = "ERROR";
            }
            tb.Text = Content;
            if (SaveContent != Content)
                ButtonSave.IsEnabled = true;
        }

        private string TreatContent(string content)
        {
            string Resp = content;
            string[] Strs = Resp.Split('€');
            if (Strs[0].Length == 0)
            {
                Resp = "0" + Resp;
            }
            if (Strs[1].Length > 2)
            {
                Resp = "ERROR";
            }
            else if (Strs[1].Length == 0)
            {
                Resp += "00";
            }
            else if (Strs[1].Length == 1)
            {
                Resp += "0";
            }
            return Resp;
        }

        private async void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            App.DB.ResetConfig();
            await App.DB.SaveConfigs();
        }

        private void TextBoxType5_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if(TextBoxType5.Text != CurrentLaundryEuroPerKg)
            {
                ButtonSave.IsEnabled = true;
            }
        }
    }
}
