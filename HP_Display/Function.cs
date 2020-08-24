using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace HP_Display
{
    class Function
    {
        private void WinStatus() 
        {

            ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PnPEntity");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject Obj in queryCollection)
            {
                if (Obj["Name"] != null /*&& Obj["Name"].ToString().Contains("PnP")*/)
                {
                    Console.WriteLine("Name : {0} \t Status : {1}", Obj["Name"], Obj["Status"]);
                    //statusBox.AppendText($"Name : {Obj["Name"]} \t Status : {Obj["Status"]}" + Environment.NewLine);
                }
            }

        }
    }
}
