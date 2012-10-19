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
        public void ExtractEntryFromMasterTextTest()
        {
            _voleur.ExtractEntryFromMasterText(RES.dantri_master_text);
        }

        [TestMethod]
        public void ExtractEntryContentTest()
        {
            _voleur.ExtractEntryContent(new Entry(), RES.dantri_entry_text);
        }

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



        [TestMethod]
        public void ExtractEntryContentTest2()
        {
            var db = _voleur.Db;
            var s = db.Sources.Single(so => so.Name == "DanTri");
            var empty = db.Entries.ToList().Where(e => e.EntrySource != null && e.EntrySource.Source == s && string.IsNullOrEmpty(e.Content)).ToList();

            var idx = 0;
            foreach (var entry in empty)
            {
                idx++;
                if (idx % 100 == 0)
                    db.SaveChanges();

                _voleur.Handle(entry);
                if (entry.GetSkipped())
                    continue;

                if (string.IsNullOrEmpty(entry.Content))
                    throw new Exception("Cannot be null");
                Debug.WriteLine(">>" + entry.Title);
            }

            db.SaveChanges();

        }
    }
}
