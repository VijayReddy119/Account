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
        protected bool t_2 = false;
        private List<AccountFinance.account> account = new List<AccountFinance.account>();
        public Trail_balance()
        {
            InitializeComponent();
            Load_data(1);
        }

        private void Load_data(int n = 1)
        {
            account = dataAccess.Load_acc_db("", "", n, date_trail.SelectedDate.Value.ToString("dd-MM-yyyy"), false);
            Output.ItemsSource = account;
            Decimal num1 = new Decimal();
            Decimal num2 = new Decimal();
            foreach (AccountFinance.account account in account)
            {
                num1 += account.bal_pos;
                num2 += account.bal_neg;
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
            t_2 = false;
            trail_bal_txt.Text = "Trail Balance - 1";
            create_acc.Visibility = Visibility.Hidden;
        }

        private void trail_bal_2_Click(object sender, RoutedEventArgs e)
        {
            Load_data(2);
            t_2 = true;
            trail_bal_txt.Text = "Trail Balance - 2";
            create_acc.Visibility = Visibility.Hidden;
        }

        private void date_trail_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!t_2)
                return;
            Load_data(2);
        }

        private void t1_report_btn_Click(object sender, RoutedEventArgs e)
        {
            account = dataAccess.Load_acc_db("", "", 1, date_trail.SelectedDate.Value.ToString("dd-MM-yyyy"), false);
            if (account == null)
                return;
            new Report_Window("trail_bal", account, (List<records>)null, "TrailBal.rdlc", new List<string>()
      {
        "Reciept",
        "Name",
        "Payment"
      }).Show();
        }

        private void t2_report_btn_Click(object sender, RoutedEventArgs e)
        {
            account = dataAccess.Load_acc_db("", "", 2, date_trail.SelectedDate.Value.ToString("dd-MM-yyyy"), false);
            if (account == null)
                return;
            new Report_Window("trail_bal", account, (List<records>)null, "TrailBal.rdlc", new List<string>()
      {
        "Reciept",
        "Name",
        "Payment"
      }).Show();
        }

        private void trail_bal_3_Click(object sender, RoutedEventArgs e)
        {
            Load_data(3);
            t_2 = false;
            trail_bal_txt.Text = "FINAL ACCOUNT";
            create_acc.Visibility = Visibility.Visible;
        }

        private void t3_report_btn_Click(object sender, RoutedEventArgs e)
        {
            account = dataAccess.Load_acc_db("", "", 3, date_trail.SelectedDate.Value.ToString("dd-MM-yyyy"), false);
            if (account == null)
                return;
            new Report_Window("trail_bal", account, (List<records>)null, "TrailBal.rdlc", new List<string>() { "Reciept", "Name", "Payment" }).Show();
        }

        private void create_acc_Click(object sender, RoutedEventArgs e)
        {
            dataAccess.CreateAcc();

            Load_data(1);
            t_2 = false;
            trail_bal_txt.Text = "Trail Balance - 1";
            create_acc.Visibility = Visibility.Hidden;
        }
    }
}
