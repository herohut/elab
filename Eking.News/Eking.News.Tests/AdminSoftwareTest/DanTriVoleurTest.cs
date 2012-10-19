using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Eking.News.AdminSoftware.ContentProviders;
using Eking.News.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eking.News.Tests.AdminSoftwareTest
{
    [TestClass]
    public class DanTriVoleurTest
    {
        [TestMethod]
        public void DoJobTest()
        {
            _voleur.DoJob();
        }

        readonly DanTriVoleur _voleur = new DanTriVoleur();


        [TestMethod]
        public void CleanupDuplication()
        {
            //var db = _voleur.Db;
            var db = new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServerRemote"].ConnectionString);



            var entries = db.Entries.ToList();
            var dist = entries.GroupBy(g => g.Title);
            var keep = new List<Entry>();
            foreach (var distitem in dist)
            {
                var items = distitem.ToList();
                var val = items.FirstOrDefault(i => !string.IsNullOrEmpty(i.Content));
                var keepme = val ?? items[0];
                keep.Add(keepme);
                entries.Remove(keepme);
            }

            foreach (var entry in entries)
            {
                if (entry.EntrySource != null)
                    db.EntrySources.DeleteObject(entry.EntrySource);
                db.Entries.DeleteObject(entry);
            }

            db.SaveChanges();
        }
    }
}
