﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NetEasePlayer_UWP.Models
{
    [DataContract]
    class Danmaku
    {
        [DataMember(Name = "channel_id")] public string Channel_id { get; set; }
        [DataMember(Name = "type")] public string Mode { get; set; }
        [DataMember(Name = "date")] public DateTime Date { get; set; }
        [DataMember(Name = "danmaku")] public string Text { get; set; }
        [DataMember(Name = "offset")] public TimeSpan Offset { get; set; }
    }
    // offset = Date - beginTime
    class DanmakuManager
    {
        public static DanmakuManager Instance = new DanmakuManager();

        private string uri = "http://45.77.107.93:8080";
        private HttpClient client = new HttpClient();
        private DataContractJsonSerializer danmakuSerializer = new DataContractJsonSerializer(typeof(Danmaku));
        private DataContractJsonSerializer danmakuArraySerializer = new DataContractJsonSerializer(typeof(Danmaku[]));

        public List<Danmaku> GetInitDanmaku()
        {
            List<Danmaku> ret = new List<Danmaku>();
            Danmaku d1 = new Danmaku
            {
                Channel_id = "test",
                Mode = "scroll",
                Date = new DateTime(2018, 05, 05),
                Text = "test1",
                Offset = TimeSpan.FromSeconds(3)
            };
            Danmaku d2 = new Danmaku
            {
                Channel_id = "test",
                Mode = "scroll",
                Date = new DateTime(2018, 05, 05),
                Text = "test222",
                Offset = TimeSpan.FromSeconds(10)
            };
            ret.Add(d1);ret.Add(d2);
            return ret;
        }
        //向服务器 POST 弹幕
        public void AddDanmaku(Danmaku d)
        {
            string url = uri + "/upload_danmaku";
            var request = (HttpWebRequest)WebRequest.Create(url);

            var postData = "channel_id="+d.Channel_id.ToString();
                postData += "&type=" + d.Mode.ToString();
                postData += "&date=" + d.Date.ToString("yyyy-MM-dd HH:mm:ss");
                postData += "&danmaku=" + d.Text.ToString();
            var data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
            
        }
        //从服务器请求弹幕
        // POST channel_id&beg_date&end_date
        // 解析 返回的xml文件
        public List<Danmaku> QueryDanmaku(String channel_id, DateTime begin, DateTime end)
        {
            List<Danmaku> ret = new List<Danmaku>();
            string url = uri + "/download_danmaku";
            var request = (HttpWebRequest)WebRequest.Create(url);

            var postData = "channel_id=" + channel_id;
            postData += "&beg_date=" + begin.ToString("yyyy-MM-dd HH:mm:ss");
            postData += "&end_date=" + end.ToString("yyyy-MM-dd HH:mm:ss"); 
            
            var data = Encoding.UTF8.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                //解析xml获得List<Danmaku>
                //....
                //计算每个Danmaku.offset


                Debug.WriteLine(responseString.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }

            return ret;
        }
        public async Task<Danmaku[]> Query()
        {
            var reponse = await client.GetAsync("http://localhost:56887/api/Students");
            var json = await reponse.Content.ReadAsStringAsync();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

            return danmakuArraySerializer.ReadObject(stream) as Danmaku[];
        }
    }
}
