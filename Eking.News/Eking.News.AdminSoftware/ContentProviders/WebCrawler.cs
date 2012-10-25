using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Eking.News.AdminSoftware.ContentProviders
{
    public class WebCrawler
    {
        public void VisitLink(LinkModel link)
        {
            if (link.ParentLevel > depth)
                depth = link.ParentLevel;
            if (_handleLinks.Count % 20 == 0)
            {
                _db.SaveChanges();
            }

            // 1. Download html
            // Save data
            link.Content = _wc.DownloadString(link.Url);
            _db.RawEntryModels.Add(new RawEntryModel() { Url = link.Url, Content = link.Content, Parent = null });

            // 2. Extract a
            // Filter a

            var doc = new HtmlDocument();
            doc.LoadHtml(link.Content);
            var nodes = doc.DocumentNode.SelectNodes("//a");

            // 3. For a VisitLink
            foreach (var node in nodes)
            {
                if (node.Attributes["href"] == null)
                    continue;
                var url = node.Attributes["href"].Value;
                // Filter URL
                var regx1 = new Regex(@"\.([h][t][m][l])?$");
                var regx2 = new Regex(@"\.([h][t][m])?$");
                var regx3 = new Regex(@"\.([a-z][a-z][a-z])?$");
                // File case
                if (regx3.IsMatch(url) && !(regx1.IsMatch(url) || regx2.IsMatch(url)))
                    continue;

                if (url == "/" || url == "/?removecookie=true")
                    continue;
                if (!url.StartsWith("/") && !url.Contains(InitLink))
                    continue;

                if (url.StartsWith("/"))
                    url = InitLink + url;

                if (_handleLinks.Contains(url))
                    continue;

                System.Diagnostics.Debug.WriteLine(">> Handle:" + url);
                link = new LinkModel { Url = url, ParentLevel = link.ParentLevel + 1 };
                //_newLinks.Add(link);
                _handleLinks.Add(url);
                VisitLink(link);
            }
        }

        WebCrawlerEntities _db = new WebCrawlerEntities();

        public void DoJob()
        {
            //_db.Database.Delete();
            //_db.Database.Create();
            var init = new LinkModel { Url = InitLink };
            VisitLink(init);
        }

        public string InitLink { get; set; }
        private readonly HashSet<string> _handleLinks = new HashSet<string>();
        //private readonly HashSet<LinkModel> _newLinks = new HashSet<LinkModel>();
        private int depth;

        public WebCrawler()
        {
            _wc = new WebClient { Encoding = Encoding.UTF8 };
        }
        private readonly WebClient _wc;
    }

    public class LinkModel
    {
        public string Url { get; set; }
        public string Content { get; set; }
        public LinkModel Parent { get; set; }
        public int ParentLevel { get; set; }
    }
}
