using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Windows;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for Report_Window.xaml
    /// </summary>
    public partial class Report_Window : Window
    {
        public Report_Window(string data_source = "", List<account> acc_list = null, List<records> rec_list = null, string file_name = "", List<string> col_name = null)
        {
            InitializeComponent();
            DataTable dataSourceValue = new DataTable();

            if (file_name == "CapitalAcc_Report.rdlc")
            {
                foreach (var col_ in col_name)
                    dataSourceValue.Columns.Add(col_);
                foreach (var row in rec_list)
                    dataSourceValue.Rows.Add(row.date, row.slno, row.name, row.details, row.days, row.reciept, row.reciept_int, row.payment, row.payment_int);
            }
            else if (file_name == "ChitsList_Report.rdlc")
            {
                foreach (var col_ in col_name)
                    dataSourceValue.Columns.Add(col_);
                foreach (var acc in acc_list)
                    dataSourceValue.Rows.Add(acc.date, acc.slno, acc.name, acc.reciept, acc.payment, acc.balance);
            }
            else if (file_name == "Partys_Report.rdlc" || file_name == "Partys_Report_VillageWise.rdlc")
            {
                foreach (var col_ in col_name)
                    dataSourceValue.Columns.Add(col_);
                foreach (var acc in acc_list)
                    dataSourceValue.Rows.Add(acc.date, acc.slno, acc.name, acc.village, acc.reciept, acc.payment, acc.balance, acc.interest);
            }
            else if (file_name == "Single_Cust_Report.rdlc")
            {
                PropertyInfo[] Props = typeof(records).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    dataSourceValue.Columns.Add(prop.Name);
                }
                foreach (var item in rec_list)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {

                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataSourceValue.Rows.Add(values);
                }
            }
            else if (file_name == "Sample_Report.rdlc")
            {
                PropertyInfo[] Props = typeof(account).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    dataSourceValue.Columns.Add(prop.Name);
                }
                foreach (var item in acc_list)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {

                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataSourceValue.Rows.Add(values);
                }
            }
            else
            {
                if (rec_list != null && acc_list == null)
                {
                    foreach (string columnName in col_name)
                        dataSourceValue.Columns.Add(columnName);
                    foreach (records rec in rec_list)
                        dataSourceValue.Rows.Add((object)rec.date, (object)rec.slno, (object)rec.name, (object)rec.details, (object)rec.reciept, (object)rec.payment, (object)rec.interest);
                }
                else if (acc_list != null && rec_list == null)
                {
                    foreach (string columnName in col_name)
                        dataSourceValue.Columns.Add(columnName);
                    if (data_source == "trail_bal")
                    {
                        foreach (account acc in acc_list)
                        {
                            dataSourceValue.Rows.Add((object)acc.bal_pos, (object)acc.name, (object)acc.bal_neg);
                        }
                    }
                    else
                    {
                        foreach (account acc in acc_list)
                            dataSourceValue.Rows.Add((object)acc.date, (object)acc.slno, (object)acc.name, (object)acc.village, (object)acc.type, (object)acc.interest, (object)acc.reciept, (object)acc.payment);
                    }
                }
            }

            Rpt.LocalReport.DataSources.Add(new ReportDataSource(data_source, dataSourceValue));
            Rpt.LocalReport.ReportEmbeddedResource = "BHAGAVANDVSOFTWARE." + file_name;
            Rpt.ProcessingMode = ProcessingMode.Local;
            Rpt.RefreshReport();
        }
    }
}
