using System;
using System.Windows.Forms;
using WindowsExplorer.Controllers;

namespace WindowsExplorer.Views
{
    public partial class Explorer : Form, IExplorer
    {
        private readonly IControllerExplorer _controller;

        public Explorer(IControllerExplorer controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _controller.View = this;
            InitializeComponent();
        }

        public void SetRootNode(TreeNode rootNode)
        {
            treeView1.Nodes.Add(rootNode);
        }

        public void SetListView(ListViewItem[] items)
        {
            this.listView1.Items.Clear();
            listView1.Items.AddRange(items);
        }

        private void Explorer_Load(object sender, EventArgs e)
        {
            _controller.SetRootNode();
        }

        private async void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            await _controller.SetInfoForDirectoryAsync(e.Node);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            const char desc = '\u25b2';
            const char asc = '\u25bc';

            _controller.InvertSort();

            listView1.Columns[e.Column].Text = _controller.SortDescending
                ? listView1.Columns[e.Column].Text.Replace(desc, asc)
                : listView1.Columns[e.Column].Text.Replace(asc, desc);
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            _controller.AddDirectoryForNode(e.Node);
        }

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            _controller.ClearDirectoryForNode(e.Node);
        }
    }
}
