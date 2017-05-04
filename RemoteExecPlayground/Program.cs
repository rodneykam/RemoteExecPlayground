using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;

namespace RemoteExecPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            var password = new SecureString();
            Array.ForEach("D3vOp$Admin".ToCharArray(), password.AppendChar);
            PSCredential credential = new PSCredential("RHCQA\\rhcqaAdmin", password);

            WSManConnectionInfo connectionInfo = new WSManConnectionInfo();
            connectionInfo.ComputerName = "RHC-HSQAWEB01";
            connectionInfo.Credential = credential;
            Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            runspace.Open();
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.AddScript("Get-Service | where {$_.DisplayName -match \"Relay*\"}");
                var results = ps.Invoke();
                foreach (var result in results)
                {
                    Console.WriteLine(result.Members["DisplayName"].Value);
                    Console.WriteLine(result.ToString());
                    Console.WriteLine(result.Members["Status"].Value);
                    Console.WriteLine("===========================");
                }
            }
            runspace.Close();

            Console.WriteLine("Done!");
            Console.ReadKey();
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
