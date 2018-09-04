using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;

namespace VSTiBox
{
    public class MenuControl : UserControl
    {
        private VSTMenuItem mActiveMenuItem;
        private ScrollList scrollList1;
        private VSTMenuItem mBaseMenuItem;

        public VSTMenuItem[] MenuItems
        { 
            get 
            { 
                return mBaseMenuItem.MenuItems; 
            }
        }


        public MenuControl()
        {
           InitializeComponent();

           mBaseMenuItem = VSTMenuItem.CreateBase(backHandler);
           mActiveMenuItem = mBaseMenuItem;
           mBaseMenuItem.ItemsChanged += menuItemsChanged;
           scrollList1.ValueChanged += mScrollList_ValueChanged;
        }

        public int SelectedIndex
        {
            get
            {
                return scrollList1.SelectedIndex;
            }
            set
            {
                if (value >= 0 && value < MenuItems.Count())
                {
                    scrollList1.SelectedIndex = value;
                }
            }
        }

        public void SelectCurrentMenuItem()
        {
            mScrollList_ValueChanged(null, null);
        }

        void mScrollList_ValueChanged(object sender, EventArgs e)
        {
            // Call delegate : find MenuItem which has to be called
            string value = scrollList1 .Items [scrollList1 .SelectedIndex ];
            // New active menu item!
            mActiveMenuItem = mActiveMenuItem.MenuItems.First(x => x.Name == value);
            if (mActiveMenuItem.Action != null)
            {
                mActiveMenuItem.Action(mActiveMenuItem.Name);
            }

            // Open sub menu if available
            if (mActiveMenuItem.MenuItems == null || 
                mActiveMenuItem.MenuItems.Length == 0)
            {
                // Assume delegate has been called ; return to level above
                mActiveMenuItem = mActiveMenuItem.Parent;
            }
            int inx = scrollList1.SelectedIndex;
            scrollList1.Items = mActiveMenuItem.MenuItems.Select(x => x.Name).ToArray();
            // Restore selected index
            scrollList1.SelectedIndex = inx;
        }

        private void backHandler(string text)
        {
            mActiveMenuItem = mActiveMenuItem.Parent;
        }

        public VSTMenuItem AddMenuItem(string name, VSTMenuAction action)
        {
            VSTMenuItem menuItem = mBaseMenuItem.AddMenuItem(name, action, false);
            menuItem.Parent = mBaseMenuItem;
            menuItem.ItemsChanged += menuItemsChanged;
            menuItemsChanged(this, null);
            return menuItem;
        }

        public VSTMenuItem InsertMenuItem(int index, string name, VSTMenuAction action)
        {
            VSTMenuItem menuItem = mBaseMenuItem.InsertMenuItem(index, name, action, false);
            menuItem.Parent = mBaseMenuItem;
            menuItem.ItemsChanged += menuItemsChanged;
            menuItemsChanged(this, null);
            return menuItem;
        }

        public void RemoveMenuItem(VSTMenuItem menuItem)
        {
            mBaseMenuItem.RemoveMenuItem(menuItem);
            menuItem.Parent = mBaseMenuItem;
            menuItem.ItemsChanged -= menuItemsChanged;
            menuItemsChanged(this, null);
        }

        public void ClearMenuItems()
        {
            foreach (VSTMenuItem item in MenuItems)
            {
                mBaseMenuItem.RemoveMenuItem(item);
                item.Parent = mBaseMenuItem;
                item.ItemsChanged -= menuItemsChanged;
            }
            scrollList1.Items = new string[] { };
            scrollList1.Invalidate();
        }

        void menuItemsChanged(object sender, EventArgs e)
        {
            scrollList1.Items = mActiveMenuItem.MenuItems.Select(x => x.Name).ToArray();
            scrollList1.Invalidate();
        }

        private void InitializeComponent()
        {
            this.scrollList1 = new VSTiBox.ScrollList();
            this.SuspendLayout();
            // 
            // scrollList1
            // 
            this.scrollList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollList1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scrollList1.Items = new string[0];
            this.scrollList1.Location = new System.Drawing.Point(0, 0);
            this.scrollList1.Name = "scrollList1";
            this.scrollList1.SelectedIndex = 0;
            this.scrollList1.Size = new System.Drawing.Size(150, 150);
            this.scrollList1.TabIndex = 0;
            this.scrollList1.TextColor = System.Drawing.Color.White;
            // 
            // MenuControl
            // 
            this.Controls.Add(this.scrollList1);
            this.Name = "MenuControl";
            this.ResumeLayout(false);

        }

        //private void InitializeComponent()
        //{
        //    this.mScrollList = new VSTiBox.Controls.ScrollList();
        //    this.SuspendLayout();
        //    // 
        //    // scrollList2
        //    // 
        //    this.mScrollList.Dock = System.Windows.Forms.DockStyle.Fill;
        //    this.mScrollList.Location = new System.Drawing.Point(0, 0);
        //    this.mScrollList.Name = "scrollList2";
        //    this.mScrollList.SelectedIndex = 0;
        //    this.mScrollList.Size = new System.Drawing.Size(150, 150);
        //    this.mScrollList.TabIndex = 0;
        //    this.mScrollList.TextColor = System.Drawing.Color.White;
        //    // 
        //    // MenuControl
        //    // 
        //    this.Controls.Add(this.mScrollList);
        //    this.Name = "MenuControl";
        //    this.ResumeLayout(false);
        //}
    }

    public delegate void VSTMenuAction(string name);
}