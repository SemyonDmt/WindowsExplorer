using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsExplorer.Infrastructure;
using WindowsExplorer.Models;
using WindowsExplorer.Views;

namespace WindowsExplorer.Controllers
{
    public class ControllerExplorer : IControllerExplorer
    {
        private readonly IRepository _repository;
        public IExplorer View { private get; set; }
        private ObjectFromDisk[] _objectFromDisks;
        public bool SortDescending { get; private set; } = true;
        private CancellationTokenSource _cts;

        public ControllerExplorer(IRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public void SetRootNode() => View?.SetRootNode(_rootNode);

        public void AddDirectoryForNode(TreeNode node)
        {
            node.Nodes.Clear();

            var dir = node.Tag is null ?
                _repository.GetAllDisk() :
                _repository.GetSubDirectory(((DirectoryFromDisk) node.Tag).Patch);

            foreach (var dirInfo in dir)
            {
                var rootNode = new TreeNode
                {
                    Text = dirInfo.Name,
                    Tag = dirInfo,
                };
                if (dirInfo.IsContainsSubDirectory)
                    rootNode.Nodes.Add("1", "fake");

                node.Nodes.Add(rootNode);
            }
        }

        public void ClearDirectoryForNode(TreeNode node)
        {
            node.Nodes.Clear();
            node.Nodes.Add("1", "fake");
        }

        public async Task SetInfoForDirectoryAsync(TreeNode node)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _objectFromDisks = _calcProcessObj;
            View?.SetListView(_calcProcess);

            try
            {
                View?.SetListView(await Task.Run(() => InfoForDirectory(node, _cts.Token), _cts.Token));
            }
            catch
            {
                // ignored
            }

            _cts = null;
        }

        public void InvertSort()
        {
            SortDescending = !SortDescending;
            _objectFromDisks = Sort(_objectFromDisks);
            View?.SetListView(Convert(_objectFromDisks));
        }

        private ListViewItem[] InfoForDirectory(TreeNode node, CancellationToken token)
        {
            try
            {
                var objectsFromDisk = node.Tag is null
                    ? _repository.GetObjectsFromDisk()
                    : _repository.GetObjectsFromDirectory((DirectoryFromDisk) node.Tag, token);

                _objectFromDisks = Sort(objectsFromDisk);

                return Convert(_objectFromDisks);
            }
            catch (DirectoryNotFoundException)
            {
                _objectFromDisks = _dirNotExistObj;
                return _dirNotExist;
            }
            catch (UnauthorizedAccessException)
            {
                _objectFromDisks = _accessDeniedObj;
                return _accessDenied;
            }
        }

        private ObjectFromDisk[] Sort(IEnumerable<ObjectFromDisk> items)
        {
            return SortDescending ? items.OrderByDescending(od => od.Size).ToArray() : items.OrderBy(o => o.Size).ToArray();
        }

        private static ListViewItem[] Convert(IEnumerable<ObjectFromDisk> objectFromDisks)
        {
            return objectFromDisks.Select(o =>
            {
                var item = new ListViewItem(o.Name, o.IsFile ? 1 : 0);
                item.SubItems.Add(o.SizeFormat);
                return item;
            }).ToArray();
        }

        private static readonly ObjectFromDisk[] _calcProcessObj = { new ObjectFromDisk{ IsFile = false, Name = "Производится подсчет размера..." } };
        private static readonly ObjectFromDisk[] _accessDeniedObj = { new ObjectFromDisk { IsFile = false, Name = "Отказано в доступе" } };
        private static readonly ObjectFromDisk[] _dirNotExistObj = { new ObjectFromDisk { IsFile = false, Name = "Директория не существует" } };
        private static readonly ListViewItem[] _calcProcess = { new ListViewItem("Производится подсчет размера...",  0) };
        private static readonly ListViewItem[] _dirNotExist = { new ListViewItem("Директория не существует", 0) };
        private static readonly ListViewItem[] _accessDenied = { new ListViewItem("Отказано в доступе", 0) };
        private static readonly TreeNode _rootNode = new TreeNode { Text = @"All disk", Nodes = {"1", "fake" } };

    }
}
