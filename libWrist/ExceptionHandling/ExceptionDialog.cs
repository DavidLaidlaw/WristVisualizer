using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace libWrist.ExceptionHandling
{
    public partial class ExceptionDialog : Form
    {
        private const int _intSpacing = 10;

        public ExceptionDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// security-safe process.start wrapper
        /// </summary>
        private void LaunchLink(string strUrl)
        {
            try
            {
                System.Diagnostics.Process.Start(strUrl);
            }
            catch (System.Security.SecurityException) { }
        }

        private void SizeBox(RichTextBox ctl)
        {
            Graphics g = null;
            try
            {
                // note that the height is taken as MAXIMUM, so size the label for maximum desired height!
                g = Graphics.FromHwnd(ctl.Handle);
                SizeF objSizeF = g.MeasureString(ctl.Text, ctl.Font, new SizeF(ctl.Width, ctl.Height));
                g.Dispose();
                g = null;
                ctl.Height = Convert.ToInt32(objSizeF.Height) + 5;
                if (String.IsNullOrEmpty(ctl.Text))
                    ctl.Visible = false;
            }
            catch (System.Security.SecurityException) { } //do nothing; we can't set control sizes without full trust
            finally
            {
                if (g != null)
                    g.Dispose();
            }
        }

        private DialogResult DetermineDialogResult(string strButtonText)
        {
            //strip any accelerator keys we might have
            strButtonText = strButtonText.Replace("&", "");
            switch (strButtonText.ToLower())
            {
                case "abort":
                    return DialogResult.Abort;
                case "cancel":
                    return DialogResult.Cancel;
                case "ignore":
                    return DialogResult.Ignore;
                case "no":
                    return DialogResult.No;
                case "none":
                    return DialogResult.None;
                case "ok":
                    return DialogResult.OK;
                case "retry":
                    return DialogResult.Retry;
                case "yes":
                    return DialogResult.Yes;
                default:
                    return DialogResult.None;
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DetermineDialogResult(((Button)sender).Text);
        }

        private void ExceptionDialog_Load(object sender, EventArgs e)
        {
            //make sure our window is on top
            this.TopMost = true;
            this.TopMost = false;

            //More >> has to be expanded
            this.txtMore.Anchor = AnchorStyles.None;
            this.txtMore.Visible = false;

            //size the labels' height to accommodate the amount of text in them
            SizeBox(ScopeBox);
            SizeBox(ActionBox);
            SizeBox(ErrorBox);

            //now shift everything up
            lblScopeHeading.Top = ErrorBox.Top + ErrorBox.Height + _intSpacing;
            ScopeBox.Top = lblScopeHeading.Top + lblScopeHeading.Height + _intSpacing;

            lblActionHeading.Top = ScopeBox.Top + ScopeBox.Height + _intSpacing;
            ActionBox.Top = lblActionHeading.Top + lblActionHeading.Height + _intSpacing;

            lblMoreHeading.Top = ActionBox.Top + ActionBox.Height + _intSpacing;
            btnMore.Top = lblMoreHeading.Top - 3;

            this.Height = btnMore.Top + btnMore.Height + _intSpacing + 45;

            this.CenterToScreen();
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            if (btnMore.Text == ">>")
            {
                this.Height = this.Height + 300;
                txtMore.Location = new Point(lblMoreHeading.Left, lblMoreHeading.Top + lblMoreHeading.Height + _intSpacing);
                txtMore.Height = this.ClientSize.Height - txtMore.Top - 45;
                txtMore.Width = this.ClientSize.Width - 2 * _intSpacing;
                txtMore.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                txtMore.Visible = true;
                btn3.Focus();
                btnMore.Text = "<<";
            }
            else
            {
                this.SuspendLayout();
                btnMore.Text = ">>";
                this.Height = btnMore.Top + btnMore.Height + _intSpacing + 45;
                txtMore.Visible = false;
                txtMore.Anchor = AnchorStyles.None;
                this.ResumeLayout();
            }
        }

        private void Box_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            LaunchLink(e.LinkText);
        }
    }
}