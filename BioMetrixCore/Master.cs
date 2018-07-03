using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Threading;
using System.Collections.ObjectModel;
using BioMetrixCore.Info;

namespace BioMetrixCore
{
    public partial class Master : Form
    {
        string connStr =
                    "server=localhost;user=root;database=finger;port=3306;password=;";
        MySqlConnection conn;

        DeviceManipulator manipulator = new DeviceManipulator();
       // public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;

        


        private void ToggleControls(bool value)
        {
            btnBeep.Enabled = value;
            //btnDownloadFingerPrint.Enabled = value;
            btnPullData.Enabled = value;
            btnPowerOff.Enabled = value;
            btnRestartDevice.Enabled = value;
            btnGetDeviceTime.Enabled = value;
            btnEnableDevice.Enabled = value;
            btnDisableDevice.Enabled = value;
        }

        public Master()
        {
            InitializeComponent();
            ToggleControls(false);
            ShowStatusBar(string.Empty, true);
            DisplayEmpty();
            
                init();
                
        }


        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        ShowStatusBar( "The device is switched off", true);
                        DisplayEmpty();
                       
                        ToggleControls(false);
                        break;
                    }

                default:
                    break;
            }

        }


 


        public void ShowStatusBar(string message, bool type)
        {
            if (message.Trim() == string.Empty)
            {
                lblStatus.Visible = false;
                return;
            }

            lblStatus.Visible = true;
            lblStatus.Text = message;
            lblStatus.ForeColor = Color.White;

            if (type)
                lblStatus.BackColor = Color.FromArgb(79, 208, 154);
            else
                lblStatus.BackColor = Color.FromArgb(230, 112, 134);
        }


        private void PingDevice( Device device)
        {
            ShowStatusBar(string.Empty, true);

            string ipAddress = device.IP;

            bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
            if (!isValidIpA)
                throw new Exception("The Device IP is invalid !!");

            isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
            if (isValidIpA)
                ShowStatusBar(device.IP + " -> " + "The device is active", true);
            else
                ShowStatusBar(device.IP + " -> " + "Could not read any response", false);
        }

        private void GetAllUserID(ZkemClient objZkeeper, Device device)
        {
            try
            {
                ICollection<UserIDInfo> lstUserIDInfo = manipulator.GetAllUserID(objZkeeper, int.Parse(device.DeviceId));

                if (lstUserIDInfo != null && lstUserIDInfo.Count > 0)
                {
                    BindToGridView(lstUserIDInfo);
                    ShowStatusBar(device.IP + " -> " + lstUserIDInfo.Count + " records found !!", true);
                }
                else
                {
                    DisplayEmpty();
                    DisplayListOutput(device.IP + " -> " + "No records found");
                }

            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }

        private void Beep(ZkemClient objZkeeper)
        {
            objZkeeper.Beep(100);
        }

        private void DownloadFingerPrint(ZkemClient objZkeeper, Device device)
        {
            try
            {
                ShowStatusBar(string.Empty, true);

                ICollection<UserInfo> lstFingerPrintTemplates = manipulator.GetAllUserInfo(objZkeeper, int.Parse(device.DeviceId));
                if (lstFingerPrintTemplates != null && lstFingerPrintTemplates.Count > 0)
                {
                    BindToGridView(lstFingerPrintTemplates);
                    ShowStatusBar(device.IP + " -> " + lstFingerPrintTemplates.Count + " records found !!", true);
                }
                else
                    DisplayListOutput(device.IP +" -> "+"No records found");
            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }

        private Boolean GetLogsToMySql(ZkemClient objZkeeper, Device device)
        {
            Boolean status = false;
            if (device.status) {
            
            try
            {
                ShowStatusBar(string.Empty, true);

                ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, int.Parse(device.DeviceId));

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    Boolean clearedLog = false;

                    ShowStatusBar(device.IP +" -> "+ lstMachineInfo.Count + " records found !!", true);

                    Console.WriteLine("---");
                    Console.WriteLine(lstMachineInfo.Count);
                    IEnumerator enumerator = lstMachineInfo.GetEnumerator();
                        string theCommand = "INSERT INTO log (MachineNumber, IndRegID, DateTimeRecord ) VALUES ";
                        int count = 0;
                    while (enumerator.MoveNext())
                    {
                        MachineInfo item = (MachineInfo)enumerator.Current;
                        theCommand += "('" + item.MachineNumber + "', '" + item.IndRegID + "', '" + item.DateTimeRecord + "')";
                            count++;
                            if (count < lstMachineInfo.Count)
                            {
                                theCommand += ", ";
                            }
                       
                    }

                        Console.WriteLine(theCommand);
                        MySqlCommand command = conn.CreateCommand();
                        command.CommandText = theCommand;
                        Boolean mysqlInsertSuccess = false;
                        try
                        {
                            int o = command.ExecuteNonQuery();
                            mysqlInsertSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            mysqlInsertSuccess = false;
                        }
                       
                        if (mysqlInsertSuccess)
                        {
                            clearedLog = manipulator.ClearGLog(objZkeeper, int.Parse(device.DeviceId));
                        }
                        status = true;
                }
                else {
                    DisplayListOutput("No records found");
                    status = false;
                }

            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }
            }
            return status;
        }
        


        private void ClearGrid()
        {
            if (dgvRecords.Controls.Count > 2)
            { dgvRecords.Controls.RemoveAt(2); }


            dgvRecords.DataSource = null;
            dgvRecords.Controls.Clear();
            dgvRecords.Rows.Clear();
            dgvRecords.Columns.Clear();
        }
        private void BindToGridView(object list)
        {
            ClearGrid();
            dgvRecords.DataSource = list;
            dgvRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvRecords);
        }

      

        private void DisplayListOutput(string message)
        {
            if (dgvRecords.Controls.Count > 2)
            { dgvRecords.Controls.RemoveAt(2); }

            ShowStatusBar(message, false);
        }

        private void DisplayEmpty()
        {
            ClearGrid();
            dgvRecords.Controls.Add(new DataEmpty());
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        { UniversalStatic.DrawLineInFooter(pnlHeader, Color.FromArgb(204, 204, 204), 2); }



        private void PowerOff(ZkemClient objZkeeper, Device device)
        {
            this.Cursor = Cursors.WaitCursor;

            var resultDia = DialogResult.None;
            resultDia = MessageBox.Show("Do you wish to Power Off the Device ??", "Power Off Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDia == DialogResult.Yes)
            {
                bool deviceOff = objZkeeper.PowerOffDevice(int.Parse(device.DeviceId));

            }

            this.Cursor = Cursors.Default;
        }

        private void RestartDevice(ZkemClient objZkeeper,Device device)
        {

            DialogResult rslt = MessageBox.Show("Do you wish to restart the device now ??", "Restart Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rslt == DialogResult.Yes)
            {
                if (objZkeeper.RestartDevice(int.Parse(device.DeviceId)))
                    ShowStatusBar(device.IP + " -> " + "The device is being restarted, Please wait...", true);
                else
                    ShowStatusBar(device.IP + " -> " + "Operation failed,please try again", false);
            }

        }

        private void GetDeviceTime(ZkemClient objZkeeper, Device device)
        {
            int machineNumber = int.Parse(device.DeviceId);
            int dwYear = 0;
            int dwMonth = 0;
            int dwDay = 0;
            int dwHour = 0;
            int dwMinute = 0;
            int dwSecond = 0;

            bool result = objZkeeper.GetDeviceTime(machineNumber, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond);

            string deviceTime = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
            List<DeviceTimeInfo> lstDeviceInfo = new List<DeviceTimeInfo>();
            lstDeviceInfo.Add(new DeviceTimeInfo() { DeviceTime = deviceTime });
            BindToGridView(lstDeviceInfo);
        }
        

        private void EnableDevice(ZkemClient objZkeeper,Device device)
        {
            // This is of no use since i implemented zkemKeeper the other way
            bool deviceEnabled = objZkeeper.EnableDevice(int.Parse(device.DeviceId), true);

        }

     

        private void DisableDevice(ZkemClient objZkeeper, Device device)
        {
            // This is of no use since i implemented zkemKeeper the other way
            bool deviceDisabled = objZkeeper.DisableDeviceWithTimeOut(int.Parse(device.DeviceId), 3000);
        }

        

       

        private Boolean connectToMySql() {
        {
                Boolean status = false;
                 conn = new MySqlConnection(connStr);
                try
                {
                    ShowStatusBar("Connecting to MySQL...", false);
                    Console.WriteLine("Connecting to MySQL...");
                    conn.Open();
                    // Perform database operations 

                    ShowStatusBar("MySql Connected", true);
                    status = true;
                }
                catch (Exception ex)
                {
                    ShowStatusBar("Error Connecting to MySql", false);
                    Console.WriteLine(ex.ToString());
                    status = false;
                }

                return status;


            }
        }

       

        private void init() {
            while (!connectToMySql()) {
                Thread.Sleep(500);
            }
            getDevices();
            
            for (int i=0; i < devices.Count; i++)
            {
              //  Device device = devices[i].device;
                if (GetLogsToMySql(devices[i].objZkeeper, devices[i].device))
                {
                    devices[i].device.updatedAt = DateTime.Now;
                }

            }
            
            conn.Close();
            
        }

        List<Info.Combination> devices;

        private void getDevices() {
            string query = "SELECT * FROM devices";

            var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
            var reader = cmd.ExecuteReader();
            devices = new List<Info.Combination>();
            List<Info.Device> devices2 = new List<Info.Device>();
            while (reader.Read())
            {
                String DeviceName = (String)reader["DeviceName"];
                String DeviceId = (String)reader["DeviceId"];
                String IP = (String)reader["IP"];
                String Port = (String)reader["Port"];
                String Mac = (String)reader["Mac"];
                Int64 Id = (Int64)reader["Id"];

                Info.Device device = new Info.Device();
                device.DeviceId = DeviceId;
                device.DeviceName = DeviceName;
                device.IP = IP;
                device.Port = Port;
                device.Mac = Mac;
                device.Id = Id;
                
                ZkemClient objZkeeper = connectToDevice(device);
                Combination combination = new Combination();
                combination.device = device;
                combination.objZkeeper = objZkeeper;
                devices.Add(combination);
                devices2.Add(device);
            }
            reader.Close();

            BindToGridView(devices2);

        }

        private ZkemClient connectToDevice( Device device) {
            //Boolean status = false;
            ZkemClient objZkeeper =null;
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ShowStatusBar(string.Empty, true);

                string ipAddress = device.IP;
                string port = device.Port;
                

                int portNumber = 4370;
                if (!int.TryParse(port, out portNumber))
                    throw new Exception("Not a valid port number");

                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The Device IP is invalid !!");

                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The device at " + ipAddress + ":" + port + " did not respond!!");

                objZkeeper = new ZkemClient(RaiseDeviceEvent);
                device.status = objZkeeper.Connect_Net(ipAddress, portNumber);

                if (device.status)
                {
                    string deviceInfo = manipulator.FetchDeviceInfo(objZkeeper, int.Parse(device.DeviceId));
                    lblDeviceInfo.Text = deviceInfo;
                }

            }
            catch (Exception ex)
            {
                ShowStatusBar(ex.Message, false);
            }
            this.Cursor = Cursors.Default;

            return objZkeeper;
        }

        private void btnPullData_Click(object sender, EventArgs e)
        {

        }

        private void addUser(ZkemClient objZkeeper, Device device)
        {
            // manipulator.PushUserDataToDevice(objZkeeper, int.Parse(device.DeviceId), "xxxsssxxx");
            
            //ICollection<UserIDInfo> lstUserIDInfo = manipulator.GetAllUserID(objZkeeper, int.Parse(device.DeviceId));
            ICollection<UserInfo> lstUserInfo = manipulator.GetAllUserInfo (objZkeeper, int.Parse(device.DeviceId));
            //Console.WriteLine("-> user id count: " + lstUserIDInfo.Count);
            Console.WriteLine("-> user count: " + lstUserInfo.Count);
            
            UserInfo sinfo = new UserInfo();

            sinfo.EnrollNumber = "40";
            sinfo.Name = "Erfan";
            sinfo.FingerIndex = 0;
            sinfo.TmpData = "";
            sinfo.Privelage = 0;
            sinfo.Password = "123456";
            sinfo.Enabled = true;
            sinfo.iFlag = "1";
            sinfo.MachineNumber = 1;
            
            List <UserInfo> lstUserInfo2 = new List<UserInfo>();

            lstUserInfo2.Add(sinfo);
            try
            {
                manipulator.UploadFTPTemplate(objZkeeper, 1 , lstUserInfo2);

            }
            catch (Exception rrr) {
                Console.WriteLine("----------------error------------");
                Console.WriteLine(rrr);
            }
            lstUserInfo = manipulator.GetAllUserInfo(objZkeeper, int.Parse(device.DeviceId));
            //Console.WriteLine("-> user id count: " + lstUserIDInfo.Count);
            Console.WriteLine("-> user count: " + lstUserInfo.Count);

        }

        private void btnUploadUserInfo_Click(object sender, EventArgs e)
        {
            addUser(devices[0].objZkeeper, devices[0].device);
        }
    }
}
