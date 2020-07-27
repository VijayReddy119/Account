using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for AccountsDisplay.xaml
    /// </summary>
    public partial class AccountsDisplay : Page
    {
        private DataAccess dataAccess = new DataAccess();
        private List<account> acc_list = new List<account>();
        private List<records> acc_rec_list = new List<records>();
        private Dictionary<string, string> acc_id_name = new Dictionary<string, string>();
        private Dictionary<string, string> acc_name_id = new Dictionary<string, string>();
        private Dictionary<int, decimal> slno_interest = new Dictionary<int, decimal>();
        private string slno_to_use = "";
        public AccountsDisplay()
        {
            InitializeComponent();
            date_acc.SelectedDate = new DateTime?(DateTime.Now);
            
            acc_list = dataAccess.Load_acc_db();
            
            if (acc_list != null)
            {
                foreach (account acc in acc_list)
                {
                    slno_combo.Items.Add((object)acc.slno);
                    name_combo.Items.Add((object)acc.name);
                    int slno = acc.slno;
                    string key = slno.ToString();
                    string name1 = acc.name;
                    acc_id_name.Add(key, name1);
                    string name2 = acc.name;
                    slno = acc.slno;
                    string str = slno.ToString();
                    acc_name_id.Add(name2, str);

                    slno_interest.Add(acc.slno, acc.interest);
                }

                slno_combo.SelectedIndex = 0;
                name_combo.SelectedIndex = 0;
                slno_to_use = slno_combo.Text;
                
                Acc_Disp_Load(slno_to_use);

            }
        }

        private void Acc_Disp_Load(string slno_inp)
        {
            if (acc_id_name.ContainsKey(slno_inp))
            {
                string date = date_acc.SelectedDate.Value.ToString("dd-MM-yyyy");
                Decimal interest_rate = new Decimal();
                
                List<int> slno_info = dataAccess.Load_Partys();
                
                if (slno_info != null && slno_combo.SelectedValue != null)
                {
                    if (slno_info.Contains(Int32.Parse(slno_combo.SelectedValue.ToString())))
                    {
                        Output_data.Width = 880;
                        Output_data.Columns[7].Visibility = Visibility.Visible;
                        pi_total_int.Visibility = Visibility.Visible;

                        acc_rec_list = dataAccess.Load_record(sl_inp: slno_inp, date: date, interest_rate: slno_interest[Int32.Parse(slno_inp)], partys: true);
                    }
                    else
                    {
                        Output_data.Width = 780;
                        Output_data.Columns[7].Visibility = Visibility.Hidden;
                        pi_total_int.Visibility = Visibility.Hidden;

                        acc_rec_list = dataAccess.Load_record(sl_inp: slno_inp, date: date, interest_rate: slno_interest[Int32.Parse(slno_inp)], days_cal: true);
                    }
                }
                else
                {
                    acc_rec_list = dataAccess.Load_record(sl_inp: slno_inp, date: date, interest_rate: slno_interest[Int32.Parse(slno_inp)], days_cal: true);
                }

                if (acc_rec_list != null)
                {
                    Decimal num1 = new Decimal();
                    Decimal num2 = new Decimal();
                    Decimal num3 = new Decimal();
                    Decimal num4 = new Decimal();
                    Decimal inter = new Decimal();
                    Output_data.ItemsSource = acc_rec_list;
                    foreach (records accRec in acc_rec_list)
                    {
                        num1 += accRec.reciept;
                        num3 += accRec.payment;
                        num2 += accRec.reciept_int;
                        num4 += accRec.payment_int;
                        inter += accRec.interest;
                    }
                    reciept_total_record.Text = num1.ToString();
                    ri_total_record.Text = num2.ToString();
                    payment_total_record.Text = num3.ToString();
                    pi_total_record.Text = num4.ToString();
                    pi_total_int.Text = inter.ToString();
                    Decimal num5 = num1 - num3;
                    Decimal num6 = num2 - num4;
                    Decimal num7 = num5 + num6;
                    balance_to_be_paid.Text = !(num7 < Decimal.Zero) ? Math.Abs(num7).ToString() : "-" + Math.Abs(num7).ToString();
                    Decimal num8;
                    if (num5 < Decimal.Zero)
                    {
                        TextBox balanceRecAmt = balance_rec_amt;
                        num8 = Math.Abs(num5);
                        string str1 = "-" + num8.ToString();
                        balanceRecAmt.Text = str1;
                        TextBox principleAmt = principle_amt;
                        num8 = Math.Abs(num5);
                        string str2 = "-" + num8.ToString();
                        principleAmt.Text = str2;
                    }
                    else
                    {
                        balance_rec_amt.Text = Math.Abs(num5).ToString();
                        principle_amt.Text = Math.Abs(num5).ToString();
                    }
                    TextBox balanceRecInt = balance_rec_int;
                    num8 = Math.Abs(num4);
                    string str3 = num8.ToString();
                    balanceRecInt.Text = str3;
                    if (num6 < Decimal.Zero)
                    {
                        num8 = Math.Abs(num6);
                        string str1 = "-" + num8.ToString();
                        interest_acc_disp.Text = str1;
                    }
                    else
                    {
                        num8 = Math.Abs(num6);
                        string str1 = num8.ToString();
                        interest_acc_disp.Text = str1;
                    }
                }
                else
                {
                    Output_data.ItemsSource = null;
                    reciept_total_record.Text = "";
                    ri_total_record.Text = "";
                    payment_total_record.Text = "";
                    pi_total_record.Text = "";
                    balance_to_be_paid.Text = "";
                    balance_rec_amt.Text = "";
                    balance_rec_int.Text = "";
                    principle_amt.Text = "";
                    interest_acc_disp.Text = "";
                }
            }
            else
            {
                name_combo.SelectedValue = null;
                Output_data.ItemsSource = null;
                reciept_total_record.Text = "";
                ri_total_record.Text = "";
                payment_total_record.Text = "";
                pi_total_record.Text = "";
                balance_to_be_paid.Text = "";
                balance_rec_amt.Text = "";
                balance_rec_int.Text = "";
                principle_amt.Text = "";
                interest_acc_disp.Text = "";
            }
        }

        private void date_acc_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Acc_Disp_Load(slno_to_use);
        }

        private void print_btn_Click(object sender, RoutedEventArgs e)
        {
            if (acc_rec_list != null)
            {
                new Report_Window("DataSet", (List<account>)null, acc_rec_list, "Single_Cust_Report.rdlc", new List<string>()
        {
          "Date",
          "Slno",
          "Name",
          "Details",
          "Reciept",
          "Payment",
          "Interest"
        }).Show();
            }
            else
            {
                MessageBox.Show("Empty. Can't Print");
            }
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
                    name_combo.Text = "";
                    Output_data.ItemsSource = null;
                    reciept_total_record.Text = "";
                    ri_total_record.Text = "";
                    payment_total_record.Text = "";
                    pi_total_record.Text = "";
                    balance_to_be_paid.Text = "";
                    balance_rec_amt.Text = "";
                    balance_rec_int.Text = "";
                    principle_amt.Text = "";
                    interest_acc_disp.Text = "";
                }
            }
        }

        private void slno_combo_KeyUp(object sender, KeyEventArgs e)
        {
            slno_to_use = slno_combo.Text;
            if (acc_id_name.ContainsKey(slno_to_use))
            {
                name_combo.SelectedItem = acc_id_name[slno_to_use];
                Acc_Disp_Load(slno_to_use);
            }
            else
            {
                name_combo.Text = ""; 
                Output_data.ItemsSource = null;
                reciept_total_record.Text = "";
                ri_total_record.Text = "";
                payment_total_record.Text = "";
                pi_total_record.Text = "";
                balance_to_be_paid.Text = "";
                balance_rec_amt.Text = "";
                balance_rec_int.Text = "";
                principle_amt.Text = "";
                interest_acc_disp.Text = "";
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
                Output_data.ItemsSource = null;
                reciept_total_record.Text = "";
                ri_total_record.Text = "";
                payment_total_record.Text = "";
                pi_total_record.Text = "";
                balance_to_be_paid.Text = "";
                balance_rec_amt.Text = "";
                balance_rec_int.Text = "";
                principle_amt.Text = "";
                interest_acc_disp.Text = "";
            }
        }

        private void name_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(name_combo.SelectedValue != null)
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
                    Output_data.ItemsSource = null;
                    reciept_total_record.Text = "";
                    ri_total_record.Text = "";
                    payment_total_record.Text = "";
                    pi_total_record.Text = "";
                    balance_to_be_paid.Text = "";
                    balance_rec_amt.Text = "";
                    balance_rec_int.Text = "";
                    principle_amt.Text = "";
                    interest_acc_disp.Text = "";
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            slno_combo.SelectedIndex = 0;
            name_combo.SelectedIndex = 0;
            slno_to_use = slno_combo.Text;

            Acc_Disp_Load(slno_to_use);
        }
    }
}
