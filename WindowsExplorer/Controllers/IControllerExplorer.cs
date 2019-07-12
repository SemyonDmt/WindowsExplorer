using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsExplorer.Views;

namespace WindowsExplorer.Controllers
{
    public interface IControllerExplorer
    {
        IExplorer View { set; }
        bool SortDescending { get; }
        void SetRootNode();
        void AddDirectoryForNode(TreeNode node);
        void ClearDirectoryForNode(TreeNode node);
        Task SetInfoForDirectoryAsync(TreeNode node);
        void InvertSort();
    }
}