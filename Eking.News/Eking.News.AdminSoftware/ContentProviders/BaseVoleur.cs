using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using Eking.News.Models;
using HtmlAgilityPack;

namespace Eking.News.AdminSoftware.ContentProviders
{
    public abstract class BaseVoleur
    {
        protected BaseVoleur()
        {
            //_dbVoleur = new VoleurEntities();
            //Db = new NewsObjectContext();
            Db = new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServer"].ConnectionString);
            _wc = new WebClient { Encoding = Encoding.UTF8 };
        }

        //private readonly VoleurEntities _dbVoleur;
        public readonly NewsObjectContext Db;
        private readonly WebClient _wc;
        public abstract void ExtractEntryContent(Entry entry, string text);
        public abstract IEnumerable<Entry> ExtractEntryFromMasterText(string text, string link = null);
        public abstract IEnumerable<string> GetMasterLinks();
        public abstract Source GetSource();

        protected virtual void CreateGroups() { }

        public void Handle(Entry entry)
        {           
            //var chk = _dbVoleur.Logs.SingleOrDefault(l => l.Url == entry.EntrySource.Url);
            //if (chk != null)
            //    return;

            var txt = _wc.DownloadString(entry.EntrySource.Url);

            ExtractEntryContent(entry, txt);

            if (entry.GetSkipped())
            {
                System.Diagnostics.Debug.WriteLine(">> Skipped by ExtractEntryContent");
                return;
            }
            CleanUpEntry(entry);

            // entry.Raw = txt
            if (entry.EntityState == System.Data.EntityState.Added)
            {
                Db.Entries.AddObject(entry);
            }
            //_dbVoleur.Logs.AddObject(new Log { Url = entry.EntrySource.Url, Date = DateTime.Now });
        }

        public void DoJob()
        {
            if (Db.Entries.Count() != 0)
                _viewIndex = Db.Entries.Max(d => d.ViewIndex);
            CreateGroups();
            var masterLinks = GetMasterLinks();


            var query = from i in Db.Entries
                        where i.EntrySource != null && i.EntrySource.Url != null
                        select i.EntrySource.Url;

            _existingLinks = new HashSet<string>(query);

            foreach (var masterLink in masterLinks)
            {
                HandleMasterLink(masterLink);
                Db.SaveChanges();

                if (_cancelJob)
                {
                    return;
                }
            }
        }

        private HashSet<string> _existingLinks;
        private bool _cancelJob = false;

        private int _viewIndex;
        internal Entry CreateNewEntry(string title, string url, string desc = null, string imageurl = null, DateTime? date = null)
        {
            _viewIndex++;

            return new Entry
            {
                EntrySource = new EntrySource
                {
                    Source = GetSource(),
                    Url = url
                },
                Title = title,
                Description = desc,
                ImageUrl = imageurl,
                ViewIndex = _viewIndex,
                Date = date
            };
        }

        public abstract bool CancelJobIfVisitExistLink { get; }

        private readonly HashSet<string> _handledLinks = new HashSet<string>();

        private void HandleMasterLink(string masterLink)
        {
            var txt = _wc.DownloadString(masterLink);
            var entries = ExtractEntryFromMasterText(txt, masterLink);
            foreach (var entry in entries)
            {
                if (entry.EntrySource != null && _handledLinks.Contains(entry.EntrySource.Url))
                {
                    System.Diagnostics.Debug.WriteLine(">> Skip because of HANDLED entry");
                    continue;
                }

                if (entry.EntrySource != null && _existingLinks.Contains(entry.EntrySource.Url))
                {
                    System.Diagnostics.Debug.WriteLine(">> Skip because of EXISTING Link");
                    _cancelJob = CancelJobIfVisitExistLink;
                    return;
                }

                Handle(entry);
                if (entry.EntrySource != null)
                    _handledLinks.Add(entry.EntrySource.Url);

                System.Diagnostics.Debug.WriteLine(entry.Title);
            }
        }

        protected Group GetGroupByHierachyName(string name)
        {
            var group = GetGroupByHierachyNameOrDefault(name);

            if (group == null)
                throw new Exception("No group has the name");
            return group;
        }

        protected Group GetGroupByHierachyNameOrDefault(string name)
        {
            var tmp = (Db.Groups.ToList().Select(a =>
                              new
                              {
                                  FullName = a.GetFullName(),
                                  Real = a
                              }
                )).ToList();
            var query = from item in tmp
                        where item.FullName == name
                        select item.Real;
            return query.FirstOrDefault();
        }

        protected Group CreateOrGetGroupByHierachyName(string name)
        {
            if (GetGroupByHierachyNameOrDefault(name) == null)
                CreateGroupByHiearchyName(name);

            return GetGroupByHierachyName(name);
        }

        protected void CreateGroupByHiearchyName(string hName)
        {
            var names = hName.Split('/');
            Group parent = null;
            foreach (var s in names)
            {
                var chk = Db.Groups.ToList().SingleOrDefault(g => g.Name == s && g.Parent == parent);
                if (chk != null)
                {
                    parent = chk;
                    continue;
                }

                chk = new Group { Name = s, Parent = parent };
                Db.Groups.AddObject(chk);
                parent = chk;
            }

            Db.SaveChanges();
        }

        public void CleanUpData()
        {
            var counter = 0;
            foreach (var entry in Db.Entries)
            {
                counter++;
                if (counter / 20 == 0)
                    Db.SaveChanges();
                CleanUpEntry(entry);
            }
            Db.SaveChanges();
        }

        public virtual void CleanUpEntry(Entry entry)
        {
            entry.Title = entry.Title.Replace("&quot;", "\"");
        }
    }


    public abstract class BaseHtmlParserVoleur : BaseVoleur
    {
        public abstract void ExtractEntryContent(Entry entry, HtmlDocument htmlDocument);
        public abstract IEnumerable<Entry> ExtractEntryFromMasterText(HtmlDocument text, string masterLink = null);
        public override void ExtractEntryContent(Entry entry, string text)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            ExtractEntryContent(entry, doc);
        }

        public override IEnumerable<Entry> ExtractEntryFromMasterText(string text, string masterLink = null)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            return ExtractEntryFromMasterText(doc, masterLink);
        }
    }


    public static class Extension
    {
        private static readonly Dictionary<Entry, bool> Skippeds = new Dictionary<Entry, bool>();
        public static void SetSkipped(this Entry entry, bool val)
        {
            Skippeds[entry] = val;
        }

        public static bool GetSkipped(this Entry entry)
        {
            return Skippeds.ContainsKey(entry) && Skippeds[entry];
        }

        public static bool HasAttributeValue(this HtmlNode node, string attName, string attValue)
        {
            return node.Attributes.Contains(attName) && node.Attributes[attName].Value == attValue;
        }

        public static bool ContainsInOrder(this string input, params string[] items)
        {
            if (items == null || items.Length == 0)
                throw new Exception("items must not null");
            var currentIndex = -1;
            for (var i = 0; i < items.Length; i++)
            {
                var idx = input.IndexOf(items[i]);
                if (idx < 0)
                    return false;

                if (idx < currentIndex)
                    return false;
                currentIndex = idx;
            }

            return true;
        }

        public static string GetFullName(this Group group)
        {
            var name = group.Name;
            while (group.Parent != null)
            {
                group = group.Parent;
                name = name.Insert(0, group.Name + "/");
            }

            return name;
        }
    }
}
