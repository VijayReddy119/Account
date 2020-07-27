using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for ChitsList.xaml
    /// </summary>
    public partial class ChitsList : Page
    {
        private DataAccess dataAccess = new DataAccess();
        private List<account> account = new List<account>();
        private Dictionary<string, string> acc_id_name = new Dictionary<string, string>();
        private Dictionary<string, string> acc_name_id = new Dictionary<string, string>();
        private string slno_to_use = "";
        public ChitsList()
        {
            InitializeComponent();
            account = dataAccess.Load_acc_db("", "Chits", 0, "", false);
            if (account != null)
            {
                foreach (account account in account)
                {
                    ItemCollection items = slno_combo.Items;
                    int slno = account.slno;
                    string str1 = slno.ToString();
                    items.Add((object)str1);
                    name_combo.Items.Add((object)account.name);
                    Dictionary<string, string> accIdName = acc_id_name;
                    slno = account.slno;
                    string key = slno.ToString();
                    string name1 = account.name;
                    accIdName.Add(key, name1);
                    Dictionary<string, string> accNameId = acc_name_id;
                    string name2 = account.name;
                    slno = account.slno;
                    string str2 = slno.ToString();
                    accNameId.Add(name2, str2);
                }
                Acc_Disp_Load("");
            }
        }

       private void Acc_Disp_Load(string slno_inp = "")
        {
            Reset_Details();
            if (slno_inp == "")
            {
                account = dataAccess.Load_acc_db("", "Chits", 0, "", false);
            }
            else
            {
                slno_to_use = slno_inp;
                account = dataAccess.Load_acc_db(slno_to_use, "Chits", 0, "", false);
            }
            if (account == null)
                return;
            Output.ItemsSource = account;
            Decimal num1 = new Decimal();
            Decimal num2 = new Decimal();
            foreach (account account in account)
            {
                num1 += account.reciept;
                num2 += account.payment;
            }
            TextBox chitTotalR = chit_total_r;
            Decimal num3 = Math.Abs(num1);
            string str1 = num3.ToString();
            chitTotalR.Text = str1;
            TextBox chitTotalP = chit_total_p;
            num3 = Math.Abs(num2);
            string str2 = num3.ToString();
            chitTotalP.Text = str2;
            Decimal num4 = num1 - num2;
            if (num4 < Decimal.Zero)
            {
                TextBox chitBal = chit_bal;
                num3 = Math.Abs(num4);
                string str3 = "-" + num3.ToString();
                chitBal.Text = str3;
            }
            else
            {
                TextBox chitBal = chit_bal;
                num3 = Math.Abs(num4);
                string str3 = num3.ToString();
                chitBal.Text = str3;
            }
        }

        private void Reset_Details()
        {
            Output.ItemsSource = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Reset_Details();
            slno_combo.Text = "";
            name_combo.Text = "";
            chit_bal.Text = "";
            chit_total_p.Text = "";
            chit_total_r.Text = "";
            Acc_Disp_Load("");
        }

        private void print_btn_Click(object sender, RoutedEventArgs e)
        {
            if (account != null)
            {
                new Report_Window("Accountdata", acc_list: account, null, "ChitsList_Report.rdlc", new List<string>() { "Date", "Slno", "Name", "Reciept", "Payment", "Balance" }).Show();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Acc_Disp_Load("");
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
                    Output.ItemsSource = null;
                    chit_bal.Text = "";
                    chit_total_p.Text = "";
                    chit_total_r.Text = "";
                }
            }
        }

        private void slno_combo_KeyUp(object sender, KeyEventArgs e)
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
                Output.ItemsSource = null;
                chit_bal.Text = "";
                chit_total_p.Text = "";
                chit_total_r.Text = "";
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
                    Output.ItemsSource = null;
                    chit_bal.Text = "";
                    chit_total_p.Text = "";
                    chit_total_r.Text = "";
                }
            }
        }

        private void name_combo_KeyUp(object sender, KeyEventArgs e)
        {
            string key = name_combo.Text;
            if (acc_name_id.ContainsKey(key))
            {
                slno_combo.Text = acc_name_id[key];
                Acc_Disp_Load(acc_name_id[key]);
            }
            else
            {
                Output.ItemsSource = null;
                slno_combo.Text = "";
                chit_bal.Text = "";
                chit_total_p.Text = "";
                chit_total_r.Text = "";
            }
        }
    }
}
