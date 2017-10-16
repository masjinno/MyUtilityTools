using IPAddressViewer.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;

namespace IPAddressViewer
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private MainWindow view;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.InitializeAssembly();

            view = new MainWindow();
            view.Show();
        }

        /// <summary>
        /// DLLをEXEに取り込むための処理
        /// </summary>
        private void InitializeAssembly()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs e) =>
            {
                var thisAssembly = Assembly.GetExecutingAssembly();

                // Get the Name of the AssemblyFile
                var assemblyName = new AssemblyName(e.Name);
                var dllName = assemblyName.Name + ".dll";

                // Load from Embedded Resources - This function is not called if the Assembly is already
                // in the same folder as the app.
                var resources = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(dllName));
                if (resources.Any())
                {

                    // 99% of cases will only have one matching item, but if you don't,
                    // you will have to change the logic to handle those cases.
                    var resourceName = resources.First();
                    using (var stream = thisAssembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null) return null;
                        var block = new byte[stream.Length];

                        // Safely try to load the assembly.
                        try
                        {
                            stream.Read(block, 0, block.Length);
                            return Assembly.Load(block);
                        }
                        catch (IOException)
                        {
                            return null;
                        }
                        catch (BadImageFormatException)
                        {
                            return null;
                        }
                    }
                }
                // in the case the resource doesn't exist, return null.
                return null;
            };
        }
    }
}
