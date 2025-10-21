using System;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using Microsoft.Win32;
using System.Windows.Input;
using SolidWorksTools;
using System.Reflection;
using SolidWorksTools.File;

namespace INJECTOR
{
    public class Main : SwAddin
    {
        SldWorks swApp;
        int SessionCookie;
        private ModManager _moduleManager;

        #region --- Solidworks Connection ---
        public bool ConnectToSW(object ThisSW, int Cookie)
        {
            swApp = ThisSW as SldWorks;
            swApp.SetAddinCallbackInfo2(0, this, Cookie);
            SessionCookie = Cookie;
            _moduleManager = new ModManager(swApp);
            _moduleManager.LoadModules();

            // This is where UI is built

            return true;
        }
        public bool DisconnectFromSW()
        {
            // This is where UI is dismantled

            _moduleManager?.UnloadModules();
            GC.Collect();
            swApp = null;

            return true;
        }
        #endregion

        #region --- COM Registry Functions ---
        [ComRegisterFunction]
        private static void RegisterAssembly(Type t)
        {
            string Path = String.Format(@"SOFTWARE\Solidworks\Addins\{0:b}", t);
            RegistryKey Key = Registry.LocalMachine.CreateSubKey(Path);

            // Startup Int

            Key.SetValue(null, 1);
            Key.SetValue("Title", "INJECTOR");
            Key.SetValue("Description", "Modular macro library");
        }
        [ComUnregisterFunction]
        private static void UnregisterAssembly(Type t)
        {
            string Path = String.Format(@"SOFTWARE\Solidworks\Addins\{0:b}", t);
            Registry.LocalMachine.DeleteSubKey(Path);
        }
        #endregion
    }
}
