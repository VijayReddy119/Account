using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for New_Customers.xaml
    /// </summary>
    public partial class New_Customers : Page
    {
        private readonly DataAccess dataAccess = new DataAccess();
        private string acc_id = "";
        private string slno_dg = "", edit_slno = "";
        private List<account> acc_list = new List<account>();
        private List<string> villageList = new List<string>();
        private List<string> slno_list = new List<string>();
        private int slno_index = 0, list_count;
        public New_Customers()
        {
            InitializeComponent();
            date_new.Focus();
            villageList = dataAccess.Get_village_list();
            village.ItemsSource = villageList;
            acc_list = dataAccess.Load_acc_db("", "", 0, "", false);
            Output.ItemsSource = acc_list;

            slno_list = dataAccess.GetAllslno();
            slno_index = 0;
            list_count = slno_list.Count;
        }

        private void LoadData()
        {
            string text = edit_slno;
            List<account> accountList = dataAccess.Load_acc_db(inp: text);
            if (accountList != null)
            {
                acc_id = accountList[0].acc_id.ToString();
                slno.Text = accountList[0].slno.ToString();
                name.Text = accountList[0].name.ToString();
                if (accountList[0].interest.ToString() != "0")
                {
                    interest_rate.IsEnabled = true;
                    interest_rate.Text = accountList[0].interest.ToString();
                    share.IsEnabled = true;
                    share.Text = accountList[0].share.ToString();
                }
                if (accountList[0].type.ToString() == "Partys")
                {
                    village.Text = accountList[0].village.ToString();
                    village.IsEnabled = true;
                    add_village.IsEnabled = true;
                    add_village_btn.IsEnabled = true;
                }
                else
                {
                    village.IsEnabled = false;
                    add_village.IsEnabled = false;
                    add_village_btn.IsEnabled = false;
                }
                acc_type.Text = accountList[0].type.ToString();
                if (accountList[0].village.ToString() != "")
                {
                    village.IsEnabled = true;
                    village.Text = accountList[0].village.ToString();
                }
                string[] strArray = accountList[0].date.ToString().Split('-');
                date_new.SelectedDate = new DateTime?(new DateTime(int.Parse(strArray[2]), int.Parse(strArray[1]), int.Parse(strArray[0])));
                save_btn.IsEnabled = false;
                edit_btn.IsEnabled = true;
                delete_btn.IsEnabled = true;

                edit_slno_check.IsChecked = new bool?(true);
                slno_index = slno_list.IndexOf(edit_slno);

               
                for (int i = 0; i < Output.Items.Count; ++i)
                {
                    DataGridRow row = (DataGridRow)Output.ItemContainerGenerator.ContainerFromIndex(i);
                    if (row == null)
                    {
                        Output.UpdateLayout();
                        Output.ScrollIntoView(Output.Items[i]);
                        row = (DataGridRow)Output.ItemContainerGenerator.ContainerFromIndex(i);

                    }

                    TextBlock cellContent = Output.Columns[1].GetCellContent(row) as TextBlock;
                    if (cellContent != null && cellContent.Text.Equals(edit_slno))
                    {
                        object item = Output.Items[i];
                        Output.SelectedItem = item;

                        break;
                    }
                }
            }
        }

        private void Slno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                edit_slno = slno.Text;
                edit_slno_check.IsChecked = new bool?(false);
                name.Focus();
            }
            else if (e.Key == Key.Add)
            {
                if ((slno_index + 1) >= list_count)
                    return;
                slno_index += 1;
                edit_slno = slno_list[slno_index];
                slno.Text = edit_slno;
                LoadData();
            }
            else if (e.Key == Key.Subtract)
            {
                if ((slno_index - 1) < 0)
                    return;
                slno_index -= 1;
                edit_slno = slno_list[slno_index];
                slno.Text = edit_slno;
                LoadData();
            }
        }

        private void Date_new_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            slno.Focus();
        }

        private void Name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            acc_type.Focus();
        }

        private void Village_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (interest_rate.IsEnabled == false)
                {
                    Save_Data();
                }
            }
        }

        private void Acc_type_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            if (acc_type.Text == "Capital")
            {
                interest_rate.IsEnabled = true;
                interest_rate.Text = "1.5";

                share.IsEnabled = true;
                share.Focus();
            }
            else if (acc_type.Text == "Partys")
            {
                /*village.IsEnabled = true;
                add_village.IsEnabled = true;
                add_village_btn.IsEnabled = true;
                village.Focus();*/
                Save_Data();
                interest_rate.IsEnabled = false;
                village.IsEnabled = false;
                add_village.IsEnabled = false;
                add_village_btn.IsEnabled = false;
            }
            else
            {
                Save_Data();
                interest_rate.IsEnabled = false;
                village.IsEnabled = false;
                add_village.IsEnabled = false;
                add_village_btn.IsEnabled = false;
            }
        }

        private void Interest_rate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            Save_Data();
            slno.Focus();
        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            Save_Data();
        }

        private void Save_Data()
        {
            if (slno.Text == "" || name.Text == "" || (acc_type.Text == "Capital" && interest_rate.Text == ""))
            {
                MessageBox.Show("Please Fill Required Details");
            }
            else
            {
                Random random = new Random();

                string upper = village.Text.ToUpper();
                string str3 = interest_rate.Text;
                string str4 = share.Text;

                if (acc_type.Text == "Capital")
                {
                    upper = "";
                    str3 = (interest_rate.Text != "") ? interest_rate.Text : "1.5";
                    str4 = this.share.Text != "" ? this.share.Text : "0";
                }
                else if (acc_type.Text == "Partys")
                {
                    upper = village.Text.ToUpper();
                    str3 = "";
                    str4 = "0";
                }
                else
                {
                    upper = "";
                    str3 = "";
                    str4 = "0";
                }


                string text1 = slno.Text;
                string text2 = name.Text;
                string str1 = text2.Substring(0, text2.Length / 2) + "-" + random.Next().ToString();
                string text3 = acc_type.Text;
                string str2 = date_new.SelectedDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                if (str3 == "")
                    str3 = "0";
                if (!dataAccess.Save_toDB(new List<string>()
                  {
                    str1,
                    str2,
                    text1,
                    text2,
                    upper,
                    text3,
                    str3,
                    str4
                  }))
                    return;
                MessageBox.Show("Data Added");
                Reset_details();
                acc_list = dataAccess.Load_acc_db("", "", 0, "", false);
                Output.ItemsSource = acc_list;
                slno.Focus();
            }
        }

        private void Reset_details()
        {
            slno.IsReadOnly = false;
            slno.Text = "";
            name.Text = "";
            interest_rate.Text = "";
            share.Text = "";
            acc_list = dataAccess.Load_acc_db("", "", 0, "", false);
            Output.ItemsSource = acc_list;
            edit_slno_check.IsChecked = new bool?(false);
            save_btn.IsEnabled = true;
            edit_btn.IsEnabled = false;
            delete_btn.IsEnabled = false;
        }

        private void Add_village_btn_Click(object sender, RoutedEventArgs e)
        {
            Add_village_new();
        }

        private void Add_village_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            Add_village_new();
        }

        private void Add_village_new()
        {
            if (add_village.Text != "")
            {
                if (!villageList.Contains(add_village.Text))
                {
                    if (dataAccess.Save_Village(add_village.Text.ToUpper()))
                    {
                        village.ItemsSource = dataAccess.Get_village_list();
                        village.SelectedItem = (object)add_village.Text;
                        add_village.Text = "";
                        acc_type.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Cannot Add Village");
                    }
                }
                else
                {
                    MessageBox.Show("Village Exists");
                }
            }
            else
            {
                MessageBox.Show("Enter Village");
            }
        }

        private void Delete_btn_Click(object sender, RoutedEventArgs e)
        {
            int num1;
            if (slno.Text != "")
            {
                bool? isChecked = edit_slno_check.IsChecked;
                bool flag = true;
                num1 = isChecked.GetValueOrDefault() == flag & isChecked.HasValue ? 1 : 0;
            }
            else
                num1 = 0;
            if (num1 != 0)
            {
                if (!dataAccess.Del_fromDB(acc_id))
                    return;
                MessageBox.Show("Deleted");
                Reset_details();
            }
            else
            {
                MessageBox.Show("Enter valid slno");
            }
        }

        private void Reset_btn_Click(object sender, RoutedEventArgs e)
        {
            Reset_details();
        }

        private void Output_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Output.SelectedCells.Count <= 0)
                return;
            object selectedItem = Output.SelectedItem;

            string[] strArray = (Output.SelectedCells[0].Column.GetCellContent(selectedItem) as TextBlock).Text.Split('-');
            date_new.SelectedDate = new DateTime?(new DateTime(int.Parse(strArray[2]), int.Parse(strArray[1]), int.Parse(strArray[0])));

            DataGridCellInfo selectedCell = Output.SelectedCells[1];

            string text1 = (selectedCell.Column.GetCellContent(selectedItem) as TextBlock).Text;
            slno.Text = text1;

            selectedCell = Output.SelectedCells[2];
            string text2 = (selectedCell.Column.GetCellContent(selectedItem) as TextBlock).Text;
            name.Text = text2;

            selectedCell = Output.SelectedCells[3];
            string text3 = (selectedCell.Column.GetCellContent(selectedItem) as TextBlock).Text;
            village.Text = text3;
            if (text3 != "")
            {
                village.IsEnabled = true;
            }
            else
            {
                village.IsEnabled = false;
                village.Text = "";
            }

            selectedCell = Output.SelectedCells[4];
            string text4 = (selectedCell.Column.GetCellContent(selectedItem) as TextBlock).Text;
            acc_type.Text = text4;

            if (text4 == "Partys")
            {
                village.Text = text3;
                village.IsEnabled = true;
                add_village.IsEnabled = true;
                add_village_btn.IsEnabled = true;
            }
            else
            {
                village.IsEnabled = false;
                add_village.IsEnabled = false;
                add_village_btn.IsEnabled = false;
            }

            selectedCell = Output.SelectedCells[5];
            string text5 = (selectedCell.Column.GetCellContent(selectedItem) as TextBlock).Text;

            if (text5 != "0")
            {
                interest_rate.IsEnabled = true;
                interest_rate.Text = text5;
                share.IsEnabled = true;
                share.Text = (this.Output.SelectedCells[9].Column.GetCellContent(selectedItem) as TextBlock).Text;
            }
            else
            {
                interest_rate.IsEnabled = false;
                interest_rate.Text = "0";
                share.IsEnabled = false;
                share.Text = "0";
            }

            slno_dg = slno.Text;
            acc_id = dataAccess.Load_acc_db(slno_dg, "", 0, "", false)[0].acc_id.ToString();
            edit_slno_check.IsChecked = new bool?(true);
            edit_btn.IsEnabled = true;
            delete_btn.IsEnabled = true;
            save_btn.IsEnabled = false;
        }

        private void Edit_btn_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = edit_slno_check.IsChecked;
            bool flag = true;
            if (!(isChecked.GetValueOrDefault() == flag & isChecked.HasValue) || !(slno.Text != "") || !(name.Text != ""))
                return;
            string text1 = slno.Text;
            string text2 = name.Text;
            string upper = village.Text.ToUpper();
            string text3 = acc_type.Text;
            string str1 = date_new.SelectedDate.Value.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            string str2 = interest_rate.Text;
            string text4 = share.Text;
            if (str2 == "")
                str2 = "0";
            if (text4 == "")
                text4 = "0";

            if (dataAccess.Update_db(new List<string>()
          {
            acc_id,
            str1,
            text1,
            text2,
            upper,
            text3,
            str2,
            text4
          }))
            {
                MessageBox.Show("Data Updated");
                Reset_details();
                Output.ItemsSource = dataAccess.Load_acc_db("", "", 0, "", false);
            }
        }

        private void Slno_LostFocus(object sender, RoutedEventArgs e)
        {
            string text = slno.Text;
            bool? isChecked = edit_slno_check.IsChecked;
            bool flag = false;
            if (!(isChecked.GetValueOrDefault() == flag & isChecked.HasValue) || !(text != ""))
                return;
            if (dataAccess.isContain_slno(text))
            {
                edit_slno = slno.Text;
                LoadData();
            }
            else
            {
                Reset_details();
                slno.Text = text;
                name.Focus();
            }
        }

        private void Slno_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void Interest_rate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as System.Windows.Controls.TextBox).Text.Insert((sender as System.Windows.Controls.TextBox).SelectionStart, e.Text));
        }

        private void share_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void share_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                interest_rate.Focus();
        }

        private void Print_btn_Click(object sender, RoutedEventArgs e)
        {
            List<string> col_name = new List<string>() { "Date", "Slno", "Name", "Village", "Type", "Interest", "Reciept", "Payment" };
            acc_list = dataAccess.Load_acc_db("", "", 0, "", false);
            if (acc_list != null)
                new Report_Window("Accountdata", acc_list, (List<records>)null, "AccountReport.rdlc", col_name).Show();
            else
                MessageBox.Show("Empty Data");
        }

        private void acc_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (acc_type.SelectedValue != null)
            {
                if (acc_type.SelectedValue.ToString() == "Capital")
                {
                    interest_rate.IsEnabled = true;
                    interest_rate.Text = "1.5";
                    share.IsEnabled = true;

                    village.Text = "";
                    village.IsEnabled = false;
                    add_village.IsEnabled = false;
                    add_village_btn.IsEnabled = false;
                }
                else if (acc_type.SelectedValue.ToString() == "Partys")
                {
                    interest_rate.Text = "";
                    interest_rate.IsEnabled = false;
                    share.IsEnabled = false;

                    village.IsEnabled = true;
                    add_village.IsEnabled = true;
                    add_village_btn.IsEnabled = true;
                }
                else
                {
                    interest_rate.Text = "";
                    village.Text = "";
                    interest_rate.IsEnabled = false;
                    share.IsEnabled = false;

                    village.IsEnabled = false;
                    add_village.IsEnabled = false;
                    add_village_btn.IsEnabled = false;
                }
            }
        }
    }
}
