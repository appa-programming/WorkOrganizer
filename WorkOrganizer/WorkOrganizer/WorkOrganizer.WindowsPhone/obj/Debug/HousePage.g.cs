﻿

#pragma checksum "C:\APPA\Projectos\Proprios\WindowsPhone\WorkOrganizer\WorkOrganizer\WorkOrganizer\WorkOrganizer.WindowsPhone\HousePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "20D55680D9BB9AA33032E6FE2CBEE706"
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
    partial class HousePage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 65 "..\..\HousePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonGoBack_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 79 "..\..\HousePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.ButtonCreateOrEditHouse_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


