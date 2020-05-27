using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using WhatsappBroadcastHomeshop.Model;

namespace WhatsappBroadcastHomeshop
{
    class Program
    {
        public static int delaytime = 0;

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            delaytime = Convert.ToInt32(config.GetSection("MySettings").GetSection("IntervalInMinutes").Value);
            Thread _Individualprocessthread = new Thread(new ThreadStart(InvokeMethod));
            _Individualprocessthread.Start();
        }



        public static void InvokeMethod()
        {
            while (true)
            {
                //GetConnectionStrings();
                GetdataFromMySQL();
                Thread.Sleep(delaytime);
            }
        }

        //public static void GetConnectionStrings()
        //{
        //    string ServerName = string.Empty;
        //    string ServerCredentailsUsername = string.Empty;
        //    string ServerCredentailsPassword = string.Empty;
        //    string DBConnection = string.Empty;
        //    string ProgramCode = string.Empty;

        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
        //        var constr = config.GetSection("ConnectionStrings").GetSection("HomeShop").Value;
        //        MySqlConnection con = new MySqlConnection(constr);
        //        MySqlCommand cmd = new MySqlCommand("SP_HSGetAllConnectionstrings", con);
        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //        cmd.Connection.Open();
        //        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
        //        da.Fill(dt);
        //        cmd.Connection.Close();
        //        var result = dt.Rows.Count;
        //        for (int i = 0; i < result; i++)
        //        {
        //            DataRow dr = dt.Rows[i];
        //            ServerName = Convert.ToString(dr["ServerName"]);
        //            ServerCredentailsUsername = Convert.ToString(dr["ServerCredentailsUsername"]);
        //            ServerCredentailsPassword = Convert.ToString(dr["ServerCredentailsPassword"]);
        //            DBConnection = Convert.ToString(dr["DBConnection"]);
        //            ProgramCode = Convert.ToString(dr["ProgramCode"]);

        //            string ConString = "Data Source = " + ServerName + " ; port = " + 3306 + "; Initial Catalog = " + DBConnection + " ; User Id = " + ServerCredentailsUsername + "; password = " + ServerCredentailsPassword + "";
        //              GetdataFromMySQL(ConString, ProgramCode);
        //        }


        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //    finally
        //    {

        //        GC.Collect();
        //    }

            
        //}
        public static void GetdataFromMySQL()
        {
            var Programcode = string.Empty;
            var StoreCode = string.Empty;
            var CampaignCode = string.Empty;
            var MobileNumber = string.Empty;
            var EmailID = string.Empty;
            var MessageText = string.Empty;
            var SenderId = string.Empty;
            int ClientID = 0;
            int ID = 0;
            string apiResponse = string.Empty;
            string additionalInfo = string.Empty;
            string TemplateName = string.Empty;

            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
                var constr = config.GetSection("ConnectionStrings").GetSection("HomeShop").Value;
                string ClientAPIURL = config.GetSection("ConnectionStrings").GetSection("ClientAPIURL").Value;
                MySqlConnection con = new MySqlConnection(constr);
                MySqlCommand cmd = new MySqlCommand("SP_HSGetWhatsappBroadcastDetail", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                cmd.Connection.Close();
                var result = dt.Rows.Count;
                for (int i = 0; i < result; i++)
                {
                    DataRow dr = dt.Rows[i];
                    ID = Convert.ToInt32(dr["ID"]);
                    Programcode = Convert.ToString(dr["Programcode"]);
                    StoreCode = Convert.ToString(dr["StoreCode"]);
                    CampaignCode = Convert.ToString(dr["CampaignCode"]);
                    MobileNumber = Convert.ToString(dr["MobileNumber"]);
                    EmailID = Convert.ToString(dr["EmailID"]);
                    MessageText = Convert.ToString(dr["MessageText"]);
                    SenderId = Convert.ToString(dr["SenderId"]);
                    ClientID = Convert.ToInt32(dr["ClientID"]);
                    additionalInfo = Convert.ToString(dr["additionalInfo"]);
                    TemplateName = Convert.ToString(dr["TemplateName"]);

                    if (!String.IsNullOrEmpty(additionalInfo))
                    {
                        try
                        {
                            List<string> additionalList = new List<string>();
                            if (additionalInfo != null)
                            {
                                additionalList = additionalInfo.Split(",").ToList();
                            }
                            SendFreeTextRequest sendFreeTextRequest = new SendFreeTextRequest
                            {
                                To = MobileNumber.TrimStart('0').Length > 10 ? MobileNumber : "91" + MobileNumber.TrimStart('0'),
                                ProgramCode = Programcode,
                                TemplateName = TemplateName,
                                AdditionalInfo = additionalList
                            };

                            string apiReq = JsonConvert.SerializeObject(sendFreeTextRequest);
                            apiResponse = CommonService.SendApiRequest(ClientAPIURL + "api/ChatbotBell/SendCampaign", apiReq);
                            if (apiResponse.Equals("true"))
                            {
                                //UpdateResponseShare(objRequest.CustomerID, "Contacted Via Chatbot");
                            }
                        }
                        catch (Exception _ex)
                        {

                        }
                    }

                }
                

            }
            catch (Exception _ex)
            {

                throw;
            }
            finally
            {
                GC.Collect();
            }
        }
        public static void UpdateResponse(int ClientID, string Date, string ConString)
        {

            try
            {
                DataTable dt = new DataTable();
                MySqlConnection con = new MySqlConnection(ConString);
                MySqlCommand cmd1 = new MySqlCommand("SP_HSUpdateSMSBroadcastResponce", con);
                cmd1.Parameters.AddWithValue("@_clientID", ClientID);
                cmd1.Parameters.AddWithValue("@_date", Date);
                cmd1.Connection.Open();
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.ExecuteNonQuery();
                cmd1.Connection.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                GC.Collect();
            }

        }





    }
}
