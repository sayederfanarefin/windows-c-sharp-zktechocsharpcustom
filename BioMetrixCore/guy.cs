using BioMetrixCore.Info;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    class guy
    {
        private System.Threading.Timer timer;
       // string connStr = "server=localhost;user=root;database=finger;port=3306;password=;SslMode=none";
        string connStr = "server=178.128.52.75;user=root;database=sweet_hrm;port=3306;password=password;SslMode=none";
        MySqlConnection conn;

         DeviceManipulator manipulator = new DeviceManipulator();


        public void start()
        {
            timer = new System.Threading.Timer(UpdateProperty, null, 1000, 1000);
        }

        private string timeStampString()
        {
            return DateTime.Now.ToString("yyyy/MM/dd::HH:mm:ss:ffff");
        }

        private void UpdateProperty(object state)
        {
            lock (this)
            {

                Console.WriteLine(timeStampString());
                init();
            }
        }

        private Boolean connectToMySql()
        {
            {
                Boolean status = false;
                conn = new MySqlConnection(connStr);
                try
                {

                    Console.WriteLine("Connecting to MySQL...");
                    conn.Open();

                    status = true;
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                    status = false;
                }

                return status;


            }
        }
        private void addUser(Combination combination)
        {
            ZkemClient objZkeeper = combination.objZkeeper;
            Device device = combination.device;
            // manipulator.PushUserDataToDevice(objZkeeper, int.Parse(device.DeviceId), "xxxsssxxx");

            //ICollection<UserIDInfo> lstUserIDInfo = manipulator.GetAllUserID(objZkeeper, int.Parse(device.DeviceId));
            ICollection<UserInfo> lstUserInfo = manipulator.GetAllUserInfo(objZkeeper, int.Parse(device.DeviceId));
            //Console.WriteLine("-> user id count: " + lstUserIDInfo.Count);
            Console.WriteLine("-> user count: " + lstUserInfo.Count);

            UserInfo sinfo = new UserInfo();

            sinfo.EnrollNumber = "40";
            sinfo.Name = "666555666";
            sinfo.FingerIndex = 0;
            sinfo.TmpData = "Sj1TUzIxAAADfoEECAUHCc7QAAAbf2kBAAAAg6MYnH4VAIwPawCNAHRxlgB5AIgP2AB4fuUPcACAAKsPs36fACIPMABsAFNxawCpAFoPQgC4fmMONwC+ABYPPn7HAEwPUAAnAENxQgDzAEYPEAD3fqcPXwD8APEPV34JAUAOPQDwAclxxgA5AZEPHQA6f5cOiAA8AVIPrn5FAYwPxwCOAYtxhwBQAYYPaQBZf4UPVxc7B4IjRHQu8+vvch7na1hNzpXPE8+by+pZaNL6Bp/HwTa3ZgBjBONtPvkr7pBstwkHQdP3EYti+NeX2Hvm/JsNnoNvDOr1FQmkC0ppGhMnFyMjaPTajfv0IQzl/zsCiPN6h4+b8fz0CWJ5bARJEmIP6BHxcAYUNQoBByD/uofY9oX/wfHDBu2AvPrq/IoCB0YnXjQBAj0gC8IAfnwI/zfBHAHPBTRIXMDAwVHAncFWvsHAwVoDALgKCoEIAHcUAz46wTl3AWsdAEDAhwoDDiYDOMBM/8kAZU4HwTb//1g6DgMwQ/Q1Nj7B8AgDGUR6hcKEDMVvS34uQGjACQCsS3e+aIgDACdRrMIRfiF34jtY/AX+Pb5FDgCTeIxUwcK8f3QHAGyBtcLD7BQAE5DX/QVAOL/9Rv9TTwfFs6NeREYWABClH0/DTv7+//44VTrB/GwBM6XX/v/r/vyB/v3B/sD/oQsDEKfw/fv+/oZgD35nqWSg/8GYQQV+LaxTwHEIxTKuMkRZCgBqrZvCkb//fxYA6bJbwGEXwcLCiHXBBG4Qfjq70C5B+zo+/YHBWxAAg7uuxcDsw8HBwMT/RgsD+MET/EJyd80ANrxSeFUTAD8H0zOA/iEpWMH+Bg0DAcVXxMKWgLkNA/3JUMTCiYCnCgNHy0xZ/1X/zwA8tEdMZEYLAEvOJIFrwcHBwf/JAIquMcDCcsF2OwYDctdDXcMLAEHYN7/AhMHBwf/PAEybQcE//FMUxV/0xDL+HPspwaPAY3kBQ/lJVsA4CQMd/jTAncGI2xDseZ///sDBVgWRpL/CwsLB/8AFwMK+///+BhBTyzr9hVccEOYYmolEw77Cw8fFw8NOxPy8w8HBwXwE1WEZWYoJEGExFgTBX738BBAUQDoFwwdu8VaT/8cF1YFT95kQEEVY4AX8+Tp/XlsHEKicgPwjxQ0QVmT3OHNhPf8A";
            sinfo.Privelage = 0;
            sinfo.Password = "123456";
            sinfo.Enabled = true;
            sinfo.iFlag = "1";
            sinfo.MachineNumber = 1;

            List<UserInfo> lstUserInfo2 = new List<UserInfo>();

            IEnumerator enumerator = lstUserInfo.GetEnumerator();
           
            while (enumerator.MoveNext())
            {
                UserInfo item = (UserInfo)enumerator.Current;
                lstUserInfo2.Add(item);
            }


                lstUserInfo2.Add(sinfo);

            for (int l =0; l < lstUserInfo2.Count; l++) {
                Console.WriteLine("---------->" + lstUserInfo2[l].Name);
            }
            try
            {
                Console.WriteLine("size written: "+ lstUserInfo2.Count);
                objZkeeper.BatchUpdate(1);
                objZkeeper.SetUserTmpExStr(1, sinfo.EnrollNumber, sinfo.FingerIndex, 1, sinfo.TmpData);

                manipulator.UploadFTPTemplate(objZkeeper, 1, lstUserInfo2);
               

            }
            catch (Exception rrr)
            {
                Console.WriteLine("----------------error------------");
                Console.WriteLine(rrr);
            }
            lstUserInfo = manipulator.GetAllUserInfo(objZkeeper, int.Parse(device.DeviceId));
            //Console.WriteLine("-> user id count: " + lstUserIDInfo.Count);
            Console.WriteLine("-> user count: " + lstUserInfo.Count);

        }
        private async Task connectToDevice(Device device, Action<Boolean> callback)
        {

            if (PingDevice(device)) {
                ZkemClient objZkeeper = null;
                try
                {

                    string ipAddress = device.IP;
                    string port = device.Port;


                    int portNumber = 4370;
                    
                    objZkeeper = new ZkemClient(RaiseDeviceEvent);
                    device.status = objZkeeper.Connect_Net(ipAddress, portNumber);

                    if (device.status)
                    {
                        string deviceInfo = manipulator.FetchDeviceInfo(objZkeeper, int.Parse(device.DeviceId));
                        Console.WriteLine("Device at: "+ ipAddress + " is now Connected");
                    }

                    Combination combination = new Combination();
                    combination.device = device;
                    combination.objZkeeper = objZkeeper;
                    devices.Add(combination);
                    devices2.Add(device);
                    Boolean status = GetLogsToMySql(combination);
                    GetUsersToMySql(combination);
                    objZkeeper.Disconnect();
                    callback(status);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);

                }

            }
           
        }

        private Boolean PingDevice(Device device)
        {

            Boolean status = false;

            string ipAddress = device.IP;

            bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
            if (!isValidIpA) {
                status = false;
                Console.WriteLine("The Device IP is invalid !!");
            }
               
            isValidIpA = UniversalStatic.PingTheDevice(ipAddress);

            if (isValidIpA)
            {
                status = true;
                Console.WriteLine(device.IP + " -> " + "The device is active");
            }

            else {
                status = false;
                Console.WriteLine(device.IP + " -> " + "Could not read any response");
            }

            return status;
        }

        private Boolean GetLogsToMySql(Combination combination)
        {

            Device device = combination.device;
            ZkemClient objZkeeper = combination.objZkeeper;
            Boolean status = false;
            if (device.status)
            {
                Console.WriteLine("Getting logs for Device at: " + device.IP);

                try
                {
                   
                    ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, int.Parse(device.DeviceId));

                    if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                    {
                        Boolean clearedLog = false;
                        
                        Console.WriteLine(lstMachineInfo.Count);
                        IEnumerator enumerator = lstMachineInfo.GetEnumerator();
                        string theCommand = "INSERT INTO logs (machine_number, ind_reg_id, date_time_record ) VALUES ";
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

                       // Console.WriteLine(theCommand);
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
                    else
                    {
                        Console.WriteLine("changes 0");
                        status = false;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return status;
        }

        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                       
                        break;
                    }

                default:
                    break;
            }

        }


        private async void getDevices()
        {
            string query = "SELECT * FROM devices";

            var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
            var reader = cmd.ExecuteReader();
            devices = new List<Info.Combination>();

            List<Info.Device> tempDevices = new List<Info.Device>();

            while (reader.Read())
            {
                String DeviceName = (String)reader["device_name"];
                String DeviceId = (String)reader["device_id"];
                String IP = (String)reader["ip"];
                String Port = (String)reader["port"];
                String Mac = (String)reader["mac"];
                Int64 Id = (Int64)reader["id"];

                Info.Device device = new Info.Device();
                device.DeviceId = DeviceId;
                device.DeviceName = DeviceName;
                device.IP = IP;
                device.Port = Port;
                device.Mac = Mac;
                device.Id = Id;

                tempDevices.Add(device);

            }
            reader.Close();
            for (int ii=0; ii < tempDevices.Count; ii++) {
                await connectToDevice(tempDevices[ii], deviceConnected);
            }
            
            
            
        }

        private void deviceConnected(Boolean status)
        {
            // BindToGridView(devices2);
            Console.WriteLine("Done. Status:" + status.ToString());

        }

        private void init()
        {
            while (!connectToMySql())
            {
                Thread.Sleep(500);
            }
            getDevices();
            conn.Close();

        }

        List<Info.Combination> devices;
        List<Info.Device> devices2 = new List<Info.Device>();


        private Boolean GetUsersToMySql(Combination combination)
        {

            Device device = combination.device;
            ZkemClient objZkeeper = combination.objZkeeper;
            Boolean status = false;
            if (device.status)
            {
                Console.WriteLine("Getting users for Device at: " + device.IP);

                try
                {

                    ICollection<UserInfo> lstUserInfo = manipulator.GetAllUserInfo (objZkeeper, int.Parse(device.DeviceId));

                    if (lstUserInfo != null && lstUserInfo.Count > 0)
                    {
                        Boolean clearedLog = false;

                        Console.WriteLine(lstUserInfo.Count);
                        IEnumerator enumerator = lstUserInfo.GetEnumerator();
                        string theCommand = "INSERT INTO user_info (tmp_data, privilege, password, name, machine_number, i_flag, finger_index, enroll_number, enabled ) VALUES ";
                        int count = 0;
                        int countShouldTheCommandBeExecuted = 0;
                        while (enumerator.MoveNext())
                        {
                            UserInfo item = (UserInfo)enumerator.Current;

                            string checkStatement = "SELECT * FROM user_info WHERE machine_number=" + item.MachineNumber + " AND enroll_number='" + item.EnrollNumber + "' AND tmp_data='" + item.TmpData + "'";
                           // Console.WriteLine(checkStatement);

                            var cmd = new MySql.Data.MySqlClient.MySqlCommand(checkStatement, conn);
                            var reader = cmd.ExecuteReader();

                            int c = 0;
                            while (reader.Read())
                            {
                                c++;
                            }


                            reader.Close();

                            if (c > 0)
                            {
                                Console.WriteLine("----------------->");
                            }
                            else {
                                countShouldTheCommandBeExecuted++;

                                int shit = 0;
                                if (item.Enabled) { shit = 1; }
                                theCommand += "('" + item.TmpData + "', '" + item.Privelage + "', '" + item.Password + "', '" + item.Name + "', '" + item.MachineNumber + "', '" + item.iFlag + "', '" + item.FingerIndex + "', '" + item.EnrollNumber + "', '" + shit + "')";
                                count++;
                                if (count < lstUserInfo.Count)
                                {
                                    theCommand += ", ";
                                }

                            }


                        }


                        if (countShouldTheCommandBeExecuted > 0) {
                         //   Console.WriteLine(theCommand);
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
                        
                    }
                    else
                    {
                        Console.WriteLine("changes 0");
                        status = false;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }



            //addUser(combination);
            return status;
        }

    }



}
