using Microsoft.VisualBasic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for Line.xaml
    /// </summary>
    public partial class Line : Window
    {
        private DataAccess da = new DataAccess();
        public Line()
        {
            InitializeComponent();
            Output.ItemsSource = da.LoadLine_list();
        }

        private void create_db_btn_Click(object sender, RoutedEventArgs e)
        {
            if (new_line.Text != "")
            {
                DataAccess.SetDb(new_line.Text);
                if (!File.Exists(DataAccess.GetDb()))
                {
                    da.CreateDb();
                    Output.ItemsSource = da.LoadLine_list();
                    new_line.Text = "";
                }
                else
                {
                    int num = (int)MessageBox.Show("Line Exists");
                }
            }
            else
            {
                int num1 = (int)MessageBox.Show("Please Enter Line Number");
            }
        }

        private void del_db_Click(object sender, RoutedEventArgs e)
        {
            string dbname = Interaction.InputBox("Enter Line Number:", "Delete Line", "", -1, -1);
            if (dbname != "")
            {
                DataAccess.SetDb(dbname);
                if (!File.Exists(DataAccess.GetDb()))
                {
                    int num1 = (int)MessageBox.Show("Line Doesn't Exists");
                }
                else if (da.DeleteDb(dbname))
                {
                    int num2 = (int)MessageBox.Show("Line Deleted");
                    Output.ItemsSource = da.LoadLine_list();
                }
                else
                {
                    int num3 = (int)MessageBox.Show("Delete Failed");
                }
            }
            else
            {
                int num = (int)MessageBox.Show("Delete Failed");
            }
        }

        private void Output_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (object selectedItem in Output.SelectedItems)
            {
                DataAccess.SetDb(selectedItem as string);
                new MainWindow().Show();
                Close();
            }
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
