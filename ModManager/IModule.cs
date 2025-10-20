using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace INJECTOR
{
    public interface IModule
    {
        // Initialize interface

        void Initialize(ISldWorks swApp);
        void Terminate();

    }
}
