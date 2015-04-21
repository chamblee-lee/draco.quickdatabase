using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace Draco.DB.QuickDataBase.Threading
{
    class CallContextStorage : IThreadStorage
    {
        // Methods
        public void FreeNamedDataSlot(string name)
        {
            CallContext.FreeNamedDataSlot(name);
        }

        public object GetData(string name)
        {
            return CallContext.GetData(name);
        }

        public void SetData(string name, object value)
        {
            CallContext.SetData(name, value);
        }

    }
}
