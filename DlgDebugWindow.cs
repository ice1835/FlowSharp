﻿using System;
using System.Linq;
using System.Windows.Forms;

using FlowSharpLib;

namespace FlowSharp
{
    public partial class DlgDebugWindow : Form
    {
        protected CanvasController controller;

        public DlgDebugWindow(CanvasController controller)
        {
            this.controller = controller;
            InitializeComponent();
            PopulateWithShapes();
            tvShapes.ExpandAll();
        }

        public void Trace(string msg)
        {
            if (ckTraceEnabled.Checked)
            {
                if (msg.StartsWith("Route:") && ckRoutingEvents.Checked)
                {
                    tbTrace.AppendText(msg);
                }

                if (msg.StartsWith("Shape:") && ckShapeEvents.Checked)
                {
                    tbTrace.AppendText(msg);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            tvShapes.Nodes.Clear();
            PopulateWithShapes();
            tvShapes.ExpandAll();
        }

        protected void PopulateWithShapes()
        {
            foreach (GraphicElement el in controller.Elements)
            {
                TreeNode node = CreateTreeNode(el);

                if (el.Connections.Any())
                {
                    TreeNode connectors = new TreeNode("Connectors");
                    node.Nodes.Add(connectors);
                    AddConnections(connectors, el);
                }

                if (el.IsConnector)
                {
                    Connector c = (Connector)el;

                    if (c.StartConnectedShape != null)
                    {
                        node.Nodes.Add(CreateTreeNode(c.StartConnectedShape, "Start: "));
                    }

                    if (c.EndConnectedShape != null)
                    {
                        node.Nodes.Add(CreateTreeNode(c.EndConnectedShape, "End: "));
                    }
                }

                tvShapes.Nodes.Add(node);
            }
        }

        protected void AddConnections(TreeNode node, GraphicElement el)
        {
            el.Connections.ForEach(c =>
            {
                node.Nodes.Add(CreateTreeNode(c.ToElement));
            });
        }

        protected TreeNode CreateTreeNode(GraphicElement el, string prefix = "")
        {
            TreeNode node = new TreeNode(prefix + el.ToString());
            node.Tag = el;

            return node;
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            tbTrace.Text = "";
        }

        private void ckTraceEnabled_CheckedChanged(object sender, EventArgs e)
        {
            ckRoutingEvents.Enabled = ckTraceEnabled.Checked;
            ckShapeEvents.Enabled = ckTraceEnabled.Checked;
        }
    }
}
