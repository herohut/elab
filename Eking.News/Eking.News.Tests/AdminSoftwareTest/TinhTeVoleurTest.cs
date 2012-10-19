using System;
using Eking.News.AdminSoftware.ContentProviders;
using Eking.News.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eking.News.Tests.AdminSoftwareTest
{
    [TestClass]
    public class TinhTeVoleurTest
    {
        [TestMethod]
        public void DoJobTest()
        {
            _tinhte.DoJob();
        }

        readonly TinhTeVoleur _tinhte = new TinhTeVoleur();
    }
}
