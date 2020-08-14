using System;
using System.Web;
using System.IO;
using System.Net;
using System.Text;

namespace EasySystemAPI.Models
{
    public class SendMessage
    {
        public string MaskingName = "SMS Alert";
        public static void SendSMS(string[] args, string toNumber, string messageBody)
        {
            //Your code goes here
            String url = "https://brandedsmspakistan.com/sms/api/send.php";
            String result = "";
            String masking = "H3 TEST SMS";
            String message = HttpUtility.UrlEncode(messageBody);
            String strPost = "email=sajawal.superior@gmail.com&key=09e67ab9876d637c03ed4932b47927792e&masking=" + masking + "&number=" + toNumber + "&message=" + message;
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                //logger.ErrorException(ex.Message, ex);
            }
            finally
            {
                myWriter.Close();
            }
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader 
                sr.Close();
            }
        }


        //This is forward value(Dubai waly) SMS Sending class
        public void SendSMSTurab(string toNumber, string messageBody)
        {
            //Your code goes here
            String url = "https://my.forwardvaluesms.com/vendorsms/pushsms.aspx?&";
            String result = "";
            String message = HttpUtility.UrlEncode(messageBody);
            String strPost = "apikey=b7d8297f-9435-4c12-b19f-ce3221f92212&clientid=a56b9346-7003-4575-a3d9-d8cb9c1c9cb3&msisdn=" + toNumber + "&sid=Uni Solz&msg=" + message + "&fl=0";
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                //logger.ErrorException(ex.Message, ex);
            }
            finally
            {
                myWriter.Close();
            }
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader 
                sr.Close();
            }
        }


        //This is Pakistan SMS Sending class
        public void SendSMSTurabPak(string toNumber, string messageBody)
        {
            //string get10digit = toNumber.Substring(toNumber.Length - 10);
            //string completeNumber = "92" + get10digit;
            //Your code goes here
            String url = "http://bsms.supersolutions.pk/vendorsms/pushsms.aspx?";
            //String url = "http://app.smsportal.pk/app/smsapipost";
            String result = "";
            String message = HttpUtility.UrlEncode(messageBody);
            String strPost = "apikey=54e39b46-d4be-4427-b094-a3ce902b7965&clientid=7a8ef9fc-8cef-40ff-af1f-8826962e0495&msisdn=" + toNumber + "&sid=" + MaskingName + "&msg=" + message + "&fl=0";
            //String strPost = "key=5c1a8119a65ce &type= &title= &contacts=" + toNumber + "&senderid=M.M Contest &msg=" + message + "& time=&time_zone=";
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                //logger.ErrorException(ex.Message, ex);
            }
            finally
            {
                myWriter.Close();
            }
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader 
                sr.Close();
            }
        }


        public static void SendSMSTurabOld(string toNumber, string messageBody)
        {
            //var api = "http://bsms.supersolutions.pk/vendorsms/pushsms.aspx?apikey=abc&clientid=xyz&msisdn=919898xxxxxx&sid=SenderId&msg=test%20message&fl=0";

            //Your code goes here
            String url = "http://app.smsportal.pk/app/smsapipost";

            String result = "";
            String message = HttpUtility.UrlEncode(messageBody);
            String strPost = "key=5c1a8119a65ce &type= &title= &contacts=" + toNumber + "&senderid=M.M Contest &msg=" + message + "& time=&time_zone=";
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentLength = Encoding.UTF8.GetByteCount(strPost);
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                //logger.ErrorException(ex.Message, ex);
            }
            finally
            {
                myWriter.Close();
            }
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader 
                sr.Close();
            }
        }
    }
}

