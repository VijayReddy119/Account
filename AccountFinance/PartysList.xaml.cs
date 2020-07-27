using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for PartysList.xaml
    /// </summary>
    public partial class PartysList : Page
    {
        private DataAccess dataAccess = new DataAccess();
        private List<account> account = new List<account>();
        private Dictionary<string, string> acc_id_name = new Dictionary<string, string>();
        private Dictionary<string, string> acc_name_id = new Dictionary<string, string>();
        private string slno_to_use = "";
        public PartysList()
        {
            InitializeComponent();
            account = dataAccess.Load_acc_db("", "Partys", 0, "", false);
            monthly_int_date.SelectedDate = DateTime.Now;
            if (account != null)
            {
                foreach (account account in account)
                {
                    slno_combo.Items.Add(account.slno);
                    name_combo.Items.Add(account.name);
                    acc_id_name.Add(account.slno.ToString(), account.name);
                    acc_name_id.Add(account.name, account.slno.ToString());
                }
            }
            List<string> village_ls = dataAccess.Get_village_list();
            foreach (var x in village_ls)
            {
                village_combo.Items.Add(x);
            }
            Acc_Disp_Load("");
        }

        private void village_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (village_combo.SelectedValue == null)
                return;
            string village_ = village_combo.SelectedValue.ToString();
            Reset_Details();
            Acc_Disp_Load(village: village_);
        }
        private void Acc_Disp_Load(string slno_inp = "", string village = "", bool monthly_int = false, string month="")
        {
            Reset_Details();

            if (monthly_int)
            {
                if (slno_inp == "" && village == "")
                {
                    account = dataAccess.Load_acc_db("", "Partys", 0, "", false, monthly_int: true, month: month);
                }
                else if (slno_inp != "" && village == "")
                {
                    slno_to_use = slno_inp;
                    account = dataAccess.Load_acc_db(slno_to_use, "Partys", 0, "", false, monthly_int: true, month: month);
                }
                else if (village != "" && slno_inp != "")
                {
                    account = dataAccess.Load_acc_db(slno_to_use, "Partys", 0, "", false, village_sch: village, monthly_int: true, month: month);
                }
                else if (village != "" && slno_inp == "")
                {
                    account = dataAccess.Load_acc_db("", "Partys", 0, "", false, village_sch: village, monthly_int: true, month: month);
                }
            }
            else
            {
                if (slno_inp == "" && village == "")
                {
                    account = dataAccess.Load_acc_db("", "Partys", 0, "", false);
                }
                else if (slno_inp != "" && village == "")
                {
                    slno_to_use = slno_inp;
                    account = dataAccess.Load_acc_db(slno_to_use, "Partys", 0, "", false);
                }
                else if (village != "" && slno_inp != "")
                {
                    account = dataAccess.Load_acc_db(slno_to_use, "Partys", 0, "", false, village_sch: village);
                }
                else if (village != "" && slno_inp == "")
                {
                    account = dataAccess.Load_acc_db("", "Partys", 0, "", false, village_sch: village);
                }
            }


            if (account == null)
                return;
            Output.ItemsSource = account;
            Decimal num1 = new Decimal();
            Decimal num2 = new Decimal();
            Decimal int_partys = new Decimal();
            foreach (account account in account)
            {
                num1 += account.payment;
                num2 += account.reciept;
                int_partys += account.interest;
            }
            TextBox chitTotalR = partys_total_r;
            Decimal num3 = Math.Abs(num1);
            string str1 = num3.ToString();
            chitTotalR.Text = str1;
            TextBox chitTotalP = partys_total_p;
            num3 = Math.Abs(num2);
            string str2 = num3.ToString();
            chitTotalP.Text = str2;
            Decimal num4 = num1 - num2;
            if (num4 < Decimal.Zero)
            {
                TextBox chitBal = partys_bal;
                num3 = Math.Abs(num4);
                string str3 = "-" + num3.ToString();
                chitBal.Text = str3;
            }
            else
            {
                TextBox chitBal = partys_bal;
                num3 = Math.Abs(num4);
                string str3 = num3.ToString();
                chitBal.Text = str3;
            }
            partys_total_i.Text = int_partys.ToString();
        }

        private void Reset_Details()
        {
            Output.ItemsSource = null;
        }

        private void print_btn_Click(object sender, RoutedEventArgs e)
        {
            if (account != null)
                new Report_Window("Accountdata", acc_list: account, null, "Partys_Report.rdlc", new List<string>() { "Date", "Slno", "Name", "Village", "Reciept", "Payment", "Balance", "Interest" }).Show();
        }

        private void print_btn_village_wise_Click(object sender, RoutedEventArgs e)
        {
            if (account != null)
                new Report_Window("Accountdata", acc_list: account, null, "Partys_Report_VillageWise.rdlc", new List<string>() { "Date", "Slno", "Name", "Village", "Reciept", "Payment", "Balance", "Interest" }).Show();
        }
        private void print_btn_details_Click(object sender, RoutedEventArgs e)
        {
            var date_month = monthly_int_date.SelectedDate.Value.ToString("MM-yyyy");
            
            if(slno_combo.SelectedValue == null && village_combo.SelectedValue == null)
            {
                Acc_Disp_Load("", "", true, date_month);
            }
            else
            {
                if(slno_combo.SelectedValue == null && village_combo.SelectedValue != null)
                {
                    Acc_Disp_Load("", village_combo.Text, true, date_month);
                }
                else
                {
                    Acc_Disp_Load(slno_combo.Text, "", true, date_month);
                }
            }

            if (account != null)
                new Report_Window(data_source: "Accountdata", acc_list: account, file_name: "Sample_Report.rdlc").Show();
            else
                MessageBox.Show("Empty Data");
        }

        private void slno_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(slno_combo.SelectedValue != null)
            {
                slno_to_use = slno_combo.SelectedValue.ToString();
                if (acc_id_name.ContainsKey(slno_to_use))
                {
                    name_combo.SelectedItem = (object)acc_id_name[slno_to_use];
                    Acc_Disp_Load(slno_to_use);
                }
                else
                {
                    name_combo.Text="";
                }
            }
        }

        private void slno_combo_KeyUp(object sender, KeyEventArgs e)
        {
            if(slno_combo.Text == "")
            {
                slno_combo.SelectedValue = null;
                name_combo.SelectedValue = null;
                name_combo.Text = "";
                slno_combo.Text = "";
                Acc_Disp_Load(village: village_combo.Text);
            }
            else
            {
                slno_to_use = slno_combo.Text;
                if (acc_id_name.ContainsKey(slno_to_use))
                {
                    name_combo.SelectedItem = (object)acc_id_name[slno_to_use];
                    Acc_Disp_Load(slno_to_use);
                }
                else
                {
                    name_combo.Text = "";
                }
            }
        }

        private void name_combo_KeyUp(object sender, KeyEventArgs e)
        {
            if(name_combo.Text == "")
            {
                slno_combo.SelectedValue = null;
                name_combo.SelectedValue = null;
                name_combo.Text = "";
                slno_combo.Text = "";
                Acc_Disp_Load(village:village_combo.Text);
            }
            else
            {
                string key = name_combo.Text;
                if (acc_name_id.ContainsKey(key))
                {
                    slno_combo.Text = acc_name_id[key];
                    Acc_Disp_Load(acc_name_id[key]);
                }
                else
                {
                    slno_combo.Text = "";
                }
            }
        }

        private void name_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(name_combo.SelectedValue != null)
            {
                string key = name_combo.SelectedValue.ToString();
                if (acc_name_id.ContainsKey(key))
                {
                    slno_combo.Text = acc_name_id[key];
                    Acc_Disp_Load(acc_name_id[key]);
                }
                else
                {
                    slno_combo.Text = "";
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Acc_Disp_Load("");
        }
    }
}
