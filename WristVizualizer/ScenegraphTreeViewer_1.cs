using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libCoin3D;

namespace WristVizualizer
{
    public partial class ScenegraphTreeViewer : Form
    {
        private ExaminerViewer _viewer;
        private Separator _root;
        private ScenegraphNode _rootScenegraph;
        private TreeNode _initiallySelected;

        private bool _confirmedDelete = false;
        private bool _makingChanges = false; //prevents looping on ourselves

        public ScenegraphTreeViewer(ExaminerViewer viewer, Separator root)
        {
            InitializeComponent();
            this.Text = Application.ProductName;

            _viewer = viewer;
            _root = root;
            _rootScenegraph = new ScenegraphNode(_root);
            //get the currently selected node.
            Separator initiallySelectedSeparator = _viewer.getSeparatorForSelection();
            treeViewScene.Nodes.Add(buildTree(_rootScenegraph, initiallySelectedSeparator));

            if (_initiallySelected != null)
                treeViewScene.SelectedNode = _initiallySelected;

            //setup listener
            _viewer.OnNewSceneGraphLoaded += new NewSceneGraphLoadedHandler(_viewer_OnNewSceneGraphLoaded);
            _viewer.OnObjectSelected += new ObjectSelectedHandler(_viewer_OnObjectSelected);
        }

        void _viewer_OnNewSceneGraphLoaded()
        {
            //if this is the case then, we need to shut ourselves down....
            this.Close();
        }

        void _viewer_OnObjectSelected()
        {
            if (_makingChanges) return; //we're doing it, so do nothing

            //get selected node
            Separator currentlySelectedSeparator = _viewer.getSeparatorForSelection();
            if (currentlySelectedSeparator == null) return; //shouldn't be, but lets double check.
            TreeNode selectedTreeNode = findTreeNodeContainingCoinNode(currentlySelectedSeparator);
            if (selectedTreeNode != null)
            {
                _makingChanges = true;
                treeViewScene.SelectedNode = selectedTreeNode;
                _makingChanges = false;
            }
        }

        private TreeNode buildTree(ScenegraphNode node, Separator initiallySelectedNodeToFind)
        {
            TreeNode tNode = new TreeNode();
            tNode.Tag = node;
            //check for this being the currently selected node...
            if (node.isEqualSeparator(initiallySelectedNodeToFind))
                _initiallySelected = tNode;
            string formatedName = node.Name.Length == 0 ? node.TypeName : String.Format("{0} ({1})", node.TypeName, node.Name);
            tNode.Text = formatedName;
            ScenegraphNode[] children = node.getChildren();
            foreach (ScenegraphNode child in children)
                tNode.Nodes.Add(buildTree(child, initiallySelectedNodeToFind));
            return tNode;
        }

        private TreeNode findTreeNodeContainingCoinNode(Separator nodeToSearchFor)
        {
            foreach (TreeNode child in treeViewScene.Nodes)
            {
                TreeNode subSearchResult = findTreeNodeContainingCoinNodeHelper(nodeToSearchFor, child);
                if (subSearchResult != null)
                    return subSearchResult;
            }
            return null; //no luck
        }

        private TreeNode findTreeNodeContainingCoinNodeHelper(Separator nodeToSearchFor, TreeNode searchBase)
        {
            ScenegraphNode baseNode = (ScenegraphNode)searchBase.Tag;
            if (baseNode.isEqualSeparator(nodeToSearchFor))
                return searchBase;

            foreach (TreeNode child in searchBase.Nodes)
            {
                TreeNode subSearchResult = findTreeNodeContainingCoinNodeHelper(nodeToSearchFor, child);
                if (subSearchResult != null)
                    return subSearchResult;
            }

            //no luck
            return null;
        }

        private void treeViewScene_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _makingChanges = true;
            _viewer.setSelection((ScenegraphNode)e.Node.Tag);
            _makingChanges = false;
        }

        private void ScenegraphTreeViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            //clean ourselves up and get rid of the handler
            _viewer.OnNewSceneGraphLoaded -= new NewSceneGraphLoadedHandler(_viewer_OnNewSceneGraphLoaded);
            _viewer.OnObjectSelected -= new ObjectSelectedHandler(_viewer_OnObjectSelected);
        }

        private void treeViewScene_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteCurrentlySelectedNode();
                e.Handled = true;
            }
        }

        private bool checkUserConfirmsFirstDelete()
        {
            if (_confirmedDelete) return true;
            
            //if not, lets show our message
            string msg = String.Format("Warning: Deleting a node from the scenegraph can have unintended consequences!\n{0}Application may crash or throw errors if it tries to access this node after its has been deleted.\n\nAre you sure you wish to delete nodes?", Application.ProductName);
            DialogResult r = MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            if (r != DialogResult.Yes) return false;
            _confirmedDelete = true;
            return true;
        }

        private void deleteCurrentlySelectedNode()
        {
            if (treeViewScene.SelectedNode == null) return;

            TreeNode current = treeViewScene.SelectedNode;
            TreeNode parrent = current.Parent;
            if (parrent == null)
            {
                //tried to delete the _root node, yell at the user
                string msg = "You can not delete the Root node!";
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!checkUserConfirmsFirstDelete()) return; //check if we are allowed to delete
            //green light, lets find the parent and delete it :)


            ScenegraphNode currentNode = (ScenegraphNode)current.Tag;
            ScenegraphNode parrentNode = (ScenegraphNode)parrent.Tag;
            Separator parrentSeparator = parrentNode.ToSeparator();
            parrentSeparator.removeChild(currentNode);

            //remove node from scene graph and make its parrent selected
            parrent.Nodes.Remove(current); 
            treeViewScene.SelectedNode = parrent; 
        }
    }
}