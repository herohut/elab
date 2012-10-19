using System;
using Eking.News.Controllers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eking.News.Tests.Controllers
{
    [TestClass]
    public class NouvelleControllerTest
    {
        [TestMethod]
        public void GetGroupsViewModelTest()
        {
            var controller = new NouvelleController();
            var output = controller.GetGroupsViewModel(null);
        }


        [TestMethod]
        public void GetEntriesTest()
        {
            var controller = new NouvelleController();
            var group = controller.Db.Groups.Single(g => g.Name == "Technology");
            var output = controller.GetEntries(group.Id, 2);
        }
    }
}
