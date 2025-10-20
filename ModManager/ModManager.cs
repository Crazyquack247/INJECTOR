using INJECTOR.Modules;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INJECTOR
{
    public class ModManager
    {
        private readonly ISldWorks _swApp;
        private readonly int _addinId;
        private readonly List<IModule> _modules = new List<IModule>();


        public ModManager(ISldWorks swApp, int addinId)
        {
            _swApp = swApp;
            _addinId = addinId;
        }

        public void LoadModules()
        {
            _modules.AddRange(new IModule[]
            {
                // List modules here

                new Modules.OnSaveModule(),
                new Modules.X_TExport.X_TExport(_swApp)

            });

            foreach (var module in _modules)
                module.Initialize(_swApp);
        }

        public void UnloadModules()
        {
            foreach (var module in _modules)
                module.Terminate();

            _modules.Clear();
        }

        public T GetModule<T>() where T : class, IModule
        {
            foreach (var module in _modules)
            {
                if (module is T match)
                    return match;
            }
            return null;
        }
    }
}
