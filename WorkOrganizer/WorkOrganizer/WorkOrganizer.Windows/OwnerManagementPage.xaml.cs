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

            foreach (var owner in App.DB.Owners)
            {
                Grid Grid = new Grid();

                Grid.Tag = owner.IdOwner;
                Grid.Height = 80;

                Style Style = new Style { TargetType = typeof(Border) };
                Style.Setters.Add(new Setter(Border.BorderBrushProperty, "Green"));
                Style.Setters.Add(new Setter(Border.BorderThicknessProperty, "1"));
                Style.Setters.Add(new Setter(Border.PaddingProperty, "2"));
                Grid.Resources.Add("BorderStyle", Style);

                /*ColumnDefinition cd0 = new ColumnDefinition();
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
                Grid.ColumnDefinitions.Add(cd3);*/

                /*<Border Style="{StaticResource borderStyle}" Grid.Row="0" Grid.Column="0">*/
                Border b0 = new Border();
                b0.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock Name = new TextBlock();
                Name.Text = owner.Name;
                Name.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Name.TextAlignment = TextAlignment.Center;
                Name.FontWeight = FontWeights.Bold;
                b0.Child = Name;

                b0.SetValue(Grid.ColumnProperty, 0);
                Grid.Children.Add(b0);

                Grid.Tapped += Owner_Tap;
                StackOwners.Children.Add(Grid);
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
