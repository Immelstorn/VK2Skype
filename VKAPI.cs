using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using VK2Skype.Entities;

namespace VK2Skype
{
    public class VKAPI
    {
        #region vars

        private const string AccessToken ="";

        private string _responseUri;
        private string _stream;
	    private const int ChatId = 2;// 13;
        private readonly NameValueCollection _qs = new NameValueCollection();
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private int _lastMid;
        private bool _isFirst = true;
        public delegate void CaptchaEventHandler(object sender, CaptchaEventArgs e);
        public event CaptchaEventHandler CaptchaNeeded;

        #endregion

        public List<Message> MessagesGet()
        {
            if (_lastMid != 0)
            {
                _qs["last_mid"] = _lastMid.ToString();
                _isFirst = false;
            }
            else
            {
                _qs["count"] = "4";
            }

            var result = ExecuteCommandJson("messages.get", _qs);
            var rsp = JsonConvert.DeserializeObject<Response<List<object>>>(result);
            var msgList = new List<Message>();

            for (int i = 1; i < rsp.response.Count; i++)
            {
                msgList.Add(JsonConvert.DeserializeObject<Message>(rsp.response[i].ToString()));
            }

            _lastMid = msgList.Count > 0 ? msgList[0].mid : _lastMid;

            return _isFirst ? new List<Message>() : msgList;
        }

        public string GetUser(int uid)
        {
            _qs["uids"] = uid.ToString();
            _qs["fields"] = "first_name,last_name";

            var result = ExecuteCommandJson("users.get", _qs);
            var rsp = JsonConvert.DeserializeObject<Response<List<User>>>(result);
            return rsp.response[0].first_name + " " + rsp.response[0].last_name;
        }

        public string GetOnlineStatus()
        {
            _qs["chat_id"] = ChatId.ToString();
            _qs["fields"] = "online";

            var result = ExecuteCommandJson("messages.getChat", _qs);
            var rsp = JsonConvert.DeserializeObject<Response<Online>>(result);
            return rsp.response.users.Aggregate("",
                                                (current, user) =>
                                                current +
                                                (user.first_name + " " + user.last_name +
                                                 (user.online == 1 ? " online\n" : " offline\n")));
        }

        private string ExecuteCommandJson(string name, NameValueCollection qs)
        {
            var url = String.Format("https://api.vk.com/method/{0}?access_token={1}&{2}", name, AccessToken,
                                    String.Join("&", from item in qs.AllKeys select item + "=" + qs[item]));
            GetRequestAndResponse(url, out _responseUri, out _stream);
            var res = _stream;
            if (res.Contains("Captcha needed"))
            {
                if (CaptchaNeeded!=null)
                {
                    CaptchaNeeded(this, new CaptchaEventArgs("Капча"));
                }

                Logs.WriteLog("log.txt", "Captcha needed");
                var rsp = JsonConvert.DeserializeObject<Captcha>(res);
                Console.WriteLine(rsp.error.captcha_img);
                var captha = Console.ReadLine();
                qs["captcha_sid"] = rsp.error.captcha_sid;
                qs["captcha_key"] = captha;
                res = ExecuteCommandJson(name, qs);
            }
            qs.Clear();
            return res;
        }

        public int MessageSend(string msg)
        {
            _qs["chat_id"] = ChatId.ToString();
            _qs["message"] = msg;
            _qs["guid"] = (msg + DateTime.Now.Ticks).GetHashCode().ToString();

            var result = ExecuteCommandJson("messages.send", _qs);
            var rsp = JsonConvert.DeserializeObject<Response<int>>(result);

            int mid = rsp.response;

            _lastMid = mid != 0 ? mid : _lastMid;
            return mid;
        }

        private void GetRequestAndResponse(string uri, out string responseUri, out string stream,
                                           string method = "GET")
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;
            if (request != null)
            {
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0";
                request.CookieContainer = _cookieContainer;
//                request.CookieContainer.Add(new Uri(uri), new Cookie("remixtst", "-7200"));
                request.CookieContainer.Add(new Uri(uri),
                                            new Cookie("remixsid",
                                                       "5d17611cd94201b12443cff239df8d152930110f1a9a7197b4f82"));
//                request.CookieContainer.Add(new Uri(uri), new Cookie("remixrefkey", "afddcc94ad6d574642"));
                request.CookieContainer.Add(new Uri(uri), new Cookie("remixlang", "0"));
                request.CookieContainer.Add(new Uri(uri), new Cookie("remixflash", "0.0.0"));
//                request.CookieContainer.Add(new Uri(uri), new Cookie("remixfeed", "*.*.*.*.*.pr.*.*"));
                request.CookieContainer.Add(new Uri(uri), new Cookie("remixdt", "-3600"));

                request.Method = method;
            }
            stream = "";
            responseUri = "";
            if (request != null)
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        using (var receiveStream = response.GetResponseStream())
                        {
                            if (receiveStream != null)
                            {
                                var readStream = new StreamReader(receiveStream, Encoding.UTF8);
                                stream = readStream.ReadToEnd();
                            }
                        }
                        responseUri = response.ResponseUri.ToString();
                        _cookieContainer.Add(response.Cookies);
                    }
                }
        }
    }

    public class CaptchaEventArgs
    {
        public CaptchaEventArgs(string s) { Text = s; }
        public String Text { get; private set; } // readonly
    }

}