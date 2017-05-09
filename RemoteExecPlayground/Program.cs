using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Security;
using System.Text;
using System.Xml;

namespace RemoteExecPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            var command = "get-childitem";
            var parameters = new Dictionary<object, object>
            {
                {"Path", @"C:\xxx\yyy\zzz"},
                {"Force", true},
                {"Recurse", true}
            };
            //Switch Parameter
            //Switch Parameter
            var results = ExecuteRemoteProcess("RHC-HSQAWEB02", command, parameters);
            Console.WriteLine(results);
            Console.ReadKey();
        }

        public static  PSCredential GetCredential(string userName, string password)
        {
            var securePassword = new SecureString();
            Array.ForEach(password.ToCharArray(), securePassword.AppendChar);
            return new PSCredential(userName, securePassword);
        }

        public static string ExecuteRemoteProcess(string computerName, string command, Dictionary<object,object> parameters )
        {
            var credential = GetCredential(@"RHCQA\rhcqaAdmin", @"D3vOp$Admin");
            var returnStr = new StringBuilder();
            WSManConnectionInfo connectionInfo = new WSManConnectionInfo();
            connectionInfo.ComputerName = computerName;
            connectionInfo.Credential = credential;
            Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            runspace.Open();

            using (PowerShell ps = PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.AddCommand(command);
                ps.AddParameters(parameters);
                try
                {
                    var results = ps.Invoke();
                    var errors = ps.Streams.Error.ReadAll();
                    if (errors != null && errors.Count != 0)
                    {
                        foreach (var err in errors)
                        {
                            returnStr.AppendLine(err.Exception.ToString());
                        }
                        throw new Exception("Powershell Invoke Error");
                    }
                    else
                    {
                        foreach (var result in results)
                        {
                            returnStr.AppendLine(result.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    returnStr.AppendLine(e.Message);
                    returnStr.AppendLine(e.ToString());
                }
                finally
                {
                    ps.Runspace.Close();
                }
            }

            return returnStr.ToString();
        }
        public static void LocalPSExec()
        {
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.AddScript("Get-Process");
                var results = ps.Invoke();
                foreach (var result in results)
                {
                    Console.WriteLine(result.ToString());
                }
            }
            runspace.Close();
        }
    }
}
