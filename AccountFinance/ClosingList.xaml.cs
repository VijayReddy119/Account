using AccountFinance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BHAGAVANDVSOFTWARE
{
    /// <summary>
    /// Interaction logic for ClosingList.xaml
    /// </summary>
    public partial class ClosingList : Page
    {
        private static readonly Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
        DataAccess dataAccess = new DataAccess();
        private List<account> acc_list = new List<account>();
        public ClosingList()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            todays_date.SelectedDate = DateTime.Now;

            acc_list = dataAccess.Load_acc_db();

            List<string> village_list = dataAccess.Get_village_list();
            foreach (var x in village_list)
            {
                search_by_village.Items.Add(x);
            }

            acc_list = new List<account>();
            acc_list = dataAccess.timeoutList(0, DateTime.Now.ToString("dd-MM-yyyy"), closeList: true);
            Output.Items.Refresh();
            Output.ItemsSource = acc_list;
            loadSum();
        }
        private void loadSum()
        {
            int total_amt = 0, total_paid = 0, total_bal = 0;
            if (acc_list != null)
            {
                foreach (var x in acc_list)
                {
                    total_amt += int.Parse(x.reciept.ToString());
                    total_paid += int.Parse(x.payment.ToString());
                    total_bal += int.Parse(x.balance.ToString());
                }
                total_amt_sum.Text = "Total Amt: " + total_amt;
                total_paid_sum.Text = "Total Paid: " + total_paid;
                total_bal_sum.Text = "Total Balance: " + total_bal;
            }
            else
            {
                total_amt_sum.Text = "Total Amt: " + total_amt;
                total_paid_sum.Text = "Total Paid: " + total_paid;
                total_bal_sum.Text = "Total Balance: " + total_bal;
            }
        }
        private void todays_date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (todays_date.SelectedDate != null)
            {
                acc_list = dataAccess.timeoutList(0, todays_date.SelectedDate.Value.ToString("dd-MM-yyyy"), closeList: true);
                Output.ItemsSource = acc_list;
                loadSum();
            }
        }

        private void search_by_village_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(todays_date.SelectedDate != null)
            {
                if (search_by_village.SelectedValue != null)
                {
                    acc_list = dataAccess.timeoutList(0, todays_date.SelectedDate.Value.ToString("dd-MM-yyyy"), village: search_by_village.SelectedValue.ToString(), closeList: true);
                    Output.ItemsSource = acc_list;
                    loadSum();
                }
            }
            else
            {
                if (search_by_village.SelectedValue != null)
                {
                    acc_list = dataAccess.timeoutList(0, village: search_by_village.SelectedValue.ToString(), closeList: true);
                    Output.ItemsSource = acc_list;
                    loadSum();
                }
            }
        }

        private void search_by_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(todays_date.SelectedDate != null)
            {
                if (search_by_village.SelectedValue == null)
                {
                    acc_list = dataAccess.timeoutList(0, todays_date.SelectedDate.Value.ToString("dd-MM-yyyy"), closeList: true);
                    Output.ItemsSource = acc_list;
                    loadSum();
                }
                else if (search_by_village.SelectedValue != null)
                {
                    acc_list = dataAccess.timeoutList(0, todays_date.SelectedDate.Value.ToString("dd-MM-yyyy"), village: search_by_village.SelectedValue.ToString(), closeList: true);
                    Output.ItemsSource = acc_list;
                    loadSum();
                }
            }
            else
            {
                if (search_by_village.SelectedValue == null)
                {
                    acc_list = dataAccess.timeoutList(0, closeList: true);
                    Output.ItemsSource = acc_list;
                    loadSum();
                }
                else if (search_by_village.SelectedValue != null)
                {
                    acc_list = dataAccess.timeoutList(0, village: search_by_village.SelectedValue.ToString(), closeList: true);
                    Output.ItemsSource = acc_list;
                    loadSum();
                }
            }
        }
        private void print_btn_Click(object sender, RoutedEventArgs e)
        {
            Print_Data("Total_Customers_Report.rdlc", "Customer", new List<string>() { "slno", "name", "village", "total", "paid", "bal", "from_date", "to_date", "type" });
        }
        private void Print_Data(string file, string data_src, List<string> col_name)
        {
            if (acc_list != null)
            {
                
            }
            else
            {
                MessageBox.Show("Can't Print Empty");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Print_Data("Village_wise_Report.rdlc", "Village", new List<string>() { "village", "slno", "name", "total", "paid", "bal", "from_date", "to_date", "type" });
        }

        private void schquery_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(todays_date.SelectedDate != null)
            {
                if (schquery.Text != "")
                {
                    if (search_by_village.SelectedValue == null)
                    {
                        acc_list = dataAccess.timeoutList(0, todays_date.SelectedDate.Value.ToString("dd-MM-yyyy"), name: schquery.Text, closeList: true);
                        Output.ItemsSource = acc_list;
                        loadSum();
                    }
                    else if (search_by_village.SelectedValue != null)
                    {
                        acc_list = dataAccess.timeoutList(0, todays_date.SelectedDate.Value.ToString("dd-MM-yyyy"), village: search_by_village.SelectedValue.ToString(), name: schquery.Text, closeList: true);
                        Output.ItemsSource = acc_list;
                        loadSum();
                    }
                }
                else
                {
                    acc_list = dataAccess.timeoutList(0,closeList: true);
                    Output.ItemsSource = acc_list;
                    loadSum();
                }
            }
            else
            {
                if (schquery.Text != "")
                {
                    if (search_by_village.SelectedValue == null)
                    {
                        acc_list = dataAccess.timeoutList(0,name: schquery.Text, closeList: true);
                        Output.ItemsSource = acc_list;
                        loadSum();
                    }
                    else if (search_by_village.SelectedValue != null)
                    {
                        acc_list = dataAccess.timeoutList(0,village: search_by_village.SelectedValue.ToString(), name: schquery.Text, closeList: true);
                        Output.ItemsSource = acc_list;
                        loadSum();
                    }
                }
                else
                {
                    acc_list = dataAccess.timeoutList(0, closeList: true);
                    Output.ItemsSource = acc_list;
                    loadSum();
                }
            }
            loadSum();
        }

        private void date_change_btn_Checked(object sender, RoutedEventArgs e)
        {
            todays_date.SelectedDate = null;
            acc_list = new List<account>();
            acc_list = dataAccess.timeoutList(0, closeList: true);
            Output.Items.Refresh();
            Output.ItemsSource = acc_list;
            loadSum();
        }

        private void date_change_btn_Unchecked(object sender, RoutedEventArgs e)
        {
            todays_date.SelectedDate = DateTime.Now;
        }
    }
}
