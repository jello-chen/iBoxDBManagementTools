using iBoxDB.LocalServer;
using iBoxDBManagementTools.Extensions;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace iBoxDBManagementTools
{
    public partial class MainWindow : Window
    {
        private DB server;
        private DB.AutoBox db;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (server?.IsClosed() == false)
                server.Close();
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog directoryDialog = new FolderBrowserDialog();
            directoryDialog.RootFolder = Environment.SpecialFolder.DesktopDirectory;
            if(directoryDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tbFilePath.Text = directoryDialog.SelectedPath;
            }
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbFilePath.Text))
            {
                System.Windows.MessageBox.Show("Please select iBoxDB file", "Warning");
                return;
            }
            if (db == null)
            {
                server = new DB(this.tbFilePath.Text);
                db = server.Open();
            }
            var tables = db.GetDatabase().GetSchemata().Keys;
            foreach (string table in tables)
            {
                if (!table.StartsWith("__"))
                    this.cbTable.Items.Add(table);
            }
        }

        private void CbTable_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) 
            => this.tbSql.Text = $"from {e.AddedItems[0]}";

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            var sql = this.tbSql.SelectedText;
            if (string.IsNullOrWhiteSpace(sql))
            {
                sql = this.tbSql.Text;
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                System.Windows.MessageBox.Show("Please input your querying sentence", "Warning");
                return;
            }

            if (server == null || server.IsClosed())
            {
                System.Windows.MessageBox.Show("Please connect before querying", "Warning");
                return;
            }

            var ql = string.Empty;
            var args = new string[0];
            var ci = sql.IndexOf(',');
            if(ci != -1)
            {
                ql = sql.Substring(0..ci);
                if(ci < sql.Length - 1)
                {
                    args = sql.Substring(ci + 1).Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            else
            {
                ql = sql;
            }

            var locals = db.Select(ql, args);
            var dt = locals.ToDataTable();
            this.dg.ItemsSource = dt.DefaultView;
        }
    }
}
