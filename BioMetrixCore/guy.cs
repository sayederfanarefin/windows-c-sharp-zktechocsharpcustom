﻿using BioMetrixCore.Info;
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
        private void addUser(ZkemClient objZkeeper, Device device)
        {
            // manipulator.PushUserDataToDevice(objZkeeper, int.Parse(device.DeviceId), "xxxsssxxx");

            //ICollection<UserIDInfo> lstUserIDInfo = manipulator.GetAllUserID(objZkeeper, int.Parse(device.DeviceId));
            ICollection<UserInfo> lstUserInfo = manipulator.GetAllUserInfo(objZkeeper, int.Parse(device.DeviceId));
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

            List<UserInfo> lstUserInfo2 = new List<UserInfo>();

            lstUserInfo2.Add(sinfo);
            try
            {
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
                        while (enumerator.MoveNext())
                        {
                         

                            UserInfo item = (UserInfo)enumerator.Current;
                            theCommand += "('" + item.Name + "', '" + item.Privelage + "', '" + item.Password + "', '" + item.Name + "', '" + item.MachineNumber + "', '" + item.iFlag + "', '" + item.FingerIndex + "', '" + item.EnrollNumber + "', '" + 0 + "')";
                            count++;
                            if (count < lstUserInfo.Count)
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

    }



}
