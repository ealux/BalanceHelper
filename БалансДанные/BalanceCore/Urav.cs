using System;
using System.Collections.Generic;
using System.Data;

namespace Balance.Rgm
{
    public class Urav
    {
        private double? value;
        private double? nev;

        public Urav() => this.Vars = new List<UravVal>();

        public Urav(int id) : this() => this.Id = id;

        public double Value
        {
            get
            {
                if (!this.value.HasValue)
                {
                    this.value = new double?(0.0);
                    foreach (UravVal var in this.Vars)
                    {
                        Urav urav = this;
                        double? nullable = urav.value;
                        double num = var.Value * (double)var.Sign;
                        urav.value = nullable.HasValue ? new double?(nullable.GetValueOrDefault() + num) : new double?();
                    }
                }
                return this.value.Value;
            }
            set
            {
                this.value = new double?(value);
            }
        }

        public double Nev
        {
            get
            {
                if (!this.nev.HasValue)
                {
                    this.nev = new double?(0.0);
                    foreach (UravVal var in this.Vars)
                    {
                        Urav urav = this;
                        double? nev = urav.nev;
                        double num = Math.Pow(var.Value * var.Pogr, 2.0);
                        urav.nev = nev.HasValue ? new double?(nev.GetValueOrDefault() + num) : new double?();
                    }
                    this.nev = new double?(Math.Sqrt(this.nev.Value));
                }
                return this.nev.Value;
            }
        }

        public List<UravVal> Vars { get; }
        public bool Collapsed { get; set; }
        public int Id { get; set; }
        public int IdBrak { get; set; }
        public int NumCollapsed { get; set; }
        public object Tag { get; set; }
        public int Brak { get; set; }
    }

    public class UravVal
    {
        private char name;
        private int ip;
        private int iq;
        private int np;
        private int sign;
        private int dost;
        private int brak;
        private DataRow dr;
        private DataTable nt;
        private DataTable pt;
        private double? value;
        private double? pogr;
        private double pribl;
        private double fix;
        internal double d1;
        internal double d2;
        internal DataRow[] rows;
        internal int col;

        public UravVal(UravVal uv)
        {
            this.name = uv.Name;
            this.ip = uv.Ip;
            this.iq = uv.Iq;
            this.np = uv.Np;
        }

        public UravVal(UravVal uv, double value)
          : this(uv)
        {
            this.value = new double?(value);
        }

        public UravVal(UravVal uv, double value, int col)
          : this(uv, value)
        {
            this.col = col;
        }

        public UravVal(char name, int ip, int sign, int dost, DataRow dr)
        {
            this.name = name;
            this.ip = ip;
            this.sign = sign;
            this.dost = dost;
            this.dr = dr;
            if (dr != null)
                return;
            this.dost = dost;
        }

        public UravVal(char name, int ip, int sign, int dost, int brak, DataRow dr)
          : this(name, ip, sign, dost, dr)
        {
            this.brak = brak;
        }

        public UravVal(char name, int ip, int iq, int np, int sign, int dost, DataRow dr)
          : this(name, ip, sign, dost, dr)
        {
            this.iq = iq;
            this.np = np;
            this.sign = sign;
        }

        public UravVal(
          char name,
          int ip,
          int iq,
          int np,
          int sign,
          int dost,
          int brak,
          DataRow dr,
          DataTable nt)
          : this(name, ip, iq, np, sign, dost, dr)
        {
            this.brak = brak;
            this.nt = nt;
        }

        public UravVal(
          char name,
          int ip,
          int iq,
          int np,
          int sign,
          int dost,
          DataRow dr,
          DataTable nt,
          DataTable pt)
          : this(name, ip, iq, np, sign, dost, dr)
        {
            this.nt = nt;
            this.pt = pt;
        }

        public int Dost
        {
            get
            {
                return this.dost;
            }
            set
            {
                this.dost = value;
            }
        }

        public int Sign
        {
            get
            {
                return this.sign;
            }
            set
            {
                this.sign = value;
            }
        }

        public char Name
        {
            get
            {
                return this.name;
            }
        }

        public int Ip
        {
            get
            {
                return this.ip;
            }
        }

        public int Iq
        {
            get
            {
                return this.iq;
            }
        }

        public int Np
        {
            get
            {
                return this.np;
            }
        }

        public DataRow Dr
        {
            get
            {
                return this.dr;
            }
        }

        public int Brak
        {
            get
            {
                return this.brak;
            }
            set
            {
                this.brak = value;
            }
        }

        public double Value
        {
            get
            {
                if (!this.value.HasValue || double.IsNaN(this.value.Value))
                {
                    this.value = new double?(0.0);
                    if (this.name == 'p' || this.name == 'q')
                    {
                        double pogr = this.Pogr;
                        if (this.iq == 0)
                        {
                            if (this.name == 'p')
                            {
                                double num1 = (this.dr["pizmp"] != DBNull.Value ? (double)this.dr["pizmp"] : 0.0) - (this.dr["pizmo"] != DBNull.Value ? (double)this.dr["pizmo"] : 0.0);
                                this.dr["pmainves"] = num1 != 0.0 ? (object)(1.0 / ((double)this.dr["ppogfull"] * (double)this.dr["ppogfull"]) / (num1 * num1)) : (object)DBNull.Value;
                                double num2 = (this.dr["pizmpd"] != DBNull.Value ? (double)this.dr["pizmpd"] : 0.0) - (this.dr["pizmod"] != DBNull.Value ? (double)this.dr["pizmod"] : 0.0);
                                this.dr["pmainvesd"] = num2 != 0.0 ? (object)(1.0 / ((double)this.dr["ppogfulld"] * (double)this.dr["ppogfulld"]) / (num2 * num2)) : (object)DBNull.Value;
                                this.dr["_p"] = (object)(this.value = new double?((num1 * (double)this.dr["ppogfull"] + num2 * (double)this.dr["ppogfulld"]) / ((double)this.dr["ppogfull"] + (double)this.dr["ppogfulld"])));
                                if (this.dr["pizmp"] != DBNull.Value || this.dr["pizmo"] != DBNull.Value)
                                    this.dr["psred"] = (object)this.value;
                            }
                            else
                            {
                                double num1 = (this.dr["qizmp"] != DBNull.Value ? (double)this.dr["qizmp"] : 0.0) - (this.dr["qizmo"] != DBNull.Value ? (double)this.dr["qizmo"] : 0.0);
                                this.dr["qmainves"] = num1 != 0.0 ? (object)(1.0 / ((double)this.dr["qpogfull"] * (double)this.dr["qpogfull"]) / (num1 * num1)) : (object)DBNull.Value;
                                double num2 = (this.dr["qizmpd"] != DBNull.Value ? (double)this.dr["qizmpd"] : 0.0) - (this.dr["qizmod"] != DBNull.Value ? (double)this.dr["qizmod"] : 0.0);
                                this.dr["qmainvesd"] = num2 != 0.0 ? (object)(1.0 / ((double)this.dr["qpogfulld"] * (double)this.dr["qpogfulld"]) / (num2 * num2)) : (object)DBNull.Value;
                                this.dr["_q"] = this.dr["qsred"] = (object)(this.value = new double?((num1 * (double)this.dr["qpogfull"] + num2 * (double)this.dr["qpogfulld"]) / ((double)this.dr["qpogfull"] + (double)this.dr["qpogfulld"])));
                                if (this.dr["qizmp"] != DBNull.Value || this.dr["qizmo"] != DBNull.Value)
                                    this.dr["qsred"] = (object)this.value;
                            }
                        }
                        else if (this.name == 'p')
                        {
                            if (this.dr["ip"].Equals((object)this.ip))
                            {
                                double num1 = (this.dr["ippizmp"] != DBNull.Value ? (double)this.dr["ippizmp"] : 0.0) - (this.dr["ippizmo"] != DBNull.Value ? (double)this.dr["ippizmo"] : 0.0);
                                this.dr["ippmainves"] = num1 != 0.0 ? (object)(1.0 / ((double)this.dr["ipppogfull"] * (double)this.dr["ipppogfull"]) / (num1 * num1)) : (object)DBNull.Value;
                                double num2 = (this.dr["ippizmpd"] != DBNull.Value ? (double)this.dr["ippizmpd"] : 0.0) - (this.dr["ippizmod"] != DBNull.Value ? (double)this.dr["ippizmod"] : 0.0);
                                this.dr["ippmainvesd"] = num2 != 0.0 ? (object)(1.0 / ((double)this.dr["ipppogfulld"] * (double)this.dr["ipppogfulld"]) / (num2 * num2)) : (object)DBNull.Value;
                                this.dr["_ipp"] = (double)this.dr["ipppogfull"] + (double)this.dr["ipppogfulld"] == 0.0 ? (this.dr["ippizmp"] != DBNull.Value || this.dr["ippizmo"] != DBNull.Value ? (this.dr["ippsred"] = (object)(this.value = new double?(num1))) : (object)(this.value = new double?(num1))) : (this.dr["ippsred"] = (object)(this.value = new double?((num1 * (double)this.dr["ipppogfull"] + num2 * (double)this.dr["ipppogfulld"]) / ((double)this.dr["ipppogfull"] + (double)this.dr["ipppogfulld"]))));
                            }
                            else
                            {
                                double num1 = (this.dr["iqpizmp"] != DBNull.Value ? (double)this.dr["iqpizmp"] : 0.0) - (this.dr["iqpizmo"] != DBNull.Value ? (double)this.dr["iqpizmo"] : 0.0);
                                this.dr["iqpmainves"] = num1 != 0.0 ? (object)(1.0 / ((double)this.dr["iqppogfull"] * (double)this.dr["iqppogfull"]) / (num1 * num1)) : (object)DBNull.Value;
                                double num2 = (this.dr["iqpizmpd"] != DBNull.Value ? (double)this.dr["iqpizmpd"] : 0.0) - (this.dr["iqpizmod"] != DBNull.Value ? (double)this.dr["iqpizmod"] : 0.0);
                                this.dr["iqpmainvesd"] = num2 != 0.0 ? (object)(1.0 / ((double)this.dr["iqppogfulld"] * (double)this.dr["iqppogfulld"]) / (num2 * num2)) : (object)DBNull.Value;
                                this.dr["_iqp"] = (double)this.dr["iqppogfull"] + (double)this.dr["iqppogfulld"] == 0.0 ? (this.dr["iqpizmp"] != DBNull.Value || this.dr["iqpizmo"] != DBNull.Value ? (this.dr["iqpsred"] = (object)(this.value = new double?(num1))) : (object)(this.value = new double?(num1))) : (this.dr["iqpsred"] = (object)(this.value = new double?((num1 * (double)this.dr["iqppogfull"] + num2 * (double)this.dr["iqppogfulld"]) / ((double)this.dr["iqppogfull"] + (double)this.dr["iqppogfulld"]))));
                            }
                        }
                        else if (this.dr["ip"].Equals((object)this.ip))
                        {
                            double num1 = (this.dr["ipqizmp"] != DBNull.Value ? (double)this.dr["ipqizmp"] : 0.0) - (this.dr["ipqizmo"] != DBNull.Value ? (double)this.dr["ipqizmo"] : 0.0);
                            this.dr["ipqmainves"] = num1 != 0.0 ? (object)(1.0 / ((double)this.dr["ipqpogfull"] * (double)this.dr["ipqpogfull"]) / (num1 * num1)) : (object)DBNull.Value;
                            double num2 = (this.dr["ipqizmpd"] != DBNull.Value ? (double)this.dr["ipqizmpd"] : 0.0) - (this.dr["ipqizmod"] != DBNull.Value ? (double)this.dr["ipqizmod"] : 0.0);
                            this.dr["ipqmainvesd"] = num2 != 0.0 ? (object)(1.0 / ((double)this.dr["ipqpogfulld"] * (double)this.dr["ipqpogfulld"]) / (num2 * num2)) : (object)DBNull.Value;
                            this.dr["_ipq"] = (double)this.dr["ipqpogfull"] + (double)this.dr["ipqpogfulld"] == 0.0 ? (this.dr["ipqizmp"] != DBNull.Value || this.dr["ipqizmo"] != DBNull.Value ? (this.dr["ipqsred"] = (object)(this.value = new double?(num1))) : (object)(this.value = new double?(num1))) : (this.dr["ipqsred"] = (object)(this.value = new double?((num1 * (double)this.dr["ipqpogfull"] + num2 * (double)this.dr["ipqpogfulld"]) / ((double)this.dr["ipqpogfull"] + (double)this.dr["ipqpogfulld"]))));
                        }
                        else
                        {
                            double num1 = (this.dr["iqqizmp"] != DBNull.Value ? (double)this.dr["iqqizmp"] : 0.0) - (this.dr["iqqizmo"] != DBNull.Value ? (double)this.dr["iqqizmo"] : 0.0);
                            this.dr["iqqmainves"] = num1 != 0.0 ? (object)(1.0 / ((double)this.dr["iqqpogfull"] * (double)this.dr["iqqpogfull"]) / (num1 * num1)) : (object)DBNull.Value;
                            double num2 = (this.dr["iqqizmpd"] != DBNull.Value ? (double)this.dr["iqqizmpd"] : 0.0) - (this.dr["iqqizmod"] != DBNull.Value ? (double)this.dr["iqqizmod"] : 0.0);
                            this.dr["iqqmainvesd"] = num2 != 0.0 ? (object)(1.0 / ((double)this.dr["iqqpogfulld"] * (double)this.dr["iqqpogfulld"]) / (num2 * num2)) : (object)DBNull.Value;
                            this.dr["_iqq"] = (double)this.dr["iqqpogfull"] + (double)this.dr["iqqpogfulld"] == 0.0 ? (this.dr["iqqizmp"] != DBNull.Value || this.dr["iqqizmo"] != DBNull.Value ? (this.dr["iqqsred"] = (object)(this.value = new double?(num1))) : (object)(this.value = new double?(num1))) : (this.dr["iqqsred"] = (object)(this.value = new double?((num1 * (double)this.dr["iqqpogfull"] + num2 * (double)this.dr["iqqpogfulld"]) / ((double)this.dr["iqqpogfull"] + (double)this.dr["iqqpogfulld"]))));
                        }
                        if (StaticRgm.wu == 1)
                        {
                            UravVal uravVal = this;
                            double? nullable = uravVal.value;
                            uravVal.value = nullable.HasValue ? new double?(nullable.GetValueOrDefault() / 1000.0) : new double?();
                        }
                    }
                    else if (this.name == 'P' || this.name == 'Q')
                        this.value = new double?(0.0);
                    else if (this.name == 'G' || this.name == 'B')
                    {
                        if (this.name == 'G' && this.dr["g"] != DBNull.Value)
                            this.value = new double?((double)this.dr["g"] * (double)this.dr["uhom"] * (double)this.dr["uhom"] / 1000000.0);
                        else if (this.name == 'B' && this.dr["b"] != DBNull.Value)
                            this.value = new double?((double)this.dr["b"] * (double)this.dr["uhom"] * (double)this.dr["uhom"] / 1000000.0);
                    }
                    else if (this.name == 'g' || this.name == 'b')
                    {
                        double num1 = 0.0;
                        foreach (DataRow row in (InternalDataCollectionBase)this.nt.Rows)
                        {
                            if (row["ny"].Equals((object)this.ip) || row["ny"].Equals((object)this.iq))
                                num1 += (double)row["uhom"];
                        }
                        double num2 = num1 / 2.0;
                        if (this.name == 'g' && this.dr["g"] != DBNull.Value)
                            this.value = new double?((double)this.dr["g"] * num2 * num2 / 1000000.0);
                        else if (this.name == 'b' && this.dr["b"] != DBNull.Value)
                            this.value = new double?((double)this.dr["b"] * num2 * num2 / 1000000.0);
                    }
                    else if (this.name == 'a' || this.name == 'r')
                        this.value = this.name != 'a' ? new double?(this.dr["ckoi"].Equals((object)DBNull.Value) ? 0.0 : (double)this.dr["ckoi"]) : new double?(this.dr["ckor"].Equals((object)DBNull.Value) ? 0.0 : (double)this.dr["ckor"]);
                    else if (this.name == 'u')
                        this.value = new double?((double)this.dr["uizm"] / (double)this.dr["_kt"]);
                }
                return this.value.Value;
            }
            set
            {
                this.value = new double?(value);
            }
        }

        public double Pogr
        {
            get
            {
                if ((!this.pogr.HasValue || double.IsNaN(this.pogr.Value)) && this.dr != null)
                {
                    this.pogr = new double?(0.0);
                    if (this.name == 'p' || this.name == 'q' || this.name == 'u')
                    {
                        if (this.iq == 0)
                        {
                            if (this.name == 'p')
                            {
                                DataRow dr1 = this.dr;
                                double num1 = Math.Sqrt((this.dr["ppogtt"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogtt"], 2.0)) + (this.dr["ppogtn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogtn"], 2.0)) + (this.dr["ppogstn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogstn"], 2.0)) + (this.dr["ppogprib"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogprib"], 2.0)) + (this.dr["ppogdop"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogdop"], 2.0)));
                                double num2;
                                var local1 = (System.ValueType)(num2 = 1.1 * num1);
                                dr1["ppogfull"] = (object)local1;
                                DataRow dr2 = this.dr;
                                double num3 = Math.Sqrt((this.dr["ppogttd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogttd"], 2.0)) + (this.dr["ppogtnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogtnd"], 2.0)) + (this.dr["ppogstnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogstnd"], 2.0)) + (this.dr["ppogpribd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogpribd"], 2.0)) + (this.dr["ppogdopd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ppogdopd"], 2.0)));
                                double num4;
                                var local2 = (System.ValueType)(num4 = 1.1 * num3);
                                dr2["ppogfulld"] = (object)local2;
                                this.pogr = new double?(num2 != 0.0 ? 1.0 / (num2 * num2) : (0.0 + num4 != 0.0 ? 1.0 / (num4 * num4) : 0.0));
                            }
                            else if (this.name == 'q')
                            {
                                DataRow dr1 = this.dr;
                                double num1 = Math.Sqrt((this.dr["qpogtt"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogtt"], 2.0)) + (this.dr["qpogtn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogtn"], 2.0)) + (this.dr["qpogstn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogstn"], 2.0)) + (this.dr["qpogprib"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogprib"], 2.0)) + (this.dr["qpogdop"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogdop"], 2.0)));
                                double num2;
                                var local1 = (System.ValueType)(num2 = 1.1 * num1);
                                dr1["qpogfull"] = (object)local1;
                                DataRow dr2 = this.dr;
                                double num3 = Math.Sqrt((this.dr["qpogttd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogttd"], 2.0)) + (this.dr["qpogtnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogtnd"], 2.0)) + (this.dr["qpogstnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogstnd"], 2.0)) + (this.dr["qpogpribd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogpribd"], 2.0)) + (this.dr["qpogdopd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["qpogdopd"], 2.0)));
                                double num4;
                                var local2 = (System.ValueType)(num4 = 1.1 * num3);
                                dr2["qpogfulld"] = (object)local2;
                                this.pogr = new double?(num2 != 0.0 ? 1.0 / (num2 * num2) : (0.0 + num4 != 0.0 ? 1.0 / (num4 * num4) : 0.0));
                            }
                            else if (this.name == 'u')
                            {
                                DataRow dr = this.dr;
                                double num1 = this.dr["pogtn"].Equals((object)DBNull.Value) ? 0.0 : (double)this.dr["pogtn"];
                                double num2;
                                var local = (System.ValueType)(num2 = 1.1 * num1);
                                dr["pogtnfull"] = (object)local;
                                this.pogr = new double?(num2 != 0.0 ? 1.0 / (num2 * num2) : 0.0);
                            }
                        }
                        else if (this.name == 'p')
                        {
                            if (this.dr["ip"].Equals((object)this.ip))
                            {
                                DataRow dr1 = this.dr;
                                double num1 = Math.Sqrt((this.dr["ipppogtt"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogtt"], 2.0)) + (this.dr["ipppogtn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogtn"], 2.0)) + (this.dr["ipppogstn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogstn"], 2.0)) + (this.dr["ipppogprib"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogprib"], 2.0)) + (this.dr["ipppogdop"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogdop"], 2.0)));
                                double num2;
                                var local1 = (System.ValueType)(num2 = 1.1 * num1);
                                dr1["ipppogfull"] = (object)local1;
                                DataRow dr2 = this.dr;
                                double num3 = Math.Sqrt((this.dr["ipppogttd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogttd"], 2.0)) + (this.dr["ipppogtnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogtnd"], 2.0)) + (this.dr["ipppogstnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogstnd"], 2.0)) + (this.dr["ipppogpribd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogpribd"], 2.0)) + (this.dr["ipppogdopd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipppogdopd"], 2.0)));
                                double num4;
                                var local2 = (System.ValueType)(num4 = 1.1 * num3);
                                dr2["ipppogfulld"] = (object)local2;
                                this.pogr = new double?(num2 != 0.0 ? 1.0 / (num2 * num2) : (0.0 + num4 != 0.0 ? 1.0 / (num4 * num4) : 0.0));
                            }
                            else
                            {
                                DataRow dr1 = this.dr;
                                double num1 = Math.Sqrt((this.dr["iqppogtt"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogtt"], 2.0)) + (this.dr["iqppogtn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogtn"], 2.0)) + (this.dr["iqppogstn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogstn"], 2.0)) + (this.dr["iqppogprib"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogprib"], 2.0)) + (this.dr["iqppogdop"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogdop"], 2.0)));
                                double num2;
                                var local1 = (System.ValueType)(num2 = 1.1 * num1);
                                dr1["iqppogfull"] = (object)local1;
                                DataRow dr2 = this.dr;
                                double num3 = Math.Sqrt((this.dr["iqppogttd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogttd"], 2.0)) + (this.dr["iqppogtnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogtnd"], 2.0)) + (this.dr["iqppogstnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogstnd"], 2.0)) + (this.dr["iqppogpribd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogpribd"], 2.0)) + (this.dr["iqppogdopd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqppogdopd"], 2.0)));
                                double num4;
                                var local2 = (System.ValueType)(num4 = 1.1 * num3);
                                dr2["iqppogfulld"] = (object)local2;
                                this.pogr = new double?(num2 != 0.0 ? 1.0 / (num2 * num2) : (0.0 + num4 != 0.0 ? 1.0 / (num4 * num4) : 0.0));
                            }
                        }
                        else if (this.dr["ip"].Equals((object)this.ip))
                        {
                            DataRow dr1 = this.dr;
                            double num1 = Math.Sqrt((this.dr["ipqpogtt"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogtt"], 2.0)) + (this.dr["ipqpogtn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogtn"], 2.0)) + (this.dr["ipqpogstn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogstn"], 2.0)) + (this.dr["ipqpogprib"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogprib"], 2.0)) + (this.dr["ipqpogdop"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogdop"], 2.0)));
                            double num2;
                            var local1 = (System.ValueType)(num2 = 1.1 * num1);
                            dr1["ipqpogfull"] = (object)local1;
                            DataRow dr2 = this.dr;
                            double num3 = Math.Sqrt((this.dr["ipqpogttd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogttd"], 2.0)) + (this.dr["ipqpogtnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogtnd"], 2.0)) + (this.dr["ipqpogstnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogstnd"], 2.0)) + (this.dr["ipqpogpribd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogpribd"], 2.0)) + (this.dr["ipqpogdopd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["ipqpogdopd"], 2.0)));
                            double num4;
                            var local2 = (System.ValueType)(num4 = 1.1 * num3);
                            dr2["ipqpogfulld"] = (object)local2;
                            this.pogr = new double?(num2 != 0.0 ? 1.0 / (num2 * num2) : (0.0 + num4 != 0.0 ? 1.0 / (num4 * num4) : 0.0));
                        }
                        else
                        {
                            DataRow dr1 = this.dr;
                            double num1 = Math.Sqrt((this.dr["iqqpogtt"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogtt"], 2.0)) + (this.dr["iqqpogtn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogtn"], 2.0)) + (this.dr["iqqpogstn"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogstn"], 2.0)) + (this.dr["iqqpogprib"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogprib"], 2.0)) + (this.dr["iqqpogdop"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogdop"], 2.0)));
                            double num2;
                            var local1 = (System.ValueType)(num2 = 1.1 * num1);
                            dr1["iqqpogfull"] = (object)local1;
                            DataRow dr2 = this.dr;
                            double num3 = Math.Sqrt((this.dr["iqqpogttd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogttd"], 2.0)) + (this.dr["iqqpogtnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogtnd"], 2.0)) + (this.dr["iqqpogstnd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogstnd"], 2.0)) + (this.dr["iqqpogpribd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogpribd"], 2.0)) + (this.dr["iqqpogdopd"].Equals((object)DBNull.Value) ? 0.0 : Math.Pow((double)this.dr["iqqpogdopd"], 2.0)));
                            double num4;
                            var local2 = (System.ValueType)(num4 = 1.1 * num3);
                            dr2["iqqpogfulld"] = (object)local2;
                            this.pogr = new double?(num2 != 0.0 ? 1.0 / (num2 * num2) : (0.0 + num4 != 0.0 ? 1.0 / (num4 * num4) : 0.0));
                        }
                        double? pogr = this.pogr;
                        this.pogr = (pogr.GetValueOrDefault() != 0.0 ? 1 : (!pogr.HasValue ? 1 : 0)) != 0 ? this.pogr : new double?(2.0);
                    }
                    else if (this.name == 'P' || this.name == 'Q')
                    {
                        if (this.pt != null)
                            this.pogr = this.name != 'P' ? new double?((double)(int)this.pt.Rows[0]["px"]) : new double?((double)(int)this.pt.Rows[0]["pr"]);
                    }
                    else if (this.name == 'G' || this.name == 'B')
                    {
                        if (this.iq == 0)
                            this.pogr = new double?(1.0 / (double)this.dr["uhom"] * (double)this.dr["uhom"]);
                    }
                    else if (this.name != 'g')
                    {
                        int name = (int)this.name;
                    }
                }
                return !this.pogr.HasValue ? 2.0 : this.pogr.Value;
            }
        }

        public double Pribl
        {
            get
            {
                return this.pribl;
            }
            set
            {
                this.pribl = value;
            }
        }

        public double Fix
        {
            get
            {
                return this.fix;
            }
            set
            {
                this.fix = value;
            }
        }

        public void ClearValAndPogr()
        {
            this.pogr = new double?();
            this.value = new double?();
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, (object)null))
                return false;
            if (object.ReferenceEquals((object)this, obj))
                return true;
            return obj is UravVal && this.Equals((UravVal)obj);
        }

        public bool Equals(UravVal obj)
        {
            return this.name.Equals(obj.name) && this.ip.Equals(obj.ip) && this.iq.Equals(obj.iq) && this.np.Equals(obj.np);
        }

        public override string ToString()
        {
            return this.iq != 0 ? "ветвь " + (object)this.ip + " - " + (object)this.iq + " (" + (object)this.np + ") [" + (object)this.name + "]" : "узел " + (object)this.ip + " [" + (object)this.name + "]";
        }

        public double VetvNachPrib
        {
            get
            {
                if (this.iq != 0)
                {
                    if (this.name == 'p')
                        return (this.dr["_ipp"] != DBNull.Value ? (double)this.dr["_ipp"] : 0.0) - (this.dr["_iqp"] != DBNull.Value ? (double)this.dr["_iqp"] : 0.0);
                    if (this.name == 'q' && this.dr["ip"].Equals((object)this.ip))
                    {
                        if (this.dr["_ipq"] != DBNull.Value && this.dr["_iqq"] != DBNull.Value && (double)this.dr["_ipq"] == 0.0)
                            return -(double)this.dr["_iqq"];
                        if (this.dr["_ipq"] != DBNull.Value && this.dr["_iqq"] != DBNull.Value && (double)this.dr["_iqq"] == 0.0)
                            return -(double)this.dr["_ipq"];
                    }
                }
                return 0.0;
            }
        }
    }
}