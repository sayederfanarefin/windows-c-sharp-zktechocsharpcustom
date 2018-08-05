using BioMetrixCore.Info;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BioMetrixCore
{
    class guy
    {


        //178.128.223.188


        // string connStr = "server=localhost;user=root;database=finger;port=3306;password=;SslMode=none";
        //dev
        //string connStr = "server=178.128.52.75;user=root;database=sweet_hrm;port=3306;password=password;SslMode=none";

        //production
        string connStr = "server=178.128.223.188;user=root;database=sweet_hrm;port=3306;password=Sweetitech#;SslMode=none";
        MySqlConnection conn;

        string config = "";


        DeviceManipulator manipulator = new DeviceManipulator();


        

        private string timeStampString()
        {
            return DateTime.Now.ToString("yyyy/MM/dd::HH:mm:ss:ffff");
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
        private void addUser(Combination combination, UserInfo sinfo)
        {
            Console.WriteLine("Adding user " + sinfo.Name + "to Device: " + combination.device.IP);
            ZkemClient objZkeeper = combination.objZkeeper;
            Device device = combination.device;

            bool result = objZkeeper.SSR_SetUserInfo(sinfo.MachineNumber, sinfo.EnrollNumber, sinfo.Name, sinfo.Password, sinfo.Privelage, sinfo.Enabled);
            if (result)
            {
                bool resultt = objZkeeper.SetUserTmpExStr(sinfo.MachineNumber, sinfo.EnrollNumber, sinfo.FingerIndex, 1, sinfo.TmpData);

            }
        }
        private void connectToDevice(Device device)
        {


            if (device.officeCode.Equals(config) && PingDevice(device))
            {
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
                        Console.WriteLine("Device at: " + ipAddress + " is now Connected");
                    }

                    Combination combination = new Combination();
                    combination.device = device;
                    combination.objZkeeper = objZkeeper;
                    devices.Add(combination);
                    devices2.Add(device);
                    Boolean status = GetLogsToMySql(combination);
                    Boolean status2 = GetUsersToMySql(combination);

                    if (!status && !status2)
                    {
                        Console.WriteLine("---------------------------->Restart required. Device Restarting..." + device.DeviceId);
                        Boolean returned = objZkeeper.RestartDevice(Int32.Parse(device.DeviceId.Trim()));
                        Console.WriteLine(returned);
                       objZkeeper.RestartDevice(Int32.Parse(device.DeviceId));
                    }
                    objZkeeper.Disconnect();
                  
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
            if (!isValidIpA)
            {
                status = false;
                Console.WriteLine("The Device IP is invalid !!");
            }

            isValidIpA = UniversalStatic.PingTheDevice(ipAddress);

            if (isValidIpA)
            {
                status = true;
                Console.WriteLine(device.IP + " -> " + "The device is active");
            }

            else
            {
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


        private  void getDevices()
        {
            string query = "SELECT * FROM devices where officeCode = '" + config + "'";

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
                String officeCode = (String)reader["officeCode"];

                Info.Device device = new Info.Device();
                device.DeviceId = DeviceId;
                device.DeviceName = DeviceName;
                device.IP = IP;
                device.Port = Port;
                device.Mac = Mac;
                device.Id = Id;
                device.officeCode = officeCode;

                tempDevices.Add(device);

            }
            reader.Close();
            for (int ii = 0; ii < tempDevices.Count; ii++)
            {
                 connectToDevice(tempDevices[ii]);
            }



        }

        private void deviceConnected(Boolean status)
        {
            // BindToGridView(devices2);
            Console.WriteLine("Done. Status:" + status.ToString());

        }

        public void init(String config)
        {
            Console.WriteLine(timeStampString());
            this.config = config;
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
                ICollection<UserInfo> lstUserInfo = manipulator.GetAllUserInfo(objZkeeper, int.Parse(device.DeviceId));

                if (lstUserInfo != null && lstUserInfo.Count > 0)
                {
                    Boolean clearedLog = false;

                    Console.WriteLine(lstUserInfo.Count);
                    IEnumerator enumerator = lstUserInfo.GetEnumerator();
                    string theCommand = "INSERT INTO user_info (tmp_data, privilege, password, name, machine_number, i_flag, finger_index, enroll_number, enabled , officeCode) VALUES ";
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
                        { c++; }
                        reader.Close();

                        if (c > 0)
                        {
                            Console.WriteLine("----------------->");
                        }
                        else
                        {
                            countShouldTheCommandBeExecuted++;

                            int shit = 0;
                            if (item.Enabled) { shit = 1; }
                            theCommand += "('" + item.TmpData + "', '" + item.Privelage + "', '" + item.Password + "', '" + item.Name + "', '" + item.MachineNumber + "', '" + item.iFlag + "', '" + item.FingerIndex + "', '" + item.EnrollNumber + "', '" + shit + "', '" + config + "' )";
                            count++;
                            if (count < lstUserInfo.Count)
                            {
                                theCommand += ", ";
                            }
                        }
                    }


                    if (countShouldTheCommandBeExecuted > 0)
                    {
                        theCommand = theCommand.TrimEnd(' ');
                        theCommand = theCommand.TrimEnd(',');
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
                        
                    }
                    status = true;

                }
                else
                {
                    Console.WriteLine("changes 0");
                   // status = false;
                }



                pushToMachine(combination);

            }




            return status;
        }

        private void pushToMachine(Combination combination)
        {
            Console.WriteLine("pushing to machine");

            string query = "SELECT * FROM user_info WHERE officeCode = '" + config + "'";
            var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
            var reader = cmd.ExecuteReader();
            List<UserInfo> usersFromServer = new List<UserInfo>();


            while (reader.Read())
            {
                UserInfo sinfo = new UserInfo();
                sinfo.EnrollNumber = (string)reader["enroll_number"];
                sinfo.Name = (string)reader["name"];
                sinfo.FingerIndex = (int)reader["finger_index"];
                sinfo.TmpData = (string)reader["tmp_data"];



                sinfo.Privelage = (int)reader["privilege"];
                sinfo.Password = (string)reader["password"];

                var obj = (object)(sbyte)reader["enabled"];
                var enabled = (int)(sbyte)obj;

                if (enabled == 1) { sinfo.Enabled = true; } else { sinfo.Enabled = false; }

                // sinfo.Enabled = true;
                sinfo.iFlag = (string)reader["i_flag"];
                sinfo.MachineNumber = (int)reader["machine_number"];
             //   reader.Close();
                usersFromServer.Add(sinfo);
            }

            Console.WriteLine("Number of users retrived from server: " + usersFromServer.Count);


            ICollection<UserInfo> userFromDevice = manipulator.GetAllUserInfo(combination.objZkeeper, int.Parse(combination.device.DeviceId));
            for (int k = 0; k < usersFromServer.Count(); k++)
            {
                UserInfo tempUser = usersFromServer[k];

                Console.WriteLine("--------------------> User from server: " + tempUser.Name);
                Boolean isUserInDevice = false;

                IEnumerator enamurator = userFromDevice.GetEnumerator();
                while (enamurator.MoveNext())
                {
                    UserInfo item = (UserInfo)enamurator.Current;
                    if (item.EnrollNumber.Equals(tempUser.EnrollNumber) && item.TmpData.Equals(tempUser.TmpData))
                    {
                         isUserInDevice = true;
                       // Console.WriteLine("Updating " + tempUser.Name + " for device: " + combination.device.IP);
                        break;
                    }
                }
                if (!isUserInDevice)
                {
                    addUser(combination, tempUser);
                }
                else
                {
                    Console.WriteLine("I will not add the user: " + tempUser.Name);
                }


            }


            reader.Close();
        }


    }
}
