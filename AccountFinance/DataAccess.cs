using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace AccountFinance
{
    public class DataAccess
    {
        private static readonly string path_ = System.AppDomain.CurrentDomain.BaseDirectory;
        //protected string const_db = "Data source = " + path_ + "/Data/DB_list.db; Version=3;";
        protected static string old_dbpath;
        private static string db_path;
        private static string db_name;
        protected Decimal Da_tot_int;
        public void InitializeDb()
        {
            if (!Directory.Exists("New_data"))
                Directory.CreateDirectory(path_ + "/Data/New_data");
            if (!Directory.Exists("Old_data"))
                Directory.CreateDirectory(path_ + "/Data/Old_data");
            if (!Directory.Exists("Del_data"))
                Directory.CreateDirectory(path_ + "/Data/Del_data");
            /*
            if (!File.Exists(path_ + "/Data/Db_list.db"))
                SQLiteConnection.CreateFile(path_ + "/Data/Db_list.db");

            SQLiteConnection connection = new SQLiteConnection(const_db);
            connection.Open();
            SQLiteCommand sqLiteCommand = new SQLiteCommand("CREATE TABLE IF NOT EXISTS db_name_list(db_id NVARCHAR(2048) PRIMARY KEY NOT NULL, db_name NVARCHAR(2048) NOT NULL, date_Created NVARCHAR(20) NOT NULL)", connection);
            sqLiteCommand.ExecuteNonQuery();
            connection.Close();
            */
        }

        public void ModifyTableData(bool old = false)
        {
            string conn_url = "";
            if (!old)
            {
                conn_url = "Data Source=" + db_path + ";Version=3;";
            }
            else
            {
                conn_url = "Data Source=" + old_dbpath + ";Version=3;";
            }
            using (SQLiteConnection sqLiteConnection = new SQLiteConnection(conn_url))
            {
                sqLiteConnection.Open();

                //Accounts Table Modify Date and Last_Posting_Date
                List<account> acc_table = new List<account>();
                using(SQLiteDataReader acc_read = new SQLiteCommand("Select acc_id, date, last_posting_date from accounts", sqLiteConnection).ExecuteReader())
                {
                    if (acc_read.HasRows)
                    {
                        while (acc_read.Read())
                        {
                            long date_c;
                            if(long.TryParse(acc_read.GetString(1), out date_c))
                            {
                                long last_date_c;
                                if(long.TryParse(acc_read.GetString(2), out last_date_c))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                string[] date_split = acc_read.GetString(1).Split('-');
                                string date_modified = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString();
                                string last_date_modified = "";
                                
                                if(acc_read.GetString(2) != "")
                                {
                                    date_split = acc_read.GetString(2).Split('-');
                                    last_date_modified = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString();
                                }
                                acc_table.Add(new account(acc_read.GetString(0), date_modified, last_date_modified));
                            }
                        }
                    }
                }

                List<records> rec_table = new List<records>();
                using(SQLiteDataReader rec_read = new SQLiteCommand("Select posting_id, date from records", sqLiteConnection).ExecuteReader())
                {
                    if (rec_read.HasRows)
                    {
                        while (rec_read.Read())
                        {
                            long date_r;
                            if(long.TryParse(rec_read.GetString(1), out date_r))
                            {
                                continue;
                            }
                            else
                            {
                                string[] date_split = rec_read.GetString(1).Split('-');
                                string date_modified = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString();

                                rec_table.Add(new records(rec_read.GetString(0), date_modified));
                            }
                        }
                    }
                }

                foreach(account acc in acc_table)
                {
                    using(SQLiteCommand updateAcc = new SQLiteCommand("Update accounts set date=@Entry, last_posting_date=@Entry1 where acc_id=@Entry2", sqLiteConnection))
                    {
                        updateAcc.Parameters.AddWithValue("@Entry", acc.date);
                        updateAcc.Parameters.AddWithValue("@Entry1", acc.last_posting_date);
                        updateAcc.Parameters.AddWithValue("@Entry2", acc.acc_id);

                        try
                        {
                            updateAcc.ExecuteNonQuery();
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e.ToString());
                            sqLiteConnection.Close();
                            return;
                        }
                    }
                }

                foreach(records rec in rec_table)
                {
                    using(SQLiteCommand updateRec = new SQLiteCommand("Update records set date=@Entry where posting_id=@Entry1", sqLiteConnection))
                    {
                        updateRec.Parameters.AddWithValue("@Entry", rec.date);
                        updateRec.Parameters.AddWithValue("@Entry1", rec.posting_id);

                        try
                        {
                            updateRec.ExecuteNonQuery();
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e.ToString());
                            sqLiteConnection.Close();
                            return;
                        }
                    }
                }
                sqLiteConnection.Close();
            }
        }

        public static void CheckDB(bool old = false)
        {
            string conn_url = "";
            if (!old)
            {
                conn_url = "Data Source=" + db_path + ";Version=3;";
            }
            else
            {
                conn_url = "Data Source=" + old_dbpath + ";Version=3;";
            }
            using (SQLiteConnection sqLiteConnection = new SQLiteConnection(conn_url))
            {
                (sqLiteConnection).Open();
                bool flag = false;
                using (SQLiteCommand sqLiteCommand = new SQLiteCommand("PRAGMA table_info(accounts)", sqLiteConnection))
                {
                    SQLiteDataReader sqLiteDataReader = sqLiteCommand.ExecuteReader();
                    int ordinal = (sqLiteDataReader).GetOrdinal("Name");
                    while ((sqLiteDataReader).Read())
                    {
                        if ((sqLiteDataReader).GetString(ordinal).Equals("share"))
                            flag = true;
                    }
                }
                if (!flag)
                {
                    using (SQLiteCommand sqLiteCommand = new SQLiteCommand("ALTER TABLE accounts ADD share INTEGER NOT NULL DEFAULT 0", sqLiteConnection))
                        (sqLiteCommand).ExecuteNonQuery();
                }
              (sqLiteConnection).Close();
            }
        }

        public static void SetDb(string dbname)
        {
            db_path = path_ + "/Data/New_data/" + dbname + ".db";
            old_dbpath = path_ + "/Data/Old_data/" + dbname + ".db";
            db_name = dbname;
        }

        public static string GetDb()
        {
            return db_path;
        }

        public static string GetDbName()
        {
            return db_name;
        }

        private void Create_Old(bool newFile = false)
        {
            if(!newFile)
                Delete_old();
            else
                SQLiteConnection.CreateFile(old_dbpath);

            SQLiteConnection connection = new SQLiteConnection("Data source = " + old_dbpath + ";Version=3;");
            try
            {
                string commandText1 = "CREATE TABLE IF NOT EXISTS accounts (acc_id NVARCHAR(2048) NOT NULL ,date NVARCHAR(2048) NOT NULL,slno INTEGER NOT NULL,name varchar(255) NOT NULL,village NVARCHAR(2048),type NVARCHAR(2048) NOT NULL,interest_rate NUMERIC DEFAULT '0', reciept_amt NUMERIC DEFAULT 0, payment_amt NUMERIC DEFAULT 0, last_posting_date NVARCHAR(2048) NOT NULL DEFAULT '', share INTEGER NOT NULL DEFAULT 0, deleted_date NVARCHAR(2048) NOT NULL DEFAULT '')";
                string commandText2 = "CREATE TABLE IF NOT EXISTS records (posting_id NVARCHAR(2048) NOT NULL PRIMARY KEY, date NVARCHAR(2048) NOT NULL,slno INTEGER NOT NULL,name varchar(255) NOT NULL, details varchar(255) NOT NULL DEFAULT '',reciept NUMERIC DEFAULT 0,payment NUMERIC DEFAULT 0,interest NUMERIC DEFAULT 0, acc_id NVARCHAR(2048) NOT NULL, FOREIGN KEY (acc_id) REFERENCES balances(acc_id))";
                string commandText3 = "CREATE TABLE IF NOT EXISTS village(village NVARCHAR(2048) NOT NULL UNIQUE)";
                string commandText4 = "CREATE TABLE IF NOT EXISTS lineTable(line_total_reciept NUMERIC Default 0,line_total_payment NUMERIC Default 0)";

                connection.Open();
                
                SQLiteCommand sqLiteCommand1 = new SQLiteCommand(commandText1, connection);
                SQLiteCommand sqLiteCommand2 = new SQLiteCommand(commandText2, connection);
                SQLiteCommand sqLiteCommand3 = new SQLiteCommand(commandText3, connection);
                SQLiteCommand sqLiteCommand4 = new SQLiteCommand(commandText4, connection);

                SQLiteCommand linecmd = new SQLiteCommand("INSERT INTO lineTable(line_total_reciept,line_total_payment) VALUES (0,0)", connection);
                
                sqLiteCommand1.ExecuteNonQuery();
                sqLiteCommand2.ExecuteNonQuery();
                sqLiteCommand3.ExecuteNonQuery();
                sqLiteCommand4.ExecuteNonQuery();
                linecmd.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                MessageBox.Show(e.ToString());
                return;
            }
        }

        private void Delete_old()
        {
            if (!File.Exists(old_dbpath))
            {
                MessageBox.Show("Old Data is Deleted from outside. Data is Lost");
                return;
            }
            string[] strArray2 = new string[7]{path_+"/Data/Del_data/",db_name,"[Deleted_cust_data][",null, null, null,null };
            DateTime now = DateTime.Now;
            strArray2[3] = now.ToString("dd-MM-yyyy");
            strArray2[4] = "_";
            now = DateTime.Now;
            strArray2[5] = now.ToString("HH.mm");
            strArray2[6] = "].db";
            string destFileName2 = string.Concat(strArray2);
            
            File.Copy(old_dbpath, destFileName2);
            File.Delete(old_dbpath);

        }

        public bool CreateAcc()
        {
            var con = new SQLiteConnection("Data source = " + db_path);
            con.Open();

            SQLiteCommand c = new SQLiteCommand("Select type,reciept_amt, payment_amt from accounts where type IN ('Profit','Sadhar')", con);
            SQLiteDataReader r = c.ExecuteReader();
            
            if (r.HasRows)
            {
                int pro = 0, r_sum = 0, p_sum = 0;
                while (r.Read())
                {
                    r_sum += (int)(r.GetDecimal(1));
                    p_sum += (int)(r.GetDecimal(2));
                }
                pro = r_sum - p_sum;
                if (pro == 0 && r_sum!=0 && p_sum!=0)
                {
                    try
                    {
                        SQLiteCommand cmd = new SQLiteCommand("Select * from accounts", con);
                        SQLiteDataReader x = cmd.ExecuteReader();

                        List<account> acc_list = new List<account>();
                        List<string> acc_id = new List<string>();
                        decimal[] lineTotal = new decimal[2];
                        if (x.HasRows)
                        {
                            while (x.Read())
                            {
                                acc_id.Add(x.GetString(0));
                                if (x.GetString(5) != "Sadhar" && x.GetString(5) != "Profit")
                                {
                                    acc_list.Add(new account(x.GetString(0), x.GetString(1), x.GetInt32(2), x.GetString(3), x.GetString(4), x.GetString(5), x.GetDecimal(6), x.GetDecimal(7), x.GetDecimal(8), x.GetInt32(10)));
                                }
                                else
                                {
                                    acc_list.Add(new account(x.GetString(0), x.GetString(1), x.GetInt32(2), x.GetString(3), x.GetString(4), x.GetString(5), Decimal.Zero, Decimal.Zero, Decimal.Zero, x.GetInt32(10)));
                                }
                            }
                            SQLiteCommand lineCmd = new SQLiteCommand("Select * from lineTable", con);
                            SQLiteDataReader read = lineCmd.ExecuteReader();

                            if (read.HasRows)
                            {
                                while (read.Read())
                                {
                                    lineTotal[0] = read.GetDecimal(0);
                                    lineTotal[1] = read.GetDecimal(1);
                                }

                                Create_Old();

                                foreach (string id_ in acc_id)
                                {
                                    if (!Del_fromDB(id_))
                                    {
                                        read.Close();
                                        r.Close();
                                        x.Close();
                                        con.Close();
                                        return false;
                                    }
                                }

                                Random rand = new Random();
                                string[] date_split = DateTime.Now.ToString("dd-MM-yyyy").Split('-');
                                string dateTicks = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString();
                                foreach (var acc in acc_list)
                                {
                                    SQLiteCommand insertCmd = new SQLiteCommand("INSERT INTO accounts(acc_id, date, slno, name, village, type, interest_rate, reciept_amt, payment_amt, share) VALUES(@Entry1,@Entry2,@Entry3,@Entry4,@Entry5,@Entry6,@Entry7,@Entry8, @Entry9, @Entry10)", con);
                                    insertCmd.Parameters.AddWithValue("@Entry1", acc.acc_id);
                                    insertCmd.Parameters.AddWithValue("@Entry2", dateTicks);
                                    insertCmd.Parameters.AddWithValue("@Entry3", acc.slno);
                                    insertCmd.Parameters.AddWithValue("@Entry4", acc.name);
                                    insertCmd.Parameters.AddWithValue("@Entry5", acc.village);
                                    insertCmd.Parameters.AddWithValue("@Entry6", acc.type);
                                    insertCmd.Parameters.AddWithValue("@Entry7", acc.interest);
                                    insertCmd.Parameters.AddWithValue("@Entry8", Decimal.Zero);
                                    insertCmd.Parameters.AddWithValue("@Entry9", Decimal.Zero);
                                    insertCmd.Parameters.AddWithValue("@Entry10", acc.share);

                                    try
                                    {
                                        insertCmd.ExecuteNonQuery();

                                        if(acc.type != "Profit" && acc.type != "Sadhar")
                                        {
                                            string posting_id = acc.name.Substring(0, acc.name.Length / 3) + "-" + acc.slno.ToString() + "-" + rand.Next().ToString();
                                            decimal bal = acc.reciept - acc.payment;
                                            decimal reciept = decimal.Zero, payment = decimal.Zero;
                                            if(bal > 0)
                                            {
                                                reciept = bal;
                                            }
                                            else
                                            {
                                                payment = Math.Abs(bal);
                                            }

                                            List<string> inpdata = new List<string>();
                                            inpdata.Add(posting_id);
                                            inpdata.Add(dateTicks);
                                            inpdata.Add(acc.slno.ToString());
                                            inpdata.Add(acc.name);
                                            inpdata.Add("New Account Posting");
                                            inpdata.Add(reciept.ToString());
                                            inpdata.Add(payment.ToString());
                                            inpdata.Add(acc.interest.ToString());
                                            inpdata.Add(acc.acc_id);

                                            if (!Post_ac_data(inpdata))
                                            {
                                                read.Close();
                                                r.Close();
                                                x.Close();
                                                con.Close();
                                                MessageBox.Show("Error Posting New Account");
                                                return false;
                                            }
                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        read.Close();
                                        r.Close();
                                        x.Close();
                                        con.Close();
                                        MessageBox.Show(e.ToString());
                                        return false;
                                    }
                                }

                                SQLiteCommand lineIns = new SQLiteCommand("INSERT INTO lineTable(line_total_reciept, line_total_payment) VALUES(@Entry,@Entry1)", con);
                                lineIns.Parameters.AddWithValue("@Entry", lineTotal[0]);
                                lineIns.Parameters.AddWithValue("@Entry1", lineTotal[1]);

                                try
                                {
                                    lineIns.ExecuteNonQuery();
                                }
                                catch (Exception e)
                                {
                                    read.Close();
                                    r.Close();
                                    x.Close();
                                    con.Close();
                                    MessageBox.Show(e.ToString());
                                    return false;
                                }
                            }
                            else
                            {
                                read.Close();
                                r.Close();
                                x.Close();
                                con.Close();
                                MessageBox.Show("Incorrect Data or Data is Empty");
                                return false;
                            }
                            read.Close();
                        }
                        else
                        {
                            MessageBox.Show("Data is Empty");
                            r.Close();
                            x.Close();
                            con.Close();
                            return false;
                        }
                        x.Close();
                    }
                    catch (Exception e)
                    {
                        r.Close();
                        con.Close();
                        MessageBox.Show(e.ToString());
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Profit Not Equal to 0");
                    r.Close();
                    con.Close();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Profit Data Not Found");
                r.Close();
                con.Close();
                return false;
            }

            r.Close();
            con.Close();
            return true;
        }

        //Error Handling to be Updated
        public bool LoadFinalAcc(List<account> acc_int_share)
        {
            using(SQLiteConnection conn = new SQLiteConnection("Data Source=" + db_path))
            {
                conn.Open();
                Dictionary<int, string> slno_acc = new Dictionary<int, string>();
                using (SQLiteDataReader cmd = new SQLiteCommand("Select acc_id, slno from accounts where type IN ('Capital', 'Profit')", conn).ExecuteReader())
                {
                    if (cmd.HasRows)
                    {
                        while (cmd.Read())
                        {
                            slno_acc.Add(cmd.GetInt32(1), cmd.GetString(0));
                        }
                    }
                }

                Random random = new Random();
                string[] date_split = DateTime.Now.ToString("dd-MM-yyyy").Split('-');
                string date_ = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString();

                foreach (account acc in acc_int_share)
                {
                    List<string> inpdata = new List<string>();
                    inpdata.Add(acc.name.Substring(0, acc.name.Length / 3) + "-" + acc.slno + "-" + random.Next().ToString());
                    inpdata.Add(date_);
                    inpdata.Add(acc.slno.ToString());
                    inpdata.Add(acc.name.Split('-')[0]);
                    inpdata.Add(acc.name);
                    inpdata.Add(acc.bal_pos.ToString());
                    inpdata.Add(acc.bal_neg.ToString());
                    inpdata.Add("0");
                    inpdata.Add(slno_acc[acc.slno]);

                    Post_ac_data(inpdata);
                }

                int Profit_rec = 0;
                int Profit_pay = 0;
                using (SQLiteDataReader cmd = new SQLiteCommand("Select sum(r.reciept), sum(r.payment) from records r, accounts a where a.type='Profit' and r.slno == a.slno", conn).ExecuteReader())
                {
                    if (cmd.HasRows)
                    {
                        while (cmd.Read())
                        {
                            Profit_rec += cmd.GetInt32(0);
                            Profit_pay += cmd.GetInt32(1);
                        }
                    }
                }

                account profit_acc = Get_ProfitAcc();
                List<string> inppro = new List<string>();
                inppro.Add("Profit-5" + random.Next().ToString());
                inppro.Add(date_);
                inppro.Add("5");
                inppro.Add(profit_acc.name);
                inppro.Add("Final Profit");
                inppro.Add(Profit_pay.ToString());
                inppro.Add(Profit_rec.ToString());
                inppro.Add("0");
                inppro.Add(slno_acc[5]);

                Post_ac_data(inppro);

                List<List<string>> inpSadhar = new List<List<string>>();

                using (SQLiteDataReader cmd = new SQLiteCommand("Select sum(r.reciept), sum(r.payment), a.name, a.slno, a.acc_id from records r, accounts a where a.type='Sadhar' and r.slno == a.slno group by a.slno", conn).ExecuteReader())
                {
                    if (cmd.HasRows)
                    {
                        while (cmd.Read())
                        {
                            List<string> inpSadhr = new List<string>();
                            inpSadhr.Add(cmd.GetString(2).Substring(0, cmd.GetString(2).Length/3) + "-" + cmd.GetInt32(3).ToString() + "-" +random.Next().ToString());
                            inpSadhr.Add(date_);
                            inpSadhr.Add(cmd.GetInt32(3).ToString());
                            inpSadhr.Add(cmd.GetString(2));
                            inpSadhr.Add("Final Sadhar");
                            inpSadhr.Add(cmd.GetDecimal(1).ToString());
                            inpSadhr.Add(cmd.GetDecimal(0).ToString());
                            inpSadhr.Add("0");
                            inpSadhr.Add(cmd.GetString(4));

                            inpSadhar.Add(inpSadhr);
                        }
                    }
                }

                foreach(List<string> x in inpSadhar)
                {
                    Post_ac_data(x);
                }

                conn.Close();
                /*
                    Dictionary<int, List<decimal>> acc_r_p = new Dictionary<int, List<decimal>>();
                using(SQLiteDataReader cmd = new SQLiteCommand("Select slno, name, reciept_amt, payment_amt, share from accounts where share>0 and type='Capital'", conn).ExecuteReader())
                {
                    if (cmd.HasRows)
                    {
                        while (cmd.Read())
                        {
                            acc_r_p.Add(cmd.GetInt32(0), new List<decimal>() { cmd.GetDecimal(2), cmd.GetDecimal(3) });
                        }
                    }
                }

                foreach(account acc in acc_int_share)
                {
                    int reciept_amt = (int)(acc_r_p[acc.slno][0] + acc.bal_pos);
                    int payment_amt = (int)(acc_r_p[acc.slno][1] + acc.bal_neg);

                    using(SQLiteCommand updateCmd = new SQLiteCommand("Update accounts set reciept_amt=@Entry, payment_amt=@Entry1 where slno=@Entry2", conn))
                    {
                        updateCmd.Parameters.AddWithValue("@Entry", reciept_amt);
                        updateCmd.Parameters.AddWithValue("@Entry1", payment_amt);
                        updateCmd.Parameters.AddWithValue("@Entry2", acc.slno);
                        try
                        {
                            updateCmd.ExecuteNonQuery();
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e.ToString());
                            return false;
                        }
                    }
                }            
                 */
            }

            return true;
        }

        public void CreateDb()
        {
            SQLiteConnection connection = new SQLiteConnection("Data source = " + db_path);
            try
            {
                connection.Open();
                
                string commandText1 = "CREATE TABLE IF NOT EXISTS accounts (acc_id NVARCHAR(2048) PRIMARY KEY NOT NULL ,date NVARCHAR(2048) NOT NULL,slno INTEGER NOT NULL UNIQUE,name varchar(255) NOT NULL,village NVARCHAR(2048),type NVARCHAR(2048) NOT NULL,interest_rate NUMERIC DEFAULT '0', reciept_amt NUMERIC DEFAULT 0, payment_amt NUMERIC DEFAULT 0, last_posting_date VARCHAR(2048) NOT NULL DEFAULT '', share INTEGER NOT NULL DEFAULT 0)";
                string commandText2 = "CREATE TABLE IF NOT EXISTS records (posting_id NVARCHAR(2048) NOT NULL PRIMARY KEY, date NVARCHAR(2048) NOT NULL,slno INTEGER NOT NULL,name varchar(255) NOT NULL, details varchar(255) NOT NULL DEFAULT '',reciept NUMERIC DEFAULT 0,payment NUMERIC DEFAULT 0,interest NUMERIC DEFAULT 0, acc_id NVARCHAR(2048) NOT NULL, FOREIGN KEY (acc_id) REFERENCES accounts(acc_id))";
                string commandText3 = "CREATE TABLE IF NOT EXISTS village(village NVARCHAR(2048) NOT NULL UNIQUE)";
                string commandText4 = "CREATE TABLE IF NOT EXISTS lineTable(line_total_reciept NUMERIC Default 0,line_total_payment NUMERIC Default 0)";

                SQLiteCommand sqLiteCommand1 = new SQLiteCommand(commandText1, connection);
                SQLiteCommand sqLiteCommand2 = new SQLiteCommand(commandText2, connection);
                SQLiteCommand sqLiteCommand3 = new SQLiteCommand(commandText3, connection);
                SQLiteCommand sqLiteCommand4 = new SQLiteCommand(commandText4, connection);
                SQLiteCommand linecmd = new SQLiteCommand("INSERT INTO lineTable(line_total_reciept,line_total_payment) VALUES (0,0)", connection);
                SQLiteCommand profitacc = new SQLiteCommand("INSERT INTO accounts(acc_id, date, slno,name, village, type, interest_rate, reciept_amt, payment_amt, last_posting_date) VALUES ('LAABHAM-59572316', '11-12-2019', '5', 'LAABHAM KHAATAA', '', 'Profit', '0', '0', '0', '');",connection);

                sqLiteCommand1.ExecuteNonQuery();
                sqLiteCommand2.ExecuteNonQuery();
                sqLiteCommand3.ExecuteNonQuery();
                sqLiteCommand4.ExecuteNonQuery();
                linecmd.ExecuteNonQuery();
                profitacc.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception e)
            {
                connection.Close();
                MessageBox.Show("Can't Create Line"+e.ToString());
                return;
            }
            SQLiteConnection conn = new SQLiteConnection("Data source = " + old_dbpath + ";Version=3;");
            try
            {
                string commandText1 = "CREATE TABLE IF NOT EXISTS accounts (acc_id NVARCHAR(2048) NOT NULL ,date NVARCHAR(2048) NOT NULL,slno INTEGER NOT NULL,name varchar(255) NOT NULL,village NVARCHAR(2048),type NVARCHAR(2048) NOT NULL,interest_rate NUMERIC DEFAULT '0', reciept_amt NUMERIC DEFAULT 0, payment_amt NUMERIC DEFAULT 0, last_posting_date NVARCHAR(2048) NOT NULL DEFAULT '', share INTEGER NOT NULL DEFAULT 0, deleted_date NVARCHAR(2048) NOT NULL DEFAULT '')";
                string commandText2 = "CREATE TABLE IF NOT EXISTS records (posting_id NVARCHAR(2048) NOT NULL, date NVARCHAR(2048) NOT NULL,slno INTEGER NOT NULL,name varchar(255) NOT NULL, details varchar(255) NOT NULL DEFAULT '',reciept NUMERIC DEFAULT 0,payment NUMERIC DEFAULT 0,interest NUMERIC DEFAULT 0, acc_id NVARCHAR(2048) NOT NULL, FOREIGN KEY (acc_id) REFERENCES accounts(acc_id))";
                string commandText3 = "CREATE TABLE IF NOT EXISTS village(village NVARCHAR(2048) NOT NULL UNIQUE)";
                string commandText4 = "CREATE TABLE IF NOT EXISTS lineTable(line_total_reciept NUMERIC Default 0,line_total_payment NUMERIC Default 0)";

                conn.Open();
                
                SQLiteCommand sqLiteCommand1 = new SQLiteCommand(commandText1, conn);
                SQLiteCommand sqLiteCommand2 = new SQLiteCommand(commandText2, conn);
                SQLiteCommand sqLiteCommand3 = new SQLiteCommand(commandText3, conn);
                SQLiteCommand sqLiteCommand4 = new SQLiteCommand(commandText4, conn);
                SQLiteCommand linecmd = new SQLiteCommand("INSERT INTO lineTable(line_total_reciept,line_total_payment) VALUES (0,0)", conn);
                
                sqLiteCommand1.ExecuteNonQuery();
                sqLiteCommand2.ExecuteNonQuery();
                sqLiteCommand3.ExecuteNonQuery();
                sqLiteCommand4.ExecuteNonQuery();
                linecmd.ExecuteNonQuery();
                
                conn.Close();
            }
            catch (Exception e)
            {
                conn.Close();
                MessageBox.Show("Can't Create Line"+e.ToString());
                return;
            }
            /*
            SQLiteConnection con = new SQLiteConnection(const_db);
            try
            {
                con.Open();
                
                Random random = new Random();
                string str1 = db_name + "-" + random.Next().ToString();
                string str2 = DateTime.Now.ToString("dd-MM-yyyy");
                SQLiteCommand sqLiteCommand = new SQLiteCommand("Insert into db_name_list(db_id, db_name, date_Created) Values(@Entry, @Entry1, @Entry2)", con);
                sqLiteCommand.Parameters.AddWithValue("@Entry", (object)str1);
                sqLiteCommand.Parameters.AddWithValue("@Entry1", (object)db_name);
                sqLiteCommand.Parameters.AddWithValue("@Entry2", (object)str2);
                sqLiteCommand.ExecuteNonQuery();
                
                con.Close();
            }
            catch (Exception e)
            {
                con.Close();
                MessageBox.Show("Can't Create Line"+e.ToString());
                return;
            }*/
        }

        public List<string> LoadLine_list()
        {
            string[] newData = Directory.GetFiles(path_ + "/Data/New_data", "*.db");
            List<string> stringList = new List<string>();

            foreach (string x in newData)
            {
                int len_c = (path_ + "/Data/New_data/").Length;
                string[] y = x.Remove(0, len_c).Split('.');
                stringList.Add(y[0]);
            }
            /*
            using(SQLiteConnection connection = new SQLiteConnection(const_db))
            {
                connection.Open();

                using(SQLiteDataReader sqLiteDataReader = new SQLiteCommand("Select db_name from db_name_list", connection).ExecuteReader())
                {
                    if (sqLiteDataReader.HasRows)
                    {
                        while (sqLiteDataReader.Read())
                            stringList.Add(sqLiteDataReader.GetString(0));
                    }

                    sqLiteDataReader.Close();
                }

                connection.Close();
            }*/

            return stringList;
        }

        public bool DeleteDb(string dbname)
        {
            if (!File.Exists(old_dbpath))
            {
                if (!File.Exists(db_path))
                {
                    MessageBox.Show("Delete Failed. Data is Changed");
                    return false;
                }
                Create_Old(true);
            }
            string[] strArray1 = new string[7]
            {path_+"/Data/Del_data/",dbname,"[",null,null,null,null};
            DateTime now = DateTime.Now;
            strArray1[3] = now.ToString("dd-MM-yyyy");
            strArray1[4] = "_";
            now = DateTime.Now;
            strArray1[5] = now.ToString("HH.mm");
            strArray1[6] = "].db";
            string destFileName1 = string.Concat(strArray1);
            File.Copy(db_path, destFileName1);
            string[] strArray2 = new string[7]
            {path_+"/Data/Del_data/",dbname,"[Deleted_cust_data][",null,null,null,null};
            now = DateTime.Now;
            strArray2[3] = now.ToString("dd-MM-yyyy");
            strArray2[4] = "_";
            now = DateTime.Now;
            strArray2[5] = now.ToString("HH.mm");
            strArray2[6] = "].db";
            string destFileName2 = string.Concat(strArray2);
            File.Copy(old_dbpath, destFileName2);
            File.Delete(db_path);
            File.Delete(old_dbpath);
            /*
            SQLiteConnection connection = new SQLiteConnection(const_db);
            
            connection.Open();
            
            SQLiteCommand sqLiteCommand = new SQLiteCommand("Delete from db_name_list where db_name=@Entry", connection);
            sqLiteCommand.Parameters.AddWithValue("@Entry", (object)dbname);
            sqLiteCommand.ExecuteNonQuery();
            
            connection.Close();*/
            return true;
        }

        public List<string> Get_village_list()
        {
            List<string> stringList = new List<string>();
            SQLiteConnection connection = new SQLiteConnection("Data Source = " + db_path + ";Version=3;");
            try
            {
                connection.Open();
                
                SQLiteDataReader sqLiteDataReader = new SQLiteCommand("Select village from village", connection).ExecuteReader();
                if (sqLiteDataReader.HasRows)
                {
                    while (sqLiteDataReader.Read())
                        stringList.Add(sqLiteDataReader.GetString(0));
                }
                sqLiteDataReader.Close();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                MessageBox.Show("Restart the Application");
            }
            return stringList;
        }

        public bool Save_Village(string inpt)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source = " + db_path + ";Version=3;");
            try
            {
                connection.Open();
                
                SQLiteCommand sqLiteCommand = new SQLiteCommand("INSERT INTO village(village) VALUES(@Entry);", connection);
                sqLiteCommand.Parameters.AddWithValue("@Entry", (object)inpt);
                int num1 = sqLiteCommand.ExecuteNonQuery();
                
                connection.Close();
                
                if (num1 > 0)
                    return true;
                int num2 = (int)MessageBox.Show("Restart the Application");
                return false;
            }
            catch (Exception)
            {
                connection.Close();
                MessageBox.Show("Restart the Application");
                return false;
            }
        }

        public bool Save_toDB(List<string> data)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source = " + db_path + ";Version=3;");
            try
            {
                connection.Open();

                SQLiteCommand sqLiteCommand = new SQLiteCommand("INSERT INTO accounts(acc_id,date,slno,name,village,type,interest_rate, share) VALUES(@Entry,@Entry1,@Entry2,@Entry3,@Entry4,@Entry5,@Entry6,@Entry7);", connection);
                sqLiteCommand.Parameters.AddWithValue("@Entry", (object)data[0]);
                sqLiteCommand.Parameters.AddWithValue("@Entry1", (object)data[1]);
                sqLiteCommand.Parameters.AddWithValue("@Entry2", (object)data[2]);
                sqLiteCommand.Parameters.AddWithValue("@Entry3", (object)data[3]);
                sqLiteCommand.Parameters.AddWithValue("@Entry4", (object)data[4]);
                sqLiteCommand.Parameters.AddWithValue("@Entry5", (object)data[5]);
                sqLiteCommand.Parameters.AddWithValue("@Entry6", (object)data[6]);
                sqLiteCommand.Parameters.AddWithValue("@Entry7", (object)data[7]);
                int num1 = sqLiteCommand.ExecuteNonQuery();
                
                connection.Close();
                
                if (num1 > 0)
                    return true;
                int num2 = (int)MessageBox.Show("Data Insert Failed");
                return false;
            }
            catch (Exception)
            {
                connection.Close();
                MessageBox.Show("Data Insert Failed");
                return false;
            }
        }

        private Decimal[] Compute_int(string date, Decimal reciept, Decimal payment, Decimal interest_rate, string date_t, string cal_int)
        {
            if (cal_int == "LINT" || cal_int == "LINTEREST")
            {
                return new Decimal[2] { 0, 0 };
            }

            date_t = new DateTime(long.Parse(date_t)).ToString("dd-MM-yyyy");
            date = new DateTime(long.Parse(date)).ToString("dd-MM-yyyy");

            string[] strArray1 = date_t.Split('-');
            string[] strArray2 = date.Split('-');

            /*
            DateTime dateTime1 = new DateTime(int.Parse(strArray1[2]), int.Parse(strArray1[1]), int.Parse(strArray1[0]));
            DateTime dateTime2 = new DateTime(int.Parse(strArray2[2]), int.Parse(strArray2[1]), int.Parse(strArray2[0]));
            int num = (dateTime1 - dateTime2).Days + 1;
            */

            int num = (int.Parse(strArray2[2]) - int.Parse(strArray1[2])) * 12 * 30 + (int.Parse(strArray2[1]) - int.Parse(strArray1[1])) * 30 + (int.Parse(strArray2[0]) - int.Parse(strArray1[0]));

            return new Decimal[2]{
                Math.Round(Math.Abs(reciept * (Decimal) num * interest_rate / new Decimal(3000))),
                Math.Round(Math.Abs(payment * (Decimal) num * interest_rate / new Decimal(3000)))
            };
            
        }

        public List<account> Load_acc_db(string inp = "", string type = "", int trail_b = 0, string date_t = "", bool p = false, string village_sch = "", bool monthly_int = false, string month="", bool old= false)
        {
            List<account> accountList = new List<account>();

            string conn_url = "";
            if (!old)
            {
                conn_url = "Data Source = " + db_path + ";Version=3;";
            }
            else
            {
                conn_url = "Data Source = " + old_dbpath + ";Version=3;";
            }
            using (SQLiteConnection connection = new SQLiteConnection(conn_url))
            {
                connection.Open();

                switch (trail_b)
                {
                    case 1:
                        string[] date_split_1 = date_t.Split('-');
                        long datetick_1 = new DateTime(Int32.Parse(date_split_1[2]), Int32.Parse(date_split_1[1]), Int32.Parse(date_split_1[0])).Ticks;

                        date_t = "" + datetick_1;
                        using (SQLiteDataReader sqLiteDataReader = new SQLiteCommand("Select sum(r.reciept), a.name, sum(r.payment), r.slno, a.share, a.type from accounts a, records r where r.slno == a.slno and r.date<='"+ date_t +"' Group By r.name order by type", connection).ExecuteReader())
                        {
                            if (sqLiteDataReader.HasRows)
                            {
                                while (sqLiteDataReader.Read())
                                    accountList.Add(new account((sqLiteDataReader).GetDecimal(0), (sqLiteDataReader).GetString(1), (sqLiteDataReader).GetDecimal(2), (sqLiteDataReader).GetInt32(3), (sqLiteDataReader).GetInt32(4), (sqLiteDataReader).GetString(5)));
                            }
                            sqLiteDataReader.Close();
                        }
                        break;
                        /*
                    case 3:
                        using (SQLiteDataReader sqLiteDataReader = new SQLiteCommand("Select reciept_amt,name,payment_amt,type from accounts order by type", connection).ExecuteReader())
                        {
                            if (sqLiteDataReader.HasRows)
                            {
                                while (sqLiteDataReader.Read())
                                {
                                    if (sqLiteDataReader.GetString(3) != "Sadhar" && sqLiteDataReader.GetString(3) != "Profit")
                                    {
                                        accountList.Add(new account(sqLiteDataReader.GetDecimal(0), sqLiteDataReader.GetString(1), sqLiteDataReader.GetDecimal(2)));
                                    }
                                }
                                accountList.Add(new account(0, "PROFIT", 0));
                            }
                            sqLiteDataReader.Close();
                        }
                        break;
                        */
                    case 2:
                        if(date_t != "")
                        {
                            string[] date_split = date_t.Split('-');
                            long datetick = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks;

                            date_t = "" + datetick;

                            Decimal num1 = new Decimal();
                            Decimal num2 = new Decimal();

                            using (SQLiteDataReader sqLiteDataReader2 = new SQLiteCommand("SELECT sum(r.reciept), sum(r.payment) from accounts a, records r where r.slno == a.slno and a.type='Sadhar' and r.date<='" + date_t+ "'", connection).ExecuteReader())
                            {
                                if (sqLiteDataReader2.HasRows)
                                {
                                    while (sqLiteDataReader2.Read())
                                    {
                                        if(sqLiteDataReader2.GetValue(0).ToString() == "" && sqLiteDataReader2.GetValue(1).ToString() == "")
                                        {
                                            num1 += 0;
                                        }
                                        else
                                        {
                                            num1 += Math.Abs(sqLiteDataReader2.GetDecimal(0) - sqLiteDataReader2.GetDecimal(1));
                                        }
                                    }
                                }
                                sqLiteDataReader2.Close();
                            }

                            using (SQLiteDataReader sqLiteDataReader3 = new SQLiteCommand("SELECT sum(r.reciept), sum(r.payment) from accounts a, records r where r.slno == a.slno and a.type='Profit' and r.date<='" + date_t + "'", connection).ExecuteReader())
                            {
                                if (sqLiteDataReader3.HasRows)
                                {
                                    while (sqLiteDataReader3.Read())
                                    {
                                        if(sqLiteDataReader3.GetValue(0).ToString() == "" && sqLiteDataReader3.GetValue(1).ToString() == "")
                                        {
                                            num2 += 0;
                                        }
                                        else
                                        {
                                            num2 += Math.Abs(sqLiteDataReader3.GetDecimal(0) - sqLiteDataReader3.GetDecimal(1));
                                        }
                                    }
                                }
                                sqLiteDataReader3.Close();
                            }

                            Decimal int_bal = Math.Round(num2 - num1, 2);
                            Decimal num3 = new Decimal();
                            Dictionary<string, Decimal> dictionary = new Dictionary<string, Decimal>();

                            HashSet<int> intSet = new HashSet<int>();

                            using (SQLiteDataReader sqLiteDataReader4 = new SQLiteCommand("SELECT r.slno,r.name,r.date,r.reciept, r.payment, a.interest_rate from records r, accounts a where r.slno = a.slno and a.type IN ('Capital') and r.date<='" + date_t + "' order by a.name;", connection).ExecuteReader())
                            {
                                if (sqLiteDataReader4.HasRows)
                                {
                                    while (sqLiteDataReader4.Read())
                                        intSet.Add(sqLiteDataReader4.GetInt32(0));

                                    foreach (int num4 in intSet)
                                        dictionary.Add(num4.ToString(), Decimal.Zero);

                                    using (SQLiteDataReader sqLiteDataReader5 = new SQLiteCommand("SELECT r.slno, r.name, r.date, r.reciept, r.payment, a.interest_rate, r.details, a.type from records r, accounts a where r.slno = a.slno and a.type IN ('Capital') and r.date<='" + date_t + "' order by a.name;", connection).ExecuteReader())
                                    {
                                        if (sqLiteDataReader5.HasRows)
                                        {
                                            while (sqLiteDataReader5.Read())
                                            {
                                                Decimal[] numArray = Compute_int(sqLiteDataReader5.GetString(2), sqLiteDataReader5.GetDecimal(3), sqLiteDataReader5.GetDecimal(4), sqLiteDataReader5.GetDecimal(5), date_t, sqLiteDataReader5.GetString(6));

                                                if (dictionary.ContainsKey(sqLiteDataReader5.GetInt32(0).ToString()))
                                                    dictionary[sqLiteDataReader5.GetInt32(0).ToString()] += numArray[0] - numArray[1];
                                            }
                                        }

                                        sqLiteDataReader5.Close();
                                    }

                                    foreach (KeyValuePair<string, Decimal> keyValuePair in dictionary)
                                        num3 += keyValuePair.Value;

                                    int_bal -= num3;
                                    int_bal = Math.Round(int_bal, 2);

                                }
                                sqLiteDataReader4.Close();
                            }

                            using (SQLiteDataReader sqLiteDataReader = new SQLiteCommand("Select a.name, sum(r.reciept), sum(r.payment), a.interest_rate, a.type, a.slno, a.share from accounts a, records r where a.type=='Partys' and r.slno == a.slno and r.date<='" + date_t + "' GROUP BY a.name", connection).ExecuteReader())
                            {
                                if ((sqLiteDataReader).HasRows)
                                {
                                    while ((sqLiteDataReader).Read())
                                        accountList.Add(new account((sqLiteDataReader).GetDecimal(1), (sqLiteDataReader).GetString(0), (sqLiteDataReader).GetDecimal(2), (sqLiteDataReader).GetInt32(5), (sqLiteDataReader).GetInt32(6), (sqLiteDataReader).GetString(4)));
                                }
                                (sqLiteDataReader).Close();
                            }
                            using (SQLiteDataReader sqLiteDataReader = new SQLiteCommand("Select a.name, sum(r.reciept), sum(r.payment), a.interest_rate, a.type, a.slno, a.share from accounts a, records r where a.type IN ('Capital', 'Chits') and r.slno == a.slno and r.date<='" + date_t + "' GROUP BY a.name", connection).ExecuteReader())
                            {
                                if ((sqLiteDataReader).HasRows)
                                {
                                    while ((sqLiteDataReader).Read())
                                    {
                                        accountList.Add(new account((sqLiteDataReader).GetDecimal(1), (sqLiteDataReader).GetString(0), (sqLiteDataReader).GetDecimal(2), (sqLiteDataReader).GetInt32(5), (sqLiteDataReader).GetInt32(6), (sqLiteDataReader).GetString(4)));
                                        if (sqLiteDataReader.GetDecimal(3) > Decimal.Zero && sqLiteDataReader.GetString(4) == "Capital")
                                        {
                                            if (dictionary.ContainsKey(sqLiteDataReader.GetInt32(5).ToString()))
                                            {
                                                string name = (sqLiteDataReader).GetString(0) + "-Interest";
                                                string index = (sqLiteDataReader).GetInt32(5).ToString();
                                                account account = new account(name, Math.Round(dictionary[index], 2), (sqLiteDataReader).GetInt32(5));
                                                accountList.Add(account);
                                            }
                                        }
                                    }
                                    accountList.Add(new account("PROFIT", int_bal, -5));
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        if (type == "Partys")
                        {
                            Dictionary<int, decimal> inter = new Dictionary<int, decimal>();
                            if (!monthly_int)
                            {
                                using (SQLiteCommand schCmd = new SQLiteCommand("Select slno,sum(interest) from records where slno IN (Select slno from accounts where type='Partys') group by slno", connection))
                                {
                                    using (SQLiteDataReader read_schCmd = schCmd.ExecuteReader())
                                    {
                                        if (read_schCmd.HasRows)
                                        {
                                            while (read_schCmd.Read())
                                            {
                                                inter.Add(read_schCmd.GetInt32(0), read_schCmd.GetDecimal(1));
                                            }
                                        }
                                        read_schCmd.Close();
                                    }
                                }
                            }
                            else
                            {
                                using (SQLiteCommand schCmd = new SQLiteCommand("Select slno,sum(interest) from records where slno IN (Select slno from accounts where type='Partys') and date LIKE '%-" + month + "' group by slno", connection))
                                {
                                    using (SQLiteDataReader read_schCmd = schCmd.ExecuteReader())
                                    {
                                        if (read_schCmd.HasRows)
                                        {
                                            while (read_schCmd.Read())
                                            {
                                                inter.Add(read_schCmd.GetInt32(0), read_schCmd.GetDecimal(1));
                                            }
                                        }
                                        read_schCmd.Close();
                                    }
                                }
                            }

                            var disp_schCmd = new SQLiteCommand();
                            if (inp == "" && village_sch == "")
                                disp_schCmd = new SQLiteCommand("Select * from accounts where type='Partys' order by slno", connection);
                            else if (inp != "" && village_sch == "")
                                disp_schCmd = new SQLiteCommand("Select * from accounts where type='Partys' and slno=" + inp + " order by slno", connection);
                            else if (inp == "" && village_sch != "")
                                disp_schCmd = new SQLiteCommand("Select * from accounts where type='Partys' and village='" + village_sch + "' order by slno;", connection);
                            else
                                disp_schCmd = new SQLiteCommand("Select * from accounts where type='Partys' and slno=" + inp + " and village='" + village_sch + "' order by slno;", connection);

                            using (SQLiteDataReader read_disp_schCmd = disp_schCmd.ExecuteReader())
                            {
                                if (read_disp_schCmd.HasRows)
                                {
                                    while (read_disp_schCmd.Read())
                                    {
                                        decimal reciept_ = read_disp_schCmd.GetDecimal(7);
                                        decimal payment_ = read_disp_schCmd.GetDecimal(8);

                                        decimal bal_ = payment_ - reciept_;

                                        if (inter.ContainsKey(read_disp_schCmd.GetInt32(2)))
                                        {
                                            accountList.Add(new account(read_disp_schCmd.GetString(1), read_disp_schCmd.GetInt32(2), read_disp_schCmd.GetString(3), read_disp_schCmd.GetString(4), read_disp_schCmd.GetDecimal(7), read_disp_schCmd.GetDecimal(8), bal_, inter[read_disp_schCmd.GetInt32(2)], read_disp_schCmd.GetInt32(10)));
                                        }
                                        else
                                        {
                                            accountList.Add(new account(read_disp_schCmd.GetString(1), read_disp_schCmd.GetInt32(2), read_disp_schCmd.GetString(3), read_disp_schCmd.GetString(4), read_disp_schCmd.GetDecimal(7), read_disp_schCmd.GetDecimal(8), bal_, 0, read_disp_schCmd.GetInt32(10)));
                                        }
                                    }
                                }
                                read_disp_schCmd.Close();
                            }
                        }
                        else
                        {
                            if (inp == "" && type == "" && !p)
                            {
                                using (SQLiteDataReader sqLiteDataReader5 = new SQLiteCommand("Select * from accounts order by slno", connection).ExecuteReader())
                                {
                                    if (sqLiteDataReader5.HasRows)
                                    {
                                        while (sqLiteDataReader5.Read())
                                            accountList.Add(new account(sqLiteDataReader5.GetString(1), sqLiteDataReader5.GetInt32(2), sqLiteDataReader5.GetString(3), sqLiteDataReader5.GetString(4), sqLiteDataReader5.GetString(5), sqLiteDataReader5.GetDecimal(6), sqLiteDataReader5.GetDecimal(7), sqLiteDataReader5.GetDecimal(8), sqLiteDataReader5.GetInt32(10)));
                                    }

                                    sqLiteDataReader5.Close();
                                }
                                connection.Close();
                                return accountList;
                            }
                            else if (p)
                            {
                                using (SQLiteDataReader sqLiteDataReader5 = new SQLiteCommand("Select * from accounts order by slno", connection).ExecuteReader())
                                {
                                    if (sqLiteDataReader5.HasRows)
                                    {
                                        while (sqLiteDataReader5.Read())
                                            accountList.Add(new account(sqLiteDataReader5.GetString(0), sqLiteDataReader5.GetString(1), sqLiteDataReader5.GetInt32(2), sqLiteDataReader5.GetString(3), sqLiteDataReader5.GetString(4), sqLiteDataReader5.GetString(5), sqLiteDataReader5.GetDecimal(6), sqLiteDataReader5.GetDecimal(7), sqLiteDataReader5.GetDecimal(8), sqLiteDataReader5.GetInt32(10)));
                                    }
                                    sqLiteDataReader5.Close();
                                }
                                connection.Close();
                                return accountList;
                            }

                            SQLiteCommand sQLiteCommand = new SQLiteCommand();

                            if (inp != "" && type == "")
                                sQLiteCommand = new SQLiteCommand("Select * from accounts where slno='" + inp + "' order by slno", connection);
                            else if (type != "" && inp == "")
                                sQLiteCommand = new SQLiteCommand("Select * from accounts where type='" + type + "' order by slno;", connection);
                            else if (inp != "" && type != "")
                                sQLiteCommand = new SQLiteCommand("Select * from accounts where slno='" + inp + "' and type='" + type + "' order by slno;", connection);

                            using (SQLiteDataReader sqLiteDataReader7 = sQLiteCommand.ExecuteReader())
                            {
                                if (sqLiteDataReader7.HasRows)
                                {
                                    if (type == "Chits")
                                    {
                                        while (sqLiteDataReader7.Read())
                                            accountList.Add(new account(sqLiteDataReader7.GetString(1), sqLiteDataReader7.GetInt32(2), sqLiteDataReader7.GetString(3), sqLiteDataReader7.GetDecimal(7), sqLiteDataReader7.GetDecimal(8), sqLiteDataReader7.GetInt32(10)));
                                    }
                                    else
                                    {
                                        while (sqLiteDataReader7.Read())
                                            accountList.Add(new account(sqLiteDataReader7.GetString(0), sqLiteDataReader7.GetString(1), sqLiteDataReader7.GetInt32(2), sqLiteDataReader7.GetString(3), sqLiteDataReader7.GetString(4), sqLiteDataReader7.GetString(5), sqLiteDataReader7.GetDecimal(6), sqLiteDataReader7.GetDecimal(7), sqLiteDataReader7.GetDecimal(8), sqLiteDataReader7.GetInt32(10)));
                                    }
                                }
                                else
                                    accountList = new List<account>();
                                sqLiteDataReader7.Close();
                            }
                        }
                        break;
                }

                connection.Close();
            }
            return accountList;
        }

        public bool Del_fromDB(string inp)
        {
            if (!File.Exists(old_dbpath))
            {
                Create_Old();
            }

            decimal lineReciept = 0, linePayment = 0;
            SQLiteConnection connection1 = new SQLiteConnection("Data Source = " + db_path + ";Version=3;");
            SQLiteConnection connection2 = new SQLiteConnection("Data Source = " + old_dbpath + ";Version=3;");
            try
            {
                connection1.Open();
                SQLiteDataReader sqLiteDataReader1 = new SQLiteCommand("Select * from accounts where acc_id='" + inp + "'", connection1).ExecuteReader();
                if (sqLiteDataReader1.HasRows)
                {
                    connection2.Open();
                    while (sqLiteDataReader1.Read())
                    {
                        SQLiteCommand sqLiteCommand = new SQLiteCommand("INSERT INTO accounts(acc_id,date,slno,name,village,type,interest_rate,reciept_amt, payment_amt,last_posting_date,deleted_date, share) VALUES(@Entry,@Entry1,@Entry2,@Entry3,@Entry4,@Entry5,@Entry6,@Entry7,@Entry8,@Entry9, @Entry10, @Entry11);", connection2);
                        sqLiteCommand.Parameters.AddWithValue("@Entry", (object)sqLiteDataReader1.GetString(0));
                        sqLiteCommand.Parameters.AddWithValue("@Entry1", (object)sqLiteDataReader1.GetString(1));
                        sqLiteCommand.Parameters.AddWithValue("@Entry2", (object)sqLiteDataReader1.GetInt32(2));
                        sqLiteCommand.Parameters.AddWithValue("@Entry3", (object)sqLiteDataReader1.GetString(3));
                        sqLiteCommand.Parameters.AddWithValue("@Entry4", (object)sqLiteDataReader1.GetString(4));
                        sqLiteCommand.Parameters.AddWithValue("@Entry5", (object)sqLiteDataReader1.GetString(5));
                        sqLiteCommand.Parameters.AddWithValue("@Entry6", (object)sqLiteDataReader1.GetDecimal(6));
                        sqLiteCommand.Parameters.AddWithValue("@Entry7", (object)sqLiteDataReader1.GetDecimal(7));
                        sqLiteCommand.Parameters.AddWithValue("@Entry8", (object)sqLiteDataReader1.GetDecimal(8));
                        sqLiteCommand.Parameters.AddWithValue("@Entry9", (object)sqLiteDataReader1.GetString(9));
                        sqLiteCommand.Parameters.AddWithValue("@Entry10", DateTime.Now.ToString("dd-MM-yyyy"));
                        sqLiteCommand.Parameters.AddWithValue("@Entry11", (object)sqLiteDataReader1.GetInt32(10));

                        lineReciept = sqLiteDataReader1.GetDecimal(7);
                        linePayment = sqLiteDataReader1.GetDecimal(8);

                        try
                        {
                            sqLiteCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            sqLiteDataReader1.Close();
                            connection1.Close();
                            connection2.Close();
                            MessageBox.Show("Data Doesn't Exists" + e.ToString());
                            return false;
                        }
                    }
                    SQLiteDataReader sqLiteDataReader2 = new SQLiteCommand("Select * from records where acc_id='" + inp + "'", connection1).ExecuteReader();
                    if (sqLiteDataReader2.HasRows)
                    {
                        while (sqLiteDataReader2.Read())
                        {
                            SQLiteCommand sqLiteCommand = new SQLiteCommand("INSERT INTO records(posting_id,date,slno,name,details,reciept,payment,interest,acc_id) VALUES(@Entry,@Entry1,@Entry2,@Entry3,@Entry4,@Entry5,@Entry6,@Entry7,@Entry8);", connection2);
                            sqLiteCommand.Parameters.AddWithValue("@Entry", (object)sqLiteDataReader2.GetString(0));
                            sqLiteCommand.Parameters.AddWithValue("@Entry1", (object)sqLiteDataReader2.GetString(1));
                            sqLiteCommand.Parameters.AddWithValue("@Entry2", (object)sqLiteDataReader2.GetInt32(2));
                            sqLiteCommand.Parameters.AddWithValue("@Entry3", (object)sqLiteDataReader2.GetString(3));
                            sqLiteCommand.Parameters.AddWithValue("@Entry4", (object)sqLiteDataReader2.GetString(4));
                            sqLiteCommand.Parameters.AddWithValue("@Entry5", (object)sqLiteDataReader2.GetDecimal(5));
                            sqLiteCommand.Parameters.AddWithValue("@Entry6", (object)sqLiteDataReader2.GetDecimal(6));
                            sqLiteCommand.Parameters.AddWithValue("@Entry7", (object)sqLiteDataReader2.GetDecimal(7));
                            sqLiteCommand.Parameters.AddWithValue("@Entry8", (object)sqLiteDataReader2.GetString(8));
                            try
                            {
                                sqLiteCommand.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                sqLiteDataReader2.Close();
                                sqLiteDataReader1.Close();
                                connection1.Close();
                                connection2.Close();
                                MessageBox.Show("Data Doesn't Exists" + e.ToString());
                                return false;
                            }
                        }
                    }
                    sqLiteDataReader2.Close();
                    connection2.Close();

                    List<Decimal> line = Load_lineTotal();
                    decimal LineTotalReciept = line[0] - lineReciept;
                    decimal LineTotalPayment = line[1] - linePayment;
                    SQLiteCommand cmd = new SQLiteCommand("UPDATE lineTable set line_total_reciept=@Entry, line_total_payment=@Entry1;", connection1);
                    cmd.Parameters.AddWithValue("@Entry", LineTotalReciept);
                    cmd.Parameters.AddWithValue("@Entry1", LineTotalPayment);

                    cmd.ExecuteNonQuery();

                    SQLiteCommand sqLiteCommand2 = new SQLiteCommand("Delete from records where acc_id='" + inp + "'", connection1);
                    SQLiteCommand sqLiteCommand1 = new SQLiteCommand("Delete from accounts where acc_id='" + inp + "'", connection1);
                    
                    sqLiteCommand2.ExecuteNonQuery();
                    int num1 = sqLiteCommand1.ExecuteNonQuery();
                    
                    connection1.Close();
                    
                    if (num1 > 0)
                        return true;
                    int num2 = (int)MessageBox.Show("Can't Delete");
                    return false;
                }

                sqLiteDataReader1.Close();
                connection1.Close();
                int num3 = (int)MessageBox.Show("Slno Doesn't Exists");
                return false;
            }
            catch (Exception)
            {
                connection1.Close();
                connection2.Close();
                MessageBox.Show("Can't Delete");
                return false;
            }
        }

        public bool Update_db(List<string> inptxt)
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source = " + db_path + ";Version=3;");
            connection.Open();
            SQLiteCommand sqLiteCommand1 = new SQLiteCommand("Update accounts set date=@Entry, slno=@Entry1, name=@Entry2, village=@Entry3, type=@Entry4, interest_rate=@Entry5, share=@Entry7 where acc_id=@Entry6;", connection);
            sqLiteCommand1.Parameters.AddWithValue("@Entry", (object)inptxt[1]);
            sqLiteCommand1.Parameters.AddWithValue("@Entry1", (object)inptxt[2]);
            sqLiteCommand1.Parameters.AddWithValue("@Entry2", (object)inptxt[3]);
            sqLiteCommand1.Parameters.AddWithValue("@Entry3", (object)inptxt[4]);
            sqLiteCommand1.Parameters.AddWithValue("@Entry4", (object)inptxt[5]);
            sqLiteCommand1.Parameters.AddWithValue("@Entry5", (object)inptxt[6]);
            sqLiteCommand1.Parameters.AddWithValue("@Entry6", (object)inptxt[0]);
            sqLiteCommand1.Parameters.AddWithValue("@Entry7", (object)inptxt[7]);

            int num1 = 0;
            try
            {
                num1 = sqLiteCommand1.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                connection.Close();
                MessageBox.Show(e.ToString());
                return false;
            }

            SQLiteCommand sqLiteCommand2 = new SQLiteCommand("Update records set slno=@Entry, name=@Entry1 where acc_id=@Entry2", connection);
            sqLiteCommand2.Parameters.AddWithValue("@Entry", (object)inptxt[2]);
            sqLiteCommand2.Parameters.AddWithValue("@Entry1", (object)inptxt[3]);
            sqLiteCommand2.Parameters.AddWithValue("@Entry2", (object)inptxt[0]);
            int num2 = sqLiteCommand2.ExecuteNonQuery();
            connection.Close();
            if (num1 > 0 && num2 >= 0)
                return true;
            MessageBox.Show("Update Failed");
            return false;
        }

        public bool Update_rec(List<string> acc_update, List<records> acc_rec_update, List<Decimal> line_update)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data source = " + db_path + ";Version=3;"))
            {
                connection.Open();

                using (SQLiteCommand UpdCmd = new SQLiteCommand("Update accounts set reciept_amt=@Entry, payment_amt=@Entry1, last_posting_date=@Entry3 where acc_id=@Entry2;", connection))
                {
                    UpdCmd.Parameters.AddWithValue("@Entry", decimal.Parse(acc_update[1]));
                    UpdCmd.Parameters.AddWithValue("@Entry1", decimal.Parse(acc_update[2]));
                    UpdCmd.Parameters.AddWithValue("@Entry2", acc_update[0]);

                    string[] date_split = null;
                    foreach(records reco in acc_rec_update)
                    {
                        date_split = reco.date.Split('-');
                        break;
                    }
                    string dateTicks = new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString();

                    UpdCmd.Parameters.AddWithValue("@Entry3", dateTicks);

                    try
                    {
                        UpdCmd.ExecuteNonQuery();

                        using (SQLiteCommand sqLiteCommand2 = new SQLiteCommand("Update records set date=@Entry, details=@Entry1, reciept=@Entry2, payment=@Entry3, interest=@Entry4 where posting_id=@Entry5", connection))
                        {
                            using (List<records>.Enumerator enumerator = acc_rec_update.GetEnumerator())
                            {
                                if (enumerator.MoveNext())
                                {
                                    records current = enumerator.Current;
                                    sqLiteCommand2.Parameters.AddWithValue("@Entry", dateTicks);
                                    sqLiteCommand2.Parameters.AddWithValue("@Entry1", (object)current.details);
                                    sqLiteCommand2.Parameters.AddWithValue("@Entry2", (object)current.reciept);
                                    sqLiteCommand2.Parameters.AddWithValue("@Entry3", (object)current.payment);
                                    sqLiteCommand2.Parameters.AddWithValue("@Entry4", (object)current.interest);
                                    sqLiteCommand2.Parameters.AddWithValue("@Entry5", (object)current.posting_id);
                                }
                            }
                            try
                            {
                                sqLiteCommand2.ExecuteNonQuery();
                                using (SQLiteCommand sqLiteCommand3 = new SQLiteCommand("Update lineTable set line_total_reciept=@Entry, line_total_payment=@Entry1;", connection))
                                {
                                    sqLiteCommand3.Parameters.AddWithValue("@Entry", line_update[0]);
                                    sqLiteCommand3.Parameters.AddWithValue("@Entry1", line_update[1]);
                                    try
                                    {
                                        sqLiteCommand3.ExecuteNonQuery();
                                    }
                                    catch (Exception e)
                                    {
                                        connection.Close();
                                        MessageBox.Show("Line Update Failed" + e.ToString());
                                        return false;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                connection.Close();
                                MessageBox.Show("Record Update Failed" + e.ToString());
                                return false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        connection.Close();
                        MessageBox.Show("Update Failed " + e.ToString());
                        return false;
                    }
                }
                connection.Close();
            }
            return true;
        }

        public bool Delete_rec(string slno_b, string pos_id, Decimal reciept_b, Decimal payment_b, string date_ = "")
        {
            SQLiteConnection connection = new SQLiteConnection("Data Source = " + db_path + ";Version=3;");
            connection.Open();
            List<account> accountList = Load_acc_db(slno_b);
            Decimal num1 = new Decimal();
            Decimal num2 = new Decimal();
            foreach (var acc in accountList)
            {
                num1 = acc.reciept;
                num2 = acc.payment;
            }

            Decimal num3 = 0, num4 = 0;

            if (num1 == 0 && num2 == 0)
            {
                MessageBox.Show("Data Corrupted");
                connection.Close();
                return false;
            }
            else if (num1 == 0 && num2 != 0)
            {
                num4 = num2 - payment_b;
            }
            else if (num1 != 0 && num2 == 0)
            {
                num3 = num1 - reciept_b;
            }
            else
            {
                num3 = num1 - reciept_b;
                num4 = num2 - payment_b;
            }

            SQLiteCommand sqLiteCommand1 = new SQLiteCommand("Update accounts set reciept_amt=@Entry, payment_amt=@Entry1, last_posting_date=@Entry3 where slno=@Entry2", connection);
            sqLiteCommand1.Parameters.AddWithValue("@Entry", num3);
            sqLiteCommand1.Parameters.AddWithValue("@Entry1", num4);
            sqLiteCommand1.Parameters.AddWithValue("@Entry2", slno_b);
            sqLiteCommand1.Parameters.AddWithValue("@Entry3", Get_prev_postingDate(slno_b,date_));
            try
            {
                sqLiteCommand1.ExecuteNonQuery();
                
                List<Decimal> numList = Load_lineTotal();
                
                Decimal num5 = numList[0];
                Decimal num6 = numList[1];
                Decimal num7 = num5 - reciept_b;
                Decimal num8 = num6 - payment_b;
                
                SQLiteCommand sqLiteCommand2 = new SQLiteCommand("Update lineTable set line_total_reciept=@Entry, line_total_payment=@Entry1", connection);
                sqLiteCommand2.Parameters.AddWithValue("@Entry", (object)num7);
                sqLiteCommand2.Parameters.AddWithValue("@Entry1", (object)num8);
                
                try
                {
                    sqLiteCommand2.ExecuteNonQuery();

                    SQLiteCommand insertCmd = new SQLiteCommand("Select * from records where posting_id='" + pos_id + "'", connection);
                    SQLiteDataReader i_reader = insertCmd.ExecuteReader();

                    if (i_reader.HasRows)
                    {
                        List<string> inpdata = new List<string>();
                        while (i_reader.Read())
                        {
                            inpdata.Add("" + i_reader.GetString(0) + DateTime.Now);
                            inpdata.Add("" + i_reader.GetString(1));
                            inpdata.Add("" + i_reader.GetInt32(2));
                            inpdata.Add("" + i_reader.GetString(3));
                            inpdata.Add("" + i_reader.GetString(4));
                            inpdata.Add("" + i_reader.GetDecimal(5));
                            inpdata.Add("" + i_reader.GetDecimal(6));
                            inpdata.Add("" + i_reader.GetDecimal(7));
                            inpdata.Add("" + i_reader.GetString(8));
                            Post_ac_data(inpdata, db_: true);
                        }
                    }
                    else
                    {
                        i_reader.Close();
                        connection.Close();
                        MessageBox.Show("Data Was Changed Outside");
                        return false;
                    }
                    i_reader.Close();

                    SQLiteCommand sqLiteCommand3 = new SQLiteCommand("Delete from records where posting_id=@Entry", connection);
                    sqLiteCommand3.Parameters.AddWithValue("@Entry", (object)pos_id);
                    try
                    {
                        sqLiteCommand3.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Delete Failed" + e.ToString());
                        connection.Close();
                        return false;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Delete Failed" + e.ToString());
                    connection.Close();
                    return false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Can't Delete " + e.ToString());
                connection.Close();
                return false;
            }
            connection.Close();
            return true;
        }

        public List<Decimal> Load_lineTotal()
        {
            List<Decimal> numList = new List<Decimal>();

            using(SQLiteConnection connection = new SQLiteConnection("Data Source=" + db_path + ";Version=3;"))
            {
                connection.Open();

                using(SQLiteDataReader sqLiteDataReader = new SQLiteCommand("Select line_total_reciept, line_total_payment from lineTable", connection).ExecuteReader())
                {
                    if (sqLiteDataReader.HasRows)
                    {
                        while (sqLiteDataReader.Read())
                        {
                            numList.Add(sqLiteDataReader.GetDecimal(0));
                            numList.Add(sqLiteDataReader.GetDecimal(1));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data Corrupted");
                    }

                    sqLiteDataReader.Close();
                }
                connection.Close();
            }
            return numList;
        }

        public List<records> Load_record(string sl_inp = "", string date = "", Decimal interest_rate = 0, string name = "", bool p = false, bool partys = false, bool days_cal = false, bool old = false, string Monthbegin = "", string Monthend = "", string acc_id = "")
        {
            string conn_url = "";
            if (!old)
            {
                conn_url = "Data Source=" + db_path + ";Version=3;";
            }
            else
            {
                conn_url = "Data Source=" + old_dbpath + ";Version=3;";
            }

            List<records> recordsList = new List<records>();
            using (SQLiteConnection connection = new SQLiteConnection(conn_url))
            {
                connection.Open();
                SQLiteCommand sqLiteCommand = new SQLiteCommand();

                if (Monthbegin == "" && Monthend == "")
                {
                    if (!p)
                    {
                        if (sl_inp == "" && date != "" && name == "")
                            sqLiteCommand = new SQLiteCommand("Select * from records where date='" + date + "';", connection);
                        else if (sl_inp != "" && date != "" && name == "")
                            sqLiteCommand = new SQLiteCommand("Select * from records where slno=" + sl_inp + " and date<='" + date + "'", connection);
                        else if (name != "" && date != "" && sl_inp == "")
                            sqLiteCommand = new SQLiteCommand("Select * from records where name LIKE'%" + name + "%' and date<='" + date + "';", connection);

                    }
                    else
                    {
                        if (acc_id == "")
                        {
                            if (sl_inp == "" && name == "")
                                sqLiteCommand = new SQLiteCommand("Select * from records where date='" + date + "';", connection);
                            else if (sl_inp != "" && date != "")
                                sqLiteCommand = new SQLiteCommand("Select * from records where slno =" + sl_inp + " and date='" + date + "';", connection);
                            else if (name != "" && date != "")
                                sqLiteCommand = new SQLiteCommand("Select * from records where name =" + name + " and date='" + date + "';", connection);
                            else if (sl_inp != "" && date != "" && name != "")
                                sqLiteCommand = new SQLiteCommand("Select * from records where slno =" + sl_inp + " and date='" + date + "';", connection);
                        }
                        else
                        {
                            sqLiteCommand = new SQLiteCommand("Select * from records where date='" + date + "' and acc_id='" + acc_id + "';", connection);
                        }
                    }
                }
                else
                {
                    sqLiteCommand = new SQLiteCommand("Select * from records where slno=" + sl_inp + " and date>='" + Monthbegin + "' and date<='" + Monthend + "';", connection);
                }

                using (SQLiteDataReader sqLiteDataReader = sqLiteCommand.ExecuteReader())
                {
                    if (sqLiteDataReader.HasRows)
                    {
                        if (partys)
                        {
                            while (sqLiteDataReader.Read())
                                recordsList.Add(new records(sqLiteDataReader.GetString(1), sqLiteDataReader.GetInt32(2), sqLiteDataReader.GetString(3), sqLiteDataReader.GetString(4), sqLiteDataReader.GetDecimal(5), sqLiteDataReader.GetDecimal(6), sqLiteDataReader.GetDecimal(7)));
                        }
                        else
                        {
                            if (days_cal)
                            {
                                while (sqLiteDataReader.Read())
                                    recordsList.Add(new records(sqLiteDataReader.GetString(1), sqLiteDataReader.GetInt32(2), sqLiteDataReader.GetString(3), sqLiteDataReader.GetString(4), sqLiteDataReader.GetDecimal(5), sqLiteDataReader.GetDecimal(6), DateTime.Now.ToString("dd-MM-yyyy"), interest_rate));
                            }
                            else
                            {
                                while (sqLiteDataReader.Read())
                                    recordsList.Add(new records(sqLiteDataReader.GetString(0), sqLiteDataReader.GetString(1), sqLiteDataReader.GetInt32(2), sqLiteDataReader.GetString(3), sqLiteDataReader.GetString(4), sqLiteDataReader.GetDecimal(5), sqLiteDataReader.GetDecimal(6), sqLiteDataReader.GetDecimal(7)));
                            }
                        }
                    }
                    else
                        recordsList = (List<records>)null;

                    sqLiteDataReader.Close();
                }
                connection.Close();
            }
            return recordsList;
        }

        public List<string> Load_Partys(bool old = false, bool getacc_id = false)
        {
            string conn_url = "";
            if (!old)
            {
                conn_url = "Data Source=" + db_path + ";Version=3;";
            }
            else
            {
                conn_url = "Data Source=" + old_dbpath + ";Version=3;";
            }
            List<string> data = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection(conn_url))
            {
                con.Open();

                SQLiteCommand cmd = new SQLiteCommand();

                if (getacc_id)
                {
                    cmd = new SQLiteCommand("Select acc_id from accounts where type='Partys' order by slno", con);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.Add(reader.GetString(0));
                            }
                        }

                        reader.Close();
                    }
                }
                else
                {
                    cmd = new SQLiteCommand("Select slno from accounts where type='Partys' order by slno", con);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data.Add(reader.GetInt32(0).ToString());
                            }
                        }

                        reader.Close();
                    }
                }
                con.Close();
            }
            return data;
        }

        public bool Post_ac_data(List<string> inpData, bool db_ = false)
        {
            SQLiteConnection connection = new SQLiteConnection();
            if (db_)
            {
                connection = new SQLiteConnection("Data Source=" + old_dbpath + ";Version=3;");
            }
            else
            {
                connection = new SQLiteConnection("Data Source=" + db_path + ";Version=3;");
            }
            
            connection.Open();

            using (SQLiteCommand sqLiteCommand1 = new SQLiteCommand("INSERT INTO records(posting_id,date,slno,name,details,reciept,payment,interest,acc_id) VALUES(@Entry,@Entry1,@Entry2,@Entry3,@Entry4,@Entry5,@Entry6,@Entry7,@Entry8);", connection))
            {
                sqLiteCommand1.Parameters.AddWithValue("@Entry", (object)inpData[0]);
                sqLiteCommand1.Parameters.AddWithValue("@Entry1", (object)inpData[1]);
                sqLiteCommand1.Parameters.AddWithValue("@Entry2", (object)inpData[2]);
                sqLiteCommand1.Parameters.AddWithValue("@Entry3", (object)inpData[3]);
                sqLiteCommand1.Parameters.AddWithValue("@Entry4", (object)inpData[4]);
                sqLiteCommand1.Parameters.AddWithValue("@Entry5", (object)inpData[5]);
                sqLiteCommand1.Parameters.AddWithValue("@Entry6", (object)inpData[6]);
                sqLiteCommand1.Parameters.AddWithValue("@Entry7", (object)inpData[7]);
                sqLiteCommand1.Parameters.AddWithValue("@Entry8", (object)inpData[8]);

                try
                {
                    sqLiteCommand1.ExecuteNonQuery();

                    decimal num1 = !(inpData[5] == "") ? decimal.Parse(inpData[5]) : 0;
                    decimal num2 = !(inpData[6] == "") ? decimal.Parse(inpData[6]) : 0;
                    decimal num3 = 0;
                    decimal num4 = 0;

                    using (SQLiteCommand sqLiteCommand2 = new SQLiteCommand("Select reciept_amt, payment_amt from accounts where acc_id=@Entry", connection))
                    {
                        sqLiteCommand2.Parameters.AddWithValue("@Entry", (object)inpData[8]);
                        using(SQLiteDataReader sqLiteDataReader1 = sqLiteCommand2.ExecuteReader())
                        {
                            if (sqLiteDataReader1.HasRows)
                            {
                                while (sqLiteDataReader1.Read())
                                {
                                    num3 = sqLiteDataReader1.GetDecimal(0);
                                    num4 = sqLiteDataReader1.GetDecimal(1);
                                }
                            }
                            sqLiteDataReader1.Close();
                        }

                        decimal num5 = num3 + num1;
                        decimal num6 = num4 + num2;

                        using (SQLiteCommand sqLiteCommand3 = new SQLiteCommand("UPDATE accounts set reciept_amt=@Entry, payment_amt=@Entry1, last_posting_date=@Entry3 where acc_id=@Entry2", connection))
                        {
                            sqLiteCommand3.Parameters.AddWithValue("@Entry", (object)num5);
                            sqLiteCommand3.Parameters.AddWithValue("@Entry1", (object)num6);
                            sqLiteCommand3.Parameters.AddWithValue("@Entry2", (object)inpData[8]);
                            sqLiteCommand3.Parameters.AddWithValue("@Entry3", inpData[1]);

                            try
                            {
                                sqLiteCommand3.ExecuteNonQuery();
                                
                                decimal num7 = 0;
                                decimal num8 = 0;

                                using (SQLiteDataReader sqLiteDataReader2 = new SQLiteCommand("Select line_total_reciept,line_total_payment from lineTable", connection).ExecuteReader())
                                {
                                    if (sqLiteDataReader2.HasRows)
                                    {
                                        while (sqLiteDataReader2.Read())
                                        {
                                            num7 = sqLiteDataReader2.GetDecimal(0);
                                            num8 = sqLiteDataReader2.GetDecimal(1);
                                        }
                                    }
                                    sqLiteDataReader2.Close();
                                }

                                decimal num9 = num7 + num1;
                                decimal num10 = num8 + num2;

                                using (SQLiteCommand sqLiteCommand4 = new SQLiteCommand("UPDATE lineTable set line_total_reciept=@Entry, line_total_payment=@Entry1", connection))
                                {
                                    sqLiteCommand4.Parameters.AddWithValue("@Entry", (object)num9);
                                    sqLiteCommand4.Parameters.AddWithValue("@Entry1", (object)num10);
                                    try
                                    {
                                        sqLiteCommand4.ExecuteNonQuery();
                                    }
                                    catch (Exception)
                                    {
                                        connection.Close();
                                        int num11 = (int)MessageBox.Show("Posting Line Failed");
                                        return false;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                connection.Close();
                                int num7 = (int)MessageBox.Show("Posting Update Failed" + e.ToString());
                                return false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    connection.Close();
                    MessageBox.Show("Posting Failed" + e.ToString());
                    return false;
                }
            }
            connection.Close();
            return true;
        }

        public bool isContain_slno(string slno)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data source=" + db_path))
            {
                con.Open();
                using (SQLiteDataReader x = new SQLiteCommand("Select * from accounts where slno =" + int.Parse(slno) + ";", con).ExecuteReader())
                {

                    if (x.HasRows)
                    {
                        x.Close();
                        con.Close();
                        return true;
                    }
                    x.Close();
                }
                con.Close();
            }
            return false;
        }

        public string getAcc_id(string slno)
        {
            string acc = "";
            using (SQLiteConnection con = new SQLiteConnection("Data source=" + db_path))
            {
                con.Open();

                using (SQLiteDataReader x = new SQLiteCommand("Select acc_id from accounts where slno ='" + slno + "';", con).ExecuteReader())
                {
                    if (x.HasRows)
                    {
                        while (x.Read())
                        {
                            acc = x.GetString(0);
                        }
                    }
                    x.Close();
                }

                con.Close();
            }
            return acc;
        }

        public string Get_firstPostingdate(string slno = "", bool old = true)
        {
            string conn_url = "";

            if (!old)
            {
                conn_url = "Data Source=" + db_path + ";Version=3;";
            }
            else
            {
                conn_url = "Data Source=" + old_dbpath + ";Version=3;";
            }
            string last_date = "";
            using (SQLiteConnection con = new SQLiteConnection(conn_url))
            {
                con.Open();
                using (SQLiteCommand check_Cmd = new SQLiteCommand("Select date from records where slno='" + slno + "' order by date Limit 1", con))
                {
                    using (SQLiteDataReader reader_ = check_Cmd.ExecuteReader())
                    {
                        if (reader_.HasRows)
                        {
                            while (reader_.Read())
                            {
                                last_date = new DateTime(long.Parse(reader_.GetString(0))).ToString("dd-MM-yyyy");
                            }
                        }
                    }
                }
                con.Close();
            }

            return last_date;
        }

        public string Get_last_postingDate()
        {
            string last_date = "";
            using(SQLiteConnection con = new SQLiteConnection("Data source=" + db_path))
            {
                con.Open();
                using(SQLiteCommand check_Cmd = new SQLiteCommand("Select date from records order by date DESC Limit 1", con))
                {
                    using (SQLiteDataReader reader_ = check_Cmd.ExecuteReader())
                    {
                        if (reader_.HasRows)
                        {
                            while (reader_.Read())
                            {
                                last_date = new DateTime(long.Parse(reader_.GetString(0))).ToString("dd-MM-yyyy");
                            }
                        }
                    }
                }
                con.Close();
            }
            if (last_date == "")
                last_date = "No Posting Done";

            /*
            List<DateTime> dt = new List<DateTime>();
            using (SQLiteConnection con = new SQLiteConnection("Data source=" + db_path))
            {
                con.Open();
                using (SQLiteCommand check_Cmd = new SQLiteCommand("Select last_posting_date from accounts where last_posting_date!=''", con))
                {
                    using (SQLiteDataReader reader_ = check_Cmd.ExecuteReader())
                    {
                        if (reader_.HasRows)
                        {
                            string[] date_split;
                            while (reader_.Read())
                            {
                                date_split = reader_.GetString(0).Split('-');
                                dt.Add(new DateTime(int.Parse(date_split[2]), int.Parse(date_split[1]), int.Parse(date_split[0])));
                            }
                        }
                    }
                }
                con.Close();
            }

            DateTime max = DateTime.MinValue;
            foreach (DateTime d in dt)
            {
                if (DateTime.Compare(max, d) < 0)
                    max = d;
            }

            last_date = max.ToString("dd-MM-yyyy");

            if(last_date  == "01-01-0001")
                last_date = "No Posting Done";

            */
            return last_date;
        }

        public string Get_prev_postingDate(string slno = "", string date_ = "", bool old = false)
        {
            string prev_post_date = "";
            List<DateTime> dt = new List<DateTime>();
            string conn_url = "";

            if (!old)
            {
                conn_url = "Data Source=" + db_path + ";Version=3;";
            }
            else
            {
                conn_url = "Data Source=" + old_dbpath + ";Version=3;";
            }

            using (SQLiteConnection con = new SQLiteConnection(conn_url))
            {
                con.Open();

                if(slno != "")
                {
                    using (SQLiteCommand schCmd = new SQLiteCommand("Select date from records where slno='" + slno + "' order by date DESC Limit 2", con))
                    {
                        using (SQLiteDataReader reader_ = schCmd.ExecuteReader())
                        {
                            if (reader_.HasRows)
                            {
                                while (reader_.Read())
                                {
                                    prev_post_date = new DateTime(long.Parse(reader_.GetString(0))).ToString("dd-MM-yyyy");
                                }
                            }
                            /*
                            if (reader_.HasRows)
                            {
                                string[] date_split;
                                while (reader_.Read())
                                {
                                    if (reader_.GetString(0) != "" && reader_.GetString(0) != date_)
                                    {
                                        date_split = reader_.GetString(0).Split('-');
                                        dt.Add(new DateTime(int.Parse(date_split[2]), int.Parse(date_split[1]), int.Parse(date_split[0])));
                                    }
                                }
                            }*/
                        }
                    }
                }
                else
                {
                    using (SQLiteCommand schCmd = new SQLiteCommand("Select date from accounts", con))
                    {
                        using (SQLiteDataReader reader_ = schCmd.ExecuteReader())
                        {
                            if (reader_.HasRows)
                            {
                                while (reader_.Read())
                                {
                                    if (reader_.GetString(0) != "" && reader_.GetString(0) != date_)
                                    {
                                        prev_post_date = new DateTime(long.Parse(reader_.GetString(0))).ToString("dd-MM-yyyy");
                                    }
                                }
                            }
                        }
                    }
                }
                
                con.Close();
            }
            /*
            DateTime max = DateTime.MinValue;
            foreach (DateTime d in dt)
            {
                if (DateTime.Compare(max, d) < 0)
                    max = d;
            }

            prev_post_date = max.ToString("dd-MM-yyyy");

            if (prev_post_date == "01-01-0001")
                prev_post_date = "";
            */
            return prev_post_date;
        }
        
        public account Get_ProfitAcc()
        {
            account profitAcc = null;
            using(SQLiteConnection con = new SQLiteConnection("DataSource = " + db_path))
            {
                con.Open();
                using(SQLiteCommand proCmd = new SQLiteCommand("Select * from accounts where type='Profit'", con))
                {
                    using(SQLiteDataReader read = proCmd.ExecuteReader())
                    {
                        if (read.HasRows)
                        {
                            while (read.Read())
                            {
                                profitAcc = new account(read.GetString(0), read.GetString(1), read.GetInt32(2), read.GetString(3), read.GetString(4), read.GetString(5), read.GetDecimal(6), read.GetDecimal(7), read.GetDecimal(8), read.GetInt32(10));
                            }

                        }
                        else
                        {
                            MessageBox.Show("Profit Account is Not There");
                        }
                    }
                }
                con.Close();
            }

            return profitAcc;
        }

        public bool Insert_IntoTemp(records rec)
        {
            using (SQLiteConnection con = new SQLiteConnection("DataSource = " + db_path))
            {
                con.Open();

                using (SQLiteCommand CreateCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS post_temp(posting_id NVARCHAR(2048) NOT NULL PRIMARY KEY, date NVARCHAR(2048) NOT NULL, slno INTEGER NOT NULL, name varchar(255) NOT NULL, details NVARCHAR(255) NOT NULL DEFAULT '', reciept NUMERIC DEFAULT 0, payment NUMERIC DEFAULT 0, interest NUMERIC DEFAULT 0, acc_id NVARCHAR(2048) NOT NULL, FOREIGN KEY(acc_id) REFERENCES accounts(acc_id))", con))
                {
                    CreateCmd.ExecuteNonQuery();
                }

                using (SQLiteCommand insertCmd = new SQLiteCommand("INSERT INTO post_temp(posting_id, date, slno, name, details, reciept, payment, interest, acc_id) VALUES(@Entry, @Entry1, @Entry2, @Entry3, @Entry4, @Entry5, @Entry6, @Entry7, @Entry8)", con))
                {
                    string[] date_split = rec.date.Split('-');
                    insertCmd.Parameters.AddWithValue("@Entry", rec.posting_id);
                    insertCmd.Parameters.AddWithValue("@Entry1", new DateTime(Int32.Parse(date_split[2]), Int32.Parse(date_split[1]), Int32.Parse(date_split[0])).Ticks.ToString());
                    insertCmd.Parameters.AddWithValue("@Entry2", rec.slno);
                    insertCmd.Parameters.AddWithValue("@Entry3", rec.name);
                    insertCmd.Parameters.AddWithValue("@Entry4", rec.details);
                    insertCmd.Parameters.AddWithValue("@Entry5", rec.reciept);
                    insertCmd.Parameters.AddWithValue("@Entry6", rec.payment);
                    insertCmd.Parameters.AddWithValue("@Entry7", rec.interest);
                    insertCmd.Parameters.AddWithValue("@Entry8", rec.acc_id);

                    try
                    {
                        insertCmd.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        con.Close();
                        return false;
                    }
                }

                con.Close();
            }

            return true;
        }

        public bool Remove_Temp(string pos_id)
        {
            using (SQLiteConnection con = new SQLiteConnection("DataSource = " + db_path))
            {
                con.Open();

                using (SQLiteCommand remCmd = new SQLiteCommand("DELETE FROM post_temp where posting_id=@Entry", con))
                {
                    remCmd.Parameters.AddWithValue("@Entry", pos_id);
                    try
                    {
                        remCmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        con.Close();
                        return false;
                    }
                }

                con.Close();
            }

            return true;
        }

        public List<records> Get_Temp()
        {
            List<records> data_inp = new List<records>();
            using (SQLiteConnection con = new SQLiteConnection("DataSource = " + db_path))
            {
                con.Open();

                using (SQLiteCommand CreateCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS post_temp(posting_id NVARCHAR(2048) NOT NULL PRIMARY KEY, date NVARCHAR(2048) NOT NULL, slno INTEGER NOT NULL, name varchar(255) NOT NULL, details NVARCAHR(255) NOT NULL DEFAULT '', reciept NUMERIC DEFAULT 0, payment NUMERIC DEFAULT 0, interest NUMERIC DEFAULT 0, acc_id NVARCHAR(2048) NOT NULL, FOREIGN KEY(acc_id) REFERENCES accounts(acc_id))", con))
                {
                    CreateCmd.ExecuteNonQuery();
                }

                using (SQLiteCommand remCmd = new SQLiteCommand("SELECT * FROM post_temp", con))
                {
                    using (SQLiteDataReader sqLiteDataReader = remCmd.ExecuteReader())
                    {
                        if (sqLiteDataReader.HasRows)
                        {
                            while (sqLiteDataReader.Read())
                            {
                                try
                                {
                                    //MessageBox.Show(" " + sqLiteDataReader.GetInt32(4));
                                    data_inp.Add(new records(sqLiteDataReader.GetString(0), sqLiteDataReader.GetString(1), sqLiteDataReader.GetInt32(2), sqLiteDataReader.GetString(3), sqLiteDataReader.GetInt32(4).ToString(), sqLiteDataReader.GetDecimal(5), sqLiteDataReader.GetDecimal(6), sqLiteDataReader.GetDecimal(7), sqLiteDataReader.GetString(8)));
                                }
                                catch (Exception)
                                {
                                    //MessageBox.Show(" " + sqLiteDataReader.GetString(4));
                                    data_inp.Add(new records(sqLiteDataReader.GetString(0), sqLiteDataReader.GetString(1), sqLiteDataReader.GetInt32(2), sqLiteDataReader.GetString(3), sqLiteDataReader.GetString(4), sqLiteDataReader.GetDecimal(5), sqLiteDataReader.GetDecimal(6), sqLiteDataReader.GetDecimal(7), sqLiteDataReader.GetString(8)));
                                }
                            }
                        }
                    }
                }

                con.Close();
            }

            return data_inp;
        }

        public bool DropTable(string tableName = "post_temp")
        {
            using (SQLiteConnection con = new SQLiteConnection("DataSource = " + db_path))
            {
                con.Open();

                using (SQLiteCommand remCmd = new SQLiteCommand("DELETE FROM post_temp", con))
                {
                    //remCmd.Parameters.AddWithValue("@Entry", pos_id);
                    try
                    {
                        remCmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        con.Close();
                        return false;
                    }
                }

                con.Close();
            }

            return true;
        }

        public List<string> GetAllslno()
        {
            List<string> slnos = new List<string>();

            using (SQLiteConnection con = new SQLiteConnection("Data source=" + db_path))
            {
                con.Open();
                using (SQLiteCommand check_Cmd = new SQLiteCommand("Select slno from accounts", con))
                {
                    using (SQLiteDataReader x = check_Cmd.ExecuteReader())
                    {
                        if (x.HasRows)
                        {
                            while (x.Read())
                            {
                                slnos.Add(x.GetInt32(0).ToString());
                            }
                        }
                    }
                }
                con.Close();
            }

            return slnos;
        }

        public List<account> timeoutList(int days_count, string todays_date = "", string name = "", string village = "", string type = "Partys", bool closeList = false)
        {
            List<account> ls = new List<account>();

            using (SQLiteConnection con = new SQLiteConnection("Data source=" + db_path))
            {
                con.Open();

                SQLiteCommand check_Cmd = new SQLiteCommand();

                if (closeList)
                {
                    if (todays_date == "")
                    {
                        if (name != "" && type != "" && village == "")
                            check_Cmd = new SQLiteCommand("Select * from accounts where name='" + name + "' or slno LIKE '%"+name+"%' and type='" + type + "' and reciept_amt==payment_amt", con);
                        else if (name == "" && type != "" && village != "")
                            check_Cmd = new SQLiteCommand("Select * from accounts where type='" + type + "' and village='" + village + "' and reciept_amt==payment_amt", con);
                        else if (name != "" && type != "" && village != "")
                            check_Cmd = new SQLiteCommand("Select * from accounts where name='" + name + "' or slno LIKE '%" + name + "%' and village='" + village + "' and type='" + type + "' and reciept_amt==payment_amt", con);
                        else if (name == "" && type != "" && village == "")
                            check_Cmd = new SQLiteCommand("Select * from accounts where type='" + type + "' and reciept_amt==payment_amt", con);
                    }
                    else
                    {
                        if (name != "" && type != "" && village == "")
                            check_Cmd = new SQLiteCommand("Select * from accounts where name='" + name + "' or slno LIKE '%" + name + "%' and type='" + type + "' and reciept_amt==payment_amt and last_posting_date='" + todays_date + "'", con);
                        else if (name == "" && type != "" && village != "")
                            check_Cmd = new SQLiteCommand("Select * from accounts where type='" + type + "' and village='" + village + "' and reciept_amt==payment_amt and last_posting_date='" + todays_date + "'", con);
                        else if (name != "" && type != "" && village != "")
                            check_Cmd = new SQLiteCommand("Select * from accounts where name='" + name + "' or slno LIKE '%" + name + "%' and village='" + village + "' and type='" + type + "' and reciept_amt==payment_amt and last_posting_date='" + todays_date + "'", con);
                        else if (name == "" && type != "" && village == "")
                            check_Cmd = new SQLiteCommand("Select * from accounts where type='" + type + "' and reciept_amt==payment_amt and last_posting_date='" + todays_date + "'", con);
                    }
                }
                
                using (SQLiteDataReader reader_ = check_Cmd.ExecuteReader())
                {
                    if (reader_.HasRows)
                    {
                        while (reader_.Read())
                        {
                            if (closeList)
                            {
                                ls.Add(new account(reader_.GetString(0), reader_.GetString(1), reader_.GetInt32(2), reader_.GetString(3), reader_.GetString(4), reader_.GetString(5), reader_.GetDecimal(6), reader_.GetDecimal(7), reader_.GetDecimal(8), reader_.GetString(9), reader_.GetInt32(10)));
                            }
                        }
                    }
                }
                con.Close();
            }
            return ls;
        }

        public bool isPosted(string sl_inp, string date_today)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data source=" + db_path))
            {
                con.Open();
                using (SQLiteCommand check_Cmd = new SQLiteCommand("Select slno from records where date = '" + date_today + "'", con))
                {
                    using (SQLiteDataReader x = check_Cmd.ExecuteReader())
                    {
                        if (x.HasRows)
                        {
                            while (x.Read())
                            {
                                if (x.GetInt32(0).ToString() == sl_inp)
                                {
                                    con.Close();
                                    return true;
                                }
                            }
                        }
                    }
                }
                con.Close();
            }

            return false;
        }

        public bool isPosted_temp(string sl_inp, string date_today)
        {

            using (SQLiteConnection con = new SQLiteConnection("Data source=" + db_path))
            {
                con.Open();

                using (SQLiteCommand CreateCmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS post_temp(posting_id NVARCHAR(2048) NOT NULL PRIMARY KEY, date NVARCHAR(2048) NOT NULL,slno INTEGER NOT NULL,name varchar(255) NOT NULL,paid NUMERIC DEFAULT 0, acc_id NVARCHAR(2048) NOT NULL, FOREIGN KEY (acc_id) REFERENCES accounts(acc_id))", con))
                {
                    CreateCmd.ExecuteNonQuery();
                }

                using (SQLiteCommand check_Cmd = new SQLiteCommand("Select slno from post_temp where date = '" + date_today + "'", con))
                {
                    using (SQLiteDataReader x = check_Cmd.ExecuteReader())
                    {
                        if (x.HasRows)
                        {
                            while (x.Read())
                            {
                                if (x.GetInt32(0).ToString() == sl_inp)
                                {
                                    con.Close();
                                    return true;
                                }
                            }
                        }
                    }
                }
                con.Close();
            }

            return false;
        }
    }
}
