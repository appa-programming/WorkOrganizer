using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WorkOrganizer.AppLogic
{
    public static class MainPageHandler
    {
        public static void OnLoad(Func<Task<bool>> AddWorkEventBars, TaskScheduler UISyncContext)
        {
            if (!App.DB.IsLoaded)
                Task.Run(() => App.DB.Load()).ContinueWith(tsk => AddWorkEventBars(), UISyncContext);
        }

        internal static async Task OnNavigatedTo(NavigationEventArgs e, DatePicker datePickerSelect, Func<Task<bool>> addWorkEventBars)
        {
            if (e.Parameter.ToString() != "")
            {
                var Dt = (DateTime)e.Parameter;
                datePickerSelect.Date = Dt;
                //if (App.DB.IsLoaded)
                //    AddWorkEventBars();
            }
            else
            {
                datePickerSelect.Date = DateTime.Now;
            }
            await addWorkEventBars();
        }
    }
}
