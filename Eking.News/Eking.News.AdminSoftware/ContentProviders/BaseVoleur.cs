using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Eking.News.Models;
using HtmlAgilityPack;

namespace Eking.News.AdminSoftware.ContentProviders
{
    public abstract class BaseVoleur
    {
        public virtual void CleanUpEntry(Entry entry)
        {
            entry.Title = entry.Title.Replace("&quot;", "\"");

            var content = entry.Content;
            if (content == null)
                return;
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var js = doc.DocumentNode.SelectNodes("//script");

            if (js != null)
                foreach (var j in js)
                    j.Remove();

            entry.Content = doc.DocumentNode.InnerHtml;
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
                CleanUpEntry(entry);
            }
            Db.SaveChanges();
        }

        protected abstract IEnumerable<string> GetMasterLinks();

        protected BaseVoleur(NewsObjectContext db = null)
        {
            //_dbVoleur = new VoleurEntities();
            //Db = new NewsObjectContext();
            Db = db ?? new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServer"].ConnectionString);
            _wc = new WebClient { Encoding = Encoding.UTF8 };
        }

        public readonly NewsObjectContext Db;
        private readonly WebClient _wc;
        protected virtual string GetGroupHierachyByMasterLink(string link)
        {
            return null;
        }


        public void DoJob()
        {
            // Step 1: Init
            Log("DoJob:" + this.GetType().Name);
            var query = from i in Db.Entries
                        where i.EntrySource != null && i.EntrySource.Url != null
                        select i.EntrySource.Url;

            _existingLinks = new HashSet<string>(query);

            // Step2: Extract raw data
            var masterLinks = GetMasterLinks();

            var entries = new List<RawEntry>();
            foreach (var masterLink in masterLinks)
            {
                Log("MasterLink:" + masterLink);

                var txt = _wc.DownloadString(masterLink);
                var tmp = ExtractRawEntriesFromMasterText(txt).ToList();
                tmp.ForEach(t => t.GroupHierachyName = GetGroupHierachyByMasterLink(masterLink));

                entries.AddRange(tmp.Where(t => !entries.Exists(t1 => t1.SourceUrl == t.SourceUrl)));

                if (!CancelJobIfVisitExistLink)
                    continue;

                var chk = (from it in tmp
                           where _existingLinks.Contains(it.SourceUrl)
                           select it).FirstOrDefault();

                if (chk != null)
                {
                    Log("Cancel extract masterlink because CancelJobIfVisitExistLink");
                    break;
                }
            }

            entries = entries.Where(en => !_existingLinks.Contains(en.SourceUrl)).ToList();

            foreach (var entry in entries)
            {
                _handledLinks.Add(entry.SourceUrl);
                var txt = _wc.DownloadString(entry.SourceUrl);
                ReadEntryInfo(entry, txt);
                CleanUpEntry(entry);

                Log("Read Entry:" + entry.Title);
            }


            // Step3: Insert into database
            Log("Save db:" + entries.Count);
            var counter = 0;
            foreach (var rawEntry in entries)
            {
                Log("Save Entry:" + rawEntry.Title);

                counter++;
                if (counter % 20 == 0)
                    Db.SaveChanges();


                if (string.IsNullOrEmpty(rawEntry.GroupHierachyName))
                    //throw new Exception("No group");
                    continue;

                var group = CreateOrGetGroupByHierachyName(rawEntry.GroupHierachyName);
                var entry = new Entry
                    {
                        Date = rawEntry.Date,
                        Content = rawEntry.Content,
                        Description = rawEntry.Description,
                        ImageUrl = rawEntry.ImageUrl,
                        Published = rawEntry.Published,
                        ViewIndex = rawEntry.ViewIndex,
                        ViewType = rawEntry.ViewType,
                        Title = rawEntry.Title,
                    };
                entry.EntrySource = new EntrySource
                    {
                        Url = rawEntry.SourceUrl,
                        Source = GetSource()
                    };
                group.Entries.Add(entry);
            }

            Db.SaveChanges();

        }

        protected abstract Source GetSource();
        private void Log(object txt)
        {
            Debug.WriteLine(">>" + txt);
        }

        protected abstract bool CancelJobIfVisitExistLink { get; }

        protected abstract void ReadEntryInfo(RawEntry entry, string txt);

        protected abstract IEnumerable<RawEntry> ExtractRawEntriesFromMasterText(string text);

        private HashSet<string> _existingLinks;
        private readonly HashSet<string> _handledLinks = new HashSet<string>();

        private Group GetGroupByHierachyName(string name)
        {
            var group = GetGroupByHierachyNameOrDefault(name);

            if (group == null)
                throw new Exception("No group has the name");
            return group;
        }

        private Group GetGroupByHierachyNameOrDefault(string name)
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

        private Group CreateOrGetGroupByHierachyName(string name)
        {
            if (GetGroupByHierachyNameOrDefault(name) == null)
                CreateGroupByHiearchyName(name);

            return GetGroupByHierachyName(name);
        }

        private void CreateGroupByHiearchyName(string hName)
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

    }

    public class RawEntry : Entry
    {
        public string GroupHierachyName;
        public string SourceUrl;
    }

    public abstract class BaseHtmlParserVoleur : BaseVoleur
    {
        protected BaseHtmlParserVoleur(NewsObjectContext db = null)
            : base(db)
        {
        }

        protected abstract void ReadEntryInfo(RawEntry entry, HtmlDocument htmlDocument);

        protected override void ReadEntryInfo(RawEntry entry, string txt)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(txt);
            ReadEntryInfo(entry, doc);
        }


        protected override IEnumerable<RawEntry> ExtractRawEntriesFromMasterText(string text)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            return ExtractRawEntriesFromMasterText(doc);
        }

        protected abstract IEnumerable<RawEntry> ExtractRawEntriesFromMasterText(HtmlDocument doc);
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
