using System;
using System.Linq;
using Eking.News.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eking.News.Tests
{
    [TestClass]
    public class NewsObjectContextTest
    {
        [TestMethod]
        public void CreateDatabase()
        {
            var db = new NewsObjectContext();
            db.DeleteDatabase();
            db.CreateDatabase();
        }
    }
}
