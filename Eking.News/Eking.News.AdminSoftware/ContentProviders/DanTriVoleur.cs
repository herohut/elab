using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eking.News.Models;
using HtmlAgilityPack;

namespace Eking.News.AdminSoftware.ContentProviders
{
    public class DanTriVoleur : BaseHtmlParserVoleur
    {
        public DanTriVoleur(NewsObjectContext db = null) : base(db)
        {
        }

        protected override IEnumerable<string> GetMasterLinks()
        {

            for (var i = 1; i < 15; i++)
            {
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c20s20/xahoi/trang-{0}.htm", i)] = "SocialLife";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c36s36/thegioi/trang-{0}.htm", i)] = "TheWorld";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c26s26/thethao/trang-{0}.htm", i)] = "Sport";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c25s25/giaoduckhuyenhoc/trang-{0}.htm", i)] = "Education";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c167s167/tamlongnhanai/trang-{0}.htm", i)] = "Compassionate";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c76s76/kinhdoanh/trang-{0}.htm", i)] = "Business";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c730s730/vanhoa/trang-{0}.htm", i)] = "Culture";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c23s23/giaitri/trang-{0}.htm", i)] = "Entertainment";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c170s170/skphapluat/trang-{0}.htm", i)] = "Law";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c135s135/nhipsongtre/trang-{0}.htm", i)] = "YoungPeople_Love";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c130s130/tinhyeugioitinh/trang-{0}.htm", i)] = "YoungPeople_Love";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c119s119/congnghe/trang-{0}.htm", i)] = "Technology";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c111s111/otoxemay/trang-{0}.htm", i)] = "Technology/Transportation";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c202s202/diendanbandoc/trang-{0}.htm", i)] = "Blog";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c673s673/diendan/trang-{0}.htm", i)] = "Forum";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c132s132/chuyenla/trang-{0}.htm", i)] = "Mystery";
                _masterLinkToGroup[string.Format("http://tuyensinh.dantri.com.vn/c704s704/tuyensinh/trang-{0}.htm", i)] = "Recruitement";
                _masterLinkToGroup[string.Format("http://duhoc.dantri.com.vn/c734s734/duhoc/trang-{0}.htm", i)] = "AbroadStudy";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c702s702/blog/trang-{0}.htm", i)] = "Blog";
                _masterLinkToGroup[string.Format("http://dantri.com.vn/c133s133/nghenghiep/trang-{0}.htm", i)] = "Profession";
            }

            return _masterLinkToGroup.Keys.ToList();
        }

        readonly Dictionary<string, string> _masterLinkToGroup = new Dictionary<string, string>();

        private Source _source;

        protected override Source GetSource()
        {
            if (_source == null)
            {
                _source = Db.Sources.SingleOrDefault(s => s.Name == "DanTri") ??
                          new Source { HomeUrl = "http://dantri.com.vn", Name = "DanTri" };
            }

            return _source;
        }

        protected override bool CancelJobIfVisitExistLink
        {
            get { return false; }
        }

        protected override void ReadEntryInfo(RawEntry entry, HtmlDocument htmlDocument)
        {
            var node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='fon34 mt3 mr2 fon43']")
                // Tuyensinh case
                      ?? htmlDocument.DocumentNode.SelectSingleNode("//div[@class='detail-content']");
            if (node == null &&
                (htmlDocument.DocumentNode.InnerText.Contains("Hệ thống đang nâng cấp")

                || htmlDocument.DocumentNode.InnerText.Contains("Không tìm thấy dữ liệu")))
            {
                entry.SetSkipped(true);
                return;
            }

            entry.Content = node.InnerHtml.Trim();


            node = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='box26']") ??
                   htmlDocument.DocumentNode.SelectSingleNode("//div[@class='date-time']");
            var tmp = node.InnerText;

            tmp = tmp.Substring(tmp.IndexOf(',') + 1).Replace(" ", "");
            var parts = tmp.Split('/', '-', ':').Select(int.Parse).ToList();
            if (parts.Count != 5)
                throw new Exception("Unknow element");

            entry.Date = new DateTime(parts[2], parts[1], parts[0], parts[3], parts[4], 0);
        }

        public override void CleanUpEntry(Entry entry)
        {
            base.CleanUpEntry(entry);
            if (entry.Content == null)
                return;

            
            var content = entry.Content;
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var removals = new[]
            {
                new
                {
                    format = "//*[@id='{0}']",
                    items = new[]
                        {
                            "ctl00_IDContent_divTinDaDang",
                            "ctl00_IDContent_divTinMoi",
                            "ctl00_IDContent_ctl00_divEvent",
                            "div_tamlongnhanai",
                            "divComment",
                            "divSendComment"
                        }
                },
                 new
                {
                    format = "//*[@class='{0}']",
                    items = new[]
                            {
                                "emailprint",
                                "hrecipe",
                                "clearfix",
                                "clearfix mt2",
                                "fl wid310 box5 admicro",
                                "footer box19"
                            }
                },
                 new
                {
                    format = "//*[@type='{0}']",
                    items = new[]
                            {
                                "hidden"
                            }
                },
                new
                {
                    format = "//*[@itemtype='{0}']",
                    items = new[]
                            {
                                "http://schema.org/webpage"
                            }
                }
            };

            foreach (var removal in removals)
            {
                foreach (var item in removal.items)
                {
                    var tmp = doc.DocumentNode.SelectNodes(string.Format(removal.format, item));
                    if (tmp == null)
                        continue;

                    foreach (var i in tmp)
                        i.Remove();
                }

            }

            var divs =
                doc.DocumentNode.SelectNodes("//*").Where(i => i.Name != "img" && string.IsNullOrEmpty(i.InnerHtml)).
                    ToList();
            foreach (var htmlNode in divs)
                htmlNode.Remove();

            entry.Content = doc.DocumentNode.InnerHtml;
        }

        protected override string GetGroupHierachyByMasterLink(string link)
        {
            return _masterLinkToGroup[link];
        }

        protected override IEnumerable<RawEntry> ExtractRawEntriesFromMasterText(HtmlDocument text)
        {
            var tmp = new List<HtmlNodeCollection>
                {
                    text.DocumentNode.SelectNodes("//div[@class='nbmda mt3 clearfix']"),
                    text.DocumentNode.SelectNodes("//div[@class='mt3 clearfix']"),
                    text.DocumentNode.SelectNodes("//div[@class='list']/div[@class='item']")
                };


            var items = new List<HtmlNode>();
            tmp.ForEach(t =>
            {
                if (t != null)
                    items.AddRange(t);
            });

            var output = new List<RawEntry>();
            foreach (var htmlNode in items)
            {
                var node = htmlNode.SelectSingleNode(".//h1/a")
                    ??
                    htmlNode.SelectSingleNode(".//h2/a")
                    // Tuyensinh case    
                    ?? htmlNode.SelectSingleNode(".//a");



                var url = "http://dantri.com.vn" + node.Attributes["href"].Value;
                var title = node.InnerText.Trim();
                // Tuyensinh case
                if (string.IsNullOrEmpty(title))
                    title = node.Attributes["title"].Value;


                node = htmlNode.SelectSingleNode(".//div/div")
                    ?? htmlNode.SelectSingleNode(".//p");
                var des = node.InnerText;

                node = htmlNode.SelectSingleNode(".//a/img");
                var img = node == null ? null : node.Attributes["src"].Value;

                var entry = new RawEntry
                    {
                        Title = title,
                        SourceUrl = url,
                        Description = des,
                        ImageUrl = img
                    };

                output.Add(entry);
            }

            return output;
        }
    }
}
