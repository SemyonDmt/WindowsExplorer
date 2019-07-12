using System.Windows.Forms;

namespace WindowsExplorer
{
    public interface IExplorer
    {
        void SetListView(ListViewItem[] items);
        void SetRootNode(TreeNode rootNode);
    }
}