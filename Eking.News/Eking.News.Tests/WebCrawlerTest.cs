using System;
using Eking.News.AdminSoftware.ContentProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eking.News.Tests
{
    [TestClass]
    public class WebCrawlerTest
    {
        [TestMethod]
        public void DoJobTest()
        {
            var crawler = new WebCrawler {InitLink = "http://dantri.com.vn"};
            crawler.DoJob();
        }
    }
}
