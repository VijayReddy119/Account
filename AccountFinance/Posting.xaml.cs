using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for Posting.xaml
    /// </summary>
    public partial class Posting : Page
    {
        private static readonly Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
        private readonly DataAccess dataAccess = new DataAccess();
        private Dictionary<string, string> acc_id_name = new Dictionary<string, string>();
        private Dictionary<string, string> acc_name_id = new Dictionary<string, string>();
        private Dictionary<decimal, string> acc_slno_cust_id = new Dictionary<decimal, string>();
        private string date_n;
        protected string pos_id, acc_id;
        protected string edit_slno;
        protected Decimal reciept_b;
        protected Decimal payment_b;
        protected bool isEditing;
        public Posting()
        {
            InitializeComponent();
            date_n = DateTime.Now.ToString("dd-MM-yyy");
            string[] strArray = date_n.Split('-');
            date_post.SelectedDate = new DateTime?(new DateTime(int.Parse(strArray[2]), int.Parse(strArray[1]), int.Parse(strArray[0])));
            List<account> accountList = dataAccess.Load_acc_db(p: true);
            if (accountList != null)
            {
                foreach (account account in accountList)
                {
                    slno_post_combo.Items.Add((object)account.slno.ToString());
                    name_post_combo.Items.Add((object)account.name);
                    listbox_name.Items.Add((object)account.name);
                    acc_id_name.Add(account.slno.ToString(), account.name);
                    acc_name_id.Add(account.name, account.slno.ToString());
                    acc_slno_cust_id.Add(account.slno, account.acc_id);
                }
                List<Decimal> numList = dataAccess.Load_lineTotal();
                if (numList[0] > numList[1])
                {
                    total_postpos.Text = total_bal_pos.Text = start_bal_postpos.Text = (numList[0] - numList[1]).ToString();
                    total_postneg.Text = total_bal_neg.Text = start_bal_postneg.Text = "";
                }
                else if (numList[1] > numList[0])
                {
                    total_postneg.Text = total_bal_neg.Text = start_bal_postneg.Text = (numList[1] - numList[0]).ToString();
                    total_postpos.Text = total_bal_pos.Text = start_bal_postpos.Text = "";
                }
                else
                {
                    if (!(numList[0] == numList[1]))
                        return;
                    total_postpos.Text = total_bal_pos.Text = start_bal_postpos.Text = "0";
                }
            }
            Posting_update();
            last_posting_date.Text = "Last Posting Date: "+dataAccess.Get_last_postingDate();

        }

        private void Listbox_name_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (object selectedItem in listbox_name.SelectedItems)
            {
                string key = selectedItem as string;
                if (acc_name_id.ContainsKey(key))
                {
                    slno_post_combo.SelectedItem = (object)acc_name_id[key];
                    name_post_combo.SelectedItem = (object)key;
                    reciept_post.Focus();
                }
            }
        }

        private void Slno_post_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (slno_post_combo.SelectedValue == null)
                return;
            string key = slno_post_combo.SelectedValue.ToString();
            if (acc_id_name.ContainsKey(key))
            {
                name_post_combo.SelectedItem = (object)acc_id_name[key];
            }
            else
            {
                MessageBox.Show("Slno Doesn't Exist");
                slno_post_combo.Focus();
            }
        }

        private void Name_post_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (name_post_combo.SelectedValue == null)
                return;
            string key = name_post_combo.SelectedValue.ToString();
            if (acc_name_id.ContainsKey(key))
            {
                slno_post_combo.SelectedItem = (object)acc_name_id[key];
                edit_slno = slno_post_combo.SelectedValue.ToString();
            }
            else
            {
                MessageBox.Show("Invalid Name");
                slno_post_combo.Text = "";
                name_post_combo.Text = "";
                slno_post_combo.Focus();
            }
        }


        private void Slno_post_combo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            if (acc_id_name.ContainsKey(slno_post_combo.Text))
            {
                edit_slno = slno_post_combo.Text;
                name_post_combo.Focus();
            }
            else
            {
                MessageBox.Show("Slno Doesn't Exists");
                slno_post_combo.SelectedValue = null;
                name_post_combo.SelectedValue = null;
                name_post_combo.Text = "";
                slno_post_combo.Text = "";
                slno_post_combo.Focus();
            }
        }

        private void Name_post_combo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            if (acc_name_id.ContainsKey(name_post_combo.Text))
            {
                slno_post_combo.SelectedItem = acc_name_id[name_post_combo.Text];

                edit_slno = slno_post_combo.Text;
                if (MainWindow.isPosted(edit_slno, date_post.SelectedDate.Value.ToString("dd-MM-yyyy")))
                {
                    switch (MessageBox.Show("Slno Already Posted. Do you Post Again? ", "Slno Posted", MessageBoxButton.YesNo))
                    {
                        case MessageBoxResult.Yes:
                            reciept_post.Focus();
                            break;
                        case MessageBoxResult.No:
                            Reset_Details();
                            break;
                    }
                }
                else { reciept_post.Focus(); }
            }
            else
            {
                MessageBox.Show("Invalid Name");
                slno_post_combo.SelectedValue = null;
                name_post_combo.SelectedValue = null;
                name_post_combo.Text = "";
                slno_post_combo.Text = "";
                slno_post_combo.Focus();
            }
        }

        private void Date_post_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            slno_post_combo.Focus();
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

        private void Description_post_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return || isEditing)
                return;
            Post_data(false);
        }

        private void Post_data(bool update = false)
        {
            if (slno_post_combo.Text == "" || name_post_combo.Text == "" || reciept_post.Text == "" && payment_post.Text == "" || !dataAccess.isContain_slno(slno_post_combo.Text) || !acc_name_id.ContainsKey(name_post_combo.Text) || !acc_id_name.ContainsKey(slno_post_combo.Text))
            {
                MessageBox.Show("Please Fill Required Details");
                slno_post_combo.Focus();
            }
            else
            {
                Random random = new Random();
                List<string> inpData = new List<string>();

                string[] strArray = new string[5]
                { name_post_combo.Text.Substring(0, name_post_combo.Text.Length / 3),"-",slno_post_combo.Text, "-", random.Next().ToString()};

                decimal num5 = random.Next();
                strArray[4] = num5.ToString();
                string str1 = string.Concat(strArray);
                string[] date_split = date_post.SelectedDate.Value.ToString("dd-MM-yyyy").Split('-');
                string str_date = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks + "";
                inpData.Add(str1);

                inpData.Add(str_date);
                inpData.Add(slno_post_combo.Text);
                inpData.Add(name_post_combo.Text);
                inpData.Add("" + description_post.Text);

                if (reciept_post.Text == "")
                    inpData.Add("0");
                else
                    inpData.Add(reciept_post.Text);

                if (payment_post.Text == "")
                    inpData.Add("0");
                else
                    inpData.Add(payment_post.Text);

                if (interest_post.Text == "" || interest_post.Text == "0")
                    inpData.Add("0");
                else
                {
                    inpData.Add(interest_post.Text);

                    account profitAccount = dataAccess.Get_ProfitAcc();
                    if (profitAccount == null)
                    {
                        MessageBox.Show("Profit Account is Not There");
                        return;
                    }
                    
                    List<string> profit_data = new List<string>();
                    profit_data.Add(profitAccount.name.Substring(0, profitAccount.name.Length / 3) + "-" + profitAccount.slno + "-" + random.Next().ToString());
                    profit_data.Add(str_date);
                    profit_data.Add(profitAccount.slno.ToString());
                    profit_data.Add(profitAccount.name);
                    profit_data.Add(description_post.Text);
                    profit_data.Add(interest_post.Text);
                    profit_data.Add("0");
                    profit_data.Add("0");
                    profit_data.Add(profitAccount.acc_id);

                    MainWindow.Set_InpSend(profit_data);

                    if (present_bal_postpos.Text == "")
                        present_bal_postpos.Text = interest_post.Text;
                    else if (interest_post.Text != "")
                    {
                        TextBox presentBalPostpos = present_bal_postpos;
                        string str2 = (decimal.Parse(present_bal_postpos.Text) + decimal.Parse(interest_post.Text)).ToString();
                        presentBalPostpos.Text = str2;
                    }

                }
                inpData.Add(acc_slno_cust_id[decimal.Parse(slno_post_combo.Text)]);

                MainWindow.Set_InpSend(inpData);
                if (Output.Items.Count > 0)
                {
                    var border = VisualTreeHelper.GetChild(Output, 0) as Decorator;
                    if (border != null)
                    {
                        var scroll = border.Child as ScrollViewer;
                        if (scroll != null) scroll.ScrollToEnd();
                    }
                }
                if (update)
                {
                    MessageBox.Show("Updated");
                }
                decimal num1;
                if (present_bal_postpos.Text == "")
                    present_bal_postpos.Text = reciept_post.Text;
                else if (reciept_post.Text != "")
                {
                    TextBox presentBalPostpos = present_bal_postpos;
                    num1 = (decimal.Parse(present_bal_postpos.Text) + decimal.Parse(reciept_post.Text));
                    string str2 = num1.ToString();
                    presentBalPostpos.Text = str2;
                }
                if (present_bal_postneg.Text == "")
                    present_bal_postneg.Text = payment_post.Text;
                else if (payment_post.Text != "")
                {
                    TextBox presentBalPostneg = present_bal_postneg;
                    num1 = (decimal.Parse(present_bal_postneg.Text) + decimal.Parse(payment_post.Text));
                    string str2 = num1.ToString();
                    presentBalPostneg.Text = str2;
                }

                Reset_Details();

                decimal t_p, t_n;
                decimal.TryParse(total_postpos.Text, out t_p);
                decimal.TryParse(total_postneg.Text, out t_n);

                decimal x = t_p - t_n;

                Decimal[] sum_ = MainWindow.Get_sum();
                x = x + sum_[0] - sum_[1];

                if (x >= 0)
                {
                    total_bal_pos.Text = start_bal_postpos.Text = x.ToString();
                    total_bal_neg.Text = start_bal_postneg.Text = "";
                }
                else
                {
                    total_bal_neg.Text = start_bal_postneg.Text = Math.Abs(x).ToString();
                    total_bal_pos.Text = start_bal_postpos.Text = "";
                }
            }
        }

        private void Reset_Details()
        {
            slno_post_combo.Text = "";
            name_post_combo.Text = "";
            reciept_post.Text = "";
            payment_post.Text = "";
            interest_post.Text = "";
            description_post.Text = "";
            Output.ItemsSource = null;
            Output.IsEnabled = true;
            Output.ItemsSource = MainWindow.Get_InpSend();
            slno_post_combo.Focus();
            slno_edit_check.IsChecked = new bool?(false);
            pos_id = "";
            isEditing = false;
            start_bal_postneg.Text = "";
            start_bal_postpos.Text = "";

            last_posting_date.Text = "Last Posting Date: " + dataAccess.Get_last_postingDate();
            
            edit_btn.IsEnabled = false;
            del_btn.IsEnabled = false;
            send_btn.IsEnabled = true;

            Posting_update();
        }

        private void Date_post_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Posting_update();
        }

        private void Posting_update()
        {
            Output.ItemsSource = MainWindow.Get_InpSend();
            decimal[] sumDb = MainWindow.Get_sum();
            if (sumDb != null)
            {
                present_bal_postpos.Text = sumDb[0].ToString();
                present_bal_postneg.Text = sumDb[1].ToString();
            }
            decimal num1 = !(start_bal_postpos.Text == "") ? decimal.Parse(start_bal_postpos.Text) : 0;
            decimal num2 = !(start_bal_postneg.Text == "") ? decimal.Parse(start_bal_postneg.Text) : 0;
            decimal num3 = !(present_bal_postpos.Text == "") ? decimal.Parse(present_bal_postpos.Text) : 0;
            decimal num4 = !(present_bal_postneg.Text == "") ? decimal.Parse(present_bal_postneg.Text) : 0;
            decimal num5 = num1 + num3;
            decimal num6 = num2 + num4;
            if (num5 > num6)
                total_bal_pos.Text = (num5 - num6).ToString();
            else if (num5 < num6)
                total_bal_neg.Text = (num6 - num5).ToString();
            else
                total_bal_pos.Text = total_bal_neg.Text = "0";
        }

        private void Edit_btn_Click(object sender, RoutedEventArgs e)
        {
            if (slno_post_combo.SelectedValue != null)
            {
                if (MainWindow.Remove_elem(pos_id))
                {
                    Post_data(true);
                }
                else
                {
                    MessageBox.Show("Update Failed");
                }
            }
            else
            {
                MessageBox.Show("Enter Slno");
            }
        }

        private void Del_btn_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Do you want to delete: " + edit_slno, "Delete Record", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    if (MainWindow.Remove_elem(pos_id))
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

        private void Reciept_post_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Posting.regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void Payment_post_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Posting.regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void Interest_post_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Posting.regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void Reset_btn_Click(object sender, RoutedEventArgs e)
        {
            Reset_Details();
        }

        private void Output_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Output.SelectedCells.Count <= 0)
                return;
            records selectedItem = Output.SelectedItem as records;
            Output.IsEnabled = false;

            pos_id = selectedItem.posting_id;

            string[] strArray = selectedItem.date.Split('-');
            date_post.SelectedDate = new DateTime?(new DateTime(int.Parse(strArray[2]), int.Parse(strArray[1]), int.Parse(strArray[0])));

            slno_post_combo.SelectedItem = selectedItem.slno.ToString();

            edit_slno = selectedItem.slno.ToString();

            acc_id = dataAccess.getAcc_id(edit_slno);

            name_post_combo.SelectedItem = selectedItem.name;

            description_post.Text = selectedItem.details;

            reciept_post.Text = selectedItem.reciept.ToString();
            reciept_b = selectedItem.reciept;

            payment_post.Text = selectedItem.payment.ToString();
            payment_b = selectedItem.payment;

            interest_post.Text = selectedItem.interest.ToString();

            slno_edit_check.IsChecked = new bool?(true);

            isEditing = true;

            send_btn.IsEnabled = false;
            edit_btn.IsEnabled = true;
            del_btn.IsEnabled = true;
        }

        private void Clear_btn_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Do you want to Clear Posting data: ", "Clear Posting", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    List<records> rec_list = MainWindow.Get_InpSend();
                    if (rec_list != null)
                    {
                        foreach (var rec in rec_list)
                        {
                            if (!MainWindow.Remove_elem(rec.posting_id))
                            {
                                MessageBox.Show("Clear Failed");
                                return;
                            }
                            MainWindow.Reset_data_inp();
                            Reset_Details();
                            MessageBox.Show("Cleared");
                            break;
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Clear Cancelled");
                    break;
            }
        }

        private void send_btn_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Do you want to Send Posting data: ", "Send Posting", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    List<records> data = MainWindow.Get_InpSend();
                    if (data != null)
                    {
                        foreach (var x in data)
                        {
                            List<string> inpdata = new List<string>();

                            string[] date_split = x.date.Split('-');

                            inpdata.Add(x.posting_id);
                            inpdata.Add(new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString());
                            inpdata.Add(x.slno.ToString());
                            inpdata.Add(x.name);
                            inpdata.Add(x.details);
                            inpdata.Add(x.reciept.ToString());
                            inpdata.Add(x.payment.ToString());
                            inpdata.Add(x.interest.ToString());
                            inpdata.Add(x.acc_id);

                            if (!dataAccess.Post_ac_data(inpData: inpdata))
                            {
                                MessageBox.Show("Posting Failed");
                                return;
                            }
                        }
                        MessageBox.Show("Posted");
                        MainWindow.Reset_data_inp();
                        Reset_Details();

                        present_bal_postneg.Text = "";
                        present_bal_postpos.Text = "";

                        start_bal_postneg.Text = "";
                        start_bal_postpos.Text = "";

                        List<Decimal> numList = dataAccess.Load_lineTotal();
                        Decimal num = numList[0] - numList[1];

                        if (num >= Decimal.Zero)
                            total_bal_pos.Text = start_bal_postpos.Text = num.ToString();
                        else
                            total_bal_neg.Text = start_bal_postneg.Text = Math.Abs(num).ToString();

                    }

                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Send Cancelled");
                    break;
            }
        }
    }
}
