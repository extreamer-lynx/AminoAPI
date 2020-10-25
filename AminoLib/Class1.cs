using System;
using System.IO;
using System.Net;
using Vlingo.UUID;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AminoLib
{
    class QueryAnswer
    {
        public string sid;
        public string auid;
        public string account;
    
    }

    public class AminoClient
    {
        private string deviceId = "";
        private string email;
        private string password;
        private bool isLogined = false;
        private string auid;
        private string sid;
        private const string prefix = "https://service.narvii.com/api/v1";


        public AminoClient (string deviceId, string email, string password)
        {
            this.deviceId = deviceId;
            this.email = email;
            this.password = password;
            Login();
        }

        public string getCommunities(int start, int size)
        {
           return post(prefix+"/g/s/community/joined?start=" + start + "&size="+ size, "");
        }

        public string getComunnityJoinedChats(int ndcId, int start, int size)
        {
            return post(prefix + "/x" + ndcId + "/s/chat/thread?type=joined-me&start=" + start + "&size=" + size, "");
        }

        public string getThreadMessages(int ndcId, string threadId, int start, int size, string stoptime)
        {
            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/message?start=" + start + "&size=" + size + "&stoptime=" + stoptime, "");
        }

        public string getThreadMessages(int ndcId, string threadId, int start, int size)
        {
            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId +"/message?start=" + start + "&size=" + size, "");
        }

        public string sendMessageInThread(int ndcId, string threadId, string content)
        {
            long timestamp = getSecondTimestamp();
            string body = "{\"attachedObject\": null, \"content\": \""+ content + " \","+
                " \"type\": 0, \"clientRefId\": "+ timestamp + " , \"timestamp\": "+ timestamp + "} ";

            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/message", body);
        }

        private void Login()
        {
            try
            {
                WebRequest request = WebRequest.Create(prefix + "/g/s/auth/login");
                request.Headers.Add(HttpRequestHeader.UserAgent, "Dalvik/2.1.0 (Linux; U; Android 10; Redmi 8A Build/QKQ1.191014.001 test; com.narvii.amino.master/3.4.33453)");
                request.Headers.Add("NDCDEVICEID", this.deviceId);
                request.Headers.Add("NDC-MSG-SIG", getMessageSignature());
                request.Headers.Add("NDCLANG", "en");
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"email\": \"" + this.email + "\"," +
                                  "\"secret\": \"0 " +this.password + "\"," +
                                  "\"deviceID\":\"" + this.deviceId + "\"," +
                                  "\"clientType\": 100," +
                                  "\"action\": \"normal\" ," +
                                  "\"timestamp\":" + getTimestamp() + " }";

                    streamWriter.Write(json);
                }

                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {

                        JObject jObject = JObject.Parse(reader.ReadLine());
                        foreach (var pair in jObject) {
                            if (pair.Key == "auid")
                                this.auid = pair.Value.ToString();
                            if (pair.Key == "sid")
                                this.sid = pair.Value.ToString();
                        }
                    }
                    this.isLogined = true;
                }
                response.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private long getTimestamp()
        {
            DateTime foo = DateTime.Now;
            return ((DateTimeOffset)foo).ToUnixTimeMilliseconds();
        }


        private long getSecondTimestamp()
        {
            DateTime foo = DateTime.Now;
            return ((DateTimeOffset)foo).ToUnixTimeSeconds();
        }


        //workers
        private string post(string url, string json)
        {
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.UserAgent, "Dalvik/2.1.0 (Linux; U; Android 10; Redmi 8A Build/QKQ1.191014.001 test; com.narvii.amino.master/3.4.33453)");
            request.Headers.Add("NDCDEVICEID", this.deviceId);
            request.Headers.Add("NDCAUTH", "sid=" + this.sid);
            request.Headers.Add("NDC-MSG-SIG", getMessageSignature());
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";


            if (json != "")
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
            }
            string resJson = "";
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        resJson += line;
                    }
                }
            }
            response.Close();
            return resJson;
        }

        private string getMessageSignature()
        {
            TimeBasedGenerator gen = new TimeBasedGenerator();
            return gen.GenerateGuid().ToString().Replace("-","").ToUpper().Substring(0,27);
        }
    }
}
