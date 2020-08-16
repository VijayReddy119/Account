using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<records> data_inp_send = new List<records>();
        private static DataAccess dataAccess = new DataAccess();
        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";

            Line_no.Text = DataAccess.GetDbName();
            dataAccess.ModifyTableData();
        }
        public static void Set_InpSend(List<string> inp)
        {
            records rec = new records(inp[0], inp[1], Int32.Parse(inp[2]), inp[3], inp[4], decimal.Parse(inp[5]), decimal.Parse(inp[6]), decimal.Parse(inp[7]), inp[8]);
            if (dataAccess.Insert_IntoTemp(rec))
            {
                data_inp_send.Add(rec);
            }
            else
            {
                MessageBox.Show("Posting Failed. Please Restart the Application");
            }
        }
        public static List<records> Get_InpSend()
        {
            List<records> updateD = dataAccess.Get_Temp();
            return updateD;
        }
        public static void Reset_data_inp()
        {
            data_inp_send = new List<records>();
            dataAccess.DropTable();
        }
        public static bool update_elem(records inp)
        {
            if (dataAccess.Remove_Temp(inp.posting_id))
            {
                Remove_elem(inp.posting_id);
                List<string> inpdata = new List<string>();

                inpdata.Add(inp.posting_id);
                inpdata.Add(inp.date);
                inpdata.Add(inp.slno.ToString());
                inpdata.Add(inp.name);
                inpdata.Add(inp.details);
                inpdata.Add(inp.reciept.ToString());
                inpdata.Add(inp.payment.ToString());
                inpdata.Add(inp.interest.ToString());
                inpdata.Add(inp.acc_id);

                Set_InpSend(inpdata);
                return true;
            }
            else
            {
                MessageBox.Show("Update Failed");
            }
            return false;
        }
        public static bool Remove_elem(string pos_id)
        {
            for (int i = 0; i < data_inp_send.Count; i++)
            {
                if (data_inp_send[i].posting_id == pos_id)
                {
                    if (dataAccess.Remove_Temp(pos_id))
                        data_inp_send.RemoveAt(i);
                    else
                        return false;
                    return true;
                }
            }
            return false;
        }
        public static decimal[] Get_sum()
        {
            decimal[] sum = new decimal[2];
            for (int i = 0; i < data_inp_send.Count; i++)
            {
                sum[0] += data_inp_send[i].reciept;
                sum[1] += data_inp_send[i].payment;
            }
            return sum;
        }

        public static bool isPosted(string slno_inp, string date_)
        {
            if (dataAccess.isPosted(slno_inp, date_))
            {
                return true;
            }
            else if (dataAccess.isPosted_temp(slno_inp, date_))
            {
                return true;
            }
            return false;
        }

        private void newCustomers_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("New_Customers.xaml", UriKind.Relative);
        }

        private void Posting_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("Posting.xaml", UriKind.Relative);
        }

        private void totalCustomers_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("AccountsDisplay.xaml", UriKind.Relative);
        }

        private void Village_wise_list_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("CapitalAccount.xaml", UriKind.Relative);
        }

        private void chits_list_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("ChitsList.xaml", UriKind.Relative);
        }

        private void trail_bal_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("Trail_balance.xaml", UriKind.Relative);
        }

        private void daily_posting_list_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("Daily_posting.xaml", UriKind.Relative);
        }

        private void partys_list_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("PartysList.xaml", UriKind.Relative);
        }
        private void close_Click(object sender, RoutedEventArgs e)
        {
            new Line().Show();
            Close();
        }

        private void partys_closing_list_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("ClosingList.xaml", UriKind.Relative);
        }

        private void old_accounts_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("Old_accounts_display.xaml", UriKind.Relative);
        }
    }
}
