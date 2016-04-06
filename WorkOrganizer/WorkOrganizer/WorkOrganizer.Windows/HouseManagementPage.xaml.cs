﻿using System;
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
    public sealed partial class HouseManagementPage : Page
    {
        public HouseManagementPage()
        {
            this.InitializeComponent();
            var UISyncContext = TaskScheduler.FromCurrentSynchronizationContext();
            if (!App.DB.IsLoaded)
                Task.Run(() => App.DB.Load()).ContinueWith(tsk => AddHouseBars(), UISyncContext);
            else
                AddHouseBars();
        }
        private void AddHouseBars()
        {
            ProgressRing.Visibility = Visibility.Collapsed;
            ScrollHouses.Visibility = Visibility.Visible;

            StackHouses.Children.Clear();

            foreach (var house in App.DB.Houses)
            {
                Grid Grid = new Grid();

                Grid.Tag = house.IdHouse;
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
                /*ColumnDefinition cd2 = new ColumnDefinition();
                cd2.Width = new GridLength(4, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd2);
                ColumnDefinition cd3 = new ColumnDefinition();
                cd3.Width = new GridLength(1, GridUnitType.Star);
                Grid.ColumnDefinitions.Add(cd3);*/

                /*<Border Style="{StaticResource borderStyle}" Grid.Row="0" Grid.Column="0">*/
                Border b0 = new Border();
                b0.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock Name = new TextBlock();
                Name.Text = house.Name;
                Name.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Name.TextAlignment = TextAlignment.Center;
                Name.FontWeight = FontWeights.Bold;
                b0.Child = Name;

                b0.SetValue(Grid.ColumnProperty, 0);
                Grid.Children.Add(b0);

                Border b1 = new Border();
                b1.Style = Grid.Resources["BorderStyle"] as Style;

                TextBlock Owner = new TextBlock();
                Owner.Text = App.DB.Owners.FirstOrDefault(o => o.IdOwner == house.IdOwner).Name;
                Owner.Style = Application.Current.Resources["MyTextBoxStyle"] as Style;
                Owner.TextAlignment = TextAlignment.Center;
                Owner.FontWeight = FontWeights.Bold;
                b1.Child = Owner;

                b1.SetValue(Grid.ColumnProperty, 1);
                Grid.Children.Add(b1);

                Grid.Tapped += House_Tap;
                StackHouses.Children.Add(Grid);
            }
        }

        private void House_Tap(object sender, TappedRoutedEventArgs e)
        {
            var house = App.DB.Houses.FirstOrDefault(h => h.IdHouse == ((int)((Grid)sender).Tag));
            Frame.Navigate(typeof(HousePage), house);
        }

        private void ButtonGoBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void SwipeToOwner_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OwnerManagementPage));
        }

        private void ButtonAddHouse_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HousePage));
        }
    }
}
