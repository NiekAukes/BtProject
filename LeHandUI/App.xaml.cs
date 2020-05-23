using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;

namespace LeHandUI
{   
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
           
            using (var stream = new System.IO.MemoryStream(LeHandUI.Properties.Resources.Highlighting))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.RegisterHighlighting("Lua", new string[0],
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                            ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance));
                }
            }
            System.Diagnostics.Debug.WriteLine("app");
        }
        
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            StartupValues values = new StartupValues();
            values.StartFontSize = Convert.ToInt32(AdvancedMode.inst.textEditor.FontSize);
            values.StartupFileId = FileManager.currentFileId;
            values.ShowLineNumbers = Convert.ToInt32(AdvancedMode.inst.textEditor.ShowLineNumbers);

            LHregistry.SetStartupValues(values);
            Communicator.quit();
        }

    }
}
