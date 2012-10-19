using Eking.News.AdminSoftware.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eking.News.Tests.AdminSoftwareTest
{
    [TestClass]
    public class VoleurMainViewModelTest
    {
        readonly VoleurMainViewModel _model = new VoleurMainViewModel();
        [TestMethod]
        public void DoJobTest()
        {
            _model.DoJob();
        }

        [TestMethod]
        public void CleanUpdataTest()
        {
            _model.CleanUp();
        }
    }
}
