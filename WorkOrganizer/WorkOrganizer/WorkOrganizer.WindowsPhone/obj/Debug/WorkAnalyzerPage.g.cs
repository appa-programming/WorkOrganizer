﻿

#pragma checksum "C:\APPA\Projectos\Proprios\WindowsPhone\WorkOrganizer\WorkOrganizer\WorkOrganizer\WorkOrganizer.WindowsPhone\WorkAnalyzerPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "507BD6859801F90DE0F58D9BEC1DFC33"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WorkOrganizer
{
    partial class WorkAnalyzerPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 29 "..\..\WorkAnalyzerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonGoBack_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 37 "..\..\WorkAnalyzerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.DatePicker)(target)).DateChanged += this.DatePickerMonthYear_DateChanged;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 42 "..\..\WorkAnalyzerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.SwipeToManagement_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 54 "..\..\WorkAnalyzerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.RadioButton_Checked;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 58 "..\..\WorkAnalyzerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.RadioButton_Checked;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


