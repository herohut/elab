using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;

namespace GoogleGrabber2.ViewModel
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
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                _countryDomains = RES.ITEM1.Split(new[] { "\r\n" }, StringSplitOptions.None).Select(i =>
                    {
                        var items = i.Split(new[] { '(', ')', '*' });
                        return new[] { items[1].Trim(), "http://" + items[2].Trim() };
                    }).ToList();
            }
        }

        private List<string[]> _countryDomains;
        private int _currentIndex;

        private string[] CurrentCountry
        {
            get { return _countryDomains[_currentIndex]; }
        }

        public void Start1()
        {
            ChangeCountry(0);
        }

        public void Start2()
        {
            SerializeLangs();
            _lstGoogleLanguages = _lstGoogleLanguages.Where(i => !string.IsNullOrEmpty(i.Parent)).ToList();

            _currentIndex = 0;
            CurrentUrl = _lstGoogleLanguages[_currentIndex].Url;
        }

        private void SerializeLangs()
        {
            var sel = new XmlSerializer(typeof(List<GoogleLanguage>));
            var s = File.Open("output/output.xml", FileMode.Open);
            var sw = new StreamReader(s, Encoding.UTF8);
            _lstGoogleLanguages =
                ((List<GoogleLanguage>)sel.Deserialize(sw)).ToList();
            sw.Close();
            _lstGoogleLanguages.ForEach(l => l.Url = l.Url.Replace("&amp;", "&"));
        }


        private StringBuilder _sBuilder = new StringBuilder();
        private List<string[]> _otherLanguges;
        private int _otherLangIndex = 0;
        private void ChangeCountry(int index)
        {
            _currentIndex = index;
            if (!Directory.Exists("output"))
                Directory.CreateDirectory("output");

            if (!Directory.Exists("output/" + CurrentCountry[0]))
                Directory.CreateDirectory("output/" + CurrentCountry[0]);


            _lstGoogleLanguages.Add(new GoogleLanguage { Name = CurrentCountry[0], Url = CurrentCountry[1] });

            CurrentUrl = CurrentCountry[1];
        }

        public void HandleHtml1(string html)
        {
            _otherLanguges = GetOthers(html);
            ExtractInfo(_sBuilder, html, string.Format("output/{0}/{0}.html", CurrentCountry[0]));

            if (_otherLanguges != null && _otherLanguges.Count != 0)
                _lstGoogleLanguages.AddRange(
                    _otherLanguges.Select(o => new GoogleLanguage { Name = o[0], Url = o[1], Parent = CurrentCountry[0] }));

            _currentIndex++;
            if (_currentIndex < _countryDomains.Count)
                ChangeCountry(_currentIndex);
            else
            {
                var sel = new XmlSerializer(typeof(List<GoogleLanguage>));
                var s = File.Create("output/output.xml");
                var sw = new StreamWriter(s, Encoding.UTF8);
                sel.Serialize(sw, _lstGoogleLanguages);
                sw.Close();
            }
        }

        public void HandleHtml2(string html)
        {
            var c = _lstGoogleLanguages[_currentIndex];
            ExtractInfo(_sBuilder, html, string.Format("output/{0}/{0}.{1}.html", c.Parent, c.Name));

            _currentIndex++;
            if (_currentIndex < _lstGoogleLanguages.Count)
                CurrentUrl = _lstGoogleLanguages[_currentIndex].Url;
            else
            {
            }
        }

        private List<GoogleLanguage> _lstGoogleLanguages = new List<GoogleLanguage>();

        [Serializable]
        public class GoogleLanguage
        {
            public string Name { get; set; }
            public string Parent { get; set; }
            public string Url { get; set; }
            public override string ToString()
            {
                return Parent == null ? Name : Parent + "." + Name;
            }
        }

        public void ExtractInfo(StringBuilder sBuilder, string html, string fileName)
        {
            File.WriteAllText(fileName, html, Encoding.UTF8);
        }

        public List<string[]> GetOthers(string html)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes("//center/div/div/font/a");
            return nodes == null ? null : nodes.Select(n => new[] { n.InnerText, n.Attributes["href"].Value.Replace("&amp;", "&") }).ToList();
        }

        /// <summary>
        /// The <see cref="CurrentUrl" /> property's name.
        /// </summary>
        public const string CurrentUrlPropertyName = "CurrentUrl";

        private string _currentUrl = "Google.com.vn";

        /// <summary>
        /// Gets the CurrentUrl property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string CurrentUrl
        {
            get
            {
                return _currentUrl;
            }

            set
            {
                if (_currentUrl == value)
                {
                    return;
                }

                var oldValue = _currentUrl;
                _currentUrl = value;

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(CurrentUrlPropertyName, oldValue, value, true);
            }
        }


        internal void WritePhp()
        {
            var sBuilder = new StringBuilder();
            sBuilder.AppendLine("<?php");
            var template = File.ReadAllText("phpTemplate.txt");
            SerializeLangs();

            foreach (var item in _lstGoogleLanguages)
            {
                var toAppend = template;
                var fContent = File.ReadAllText(string.IsNullOrEmpty(item.Parent)
                            ? string.Format("output/{0}/{0}.html", item.Name)
                            : string.Format("output/{0}/{0}.{1}.html", item.Parent, item.Name));

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(fContent);

                toAppend = toAppend.Replace("{CountryLang}", item.Parent == null ? item.Name : item.Parent + "." + item.Name);

                //{Search}
                var node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gb_1");
                toAppend = toAppend.Replace("{Search}", node == null ? null : node.InnerText);

                //{Images}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gb_2");
                toAppend = toAppend.Replace("{Images}", node == null ? null : node.InnerText);

                // {Maps}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gb_8");
                toAppend = toAppend.Replace("{Maps}", node == null ? null : node.InnerText);

                // {News}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gb_5");
                toAppend = toAppend.Replace("{News}", node == null ? null : node.InnerText);

                // {Videos} 
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gb_12");
                toAppend = toAppend.Replace("{Videos}", node == null ? null : node.InnerText);

                // {Shopping} 
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gb_6");
                toAppend = toAppend.Replace("{Shopping}", node == null ? null : node.InnerText);

                // {Youtube}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gb_36");
                toAppend = toAppend.Replace("{Youtube}", node == null ? null : node.InnerText);

                // {Fewer}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "UNKNOWN");
                toAppend = toAppend.Replace("{Fewer}", node == null ? null : node.InnerText);

                // {More}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gbztm");
                toAppend = toAppend.Replace("{More}", node == null ? null : node.InnerText);

                // {Login}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gb_70");
                toAppend = toAppend.Replace("{Login}", node == null ? null : node.InnerText);

                // {Register}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "UNKNOWN");
                toAppend = toAppend.Replace("{Register}", node == null ? null : node.InnerText);

                // {Forgot}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "UNKNOWN");
                toAppend = toAppend.Replace("{Forgot}", node == null ? null : node.InnerText);

                // {Password}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "UNKNOWN");
                toAppend = toAppend.Replace("{Password}", node == null ? null : node.InnerText);

                // {Jaamla}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "UNKNOWN");
                toAppend = toAppend.Replace("{Jaamla}", node == null ? null : node.InnerText);

                // {Features}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "UNKNOWN");
                toAppend = toAppend.Replace("{Features}", node == null ? null : node.InnerText);


                // {Solutions}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("href") && n.Attributes["href"].Value == "/services/");
                toAppend = toAppend.Replace("{Solutions}", node == null ? null : node.InnerText);

                // {Google Search}
                var tmp = doc.DocumentNode.SelectNodes("//button");
                node = tmp == null ? null : tmp.SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gbqfba");
                if (node != null)
                    toAppend = toAppend.Replace("{Google Search}", node.InnerText);
                else
                {
                    tmp = doc.DocumentNode.SelectNodes("//input");
                    node = tmp == null ? null : tmp.SingleOrDefault(n => n.Attributes.Contains("name") && n.Attributes["name"].Value == "btnK");
                    toAppend = toAppend.Replace("{Google Search}", node == null ? null : node.Attributes["value"].Value);
                }

                // {I'm Feeling Lucky}
                tmp = doc.DocumentNode.SelectNodes("//button");
                node = tmp == null ? null : tmp.SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "gbqfbb");
                if (node != null)
                    toAppend = toAppend.Replace("{I'm Feeling Lucky}", node.InnerText);
                else
                {
                    tmp = doc.DocumentNode.SelectNodes("//input");
                    node = tmp == null ? null : tmp.SingleOrDefault(n => n.Attributes.Contains("name") && n.Attributes["name"].Value == "btnI");
                    toAppend = toAppend.Replace("{I'm Feeling Lucky}", node == null ? null : node.Attributes["value"].Value);
                }


                // {search offered in}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("id") && n.Attributes["id"].Value == "UNKNOWN");
                toAppend = toAppend.Replace("{search offered in}", node == null ? null : node.InnerText);

                // {About Google}
                node = doc.DocumentNode.SelectNodes("//a").SingleOrDefault(n => n.Attributes.Contains("href") && n.Attributes["href"].Value.EndsWith("about.html"));
                toAppend = toAppend.Replace("{About Google}", node == null ? null : node.InnerText);

                sBuilder.AppendLine(toAppend);
                sBuilder.AppendLine();
            }

            sBuilder.AppendLine("?>");

            File.WriteAllText("result.txt", sBuilder.ToString(), Encoding.UTF8);
        }
    }
}