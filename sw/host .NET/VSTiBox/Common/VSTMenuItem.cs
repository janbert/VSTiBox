using System;
using System.Collections.Generic;
using System.Text;

namespace VSTiBox
{
    public class VSTMenuItem
    {
        public VSTMenuItem Parent { get; set; }
        public string Name;
        public VSTMenuAction Action;
        protected List<VSTMenuItem> mMenuItems = new List<VSTMenuItem>();
        public VSTMenuAction Back;

        public VSTMenuItem[] MenuItems
        {
            get
            {
                return mMenuItems.ToArray();
            }
        }

        public event EventHandler ItemsChanged;

        public static VSTMenuItem CreateBase(VSTMenuAction back)
        {
            return new VSTMenuItem("Base", null, back, null, true);
        }
        
        private VSTMenuItem(string name, VSTMenuItem parent, VSTMenuAction back, VSTMenuAction action, Boolean isBase)
        {
            Name = name;
            Action = action;
            Back = back;
            if (!isBase)
            {
                VSTMenuItem backItem = new VSTMenuItem("Back", parent, back, back, true);
                backItem.Parent = parent;
                mMenuItems.Add(backItem);
            }
        }

        public VSTMenuItem AddMenuItem(string name, VSTMenuAction action, Boolean allowBack)
        {
            VSTMenuItem menuItem = new VSTMenuItem(name, this, this.Back, action, !allowBack);
            mMenuItems.Add(menuItem);
            menuItem.Parent = this;
            menuItem.ItemsChanged += itemsChanged;
            itemsChanged(this, null);
            return menuItem;
        }

        public VSTMenuItem InsertMenuItem(int index, string name, VSTMenuAction action, Boolean allowBack)
        {
            VSTMenuItem menuItem = new VSTMenuItem(name, this, this.Back, action, !allowBack);
            mMenuItems.Insert(index, menuItem);
            menuItem.Parent = this;
            menuItem.ItemsChanged += itemsChanged;
            itemsChanged(this, null);
            return menuItem;
        }

        public void RemoveMenuItem(VSTMenuItem menuItem)
        {
            mMenuItems.Remove(menuItem);
            menuItem.Parent = this;
            menuItem.ItemsChanged -= itemsChanged;
            itemsChanged(this, null);
        }

        public void RenameMenuItem(string name)
        {
            this.Name = name;
            itemsChanged(this, null);
        }

        void itemsChanged(object sender, EventArgs e)
        {
            if (ItemsChanged != null)
            {
                ItemsChanged(sender, e);
            }
        }
    }
}
