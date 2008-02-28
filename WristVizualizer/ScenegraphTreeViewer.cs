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
        private Separator _root;
        private ScenegraphNode _rootScenegraph;

        public ScenegraphTreeViewer(Separator root)
        {
            InitializeComponent();
            this.Text = Application.ProductName;

            _root = root;
            _rootScenegraph = new ScenegraphNode(_root);
            treeViewScene.Nodes.Add(buildTree(_rootScenegraph));
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
    }
}