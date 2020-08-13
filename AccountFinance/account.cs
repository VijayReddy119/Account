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

        public int share { get; set; }

        public account(string acc_id,string date,int slno,string name,string village,string type,Decimal interest,int share)
        {
            this.acc_id = acc_id;
            this.slno = slno;
            this.name = name;
            this.date = date;
            this.village = village;
            this.type = type;
            this.interest = interest;
            this.share = share;
        }

        public account(string acc_id,string date,int slno,string name,string village,string type,Decimal interest,Decimal reciept,Decimal payment,int share)
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
            this.share = share;
        }

        public account(string acc_id,string date,int slno,string name,string village,string type,Decimal interest,Decimal reciept,Decimal payment,string lastDate,int share)
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
            this.share = share;
        }

        public account(string date,int slno,string name,string village,string type,Decimal interest,Decimal reciept,Decimal payment,int share)
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
            this.share = share;
        }

        public account(string date,int slno,string name,string village,Decimal reciept,Decimal payment,Decimal balance,Decimal interest,int share)
        {
            this.date = date;
            this.slno = slno;
            this.name = name;
            this.village = village;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = Math.Round(balance, 2);
            this.interest = interest;
            this.share = share;
        }

        public account(string date,int slno,string name,Decimal reciept,Decimal payment,int share)
        {
            this.slno = slno;
            this.name = name;
            this.date = date;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = reciept - payment;
            this.share = share;
        }

        public account(Decimal reciept,string name,Decimal payment,int slno,int share,string type)
        {
            this.name = name;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
            this.balance = reciept - payment;
            if (this.balance >= Decimal.Zero)
                this.bal_pos = Math.Abs(this.balance);
            else
                this.bal_neg = Math.Abs(this.balance);
            this.slno = slno;
            this.share = share;
            this.type = type;
        }

        public account(string name, Decimal int_bal, int slno)
        {
            this.name = name;
            this.bal_neg = Decimal.Zero;
            this.bal_pos = Decimal.Zero;
            int_bal = Math.Round(int_bal);
            if (int_bal >= Decimal.Zero)
                this.bal_pos = int_bal;
            else
                this.bal_neg = Math.Abs(int_bal);
            this.slno = slno;
        }

        public account(int slno, Decimal reciept, Decimal payment)
        {
            this.slno = slno;
            this.reciept = Math.Round(reciept, 2);
            this.payment = Math.Round(payment, 2);
        }
    }
}
