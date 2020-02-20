using System;
using System.Windows.Forms;
using System.Management;
using System.Management.Automation;
using System.Data.SQLite;
//----
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace usblock
{
    public partial class Form1 : Form
    {
        private void AdminRelauncher()
        {
            if (!IsRunAsAdmin())
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Assembly.GetEntryAssembly().CodeBase;

                proc.Verb = "runas";

                try
                {
                    Process.Start(proc);
                    Application.Exit();
                        //.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("This program must be run as an administrator! \n\n" + ex.ToString());
                }
            }
        }
        private bool IsRunAsAdmin()
        {
            try
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(id);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Form1()
        {
            InitializeComponent();
            AdminRelauncher();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*ManagementObjectSearcher searcher =
                       new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
            if(!System.IO.File.Exists("MyDatabase.db"))
            {
                SQLiteConnection.CreateFile("MyDatabase.db");
            }
            SQLiteConnection conn;
            conn =
                new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");
            conn.Open();
            string sql = "CREATE TABLE IF NOT EXISTS USBList(name TEXT PRIMARY KEY, pnpID TEXT)";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            comm.ExecuteNonQuery();
            string dev = "";
            foreach (ManagementObject queryObj in searcher.Get())
            {
                Console.WriteLine("ZZZZZZZZZZZZZZ---->{0}",queryObj);
                dev = "INSERT INTO USBList(name, pnpID) VALUES (\"" + (string)queryObj["Caption"] + "\", \"" + (string)queryObj["PNPDeviceID"] + "\");";
                SQLiteCommand cc = new SQLiteCommand(dev, conn);
                try
                {
                    cc.ExecuteNonQuery();
                }
                catch
                {
                    Console.WriteLine("Exception");
                }
            }
            string q = "SELECT * FROM USBList";
            SQLiteCommand c = new SQLiteCommand(q, conn);
            SQLiteDataReader reader = c.ExecuteReader();
            while (reader.Read())
            {
                listBox1.Items.Add(reader["name"]);
                Console.WriteLine("Name: {0}\nID: {1}", reader["name"], reader["pnpID"]);
            }
            conn.Close();*/
            if (!System.IO.File.Exists("MyDatabase.db"))
            {
                SQLiteConnection.CreateFile("MyDatabase.db");
            }
            else
            {
                string blocked = "SELECT * FROM blockedUSB";
                SQLiteConnection con = new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");
                con.Open();
                SQLiteCommand comm = new SQLiteCommand(blocked, con);

                SQLiteDataReader reader = comm.ExecuteReader();
                while(reader.Read())
                {
                    listBox2.Items.Add(reader["name"]);
                }
                //MessageBox.Show("Exists!");
            }
            refresh();
        }

        private void TabPage2_Click(object sender, EventArgs e)
        {

        }

        private void TabPage1_Click(object sender, EventArgs e)
        {
            refresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string dev = listBox1.SelectedItem.ToString();
                string q = "SELECT pnpID FROM USBList WHERE NAME = '" + dev + "';";
                Console.WriteLine(q);
                SQLiteConnection conn;
                conn = new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");
                conn.Open();
                SQLiteCommand c = new SQLiteCommand(q, conn);
                SQLiteDataReader reader = c.ExecuteReader();
                string b = "CREATE TABLE IF NOT EXISTS blockedUSB(name TEXT PRIMARY KEY, pnpID TEXT);";
                SQLiteCommand block = new SQLiteCommand(b, conn);
                block.ExecuteNonQuery();
                while (reader.Read())
                {
                    //listBox1.Items.Add(reader["name"]);
                    b = "INSERT INTO blockedUSB(name,pnpID) VALUES('" + dev + "','" + reader["pnpID"] + "');";
                    block = new SQLiteCommand(b, conn);
                    try
                    {
                        block.ExecuteNonQuery();
                    }
                    catch
                    {
                        Console.WriteLine("Block exception");
                    }
                    PowerShell ps = PowerShell.Create();
                    Console.WriteLine("This is getting fucked {0}", reader["pnpID"]);
                    ps.AddCommand("Disable-PNPDevice").AddParameter("-InstanceID", reader["pnpID"]);
                    ps.AddParameter("Confirm", false);
                    ps.Invoke();
                    Console.WriteLine("Done");
                }
                listBox2.Items.Add(dev);
                for (int n = listBox1.Items.Count - 1; n >= 0; --n)
                {
                    //string removelistitem = "OBJECT";
                    if (listBox1.Items[n].ToString().Contains(dev))
                    {
                        listBox1.Items.RemoveAt(n);
                    }
                }
                string toshow = dev + " Blocked";
                //toshow += " Blocked!";
                conn.Close();
                MessageBox.Show(toshow);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                string dev = listBox2.SelectedItem.ToString();
                string q = "SELECT pnpID FROM blockedUSB WHERE NAME = '" + dev + "';";
                Console.WriteLine(q);
                SQLiteConnection conn;
                conn = new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");
                conn.Open();
                SQLiteCommand c = new SQLiteCommand(q, conn);
                SQLiteDataReader reader = c.ExecuteReader();
                while (reader.Read())
                {
                    q = "DELETE FROM blockedUSB WHERE name = '" + dev + "';";
                    PowerShell ps = PowerShell.Create();
                    Console.WriteLine("This is getting fucked {0}", reader["pnpID"]);
                    ps.AddCommand("Enable-PNPDevice").AddParameter("-InstanceID", reader["pnpID"]);
                    ps.AddParameter("Confirm", false);
                    ps.Invoke();
                    c = new SQLiteCommand(q, conn);
                    c.ExecuteNonQuery();
                }
                conn.Close();
                for (int n = listBox2.Items.Count - 1; n >= 0; --n)
                {
                    //string removelistitem = "OBJECT";
                    if (listBox2.Items[n].ToString().Contains(dev))
                    {
                        listBox2.Items.RemoveAt(n);
                    }
                }
                string toshow = dev + " Unblocked";
                //toshow += " Blocked!";
                conn.Close();
                refresh();
                MessageBox.Show(toshow);
            }
        }

        private void refresh()
        {
            listBox1.Items.Clear();
            ManagementObjectSearcher searcher =
                       new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
            //SQLiteConnection.CreateFile("MyDatabase.db");
            SQLiteConnection conn;
            conn =
                new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");
            conn.Open();
            string drop = "DROP TABLE USBList";
            string sql = "CREATE TABLE IF NOT EXISTS USBList(name TEXT PRIMARY KEY, pnpID TEXT)";
            SQLiteCommand comm = new SQLiteCommand(drop, conn);
            try
            {
                comm.ExecuteNonQuery();
            }
            catch(SQLiteException)
            {
                
            }
            comm = new SQLiteCommand(sql, conn);
            comm.ExecuteNonQuery();
            string dev = "";
            foreach (ManagementObject queryObj in searcher.Get())
            {
                Console.WriteLine("ZZZZZZZZZZZZZZ---->{0}", queryObj);
                dev = "INSERT INTO USBList(name, pnpID) VALUES (\"" + (string)queryObj["Caption"] + "\", \"" + (string)queryObj["PNPDeviceID"] + "\");";
                SQLiteCommand cc = new SQLiteCommand(dev, conn);
                try
                {
                    cc.ExecuteNonQuery();
                }
                catch
                {
                    Console.WriteLine("Exception");
                }
            }
            string q = "SELECT * FROM USBList";
            SQLiteCommand c = new SQLiteCommand(q, conn);
            SQLiteDataReader reader = c.ExecuteReader();
            while (reader.Read())
            {
                listBox1.Items.Add(reader["name"]);
                Console.WriteLine("Name: {0}\nID: {1}", reader["name"], reader["pnpID"]);
            }
            conn.Close();
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            refresh();
        }
    }
}
