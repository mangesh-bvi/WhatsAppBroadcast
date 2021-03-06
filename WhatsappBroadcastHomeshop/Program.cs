﻿using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                GetdataFromMySQL();
                Thread.Sleep(delaytime);
            }
        }

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
            MySqlConnection con = null;
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
                var constr = config.GetSection("ConnectionStrings").GetSection("HomeShop").Value;
                string ClientAPIURL = config.GetSection("ConnectionStrings").GetSection("ClientAPIURL").Value;
                con = new MySqlConnection(constr);
                MySqlCommand cmd = new MySqlCommand("SP_HSGetWhatsappBroadcastDetail", con)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Connection.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                cmd.Connection.Close();
               
                for (int i = 0; i < dt.Rows.Count; i++)
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
                            Thread.Sleep(30000);
                            if (apiResponse.Equals("true"))
                            {
                                string Responcetext = "Success";
                                UpdateResponse(ID, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Responcetext, 1);
                                string bellMobileNumber = MobileNumber.TrimStart('0').Length > 10 ? MobileNumber : "91" + MobileNumber.TrimStart('0');
                                MakeBellActive(bellMobileNumber, Programcode, ClientAPIURL);
                            }
                            else
                            {
                                string Responcetext = "Fail";
                                UpdateResponse(ID, DateTime.Now.ToString(), Responcetext, 2);
                            }
                        }
                        catch (Exception ex)
                        {
                            string Responcetext = ex.ToString();
                            UpdateResponse(ID, DateTime.Now.ToString(), Responcetext, 2);
                        }
                    }

                }
               
            }
            catch
            { 
               
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
                GC.Collect();
            }
        }
        public static void UpdateResponse(int ID, string Date, string Responcetext, int IsSend)
        {
            MySqlConnection con = null;
            try
            {
                DataTable dt = new DataTable();
                IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
                var constr = config.GetSection("ConnectionStrings").GetSection("HomeShop").Value;
                con = new MySqlConnection(constr);
                MySqlCommand cmd = new MySqlCommand("SP_HSUpdateBroadcastResponce", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@_iD", ID);
                cmd.Parameters.AddWithValue("@_date", Date);
                cmd.Parameters.AddWithValue("@_responcetext", Responcetext);
                cmd.Parameters.AddWithValue("@_isSend", IsSend);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch 
            {
                
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
                GC.Collect();
            }

        }


        /// <summary>
        /// MakeBellActive
        /// </summary>
        /// <param name="Mobilenumber"></param>
        /// <param name="ProgramCode"></param>
        /// <param name="ClientAPIURL"></param>
        /// <param name="TenantID"></param>
        /// <param name="UserID"></param>
        public static void MakeBellActive(string Mobilenumber, string ProgramCode, string ClientAPIURL)
        {
            try
            {
                NameValueCollection Params = new NameValueCollection
                {
                    { "Mobilenumber", Mobilenumber },
                    { "ProgramCode", ProgramCode }
                };
                string apiResponsechatbotBellMakeBellActive = CommonService.SendParamsApiRequest(ClientAPIURL + "api/ChatbotBell/MakeBellActive", Params);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
