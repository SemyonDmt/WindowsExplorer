using System.Windows.Forms;

namespace WindowsExplorer.Views
{
    public interface IExplorer
    {
        void SetListView(ListViewItem[] items);
        void SetRootNode(TreeNode rootNode);
    }
}