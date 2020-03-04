using System;
using System.IO;
using System.Windows.Forms;

namespace WebBrowser
{
    public partial class Form1 : Form
    {
        int i = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((System.Windows.Forms.WebBrowser) tabControl1.SelectedTab.Controls[0]).GoBack();
        }

        private void forwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((System.Windows.Forms.WebBrowser)tabControl1.SelectedTab.Controls[0]).GoForward();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((System.Windows.Forms.WebBrowser)tabControl1.SelectedTab.Controls[0]).Refresh();
        }

        private void goToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabCount == 0)
            {
                addToolStripMenuItem_Click(null, null);
            }

            if (!string.IsNullOrEmpty(toolStripTextBox1.Text))
            {
                ((System.Windows.Forms.WebBrowser)tabControl1.SelectedTab.Controls[0]).Navigate(toolStripTextBox1.Text);
            }
        }

        private void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) 13)
            {
                e.Handled = true;
                goToolStripMenuItem_Click(null, null);
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.WebBrowser web = new System.Windows.Forms.WebBrowser();
            SetWebBrowserCompatiblityLevel();
            web.Visible = true;
            web.ScriptErrorsSuppressed = true;
            web.Dock = DockStyle.Fill;
            web.DocumentCompleted += Web_DocumentCompleted;
            tabControl1.TabPages.Add("New Page");
            tabControl1.SelectTab(i);
            tabControl1.SelectedTab.Controls.Add(web);
            i++;
        }

        private void Web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            tabControl1.SelectedTab.Text = ((System.Windows.Forms.WebBrowser) tabControl1.SelectedTab.Controls[0]).DocumentTitle;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
                i--;
            }
        }

        #region Browser Compatiblity

        private static void SetWebBrowserCompatiblityLevel()
        {
            string appName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            int lvl = 1000 * GetBrowserVersion();
            bool fixVShost = File.Exists(Path.ChangeExtension(Application.ExecutablePath, ".vshost.exe"));
            WriteCompatiblityLevel("HKEY_LOCAL_MACHINE", appName + ".exe", lvl);
            if (fixVShost) WriteCompatiblityLevel("HKEY_LOCAL_MACHINE", appName + ".vshost.exe", lvl);
            WriteCompatiblityLevel("HKEY_CURRENT_USER", appName + ".exe", lvl);
            if (fixVShost) WriteCompatiblityLevel("HKEY_CURRENT_USER", appName + ".vshost.exe", lvl);
        }

        private static void WriteCompatiblityLevel(string root, string appName, int lvl)
        {
            try
            {
                Microsoft.Win32.Registry.SetValue(root + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", appName, lvl);
            }
            catch (Exception) { }
        }

        public static int GetBrowserVersion()
        {
            string strKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer";
            string[] ls = new string[] { "svcVersion", "svcUpdateVersion", "Version", "W2kVersion" };
            int maxVer = 0;
            for (int i = 0; i < ls.Length; ++i)
            {
                object objVal = Microsoft.Win32.Registry.GetValue(strKeyPath, ls[i], "0");
                string strVal = Convert.ToString(objVal);
                if (strVal != null)
                {
                    int iPos = strVal.IndexOf('.');
                    if (iPos > 0)
                        strVal = strVal.Substring(0, iPos);

                    int res = 0;
                    if (int.TryParse(strVal, out res))
                        maxVer = Math.Max(maxVer, res);
                }
            }
            return maxVer;
        }

        #endregion
    }
}