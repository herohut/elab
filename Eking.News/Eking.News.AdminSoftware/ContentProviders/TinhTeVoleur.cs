﻿using System;
using System.Collections.Generic;
using System.Linq;
using Eking.News.Models;
using HtmlAgilityPack;

namespace Eking.News.AdminSoftware.ContentProviders
{
    public class TinhTeVoleur : BaseHtmlParserVoleur
    {
        public TinhTeVoleur(NewsObjectContext db = null)
            : base(db)
        {
        }

        protected override IEnumerable<string> GetMasterLinks()
        {
            var output = new List<string> { "http://www.tinhte.vn/" };
            for (int i = 2; i < 10; i++)
            {
                output.Add("http://www.tinhte.vn/?page=" + i);
            }

            return output;
        }

        private Source _source;

        protected override Source GetSource()
        {
            if (_source == null)
            {
                _source = Db.Sources.SingleOrDefault(s => s.Name == "TinhTe") ??
                          new Source { HomeUrl = "http://tinhte.vn", Name = "TinhTe" };
            }

            return _source;
        }

        protected override bool CancelJobIfVisitExistLink
        {
            get { return true; }
        }

        protected override void ReadEntryInfo(RawEntry entry, HtmlDocument document)
        {
            var node = document.DocumentNode.SelectSingleNode("//div[@class='titleBar']");

            var title = node.ChildNodes["h1"].InnerText;

            node = document.DocumentNode.SelectSingleNode("//span[@class='crumbs']");
            var groupInfo = node.InnerText;

            node = document.DocumentNode.SelectSingleNode("//blockquote[@class='messageText ugc baseHtml']");
            var content = node.InnerHtml.Trim();

            if (string.IsNullOrEmpty(content))
                throw new Exception("Unexpected text");

            string group = null;

            if (groupInfo.ContainsInOrder("Điện thoại", "iOS (iPad, iPhone, iPod Touch)"))
                group = "Technology/Mobile, Tablet/Apple";
            else if (groupInfo.ContainsInOrder("Điện thoại", "Android"))
                group = "Technology/Mobile, Tablet/Android";
            else if (groupInfo.ContainsInOrder("Điện thoại", "BlackBerry"))
                group = "Technology/Mobile, Tablet/BlackBerry";
            else if (groupInfo.ContainsInOrder("Điện thoại", "Windows Phone"))
                group = "Technology/Mobile, Tablet/Windows Phone";
            else if (groupInfo.ContainsInOrder("Điện thoại", "Symbian"))
                group = "Technology/Mobile, Tablet/Symbian";
            else if (groupInfo.ContainsInOrder("Điện thoại", "Meego"))
                group = "Technology/Mobile, Tablet/Meego";
            else if (groupInfo.ContainsInOrder("Điện thoại", "Palm - HP WebOS"))
                group = "Technology/Mobile, Tablet/Misc";

            else if (groupInfo.ContainsInOrder("Điện thoại", "Điện thoại phổ thông"))
                group = "Technology/Mobile, Tablet/Misc";

            else if (groupInfo.ContainsInOrder("Điện thoại", "Lập trình cho Di động"))
                group = "Technology/Mobile, Tablet/Misc";

            else if (groupInfo.ContainsInOrder("Tin công nghệ", "Tin PR - Khuyến mãi"))
                group = "Technology/Misc";

            else if (groupInfo.ContainsInOrder("Tin công nghệ", "Tin tức - Sự kiện"))
                group = "Technology/Misc";
            else if (groupInfo.ContainsInOrder("Tin công nghệ", "Thăm dò công nghệ"))
                return;

            else if (groupInfo.ContainsInOrder("Khoa học &amp; Công nghệ", "Dịch vụ trực tuyến"))
                group = "Technology/OnlineServices";

            else if (groupInfo.ContainsInOrder("Khoa học &amp; Công nghệ", "Khoa học &amp; Công nghệ"))
                group = "Technology";

            else if (groupInfo.ContainsInOrder("Máy ảnh số - Nhiếp ảnh", "Máy ảnh số"))
                group = "Technology/Photograph/Camera";

            else if (groupInfo.ContainsInOrder("Máy ảnh số - Nhiếp ảnh", "Máy quay phim"))
                group = "Technology/Photograph/Filming";

            else if (groupInfo.ContainsInOrder("Phương tiện vận chuyển", "Xe ô tô"))
                group = "Technology/Transportation/Car";

            else if (groupInfo.ContainsInOrder("Phương tiện vận chuyển", "Xe máy"))
                group = "Technology/Transportation/Motobike";
            else if (groupInfo.ContainsInOrder("Phương tiện vận chuyển"))
                group = "Technology/Transportation/Misc";


            else if (groupInfo.ContainsInOrder("Máy tính- Laptop", "Máy tính Windows"))
                group = "Technology/Computer/Windows";
            else if (groupInfo.ContainsInOrder("Máy tính- Laptop", "Máy tính Apple - Mac OS X"))
                group = "Technology/Computer/Apple";
            else if (groupInfo.ContainsInOrder("Máy tính- Laptop", "Máy tính Linux - Chrome OS"))
                group = "Technology/Computer/Linux, Chrome OS";


            else if (groupInfo.ContainsInOrder("Cuộc thi &amp; Sự kiện công nghệ"))
                group = "Technology/Misc";

            else if (groupInfo.ContainsInOrder("Viễn thông - Internet", "Mạng di động"))
                group = "Technology/Telecommunication/MobileNetwork";

            else if (groupInfo.ContainsInOrder("Diễn đàn Tinh tế"))
                group = "Economy";

            else
                throw new Exception("Unknown category");
            entry.GroupHierachyName = group;
            entry.Title = title;
            entry.Content = content;
        }


        protected override IEnumerable<RawEntry> ExtractRawEntriesFromMasterText(HtmlDocument document)
        {
            var res = new Dictionary<DayOfWeek, string>
                {
                    {DayOfWeek.Monday, "Thứ hai"},
                    {DayOfWeek.Tuesday, "Thứ ba"},
                    {DayOfWeek.Wednesday, "Thứ tư"},
                    {DayOfWeek.Thursday, "Thứ năm"},
                    {DayOfWeek.Friday, "thứ sáu"},
                    {DayOfWeek.Saturday, "Thứ bảy"},
                    {DayOfWeek.Sunday, "Chủ nhật"},
                };

            var lst = new[]
                {
                    "Hôm nay",
                    "Hôm qua",
                    "Thứ hai", 
                    "Thứ ba", 
                    "Thứ tư", 
                    "Thứ tư", 
                    "Thứ năm", 
                    "thứ sáu", 
                    "Thứ bảy", 
                    "Chủ nhật"
                };

            var dic = new Dictionary<string, DateTime>
            {
            {lst[0], DateTime.Now.Date}, 
            {lst[1], DateTime.Now.Date.AddDays(-1)}
            };

            var dat = DateTime.Now.Date.AddDays(-2);
            while (!lst.All(dic.ContainsKey))
            {
                dic[res[dat.DayOfWeek]] = dat;
                dat = dat.AddDays(-1);
            }

            var output = new List<RawEntry>();
            var recents = document.DocumentNode.SelectNodes("//div[@class='section sectionMain recentNews']");
            foreach (var recent in recents)
            {
                var node = recent.SelectSingleNode(".//a[@class='newsTitle']");
                var title = node.InnerText;
                var url = "http://www.tinhte.vn/" + node.Attributes["href"].Value;
                node = recent.SelectSingleNode(".//img");
                if (node == null)
                    throw new Exception("NO image");
                var image = node.Attributes["src"].Value;

                node = recent.SelectSingleNode(".//div[@class='newsText']");
                var des = node.InnerText.Trim();

                node = recent.SelectSingleNode(".//span[@class='posted']");
                var txt = node.ChildNodes[3].InnerText;
                var date = DateTime.Now;
                if (txt == null)
                    throw new Exception("Unexpected");

                var weekDay = dic.Keys.SingleOrDefault(txt.Contains);
                if (txt.Contains("phút trước"))
                {
                    date = date.AddSeconds(-int.Parse(txt.Replace("phút trước", "").Trim()));
                }
                else if (weekDay != null)
                {
                    var time = DateTime.Parse(txt.Replace(weekDay + " lúc", ""));
                    date = dic[weekDay].AddHours(time.Hour).AddMinutes(time.Minute);
                }

                else
                {
                    var arr = txt.Split('/').Select(int.Parse).ToList();
                    if (arr.Count != 3)
                        throw new Exception("Unknown");

                    var year = arr[2] + 2000;
                    if (year > 2050)
                        year = year - 1000;
                    if (year > 2050)
                        throw new Exception("Check year");

                    date = new DateTime(year, arr[1], arr[0]);
                }


                var entry = new RawEntry
                {
                    SourceUrl = url,
                    Title = title,
                    Description = des,
                    ImageUrl = image,
                    Date = date
                };

                output.Add(entry);
            }

            return output;
        }
    }
}
