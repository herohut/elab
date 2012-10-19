using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Eking.News.AdminSoftware.ContentProviders;
using Eking.News.Models;
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
        public VoleurMainViewModel(NewsObjectContext db = null)
        {
            if (IsInDesignMode)
            {
                return;
            }

            Db = db ?? new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServer"].ConnectionString);

            _voleurs = new Dictionary<string, BaseVoleur>
                {
                    {"DanTri", new DanTriVoleur(db)},
                    {"TinhTe", new TinhTeVoleur(db)},
                };
        }

        protected readonly NewsObjectContext Db;


        public void DoJob()
        {
            foreach (var voleur in _voleurs.Values)
            {
                voleur.DoJob();
            }
        }
      

        public void CleanUpData()
        {
            var counter = 0;
            var entries = Db.Entries.ToList();
            foreach (var entry in entries)
            {
                counter++;
                if (counter % 40 == 0)
                {
                    Log("Clean" + counter);
                    Db.SaveChanges();
                }

                if (entry.EntrySource != null && entry.EntrySource.Source != null)
                {
                    _voleurs[entry.EntrySource.Source.Name].CleanUpEntry(entry);
                }
            }
            Db.SaveChanges();
        }

        private void Log(object txt)
        {
            Debug.WriteLine(">>" + txt);
        }
        
        private readonly Dictionary<string, BaseVoleur> _voleurs;
    }
}