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

        public ScenegraphTreeViewer(ExaminerViewer viewer, Separator root)
        {
            InitializeComponent();
            this.Text = Application.ProductName;

            _viewer = viewer;
            _root = root;
            _rootScenegraph = new ScenegraphNode(_root);
            treeViewScene.Nodes.Add(buildTree(_rootScenegraph));

            //setup listener
            _viewer.OnNewSceneGraphLoaded += new NewSceneGraphLoadedHandler(_viewer_OnNewSceneGraphLoaded);
        }

        void _viewer_OnNewSceneGraphLoaded()
        {
            //if this is the case then, we need to shut ourselves down....
            this.Close();
        }

        private TreeNode buildTree(ScenegraphNode node)
        {
            TreeNode tNode = new TreeNode();
            tNode.Tag = node;
            string formatedName = node.Name.Length == 0 ? node.TypeName : String.Format("{0} ({1})", node.TypeName, node.Name);
            tNode.Text = formatedName;
            ScenegraphNode[] children = node.getChildren();
            foreach (ScenegraphNode child in children)
                tNode.Nodes.Add(buildTree(child));
            return tNode;
        }

        private void treeViewScene_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _viewer.setSelection((ScenegraphNode)e.Node.Tag);
        }

        private void ScenegraphTreeViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            //clean ourselves up and get rid of the handler
            _viewer.OnNewSceneGraphLoaded -= new NewSceneGraphLoadedHandler(_viewer_OnNewSceneGraphLoaded);
        }
    }
}