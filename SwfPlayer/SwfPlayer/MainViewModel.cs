using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace SwfPlayer
{
    public class MainViewModel : EntityObject
    {
        public List<SwfCategory> Categories { get; set; }


        public void LoadXml()
        {
            Categories = new List<SwfCategory>();

            XElement xele = XElement.Load("SwfList.xml");
            var files = xele.DescendantsAndSelf("File").ToList();
            foreach (var xElement in files)
            {
                var file = new SwfItem { FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Movies", xElement.Attribute("Path").Value), Title = xElement.Attribute("Title").Value };
                var categoryName = xElement.Attribute("Category").Value;
                var category = Categories.FirstOrDefault(c => c.Name == categoryName);
                if (category == null)
                {
                    category = new SwfCategory { Name = categoryName, IsExpanded = true };
                    Categories.Add(category);
                    category.PropertyChanged += (s, a) =>
                                                    {
                                                        if (a.PropertyName != SwfCategory.SelItemPropertyName)
                                                            return;
                                                        this.SelectedFile = ((SwfCategory)s).SelItem;
                                                    };
                }

                category.AddItem(file);
            }
        }


        #region SliderValue

        public const string SliderValuePropertyName = "SliderValue";

        private double _sliderValue = 0;

        public double SliderValue
        {
            get { return _sliderValue; }

            set
            {
                if (_sliderValue == value) { return; }
                _sliderValue = value;
                // Update bindings, no broadcast
                ReportPropertyChanged(SliderValuePropertyName);
            }
        }
        #endregion SliderValue



        #region SliderMaximum

        public const string SliderMaximumPropertyName = "SliderMaximum";

        private double _sliderMaximum = 1;

        public double SliderMaximum
        {
            get { return _sliderMaximum; }

            set
            {
                if (_sliderMaximum == value) { return; }
                _sliderMaximum = value;
                // Update bindings, no broadcast
                ReportPropertyChanged(SliderMaximumPropertyName);
            }
        }
        #endregion SliderMaximum




        #region SelectedFile

        public const string SelectedFilePropertyName = "SelectedFile";

        private SwfItem _selectedFile = null;

        public SwfItem SelectedFile
        {
            get { return _selectedFile; }

            set
            {
                if (_selectedFile == value) { return; }
                _selectedFile = value;
                // Update bindings, no broadcast
                ReportPropertyChanged(SelectedFilePropertyName);
            }
        }
        #endregion SelectedFile


        public const string SelCategoryPropertyName = "SelCategory";

        private SwfCategory _selCategory = null;

        public SwfCategory SelCategory
        {
            get { return _selCategory; }

            set
            {
                if (_selCategory == value) { return; }
                _selCategory = value;
                // Update bindings, no broadcast
                ReportPropertyChanged(SelCategoryPropertyName);

                foreach (var swfCategory in Categories)
                {
                    if (value == null)
                    {
                        swfCategory.IsExpanded = true;
                        PromptTextVisibility = Visibility.Visible;
                        continue;
                    }
                    PromptTextVisibility = Visibility.Hidden;
                    swfCategory.IsExpanded = swfCategory == value;
                    
                }
            }
        }

        #region IsPlaying

        public const string IsPlayingPropertyName = "IsPlaying";

        private bool _isPlaying = false;

        public bool IsPlaying
        {
            get { return _isPlaying; }

            set
            {
                if (_isPlaying == value) { return; }
                _isPlaying = value;
                // Update bindings, no broadcast
                ReportPropertyChanged(IsPlayingPropertyName);
            }
        }
        #endregion IsPlaying

        #region PromptTextVisibility

        public const string PromptTextVisibilityPropertyName = "PromptTextVisibility";

        private Visibility _promptTextVisibility = Visibility.Visible;

        public Visibility PromptTextVisibility
        {
            get { return _promptTextVisibility; }

            set
            {
                if (_promptTextVisibility == value) { return; }
                _promptTextVisibility = value;
                // Update bindings, no broadcast
                ReportPropertyChanged(PromptTextVisibilityPropertyName);
            }
        }
        #endregion PromptTextVisibility


    }
}
