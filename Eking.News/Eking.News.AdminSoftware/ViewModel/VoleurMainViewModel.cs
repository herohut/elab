using System.Collections.Generic;
using Eking.News.AdminSoftware.ContentProviders;
using GalaSoft.MvvmLight;

namespace Eking.News.AdminSoftware.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class VoleurMainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public VoleurMainViewModel()
        {
            if (IsInDesignMode)
            {
                return;
            }

            _voleurs = new List<BaseVoleur>
                {
                    new DanTriVoleur(),
                    new TinhTeVoleur()
                };
        }


        public void DoJob()
        {
            foreach (var voleur in _voleurs)
            {
                voleur.DoJob();
            }
        }

        public void CleanUp()
        {
            foreach (var baseVoleur in _voleurs)
            {
                baseVoleur.CleanUpData();
            }
        }


        private readonly List<BaseVoleur> _voleurs;
    }
}