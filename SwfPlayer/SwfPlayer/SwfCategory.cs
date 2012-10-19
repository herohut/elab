using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;

namespace SwfPlayer
{
    public class SwfCategory: EntityObject
    {

        public ObservableCollection<SwfItem> Items { get; set; }

        //public bool IsExpanded { get; set; }

        #region IsExpanded

        public const string IsExpandedPropertyName = "IsExpanded";

        private bool _isExpanded = false;

        public bool IsExpanded
        {
            get { return _isExpanded; }

            set
            {
                if (_isExpanded == value) { return; }
                _isExpanded = value;
                // Update bindings, no broadcast
                ReportPropertyChanged(IsExpandedPropertyName);
            }
        }
        #endregion IsExpanded


        public string Name { get; set; }

        internal void AddItem(SwfItem file)
        {
            if (this.Items == null)
                this.Items = new ObservableCollection<SwfItem>();
            this.Items.Add(file);
            file.Category = this;
        }

        public override string ToString()
        {
            return Name; 
        }

        #region SelItem

        public const string SelItemPropertyName = "SelItem";

        private SwfItem _selItem = null;

        public SwfItem SelItem
        {
            get { return _selItem; }

            set
            {
                if (_selItem == value) { return; }
                _selItem = value;
                // Update bindings, no broadcast
                ReportPropertyChanged(SelItemPropertyName);
            }
        }
        #endregion SelItem

    }
}
