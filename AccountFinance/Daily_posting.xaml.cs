using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for Daily_posting.xaml
    /// </summary>
    public partial class Daily_posting : Page
    {
        private DataAccess dataAccess = new DataAccess();
        private List<records> rec_list = new List<records>();
        private Dictionary<string, string> acc_id_name = new Dictionary<string, string>();
        private Dictionary<string, string> acc_name_id = new Dictionary<string, string>();
        private Dictionary<int, string> acc_slno_cust_id = new Dictionary<int, string>();
        private string pos_id = "", acc_id = "", edit_slno;
        private decimal reciept_b = 0, payment_b = 0;
        public Daily_posting()
        {
            InitializeComponent();
            date_daily.SelectedDate = new DateTime?(DateTime.Now);
            
            Load_Posting_data(date_daily.SelectedDate.Value.ToString("dd-MM-yyyy"), "", "");
            
            List<account> accountList = dataAccess.Load_acc_db(p:true);
            
            if (accountList != null)
            {
                foreach (account account in accountList)
                {
                    slno_post_combo.Items.Add((object)account.slno.ToString());
                    name_post_combo.Items.Add((object)account.name);
                    acc_id_name.Add(account.slno.ToString(), account.name);
                    acc_name_id.Add(account.name, account.slno.ToString());
                    acc_slno_cust_id.Add(account.slno, account.acc_id);
                }
            }
        }

        private void Load_Posting_data(string date_t, string slno = "", string name = "")
        {
            string[] date_spl = date_t.Split('-');
            date_t = new DateTime(Int32.Parse(date_spl[2]), Int32.Parse(date_spl[1]), Int32.Parse(date_spl[0])).Ticks.ToString();

            if (slno == "" && name == "")
                rec_list = dataAccess.Load_record(date: date_t, p: true);
            else if (slno != "" && name == "")
                rec_list = dataAccess.Load_record(sl_inp: slno,date: date_t, p: true);
            else if (slno == "" && name != "")
                rec_list = dataAccess.Load_record(date: date_t, name: name, p: true);

            Output.ItemsSource = rec_list;
            Decimal num1 = new Decimal();
            Decimal num2 = new Decimal();
            if (rec_list != null)
            {
                foreach (records rec in rec_list)
                {
                    num1 += rec.reciept;
                    num2 += rec.payment;
                }
            }
            total_postpos.Text = num1.ToString();
            total_postneg.Text = num2.ToString();
        }


        private void Slno_post_combo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            name_post_combo.Focus();
        }

        private void Name_post_combo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            reciept_post.Focus();
        }

        private void Reciept_post_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            payment_post.Focus();
        }

        private void Payment_post_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            description_post.Focus();
        }

        private void Interest_post_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            description_post.Focus();
        }
        private static readonly Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
        private void Reciept_post_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void Payment_post_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void Interest_post_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void date_daily_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Load_Posting_data(date_t: date_daily.SelectedDate.Value.ToString("dd-MM-yyyy"));
        }

        private void slno_daily_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (slno_daily.Text == "")
                Load_Posting_data(date_t: date_daily.SelectedDate.Value.ToString("dd-MM-yyyy"));
            else
                Load_Posting_data(date_t: date_daily.SelectedDate.Value.ToString("dd-MM-yyyy"),slno: slno_daily.Text);
        }

        private void name_daily_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (name_daily.Text == "")
                Load_Posting_data(date_t: date_daily.SelectedDate.Value.ToString("dd-MM-yyyy"));
            else
                Load_Posting_data(date_t: date_daily.SelectedDate.Value.ToString("dd-MM-yyyy"),name: name_daily.Text);
        }

        private void print_btn_Click(object sender, RoutedEventArgs e)
        {
            if (rec_list != null && slno_daily.Text == "")
            {
                new Report_Window("DataSet", (List<account>)null, rec_list, "Record_Report.rdlc", new List<string>() { "Date", "Slno", "Name", "Details", "Reciept", "Payment", "Interest" }).Show();
            }
            else if (rec_list != null && slno_daily.Text != "")
            {
                new Report_Window("DataSet", (List<account>)null, rec_list, "Single_Cust_Report.rdlc").Show();
            }
            else
            {
                int num = (int)MessageBox.Show("Select Valid Date or Name");
            }
        }

        private void Output_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Output.SelectedCells.Count <= 0)
                return;
            records record_ = Output.SelectedItem as records;

            slno_post_combo.SelectedItem = record_.slno.ToString();
            name_post_combo.SelectedItem = record_.name;

            edit_slno = record_.slno.ToString();

            pos_id = record_.posting_id;

            acc_id = dataAccess.getAcc_id(edit_slno);

            string[] date_split = record_.date.Split('-');
            date_post.SelectedDate = new DateTime(int.Parse(date_split[2]), int.Parse(date_split[1]), int.Parse(date_split[0]));

            reciept_post.Text = record_.reciept.ToString();
            reciept_b = record_.reciept;

            payment_post.Text = record_.payment.ToString();
            payment_b = record_.payment;

            interest_post.Text = record_.interest.ToString();

            description_post.Text = record_.details;
            edit_btn_post.IsEnabled = true;
            del_btn_post.IsEnabled = true;
        }

        private void del_btn_post_Click(object sender, RoutedEventArgs e)
        {
            string date_ = date_daily.SelectedDate.Value.ToString("dd-MM-yyyy");
            switch (MessageBox.Show("Do you want to delete: " + edit_slno, "Delete Record", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    if (dataAccess.Delete_rec(edit_slno, pos_id, reciept_b, payment_b))
                    {
                        MessageBox.Show("Deleted");
                        Reset_Details();
                        break;
                    }
                    MessageBox.Show("Can't Delete");
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Delete Cancelled");
                    break;
            }
        }

        private void edit_btn_post_Click(object sender, RoutedEventArgs e)
        {
            if (slno_post_combo.Text == "" || name_post_combo.Text == "" || (reciept_post.Text == "" && payment_post.Text == "") || !dataAccess.isContain_slno(slno_post_combo.Text) || !acc_name_id.ContainsKey(name_post_combo.Text) || !acc_id_name.ContainsKey(slno_post_combo.Text))
            {
                MessageBox.Show("Fill all Required Details");
            }
            else
            {
                if (edit_slno == slno_post_combo.Text)
                {
                    List<account> accountList = dataAccess.Load_acc_db(inp: edit_slno);

                    Decimal num1 = new Decimal();
                    Decimal num2 = new Decimal();
                    foreach (var account in accountList)
                    {
                        num1 = account.reciept;
                        num2 = account.payment;
                    }

                    Decimal reciept = !(reciept_post.Text == "") && !(reciept_post.Text == "0") && !(reciept_post.Text.Trim() == " ") ? Decimal.Parse(reciept_post.Text) : new Decimal();
                    Decimal payment = !(payment_post.Text == "") && !(payment_post.Text == "0") && !(payment_post.Text.Trim() == " ") ? Decimal.Parse(payment_post.Text) : new Decimal();

                    Decimal num5 = num1 - reciept_b + reciept;
                    Decimal num6 = num2 - payment_b + payment;


                    List<string> acc_update = new List<string>()
                    {
                        dataAccess.getAcc_id(edit_slno),
                        num5.ToString(),
                        num6.ToString()
                    };

                    string[] date_split = date_post.SelectedDate.Value.ToString("dd-MM-yyyy").Split('-');
                    string dateTicks = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString();
                    List<records> acc_rec_update = new List<records>()
                    {
                        new records(pos_id, dateTicks, int.Parse(edit_slno), name_post_combo.Text, description_post.Text, reciept, payment, Decimal.Parse(interest_post.Text))
                    };

                    List<Decimal> numList = dataAccess.Load_lineTotal();

                    num5 = numList[0];
                    Decimal num7 = numList[1];
                    num5 = num5 - reciept_b + reciept;
                    Decimal num8 = num7 - payment_b + payment;

                    if (!dataAccess.Update_rec(acc_update, acc_rec_update, new List<Decimal>() { num5, num8 }))
                        return;
                    MessageBox.Show("Updated");
                    Reset_Details();
                }
                else
                {
                    if (dataAccess.Delete_rec(edit_slno, pos_id, reciept_b, payment_b))
                    {
                        Random rand = new Random();
                        string[] date_split = date_post.SelectedDate.Value.ToString("dd-MM-yyyy").Split('-');
                        string dateTicks = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString();

                        List<string> inpdata = new List<string>()
                        {
                            slno_post_combo.Text+"-"+name_post_combo.Text.Substring(0,name_post_combo.Text.Length/3)+"-"+rand.Next(),
                            dateTicks,
                            slno_post_combo.Text,
                            name_post_combo.Text,
                            description_post.Text,
                            reciept_post.Text,
                            payment_post.Text,
                            interest_post.Text,
                            acc_slno_cust_id[Int32.Parse(slno_post_combo.Text)]
                        };

                        if (dataAccess.Post_ac_data(inpdata))
                        {
                            MessageBox.Show("Updated");
                            Reset_Details();
                        }
                    }
                }
            }

        }

        private void slno_post_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (slno_post_combo.SelectedValue == null)
                return;
            string key = slno_post_combo.SelectedValue.ToString();
            if (acc_id_name.ContainsKey(key))
                name_post_combo.SelectedItem = (object)acc_id_name[key];
        }

        private void name_post_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (name_post_combo.SelectedValue == null)
                return;
            string key = name_post_combo.SelectedValue.ToString();
            if (acc_name_id.ContainsKey(key))
            {
                slno_post_combo.SelectedItem = (object)acc_name_id[key];
            }
        }

        private void del_btn_Click(object sender, RoutedEventArgs e)
        {
            string date_ = date_daily.SelectedDate.Value.ToString("dd-MM-yyyy");
            switch (MessageBox.Show("Do you want to Delete Posting on " + date_, "Delete Record", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    foreach (var records in rec_list)
                    {
                        if (!dataAccess.Delete_rec(slno_b: records.slno.ToString(), pos_id: records.posting_id, reciept_b: records.reciept, payment_b: records.payment, date_: date_))
                        {
                            MessageBox.Show("Can't Delete");
                            return;
                        }
                    }
                    MessageBox.Show("Deleted");
                    Reset_Details();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Delete Cancelled");
                    break;
            }
        }

        private void reset_btn_Click(object sender, RoutedEventArgs e)
        {
            Reset_Details();
        }

        private void Reset_Details()
        {
            date_daily.SelectedDate = new DateTime?(DateTime.Now);
            Load_Posting_data(date_daily.SelectedDate.Value.ToString("dd-MM-yyyy"), "", "");

            slno_post_combo.Text = "";
            name_post_combo.Text = "";
            description_post.Text = "";
            payment_post.Text = "";
            reciept_post.Text = "";
            interest_post.Text = "";

            edit_btn_post.IsEnabled = false;
            del_btn_post.IsEnabled = false;
        }
    }
}
