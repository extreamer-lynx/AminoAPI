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


        public AminoClient(string deviceId, string email, string password)
        {
            this.deviceId = deviceId;
            this.email = email;
            this.password = password;
            Login();
        }

        public enum usersType
        {
            Curators = 1,
            Featured = 2,
            Leaders = 3,
            Summary = 4
        };

        // --- USER ENDPOINT ---

        public string getHeadlines(int start, int size)
        {
            return get(prefix + "/g/s/feed/headlines?start=" + start + "&size=" + size, "");
        }

        public string getAccount()
        {
            return get(prefix + "/g/s/account", "");

        }

        public string getAffiliations()
        {
            return get(prefix + "/g/s/account/affiliations?type=active", "");
        }

        public string getWallet()
        {
            return get(prefix + "/g/s/wallet", "");
        }

        public string getAUID()
        {
            return get(prefix + "/g/s/auid?deviceId="+this.deviceId, "");
        }

        public string getPublickBlockList()
        {
            return get(prefix + "/g/s/block/full-list", "");
        }

        public string searchUser(string query)
        {
            return get(prefix + "/g/s/search/amino-id-and-link?q=" + query + "&searchId=" + getTimeSig(), "");
        }

        public string getMembership()
        {
            return get(prefix + "/g/s/membership", "");
        }


        // --- COMUNIITY ENDPOINT ---

        public string getCommunityInfo(int ndcId)
        {
            return get(prefix + "/g/s-x"+ndcId+"/community/info?withInfluencerList=1&withTopicList=true", "");
        }

        public string getCommunityKindred(int ndcId, int start, int size)
        {
            return get(prefix + "/g/s-x" + ndcId + "/community/kindred?start=" + start + "&size=" + size, "");
        }

        public string getCommunities(int start, int size)
        {
            return post(prefix + "/g/s/community/joined?start=" + start + "&size=" + size, "");
        }

        public string linkIdentify(string url)
        {
            return get(prefix + "/g/s/community/link-identify?q=" + url, "");
        }

        public string getCommunityBlogCategory(int ndcId, int size)
        {
            return get(prefix + "/x" + ndcId + "/s/blog-category?size="+ size, "");
        }

        public string checkIn(int ndcId, int timezone)
        {
            string body = "{ \"timezone\":" + timezone + ", \"timestamp\":"+getTimestamp()+" }";
            return post(prefix + "/x" + ndcId + "/s/check-in", body);
        }

        public string checkInLottery(int ndcId, int timezone)
        {
            string body = "{ \"timezone\":" + timezone + ", \"timestamp\":" + getTimestamp() + " }";
            return post(prefix + "/x" + ndcId + "/s/check-in/lottery", body);
        }

        public string joinCommunity(int ndcId)
        {
            return post(prefix + "/x" + ndcId + "/s/community/join", "");
        }

        public string leaveCommunity(int ndcId)
        {
            return post(prefix + "/x" + ndcId + "/s/community/leave", "");
        }

        public string getStickerCollection(int ndcId)
        {
            return get(prefix + "/x" + ndcId + "/s/sticker-collection?type=my-active-collection&includeStickers=true", "");
        }
        
        public string getUserProfiles(int ndcId, usersType type, int start, int size)
        {
            string types = type == usersType.Curators ? "curators" : type == usersType.Featured ? "featured" : type == usersType.Leaders ? "leaders" : type == usersType.Summary ? "summary" : null;
            return get(prefix + "/x" + ndcId + "/s/user-profile?type=" + types+"&start=" + start + "&size=" + size, "");
        }


        // --- CHAT ENDPOINT ---

        public string joinCommunityChat(int ndcId, string threadId, string uid)
        {
            return post(prefix + "/x" + ndcId + "/s/chat/thread/"+ threadId + "/member/" + uid, "");
        }

        public string leaveCommunityChat(int ndcId, string threadId, string uid)
        {
            return delete(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/member/" + uid, "");
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

        public string deleteMessage(int ndcId, string threadId, string message)
        {
            return delete(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/message/" + message,"");
        }

        public string sendAvesomeMessage(int ndcId, string threadId, string content)
        {
            long timestamp = getSecondTimestamp();
            string body = "{\"attachedObject\": null, \"content\": \"" + content + " \"," +
                " \"type\": 100, \"clientRefId\": " + timestamp + " , \"timestamp\": " + timestamp + "} ";

            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/message", body);
        }

        public string getTippingList(int ndcId, string threadId, int start, int size)
        {
            return get(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/tipping/tipped-users?start=" + start + "&size=" + size, "");
        }

        public string tipChat(int ndcId, string threadId, int coins)
        {
            string body = "{\"coins\": "+coins+ ",\"tippingContext\": {\"transactionId\":\"" + getTimeSig() + "\"}, \"timestamp\":" + getTimestamp() + "}";
            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/tipping", body);
        }

        // --- CO-HOST ---

        public string getCoHostList(int ndcId, string threadId, int start, int size)
        {
            return get(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/co-host?size=" + size, "");
        }

        public string addCoHost(int ndcId, string threadId, string uid)
        {
            string body = "{\"uidList\": [\""+uid+"\"], \"timestamp\":"+getTimestamp()+"}";
            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/co-host", body);
        }

        public string changeAnouncement(int ndcId, string threadId, string message)
        {
            string body = "{\"extensions\": {\"announcement\": \""+message+"\"}, \"timestamp\":" + getTimestamp() + "}";
            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId, body);
        }

        public string inviteUsers(int ndcId, string threadId, string uid)
        {
            string body = "{\"uids\": [\"" + uid + "\"], \"timestamp\":" + getTimestamp() + "}";
            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/member/invite", body);
        }

        public string deleteFromChat(int ndcId, string threadId, string uid)
        {
            return delete(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/member/" + uid + "?allowRejoin=0", "");
        }

        public string deleteFromChatWithFlag(int ndcId, string threadId, string uid)
        {
            return delete(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/member/"+uid+ "?allowRejoin=1", "");
        }

        public string changeVVChatPremision(int ndcId, string threadId, int type)
        {
            string body = "{\"vvChatJoinType\": " + type + ", \"timestamp\":" + getTimestamp() + "}";
            return post(prefix + "/x" + ndcId + "/s/chat/thread/" + threadId + "/vvchat-permission", body);
        }

        // --- LIVE LAYER ---

        public string liveLayersPublicChats(int ndcId, int start, int size)
        {
            return get(prefix + "/x" + ndcId + "/s/live-layer/public-chats?start=" + start + "&size=" + size, "");
        }

        public string liveLayersUsersOnline(int ndcId, int start, int size)
        {
            return get(prefix + "/x" + ndcId + "/s/live-layer?topic=ndtopic%3Ax$"+ndcId+ "%3Aonline-members&start=" + start + "&size=" + size, "");
        }

        // --- WORKERS ---

        private void Login()
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
                                  "\"secret\": \"0 " + this.password + "\"," +
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
                        foreach (var pair in jObject)
                        {
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

        private string post(string url, string json)
        {
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.UserAgent, "Dalvik/2.1.0 (Linux; U; Android 10; Redmi 8A Build/QKQ1.191014.001 test; com.narvii.amino.master/3.4.33453)");
            request.Headers.Add("NDCDEVICEID", this.deviceId);
            request.Headers.Add("NDCAUTH", "sid=" + this.sid);
            request.Headers.Add("AUID", this.auid);
            request.Headers.Add("NDCLANG", "en");
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

        private string get(string url, string json)
        {
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.UserAgent, "Dalvik/2.1.0 (Linux; U; Android 10; Redmi 8A Build/QKQ1.191014.001 test; com.narvii.amino.master/3.4.33453)");
            request.Headers.Add("NDCDEVICEID", this.deviceId);
            request.Headers.Add("NDCAUTH", "sid=" + this.sid);
            request.Headers.Add("AUID", this.auid);
            request.Headers.Add("NDCLANG", "en");
            request.Headers.Add("NDC-MSG-SIG", getMessageSignature());
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "GET";

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

        private string delete(string url, string json)
        {
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.UserAgent, "Dalvik/2.1.0 (Linux; U; Android 10; Redmi 8A Build/QKQ1.191014.001 test; com.narvii.amino.master/3.4.33453)");
            request.Headers.Add("NDCDEVICEID", this.deviceId);
            request.Headers.Add("NDCAUTH", "sid=" + this.sid);
            request.Headers.Add("AUID", this.auid);
            request.Headers.Add("NDCLANG", "en");
            request.Headers.Add("NDC-MSG-SIG", getMessageSignature());
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "DELETE";


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

        private string getTimeSig()
        {
            TimeBasedGenerator gen = new TimeBasedGenerator();
            return gen.GenerateGuid().ToString();
        }
    }
}
