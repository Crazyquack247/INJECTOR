using SolidWorks.Interop.sldworks;
using System.Collections.Generic;

namespace INJECTOR.ModManager
{
    public class ModManager
    {
        private readonly ISldWorks swApp;
        private readonly int _addinId;
        private readonly List<IModule> _modules = new List<IModule>();

        public ModManager(ISldWorks swApp, int addinId)
        {
            this.swApp = swApp;
            _addinId = addinId;
        }

        public void LoadModules()
        {
            _modules.AddRange(new IModule[]
            {
                // List modules here

                new Modules.OnSaveModule(),
            });

            foreach (var module in _modules)
                module.Initialize(swApp);
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
