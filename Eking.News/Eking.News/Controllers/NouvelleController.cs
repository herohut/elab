using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;
using Eking.News.Models;

namespace Eking.News.Controllers
{
    public class NouvelleController : Controller
    {
        //
        // GET: /Nouvelle/
        public ActionResult Index()
        {
            ViewBag.Groups = Db.Groups.Where(g => g.Parent == null).ToList();
            return View();
        }

        private NewsObjectContext _db;
        public NewsObjectContext Db
        {
            get { return _db ?? (_db = new NewsObjectContext(ConfigurationManager.ConnectionStrings["EnewsSqlServer"].ConnectionString)); }
        }


        //private CachedDatabaseContainer _indexing;
        //private CachedDatabaseContainer Indexing
        //{
        //    get { return _indexing ?? (_indexing = new CachedDatabaseContainer()); }
        //}

        public PartialViewResult GetEntries(int groupId, int page)
        {
            page = page + 1;
            var gr = Db.Groups.Single(g => g.Id == groupId);

            var model = gr.Entries.Skip((page - 1) * WebConfig.GroupViewItemPerPage).Select(EntryViewModel.FromEntry).Take(
                WebConfig.GroupViewItemPerPage);
            return !model.Any() ? null : PartialView("EntriesView", model);


        }

        public string CleanCachedData()
        {
            var cachedFile = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "cached.dat");
            if (System.IO.File.Exists(cachedFile))
                System.IO.File.Delete(cachedFile);
            return "Cleaned";
        }

        public PartialViewResult GroupView(int id)
        {
            ViewBag.RenderByAction = true;
            return PartialView(new GroupViewModel
            {
                Id = id,
                Name = Db.Groups.Single(g => g.Id == id).Name
            });
        }

        public PartialViewResult GroupsView(int[] groupIds)
        {
            // Cache Index
            var cachedFile = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "cached.dat");
            GroupsViewModel model;

            if (groupIds == null && System.IO.File.Exists(cachedFile))
            {
                var stream = System.IO.File.OpenRead(cachedFile);
                model = (GroupsViewModel)new BinaryFormatter().Deserialize(stream);
                stream.Close();
                goto NEXT1;
            }

            model = GetGroupsViewModel(groupIds);

            if (groupIds == null)
            {
                var write = System.IO.File.OpenWrite(cachedFile);
                new BinaryFormatter().Serialize(write, model);
                write.Close();
            }

        NEXT1:
            return PartialView(model);
        }

        public GroupsViewModel GetGroupsViewModel(IEnumerable<int> groupIds)
        {
            IEnumerable<Group> grs = (groupIds == null ? Db.Groups.Where(g => g.Entries.Count > 0)
                                  : Db.Groups.Where(g => groupIds.Contains(g.Id) && g.Entries.Count > 0)).ToList();

            var model = new GroupsViewModel { Groups = new List<GroupViewModel>() };

            var fetch = Db.Entries.ToList();

            foreach (var gr in grs)
            {
                var g = new GroupViewModel { Name = gr.Name, Id = gr.Id };
                model.Groups.Add(g);

                g.Entries = fetch.Where(f => f.Group == gr).OrderByDescending(d => d.Date).Take(WebConfig.HomeViewItemPerGroup).Select(EntryViewModel.FromEntry).ToList();

                //g.Entries = gr.Entries.Take(WebConfig.HomeViewItemPerGroup).Select(EntryViewModel.FromEntry).ToList();
            }

            return model;
        }

        BinaryFormatter _formatter = new BinaryFormatter();

        public PartialViewResult EntryView(int id)
        {
            //var vm = Indexing.EntryViews.SingleOrDefault(en => en.Id == id);
            //if (vm == null || vm.Data == null)
            return PartialView(GetEntryViewModel(id));

            //var stream = new MemoryStream(vm.Data);
            //var data = _formatter.Deserialize(stream);
            //stream.Close();

            //return PartialView(data);
        }

        public EntryViewModel GetEntryViewModel(int id)
        {
            var entry = Db.Entries.Single(en => en.Id == id);
            var vm = EntryViewModel.FromEntry(entry);
            vm.Tags = new List<string>()
                {
                    "Eking",
                    "Hero",
                    "News"
                };
            vm.BreadCrumbGroups = new List<GroupViewModel>();
            var g = entry.Group;
            while (g != null)
            {
                vm.BreadCrumbGroups.Insert(0, new GroupViewModel { Id = g.Id, Name = g.Name });
                g = g.Parent;
            }
            vm.InGroupEntries = new List<EntryViewModel>();
            vm.RelatedEntries = new List<EntryViewModel>();
            if (entry.Group == null)
                return vm;
            foreach (var en in entry.Group.Entries.Take(5))
            {
                vm.InGroupEntries.Add(new EntryViewModel { Id = en.Id, Title = en.Title });
                vm.RelatedEntries.Add(new EntryViewModel { Id = en.Id, Title = en.Title });
            }

            return vm;
        }

        public PartialViewResult EntryContentView(int id)
        {
            var entry = Db.Entries.Single(i => i.Id == id);
            return EntryContentView(entry);
        }

        public PartialViewResult EntryContentView(Entry entry)
        {
            return PartialView(EntryViewModel.FromEntry(entry));
        }

        //static List<string> _list = new List<string>();

        private static string RandomRenderType()
        {
            var t = RenTypes[Ran.Next(0, RenTypes.Count())];
            return t;
        }

        private static readonly string[] RenTypes = new[] { "4x4", "2x4", "3x4", "2x2", "2x1" };
        private static readonly Random Ran = new Random();
    }

    class Samples
    {
        public static GroupsViewModel SampleGroupsViewModel()
        {
            var op = new GroupsViewModel
            {
                Groups =
                    new List<GroupViewModel> {
                            SampleGroupViewModel("Technology"),
                            SampleGroupViewModel("Science"),
                            SampleGroupViewModel("SocialLife")
                        }
            };


            return op;
        }

        public static GroupViewModel SampleGroupViewModel(string name)
        {
            var op = new GroupViewModel
            {
                Name = name,
                BreadCrumbItems = new List<string> { "Technology", "Science" },
                Entries = new List<EntryViewModel>(),
                EntryCount = 10000,
                Id = -1
            };
            op.Entries.Add(new EntryViewModel { Title = "RenderType1", PostedDate = DateTime.Now, RenderType = "RenderType1" });
            op.Entries.Add(new EntryViewModel { Title = "RenderType2", PostedDate = DateTime.Now, RenderType = "RenderType2" });
            op.Entries.Add(new EntryViewModel { Title = "RenderType2", PostedDate = DateTime.Now, RenderType = "RenderType3" });
            op.Entries.Add(new EntryViewModel { Title = "RenderType2", PostedDate = DateTime.Now, RenderType = "RenderType4" });
            for (var i = 0; i < 20; i++)
            {
                op.Entries.Add(new EntryViewModel { Title = "Entry " + i, PostedDate = DateTime.Now });
            }

            return op;
        }
    }

    [Serializable]
    public class GroupsViewModel
    {
        public List<GroupViewModel> Groups { get; set; }
    }

    [Serializable]
    public class GroupViewModel
    {
        [Required]
        public string Name { get; set; }
        public List<string> BreadCrumbItems { get; set; }
        public List<EntryViewModel> Entries { get; set; }
        public int EntryCount { get; set; }
        public int Id { get; set; }
    }

    [Serializable]
    public class EntryViewModel
    {
        public List<EntryViewModel> RelatedEntries { get; set; }
        public List<EntryViewModel> InGroupEntries { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public DateTime PostedDate { get; set; }
        public string ImageUrl { get; set; }
        public string PostMan { get; set; }
        public string RenderType { get; set; }
        public int Id { get; set; }
        public string Desc { get; set; }
        public int W;
        public int H;
        public List<string> Tags { get; set; }
        public List<GroupViewModel> BreadCrumbGroups { get; set; }
        public string SourceText { get; set; }
        public string SourceUrl { get; set; }
        public string GroupName { get; set; }

        public static EntryViewModel FromEntry(Entry entry)
        {
            var vm = new EntryViewModel
                {
                    Content = entry.Content,
                    Title = entry.Title,
                    PostedDate = entry.Date == null ? DateTime.MinValue : (DateTime)entry.Date,
                    ImageUrl = entry.ImageUrl,
                    RelatedEntries =
                        new List<EntryViewModel> { new EntryViewModel { Title = "Ref1" }, new EntryViewModel { Title = "Ref2" } },
                    PostMan = "Hero",
                    RenderType = entry.ViewType,
                    Id = entry.Id,
                    Desc = entry.Description,
                    SourceText = entry.EntrySource == null || entry.EntrySource.Source == null ? null : entry.EntrySource.Source.Name,
                    SourceUrl = entry.EntrySource == null ? null : entry.EntrySource.Url,
                    GroupName = entry.Group == null ? null : entry.Group.Name
                };

            if (vm.RenderType == null)
                vm.RenderType = "2x4";

            var sp = vm.RenderType.Split('x');
            vm.W = int.Parse(sp[0]);
            vm.H = int.Parse(sp[1]);

            return vm;
        }

    }

}
