using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WorkOrganizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OwnerManagementPage : Page
    {
        public OwnerManagementPage()
        {
            this.InitializeComponent();
            var UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();
            if (!App.DB.IsLoaded)
                Task.Run(() => App.DB.Load()).ContinueWith(tsk => AddOwnerBars(), UISyncContext);
            else
                AddOwnerBars();
        }
        private void AddOwnerBars()
        {
            ProgressRing.Visibility = Visibility.Collapsed;
            ScrollOwners.Visibility = Visibility.Visible;

            StackOwners.Children.Clear();

            foreach (var owner in App.DB.ActiveOwners)
            {
                Grid MainGrid = new Grid();
                MainGrid.Name = "MainGrid" + owner.IdOwner;

                ColumnDefinition maincd0 = new ColumnDefinition();
                maincd0.Width = new GridLength(6, GridUnitType.Star);
                MainGrid.ColumnDefinitions.Add(maincd0);
                ColumnDefinition maincd1 = new ColumnDefinition();
                maincd1.Width = new GridLength(1, GridUnitType.Star);
                MainGrid.ColumnDefinitions.Add(maincd1);

                Grid LeftGrid = new Grid();

                LeftGrid.Tag = owner.IdOwner;
                LeftGrid.Height = 80;

                Style Style = new Style { TargetType = typeof(Border) };
                Style.Setters.Add(new Setter(Border.BorderBrushProperty, "Green"));
                Style.Setters.Add(new Setter(Border.BorderThicknessProperty, "1"));
                Style.Setters.Add(new Setter(Border.PaddingProperty, "2"));
                LeftGrid.Resources.Add("BorderStyle", Style);

                /*ColumnDefinition cd0 = new ColumnDefinition();
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
                LeftGrid.ColumnDefinitions.Add(cd3);*/

                /*<Border Style="{StaticResource borderStyle}" LeftGrid.Row="0" LeftGrid.Column="0">*/
                Border b0 = new Border();
                b0.Style = LeftGrid.Resources["BorderStyle"] as Style;

                TextBlock Name = new TextBlock();
                Name.Text = owner.Name;
                Name.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Name.TextAlignment = TextAlignment.Center;
                Name.FontWeight = FontWeights.Bold;
                b0.Child = Name;

                b0.SetValue(Grid.ColumnProperty, 0);
                LeftGrid.Children.Add(b0);

                LeftGrid.Tapped += Owner_Tap;

                //------------------------------------------------

                Border b2 = new Border();
                b2.Style = LeftGrid.Resources["BorderStyle"] as Style;

                Button Delete = new Button();
                Delete.HorizontalAlignment = HorizontalAlignment.Center;
                Delete.VerticalAlignment = VerticalAlignment.Center;
                Delete.Width = 32;
                Delete.Height = 32;
                Delete.Tag = owner.IdOwner;
                string Template = "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                                        "<Image Source=\"/Assets/Minus.png\" />" +
                                    "</ControlTemplate>";
                Delete.Template = (ControlTemplate)XamlReader.Load(Template);
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets//Minus.png"));
                Delete.Content = img;
                Delete.Click += Delete_Click;

                b2.Child = Delete;

                LeftGrid.SetValue(Grid.ColumnProperty, 0);
                b2.SetValue(Grid.ColumnProperty, 1);

                MainGrid.Children.Add(LeftGrid);
                MainGrid.Children.Add(b2);

                StackOwners.Children.Add(MainGrid);
            }
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var Btn = sender as Button;
            var Owner = App.DB.Owners.Find(o => o.IdOwner.ToString() == Btn.Tag.ToString());
            var MsgDialog = new MessageDialog("Are you sure you want to remove this owner from your list ?");

            MsgDialog.Commands.Add(new UICommand("Yes", null, "YES"));
            MsgDialog.Commands.Add(new UICommand("No", null, "NO"));
            var op = await MsgDialog.ShowAsync();
            if ((string)op.Id == "YES")
            {
                var Msg = await App.DB.RemoveOwner(Owner.IdOwner);
                if (Msg.State == Specs.DatabaseMessageState.ERROR)
                {
                    Owner.IsInvisible = false;
                    // TODO ERROR
                }
                else
                {
                    var Grid = (Grid)FindName("MainGrid" + Owner.IdOwner);
                    StackOwners.Children.Remove(Grid);
                }
            }
        }

        private void Owner_Tap(object sender, TappedRoutedEventArgs e)
        {
            var owner = App.DB.Owners.FirstOrDefault(o => o.IdOwner == ((int)((Grid)sender).Tag));
            Frame.Navigate(typeof(OwnerPage), owner);
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void ButtonAddOwner_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OwnerPage));
        }
    }
}
