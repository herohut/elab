using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Objects;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Eking.News.AdminSoftware;
using Eking.News.Controllers;
using Eking.News.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eking.News.Tests
{
    [TestClass]
    public class Helper
    {
        [TestMethod]
        public void CreateDatabase()
        {
            var db = new NewsObjectContext();
            db.DeleteDatabase();
            db.CreateDatabase();
        }

        [TestMethod]
        public void CreateDatabaseSqlServer()
        {
            var db = new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServer"].ConnectionString);
            db.CreateDatabase();
        }



        [TestMethod]
        public void TestDatabaseSqlServer()
        {
            var db = new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServer"].ConnectionString);
            dynamic count = db.Entries.Count();
        }


        [TestMethod]
        public void CreateVoleurLogDatabase()
        {
            var db = new VoleurEntities();
            db.DeleteDatabase();
            db.CreateDatabase();
        }

        [TestMethod]
        public void CreateDatabases()
        {
            CreateDatabase();
            CreateVoleurLogDatabase();
        }


        [TestMethod]
        public void CheckSmth()
        {
            var db = new NewsObjectContext();
        }

        [TestMethod]
        public void CleanupDuplicatedTitles()
        {
            var db = new NewsObjectContext();
            var count = db.Entries.Count();


            var query = db.Entries.ToList();

            var removal = new List<Entry>();
            for (int i = query.Count - 1; i >= 0; i--)
            {
                var item = query[i];
                var exist = query.Exists(q => q != item && q.Title == item.Title);
                if (exist)
                    removal.Add(item);
            }

            foreach (var entry in removal)
            {
                db.EntrySources.DeleteObject(entry.EntrySource);
                db.Entries.DeleteObject(entry);
            }

            db.SaveChanges();
        }

        [TestMethod]
        public void FixGroupEntries()
        {
            var db = new NewsObjectContext();
            var g1 = db.Groups.SingleOrDefault(g => g.Name == "YoungPeople_Love");
            var g2 = db.Groups.SingleOrDefault(g => g.Name == "YoungPeople & Love");
            foreach (var entry in g2.Entries.ToList())
            {
                entry.Group = g1;
            }

            db.Groups.DeleteObject(g2);
            db.SaveChanges();
        }

        [TestMethod]
        public void CreateIndexingDb()
        {
            var indexingDb = new CachedDatabaseContainer();
            indexingDb.DeleteDatabase();
            indexingDb.CreateDatabase();
        }

        [TestMethod]
        public void IndexingEntries()
        {
            var db = new NewsObjectContext();
            var indexingDb = new CachedDatabaseContainer();

            var formatter = new BinaryFormatter();

            var controller = new NouvelleController();
            var counter = 0;
            foreach (var id in db.Entries.Select(en => en.Id))
            {
                counter++;
                if (counter % 5 == 0)
                    Debug.WriteLine(">> Indexed: " + counter);
                var chk = indexingDb.EntryViews.SingleOrDefault(e => e.Id == id);
                if (chk != null)
                    continue;

                chk = new EntryView { Id = id };
                var stream = new MemoryStream();
                formatter.Serialize(stream, controller.GetEntryViewModel(id));
                chk.Data = stream.ToArray();
                indexingDb.EntryViews.AddObject(chk);
                stream.Close();


                indexingDb.SaveChanges();
            }
        }

        [TestMethod]
        public void CheckNullGroupEntries()
        {
            var db = new NewsObjectContext();
            var entries = db.Entries.Where(en => en.Group == null).ToList();

            foreach (var entry in entries)
            {
                db.EntrySources.DeleteObject(entry.EntrySource);
                db.Entries.DeleteObject(entry);
            }
            db.SaveChanges();
        }

        [TestMethod]
        public void SyncData()
        {
            var remote = new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServerRemote"].ConnectionString);
            var remoteGroups = remote.Groups.ToDictionary(a => a.Id, b => b);
            var entrySource = remote.Sources.ToDictionary(a => a.Id, b => b);

            var entries1 = remote.Entries.Select(e => new
                {
                    e.Id,
                    e.Title,
                    Content = ""
                }).ToList();

            var local = new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServerLocal"].ConnectionString);

            var entries2 = local.Entries.Select(e => e).ToList();
            var query = (from e in entries2
                         where !entries1.Exists(e1 => e1.Title == e.Title)
                         select e).ToList();

            for (int i = 0; i < query.Count; i++)
            {
                var entry = query[i];
                if (i % 20 == 0)
                    remote.SaveChanges();

                if (entry.Group == null)
                    continue;
                if (!remoteGroups.ContainsKey(entry.Group.Id))
                    throw new Exception("Group not exist");

                if (!entrySource.ContainsKey(entry.EntrySource.Source.Id))
                    throw new Exception("EntrySource not exist");
                // clone
                var newEntry = new Entry
                    {
                        Content = entry.Content,
                        Date = entry.Date,
                        Description = entry.Description,
                        Title = entry.Title,
                        ImageUrl = entry.ImageUrl,
                        ViewType = entry.ViewType,
                        ViewIndex = entry.ViewIndex,
                        Group = remoteGroups[entry.Group.Id],
                        Published = entry.Published,
                        EntrySource =
                            new EntrySource
                            {
                                Url = entry.EntrySource.Url,
                                Source = entrySource[entry.EntrySource.Source.Id]
                            }
                    };
                remote.Entries.AddObject(newEntry);
            }
            remote.SaveChanges();

            // Empty entry
            var entries3 = (from e in remote.Entries
                            where e.Content == null
                            select
                               e).ToList();

            foreach (var entry in entries3)
            {
                var tmp = entries2.SingleOrDefault(e => e.Title == entry.Title);
                if (tmp == null)
                    continue;
                entry.Content = tmp.Content;
            }

            remote.SaveChanges();
        }
    }
}
