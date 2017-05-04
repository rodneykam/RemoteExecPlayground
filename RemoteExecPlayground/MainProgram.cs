using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;

namespace RemoteExecPlayground
{
    class Program
    {
 
        public static void Main()
        {

            ExecuteTestProcesses();
            Console.ReadKey();
        }

        public static void ExecuteTestProcesses()
        {
            //Get the object on which the method will be invoked
            ManagementClass processClass =
            new ManagementClass("Win32_Process");

            // Option 1: Invocation using parameter objects
            //================================================

            //Get an input parameters object for this method
            ManagementBaseObject inParams =
            processClass.GetMethodParameters("Create");

            //Fill in input parameter values
            inParams["CommandLine"] = "calc.exe";

            //Execute the method
            ManagementBaseObject outParams =
            processClass.InvokeMethod("Create", inParams, null);

            //Display results
            //Note: The return code of the method is provided
            // in the "returnValue" property of the outParams object
            Console.WriteLine("Creation of calculator " +
                "process returned: " + outParams["returnValue"]);
            Console.WriteLine("Process ID: " + outParams["processId"]);

            // Option 2: Invocation using args array
            //=======================================

            //Create an array containing all arguments for the method
            object[] methodArgs = { "notepad.exe", null, null, 0 };

            //Execute the method
            object result = processClass.InvokeMethod("Create", methodArgs);

            //Display results
            Console.WriteLine("Creation of process returned: " + result);
            Console.WriteLine("Process id: " + methodArgs[3]);
        }

    }

}
