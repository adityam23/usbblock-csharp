using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Management.Automation;
using System.Data.SQLite;



namespace usblock
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /*class thisUSB
        {
            String name;
            String PnpDevID;

            public String GetName()
            {
                return name;
            }
            public String GetPNPDevID()
            {
                return PnpDevID;
            }
            public thisUSB()
            {
                this.name = "";
                this.PnpDevID = "";
            }
            public thisUSB(String n, String p)
            {
                this.name = n;
                this.PnpDevID = p;
            }
            public void SetName(String n)
            {
                this.name = n;
            }
            public void SetPNPDevID(String p)
            {
                this.PnpDevID = p;
            }
        }*/
        class USBBlocker
        {
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        /*static void Main()
        {

            ManagementObjectSearcher searcher =
                   new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
            thisUSB otherDrive = new thisUSB();
            Console.WriteLine(searcher);

            foreach (ManagementObject queryObj in searcher.Get())
            {
                Console.WriteLine("in loop");
                thisUSB drive = new thisUSB((String)queryObj["Caption"], (String)queryObj["PNPDeviceID"]);
                //Console.WriteLine("-----------------------------------");
                //Console.WriteLine("Win32_USBDevice instance");
                //Console.WriteLine("-----------------------------------");
                Console.WriteLine("name: {0}", drive.GetName());
                //Console.WriteLine("Description: {0}", queryObj["Description"]);
                //Console.WriteLine("DeviceID: {0}", queryObj["DeviceID"]);
                //Console.WriteLine("Manufacturer: {0}", queryObj["Manufacturer"]);
                //Console.WriteLine("Name: {0}", queryObj["Name"]);
                Console.WriteLine("PNPDeviceID: {0}", drive.GetPNPDevID());
                otherDrive.SetName(drive.GetName());
                otherDrive.SetPNPDevID(drive.GetPNPDevID());
                //Console.WriteLine("Model: {0}", queryObj["Model"]);
                //Console.WriteLine("SerialNumber: {0}", queryObj["SerialNumber"]);
            }
            Console.WriteLine("Escaping the loop");
            PowerShell ps = PowerShell.Create();
            Console.WriteLine("This is getting fucked {0}", otherDrive.GetPNPDevID());
            ps.AddCommand("Disable-PNPDevice").AddParameter("-InstanceID", otherDrive.GetPNPDevID());
            ps.AddParameter("Confirm", false);
            ps.Invoke();
            Console.WriteLine("fucked");
            Console.ReadLine();
        }*/
    }

    //static void Main()
    //{
    //    Application.EnableVisualStyles();
    //    Application.SetCompatibleTextRenderingDefault(false);
    //    Application.Run(new Form1());
    //}
}
}
