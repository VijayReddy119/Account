using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for Trail_balance.xaml
    /// </summary>
    public partial class Trail_balance : Page
    {
        private DataAccess dataAccess = new DataAccess();
        protected string t_ = "";
        private List<account> account_list = new List<account>();
        private List<account> acc_int_share = new List<account>();
        public Trail_balance()
        {
            InitializeComponent();
            t_ = "1";
            Load_data(1);
        }

        private void Load_data(int n = 1, bool compute = false)
        {
            if (!compute)
            {
                account_list = dataAccess.Load_acc_db(trail_b: n, date_t: date_trail.SelectedDate.Value.ToString("dd-MM-yyyy"));
                Output.ItemsSource = account_list;
            }

            Decimal num1 = new Decimal();
            Decimal num2 = new Decimal();
            foreach (account account_list in account_list)
            {
                num1 += account_list.bal_pos;
                num2 += account_list.bal_neg;
            }
            Decimal num4 = num1 - num2;
            Decimal num5;
            if (num1 >= Decimal.Zero)
            {
                tot_reciept.Text = num1.ToString();
            }
            else
            {
                TextBox totReciept = tot_reciept;
                num5 = Math.Abs(num1);
                string str = "-" + num5.ToString();
                totReciept.Text = str;
            }
            if (num2 >= Decimal.Zero)
            {
                tot_payment.Text = num2.ToString();
            }
            else
            {
                TextBox totPayment = tot_payment;
                num5 = Math.Abs(num2);
                string str = "-" + num5.ToString();
                totPayment.Text = str;
            }
            if (num4 >= Decimal.Zero)
            {
                cash_in_hand.Text = num4.ToString();
            }
            else
            {
                TextBox cashInHand = cash_in_hand;
                num5 = Math.Abs(num4);
                string str = "-" + num5.ToString();
                cashInHand.Text = str;
            }
        }

        private void trail_bal_1_Click(object sender, RoutedEventArgs e)
        {
            Load_data(1);
            t_ = "1";
            trail_bal_txt.Text = "Trail Balance - 1";
            create_acc.Visibility = Visibility.Hidden;
            load_Acc_Final.Visibility = Visibility.Hidden;
        }

        private void trail_bal_2_Click(object sender, RoutedEventArgs e)
        {
            Load_data(2);
            t_ = "2";
            trail_bal_txt.Text = "Trail Balance - 2";
            create_acc.Visibility = Visibility.Hidden;
            load_Acc_Final.Visibility = Visibility.Hidden;
        }

        private void date_trail_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (t_ == "2")
                Load_data(2, false);
            else if (t_ == "3")
            {
                Load_data(2, false);
                account_list = show_Final_Acc(false);
                Load_data(0, true);
                Output.ItemsSource = account_list;
            }
            else if (t_ == "1")
                Load_data();
        }

        private void t1_report_btn_Click(object sender, RoutedEventArgs e)
        {
            account_list = dataAccess.Load_acc_db("", "", 1, date_trail.SelectedDate.Value.ToString("dd-MM-yyyy"), false);
            if (account_list == null)
                return;
            new Report_Window("trail_bal", account_list, (List<records>)null, "TrailBal.rdlc", new List<string>()
      {
        "Reciept",
        "Name",
        "Payment"
      }).Show();
        }

        private void t2_report_btn_Click(object sender, RoutedEventArgs e)
        {
            account_list = dataAccess.Load_acc_db("", "", 2, date_trail.SelectedDate.Value.ToString("dd-MM-yyyy"), false);
            if (account_list == null)
                return;
            new Report_Window("trail_bal", account_list, (List<records>)null, "TrailBal.rdlc", new List<string>()
      {
        "Reciept",
        "Name",
        "Payment"
      }).Show();
        }

        private List<account> show_Final_Acc(bool onlyCapital = false)
        {
            List<account> accountList = new List<account>();
            acc_int_share = new List<account>();
            int num1 = 0;
            Dictionary<int, Dictionary<string, int>> dictionary1 = new Dictionary<int, Dictionary<string, int>>();
            int num2 = 0;
            for (int index = 0; index < account_list.Count; ++index)
            {
                if (account_list[index].slno == -5)
                    num2 += (int)account_list[index].bal_pos;
                if (account_list[index].share > 0)
                {
                    Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
                    dictionary2.Add(account_list[index].name, account_list[index].share);
                    num1 += account_list[index].share;
                    dictionary1.Add(account_list[index].slno, dictionary2);
                }
            }
            int num3 = 0;
            for (int index = 0; index < account_list.Count - 1; ++index)
            {
                if (account_list[index].type == "Partys")
                    num3 += (int)account_list[index].bal_neg;
                else if (account_list[index].type == "Capital" && account_list[index].share >= 0)
                {
                    accountList.Add(new account(account_list[index].bal_pos, account_list[index].name, account_list[index].bal_neg, account_list[index].slno, account_list[index].share, account_list[index].type));
                    if (account_list[index].slno == account_list[index + 1].slno)
                    {
                        int slno = account_list[index].slno;
                        string name = account_list[index].name;
                        int num5 = num2 * dictionary1[slno][name] / num1;
                        accountList.Add(new account(account_list[index + 1].bal_pos, name + " -Interest", Decimal.Zero, slno, 0, "Interest"));
                        accountList.Add(new account((Decimal)num5, name + " -Share (" + dictionary1[slno][name].ToString() + ")", Decimal.Zero, slno, 0, "SHARE"));

                        acc_int_share.Add(new account(account_list[index + 1].bal_pos, name + " -Interest", Decimal.Zero, slno, 0, "Interest"));
                        acc_int_share.Add(new account((Decimal)num5, name + " -Share (" + dictionary1[slno][name].ToString() + ")", Decimal.Zero, slno, 0, "SHARE"));

                        ++index;
                    }
                    else if (account_list[index].share > 0 && account_list[index].slno != account_list[index + 1].slno)
                    {
                        int slno = account_list[index].slno;
                        string name = account_list[index].name;
                        int num5 = num2 * dictionary1[slno][name] / num1;
                        accountList.Add(new account((Decimal)num5, name + " -Share", Decimal.Zero, slno, 0, "SHARE"));

                        acc_int_share.Add(new account((Decimal)num5, name + " -Share", Decimal.Zero, slno, 0, "SHARE"));
                    }
                }
            }
            if (onlyCapital)
                return accountList;

            accountList.Add(new account(Decimal.Zero, "Partys", (Decimal)num3, -123, 0, "Partys"));

            for (int index = 0; index < account_list.Count; ++index)
            {
                if (account_list[index].type == "Chits")
                {
                    accountList.Add(new account(account_list[index].bal_pos, account_list[index].name, account_list[index].bal_neg, account_list[index].slno, account_list[index].share, account_list[index].type));
                }
            }

            accountList.Add(new account(Decimal.Zero, "PROFIT", Decimal.Zero, -5, 0, "PROFIT"));
            return accountList;
        }

        private void Load_Final()
        {
            Load_data(2, false);
            account_list = show_Final_Acc(false);
            Output.ItemsSource = null;
            Output.ItemsSource = account_list;
            Load_data(1, true);
            t_ = "3";
        }

        private void trail_bal_3_Click(object sender, RoutedEventArgs e)
        {
            Load_Final();
            t_ = "3";
            trail_bal_txt.Text = "FINAL account_list";
            create_acc.Visibility = Visibility.Visible;
            load_Acc_Final.Visibility = Visibility.Visible;
        }

        private void t3_report_btn_Click(object sender, RoutedEventArgs e)
        {
            Load_data(2, false);
            account_list = show_Final_Acc(false);

            if (account_list == null)
                return;
            new Report_Window("trail_bal", account_list, (List<records>)null, "TrailBal.rdlc", new List<string>() { "Reciept", "Name", "Payment" }).Show();
        }

        private void create_acc_Click(object sender, RoutedEventArgs e)
        {
            if (dataAccess.CreateAcc())
            {
                Load_data(1);
                t_ = "1";
                trail_bal_txt.Text = "Trail Balance - 1";
                create_acc.Visibility = Visibility.Hidden;
            }
        }

        private void load_Acc_Final_Click(object sender, RoutedEventArgs e)
        {
            if (dataAccess.LoadFinalAcc(acc_int_share))
            {
                MessageBox.Show("Posted");
            }
        }

        private void Capital_acc_Final_Click(object sender, RoutedEventArgs e)
        {
            Load_data(2, false);
            if (account_list == null)
                return;
            new Report_Window("Capital_Acc_Final", show_Final_Acc(true), (List<records>)null, "Capital_acc_Final.rdlc", (List<string>)null).Show();
            Load_Final();
        }
    }
}
