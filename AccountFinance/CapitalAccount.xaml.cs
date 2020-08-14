using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for CapitalAccount.xaml
    /// </summary>
    public partial class CapitalAccount : Page
    {
        private DataAccess dataAccess = new DataAccess();
        private Dictionary<string, string> acc_id_name = new Dictionary<string, string>();
        private Dictionary<string, string> acc_name_id = new Dictionary<string, string>();
        private List<account> account = new List<account>();
        private List<records> acc_rec_list = new List<records>();
        private string slno_to_use;
        public CapitalAccount()
        {
            InitializeComponent();
            date_acc.SelectedDate = new DateTime?(DateTime.Now);
            account = dataAccess.Load_acc_db("", "Capital", 0, "", false, "");
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
                slno_combo.SelectedIndex = 0;
                name_combo.SelectedIndex = 0;
                if(slno_combo.SelectedValue != null)
                {
                    Acc_Disp_Load(slno_combo.SelectedValue.ToString());
                }
            }
        }
        private void Acc_Disp_Load(string slno_inp)
        {
            Reset_Details();
            string[] date_spli = date_acc.SelectedDate.Value.ToString("dd-MM-yyyy").Split('-');
            string date = new DateTime(Int32.Parse(date_spli[2]), Int32.Parse(date_spli[1]), Int32.Parse(date_spli[0])).Ticks.ToString();
            Decimal interest_rate = new Decimal();
            foreach (account account in account)
            {
                if (account.slno.ToString() == slno_inp)
                {
                    interest_rate = account.interest;
                    break;
                }
            }

            acc_rec_list = dataAccess.Load_record(sl_inp:slno_inp, date: date,interest_rate: interest_rate, days_cal:true);
            if (acc_rec_list != null)
            {
                Decimal num1 = new Decimal();
                Decimal num2 = new Decimal();
                Decimal num3 = new Decimal();
                Decimal num4 = new Decimal();

                Output.ItemsSource = acc_rec_list;

                foreach (records accRec in acc_rec_list)
                {
                    num1 += accRec.reciept;
                    num2 += accRec.payment;
                    num3 += accRec.reciept_int;
                    num4 += accRec.payment_int;
                }
                principle_amt.Text = Math.Abs(num1).ToString();
                TextBox principleAmtInt = principle_amt_int;
                Decimal num6 = Math.Abs(num2);
                string str1 = num6.ToString();
                principleAmtInt.Text = str1;
                Decimal num7 = Math.Abs(num1 - num2 + (num3 - num4));
                if (num1 < num2)
                    total_amt_neg.Text = num7.ToString();
                else
                    total_amt_pos.Text = num7.ToString();
                if (num3 < num4)
                {
                    TextBox intNeg = int_neg;
                    num6 = Math.Abs(num3 - num4);
                    string str2 = num6.ToString();
                    intNeg.Text = str2;
                }
                else
                {
                    TextBox intPos = int_pos;
                    num6 = Math.Abs(num3 - num4);
                    string str2 = num6.ToString();
                    intPos.Text = str2;
                }
            }
            else
                Reset_Details();
        }

        private void Reset_Details()
        {
            Output.ItemsSource = null;
            principle_amt.Text = "";
            principle_amt_int.Text = "";
            total_amt_neg.Text = "";
            total_amt_pos.Text = "";
            int_pos.Text = "";
            int_neg.Text = "";
        }
        private void date_acc_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (slno_combo.SelectedValue == null)
                return;
            slno_to_use = slno_combo.SelectedValue.ToString();
            Acc_Disp_Load(slno_to_use);
        }

        private void print_btn_Click(object sender, RoutedEventArgs e)
        {
            if (acc_rec_list == null)
                return;
            new Report_Window("DataSet", (List<account>)null, rec_list: acc_rec_list, "CapitalAcc_Report.rdlc", new List<string>() { "Date", "Slno", "Name", "Details", "Days", "Reciept", "Reciept_Int", "Payment", "Payment_Int" }).Show();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            slno_combo.SelectedIndex = 0;
            name_combo.SelectedIndex = 0;
            if(slno_combo.SelectedValue != null)
            {
                Acc_Disp_Load(slno_combo.SelectedValue.ToString());
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
                    Reset_Details();
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
                slno_combo.Text = "";
                Output.ItemsSource = null;
                Reset_Details();
            }
        }


        private void slno_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (slno_combo.SelectedValue != null)
            {
                slno_to_use = slno_combo.SelectedValue.ToString();
                if (acc_id_name.ContainsKey(slno_to_use))
                {
                    name_combo.SelectedItem = (object)acc_id_name[slno_to_use];
                    Acc_Disp_Load(slno_to_use);
                }
                else
                {
                    name_combo.Text = "";
                    Output.ItemsSource = null;
                    Reset_Details();
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
                Reset_Details();
            }
        }
    }
}
