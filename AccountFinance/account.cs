using System;

namespace AccountFinance
{
    public class account
    {
        public int slno { get; set; }

        public string name { get; set; }

        public string village { get; set; }

        public string date { get; set; }

        public string type { get; set; }

        public Decimal interest { get; set; }

        public string acc_id { get; set; }

        public Decimal reciept { get; set; }

        public Decimal payment { get; set; }

        public Decimal balance { get; set; }

        public Decimal bal_pos { get; set; }

        public Decimal bal_neg { get; set; }

        public string last_posting_date { get; set; }

        public account(string acc_id, string date, int slno, string name, string village, string type, Decimal interest)
        {
            this.acc_id = acc_id;
            this.slno = slno;
            this.name = name;
            this.date = date;
            this.village = village;
            this.type = type;
            this.interest = interest;
        }

        public account(string acc_id, string date, int slno, string name, string village, string type, Decimal interest, Decimal reciept, Decimal payment)
        {
            this.acc_id = acc_id;
            this.slno = slno;
            this.name = name;
            this.date = date;
            this.village = village;
            this.type = type;
            this.interest = interest;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = reciept - payment;
        }

        public account(string acc_id, string date, int slno, string name, string village, string type, Decimal interest, Decimal reciept, Decimal payment, string lastDate)
        {
            this.acc_id = acc_id;
            this.slno = slno;
            this.name = name;
            this.date = date;
            this.village = village;
            this.type = type;
            this.interest = interest;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = reciept - payment;
            this.last_posting_date = lastDate;
        }

        public account(string date, int slno, string name, string village, string type, Decimal interest, Decimal reciept, Decimal payment)
        {
            this.slno = slno;
            this.name = name;
            this.date = date;
            this.village = village;
            this.type = type;
            this.interest = interest;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = reciept - payment;
        }

        public account(string date, int slno, string name, string village, decimal reciept, decimal payment, decimal balance, decimal interest)
        {
            this.date = date;
            this.slno = slno;
            this.name = name;
            this.village = village;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = Math.Round(balance, 2);
            this.interest = interest;
        }
        public account(string date, int slno, string name, Decimal reciept, Decimal payment)
        {
            this.slno = slno;
            this.name = name;
            this.date = date;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = reciept - payment;
        }

        public account(Decimal reciept, string name, Decimal payment)
        {
            this.name = name;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = reciept - payment;
            if (this.balance >= Decimal.Zero)
                this.bal_pos = Math.Abs(this.balance);
            else
                this.bal_neg = Math.Abs(this.balance);
        }

        public account(string name, Decimal int_bal)
        {
            this.name = name;
            this.bal_neg = Decimal.Zero;
            this.bal_pos = Decimal.Zero;
            if (int_bal >= Decimal.Zero)
                this.bal_pos = int_bal;
            else
                this.bal_neg = Math.Abs(int_bal);
        }

        public account(int slno, Decimal reciept, Decimal payment)
        {
            this.slno = slno;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
        }
    }
}
