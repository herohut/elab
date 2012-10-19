using System.Configuration;
using Eking.News.AdminSoftware.ViewModel;
using Eking.News.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eking.News.Tests.AdminSoftwareTest
{
    [TestClass]
    public class VoleurMainViewModelTest
    {
        readonly VoleurMainViewModel _model = new VoleurMainViewModel(new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServerLocal"].ConnectionString));
        //readonly VoleurMainViewModel _model = new VoleurMainViewModel();
        [TestMethod]
        public void DoJobTest()
        {
            _model.DoJob();
        }

        [TestMethod]
        public void CleanUpdataTest()
        {
            _model.CleanUpData();
        }
        
    }
}
