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
            timer = new System.Threading.Timer(UpdateProperty, null, 10000, 10000);
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
        private void addUser(Combination combination, UserInfo sinfo)
        {
            ZkemClient objZkeeper = combination.objZkeeper;
            Device device = combination.device;
           
            bool result = objZkeeper.SSR_SetUserInfo(sinfo.MachineNumber, sinfo.EnrollNumber, sinfo.Name, sinfo.Password, sinfo.Privelage, sinfo.Enabled);
            if (result) {
                bool resultt = objZkeeper.SetUserTmpExStr(sinfo.MachineNumber, sinfo.EnrollNumber, sinfo.FingerIndex, 1,sinfo.TmpData);
                
            }
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
                    Console.WriteLine("Debug...........Getting users for Device at: " + device.IP);

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
                            Console.WriteLine(checkStatement);

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
                            theCommand = theCommand.TrimEnd(' ');
                            theCommand = theCommand.TrimEnd(',');
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

            UserInfo sinfo = new UserInfo();
            sinfo.EnrollNumber = "41";
            sinfo.Name = "Sayed Erfan Arefin";
            sinfo.FingerIndex = 0;
            sinfo.TmpData = "TTNTUzIxAAAEcHcECAUHCc7QAAAccWkBAAAAhJ0kXHBEAJYPjwCVAJF/IwBYABIOewBlcJsPHQBpAEsOSHBrAJAPaACoAAp/fwBvAIwPeAB7cJcPbQCHAEoPMnCPAIYPnQBWAJZ/ZgCeAIsPlAC2cIUPfwDBANQPqnDCAJEPQgACAHh/kwDaAI4PggDjcHAPaAD8AL4PwXADARoPRADBAf1/VAAIAWsPOgAJcaUOkwAPAUgP63AlAZ0OegDjAXJ//QAmAbYO7AAucVQPxQArAeAPgXAsAYYP4QCCASB/eABRARIPNABQcaIOqABXAdoOKXBbAccPOgrbi/NyQwlTiPv9fIWH9GJ+KgR3BfoLuQ0bC/cA9Yd7/k5/dIatg/mDvIJJj8v5rvxWBrIPOXaog80H0vtn+yZ32/6TCHsEBP5eeB9/VQvi+GcDkvQbi3OAVgW2gKdx3Ps+DOfyX3zejTIBVXvK9IcJdWJ/hZ4OqvYajYcOkJBNi4KBZHQFgsf9IfI1C6KPQnhXGYcU6Ot4+zIKIfPu6U5nlRuVZiuXB1xrkNbuenIfDlaLkBML+E4GeIEuiv4C+44273b0eYGr7FoI73w6/Qead3Ryt4Oj5KLbCgQggQEG7SJYDQELBf//URTBWAkAfhDW/mY1DAByEhpikFrEsAwAYxQXVaH/+wwLAF4ZHsGTWMR3AZsZGsDBO8L5eAE9HBZKYtQAUW8bWUJZWv8FwBZwOSMWVHjBO8DEsPyUwAQANe8aWHQBKS8TSxTFJTNjwf9twFNnO1xkeQEVOBrCWARlDHATQxfCVGrMAFg4FsDCwD/BwQAhJw1OCQAKWNb/eiQFACRcF4jCAFQdEVFZDgBqtBZvIcFkSQwAgrcWUrBXWwgAC3MxIvspFwATdwb/gMD7GFr/wVvA/jkUBGOAAP1lU1Y6wPuwasADAA+Jxf8KcHCJD1n9ZAXAxlAJAAmLE8U4xPgnDwAJlRDEjm0zPAwAE6T9/wT8f47A/2AGAE1qhsSyhRAAVbMGgP77LcFKJQYACXMDxk0PAQ2/QMAFX/slwYYMAIHC1f/Fj8HAWMD7BMU+x/CJFQAOxPA7/1RBwVXAU8EswwC2tRJVwgYARgIAxI8yBAA+yn2zCgT524/BhHDBzwCSqxZk/2j+BcVD7ASNDwAQ7ec7//qwwf4+PcAWxRH9l8H+K8A2/YJY+y4IAGT5gMJYwHh9AWz8CTtX+0IBcA3/QGUEEAwCEjYFEMYGHFbAEF13+ykHEFEIssPHJggQUwxtwgfChLMZEBIX1v73KvtIR1hLQBYQ1iPesP87Kf49wDpgxLD/BxB2J4ABxMbvBRDyKCnBPgUUWC1XwZ4IEEcsc7DCw8aMBhACLSZAxQMQ4kkpBwQUDE4XVwgQpJGei7bHwhQQLFUD//mw/P/+/f3/BPz7j8H+/z0DEL9WJrUGEKVhoMIFxMi4EBBAZL0uOP74jfz+wTU0DNVaYLD//Pz8//o6wPqOEhDNZ7fEt3fHDcXBjf8=";
            sinfo.Privelage = 0;
            sinfo.Password = "123456";
            sinfo.Enabled = true;
            sinfo.iFlag = "1";
            sinfo.MachineNumber = 1;

            addUser(combination, sinfo);
            return status;
        }

    }



}
