using System;
using System.Windows.Forms;

namespace AccountFinance
{
    public class records
    {
        public string acc_id { get; set; }

        public string posting_id { get; set; }

        public string date { get; set; }

        public int slno { get; set; }

        public string name { get; set; }

        public string details { get; set; }

        public Decimal reciept { get; set; }

        public Decimal payment { get; set; }

        public Decimal interest { get; set; }

        public Decimal reciept_int { get; set; }

        public Decimal payment_int { get; set; }

        public int days { get; set; }

        public records(string posting_id, string date, int slno, string name, string details, Decimal reciept, Decimal payment, Decimal interest)
        {
            this.posting_id = posting_id;
            this.date = new DateTime(long.Parse(date)).ToString("dd-MM-yyyy");
            this.slno = slno;
            this.name = name;
            this.details = details;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.interest = interest;
        }
        public records(string posting_id, string date, int slno, string name, string details, Decimal reciept, Decimal payment, Decimal interest, string acc_id)
        {
            this.posting_id = posting_id;
            this.date = new DateTime(long.Parse(date)).ToString("dd-MM-yyyy");
            this.slno = slno;
            this.name = name;
            this.details = details;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.interest = interest;
            this.acc_id = acc_id;
        }

        public records(string date, int slno, string name, string details, Decimal reciept, Decimal payment, string date_new_x, Decimal interest_rate)
        {
            this.date = new DateTime(long.Parse(date)).ToString("dd-MM-yyyy");
            this.slno = slno;
            this.name = name;
            this.details = details;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);

            string[] strArray1 = this.date.Split('-');
            string[] strArray2 = date_new_x.Split('-');

            int year = Int32.Parse(strArray2[2]) - Int32.Parse(strArray1[2]);
            int month = Int32.Parse(strArray2[1]) - Int32.Parse(strArray1[1]);
            int days_compu = Int32.Parse(strArray2[0]) - Int32.Parse(strArray1[0]);

            this.days = (year * 12 * 30) + (month * 30) + (days_compu);
            
            /*
            DateTime dateTime = new DateTime(int.Parse(strArray1[2]), int.Parse(strArray1[1]), int.Parse(strArray1[0]));
            DateTime newdate = new DateTime(int.Parse(strArray2[2]), int.Parse(strArray2[1]), int.Parse(strArray2[0]));
            this.days = (new DateTime(int.Parse(strArray2[2]), int.Parse(strArray2[1]), int.Parse(strArray2[0])) - dateTime).Days + 1;
            */

            this.reciept_int = Math.Round(reciept * (Decimal)this.days * interest_rate / new Decimal(3000), 2);
            this.payment_int = Math.Round(payment * (Decimal)this.days * interest_rate / new Decimal(3000), 2);
        }

        public records(string date, int slno, string name, string details, Decimal reciept, Decimal payment, Decimal interest_rate)
        {
            this.date = new DateTime(long.Parse(date)).ToString("dd-MM-yyyy");
            this.slno = slno;
            this.name = name;
            this.details = details;
            this.reciept = reciept;
            this.payment = payment;
            this.interest = interest_rate;
        }

        public records(string posting_id, string date)
        {
            this.posting_id = posting_id;
            this.date = date;
        }
    }
}
