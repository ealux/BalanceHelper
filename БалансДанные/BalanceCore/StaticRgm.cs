// Decompiled with JetBrains decompiler
// Type: Balance.Rgm.StaticRgm
// Assembly: Rgm, Version=1.0.6136.17908, Culture=neutral, PublicKeyToken=null
// MVID: 526B4102-62C1-4880-A947-0A573489DF22
// Assembly location: D:\Program Files (x86)\Balance4\plugins\Rgm.dll

using Balance.MathBal;
using BalanceCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Balance.Rgm
{
    internal class StaticRgm
    {
        public Log log{ get; set; }
        private static string unbalDesc = string.Empty;
        internal static double rt = 0.0;
        internal static double kf = 0.0;
        internal static double tgf = 0.0;
        internal static double pf = 0.0;
        internal static int rq = 1;
        internal static int ru = 1;
        internal static int onprib = 1;
        internal static int wu = 0;
        internal static int TurnRowBreak = -1;
        internal static int TurnColBreak = -1;
        internal static double porog = 20.0;
        internal static double mbrak = 0.001;
        internal static uint nodepred = 5;
        internal static uint vetvpred = 5;
        private readonly List<UravVal> valuePribl = new List<UravVal>();
        private readonly List<Urav> uravNodeVetv = new List<Urav>();
        private readonly List<UravVal> valueProiz = new List<UravVal>();
        private readonly List<UravVal> valueCollapsed = new List<UravVal>();
        private readonly List<Urav> uravCollapsed = new List<Urav>();
        private DataTable node;
        private DataTable vetv;
        private DataTable area;
        private DataTable regim;
        private DataTable rejection;
        private DataTable ves;
        private DataTable balance;
        private static double unbalSum;
        private static double unbalMax;
        private static UravVal unbalMaxVU;

        internal StaticRgm() => this.InitTable();

        private void InitTable()
        {
            this.node = AddinStaticRgm.ID.GetTable("node");
            this.vetv = AddinStaticRgm.ID.GetTable("vetv");
            this.area = AddinStaticRgm.ID.GetTable("area");
            this.regim = AddinStaticRgm.ID.GetTable("regim");
            this.ves = AddinStaticRgm.ID.GetTable("ves");
            this.rejection = AddinStaticRgm.ID.GetTable("rejection");
            this.balance = AddinStaticRgm.ID.GetTable("balance");
            this.GetVetvName(this.node, this.vetv);
        }

        private void GetVetvName(DataTable node, DataTable vetv)
        {
            foreach (DataRow dataRow in vetv.Select("name is null"))
            {
                string str = "";
                DataRow[] dataRowArray1 = node.Select("ny = " + dataRow["ip"] + " and name is not null");
                DataRow[] dataRowArray2 = node.Select("ny = " + dataRow["iq"] + " and name is not null");
                if (dataRowArray1.Length > 0)
                    str = dataRowArray1[0]["name"].ToString() + " - ";
                if (dataRowArray2.Length > 0)
                    str += (string)dataRowArray2[0]["name"];
                if (!string.IsNullOrEmpty(str))
                    dataRow["name"] = (object)str;
            }
        }

        private void ClearAll()
        {
            this.valuePribl.Clear();
            this.uravNodeVetv.Clear();
            this.valueProiz.Clear();
            this.valueCollapsed.Clear();
            this.uravCollapsed.Clear();
            foreach (DataRow row in (InternalDataCollectionBase)this.node.Rows)
            {
                row["pmainvessred"] = row["qmainvessred"] = row["psred"] = row["qsred"] = (object)DBNull.Value;
                row["pmainves"] = row["pmainvesd"] = row["psred"] = (object)DBNull.Value;
                row["qmainves"] = row["qmainvesd"] = row["qsred"] = (object)DBNull.Value;
                row["pbrak"] = row["qbrak"] = (object)0;
                row["_p"] = row["_q"] = (object)DBNull.Value;
            }
            foreach (DataRow row in (InternalDataCollectionBase)this.vetv.Rows)
            {
                row["ippmainvessred"] = row["iqpmainvessred"] = row["ipqmainvessred"] = row["iqqmainvessred"] = row["ippsred"] = row["iqpsred"] = row["ipqsred"] = row["iqqsred"] = (object)DBNull.Value;
                row["ippmainves"] = row["ippmainvesd"] = row["ippsred"] = (object)DBNull.Value;
                row["iqpmainves"] = row["iqpmainvesd"] = row["iqpsred"] = (object)DBNull.Value;
                row["ipqmainves"] = row["ipqmainvesd"] = row["ipqsred"] = (object)DBNull.Value;
                row["iqqmainves"] = row["iqqmainvesd"] = row["iqqsred"] = (object)DBNull.Value;
                row["ippbrak"] = row["iqpbrak"] = row["ipqbrak"] = row["iqqbrak"] = (object)0;
                row["_ipp"] = row["_iqp"] = row["_ipq"] = row["_iqq"] = (object)DBNull.Value;
            }
        }

        public void RgmThread()
        {
            this.InitTable();
            double num1 = (double)this.regim.Rows[0]["dp"];
            int num2 = (int)this.regim.Rows[0]["onf"];
            StaticRgm.rt = (double)this.regim.Rows[0]["rt"];
            StaticRgm.kf = (double)this.regim.Rows[0]["kf"];
            StaticRgm.tgf = (double)this.regim.Rows[0]["tgf"];
            StaticRgm.pf = (double)this.regim.Rows[0]["pf"] * StaticRgm.rt;
            int num3 = (int)this.regim.Rows[0]["onv"];
            uint num4 = (uint)this.regim.Rows[0]["lt"];
            int num5 = (int)this.regim.Rows[0]["sb"];
            StaticRgm.rq = (int)this.regim.Rows[0]["rq"];
            StaticRgm.ru = (int)this.regim.Rows[0]["ru"];
            StaticRgm.onprib = (int)this.regim.Rows[0]["onprib"];
            StaticRgm.wu = (int)this.regim.Rows[0]["wu"];
            uint num6 = (uint)this.rejection.Rows[0]["oniter"];
            StaticRgm.porog = (double)(int)this.rejection.Rows[0]["porog"];
            StaticRgm.mbrak = (double)this.ves.Rows[0]["mbrak"];
            StaticRgm.nodepred = (uint)this.rejection.Rows[0]["nodepred"];
            StaticRgm.vetvpred = (uint)this.rejection.Rows[0]["vetvpred"];
            StaticRgm.TurnRowBreak = -1;
            StaticRgm.TurnColBreak = -1;
            double[] matrE = (double[])null;
            this.ClearAll();
            //log.AddMessage("Расчет энергораспределения", Log.MessageType.Info);
            //AddinStaticRgm.IP.AddMessage(0, "Расчет энергораспределения", 0, 4);
            //AddinStaticRgm.IP.AddMessage(1, string.Format("{0,-10}{1,-40}{2,15}{3,50}", (object) "Ит", (object) "Сумм.неб.", (object) "Мак.неб", (object) "Изм."), 0, 0);
            new CalcSopr().GetSopr();
            this.GetBeginApproximation(this.valuePribl);
            this.GetUravNodeVetv(this.uravNodeVetv);
            this.GetValueProiz(this.valueProiz);
            for (int index = 1; (long)index <= (long)num4; ++index)
            {
                double[] matrV;
                MatrRC matrJ;
                do
                {
                    matrV = this.GetMatrV(this.uravNodeVetv);
                    matrJ = this.GetMatrJ(this.uravNodeVetv, this.valueProiz);
                }
                while (num2 == 1 && this.CollapseJV(this.uravNodeVetv, ref matrJ, ref matrV) == -1);
                if (index == 1)
                    matrE = this.GetMatrE(this.uravNodeVetv);
                this.CalcObjectiveFunction(this.uravNodeVetv, matrV, matrE);
                MatrR C = new MatrR(matrJ.Col, matrJ.Col);
                double[] B = new double[matrJ.Col];
                this.GetMatrCB(matrJ, matrE, matrV, ref C, ref B);
                StaticRgm.GetMatrResh(C, B);
                if (num2 == 1)
                    this.CalcUravCollapse(this.uravCollapsed, B);
                this.ChangePribl(this.valuePribl, B);
                //log.AddMessage(string.Format("{0,-10:00}{1,-40:0.00}{2,15:0.00}{3,50}", (object)index, (object)StaticRgm.unbalSum, (object)StaticRgm.unbalMax, (object)StaticRgm.unbalMaxVU) + "\n", Log.MessageType.Info);
                //AddinStaticRgm.IP.AddMessage(1, string.Format("{0,-10:00}{1,-40:0.00}{2,15:0.00}{3,50}", (object) index, (object) StaticRgm.unbalSum, (object) StaticRgm.unbalMax, (object) StaticRgm.unbalMaxVU), 0, 4);
                if (num1 <= StaticRgm.unbalMax && StaticRgm.unbalSum <= 1E+100)
                {
                    double unbalSum = StaticRgm.unbalSum;
                    if (num3 == 1 && StaticRgm.ru == 1)
                        this.TestV(this.valuePribl);
                    if (num5 == 1 && (long)num6 <= (long)index)
                    {
                        this.CalcPQ(this.valuePribl);
                        this.CheckedRejection(this.uravNodeVetv, matrV, ref matrE);
                    }
                }
                else
                    break;
            }
            if (StaticRgm.unbalSum > 1E+100)
            {
                //AddinStaticRgm.IP.AddMessage(1, "Режим разошёлся", 0, 1);
            }
            else
            {
                this.CalcPQ(this.valuePribl);
                this.CalcAreaVetv();
                this.CalcAreaNode();
                this.CalcAreaNodeLoss();
                this.CalcBalance();
                //if (e.Argument is int && (int) e.Argument == 1)
                //  this.CalcIzb();
                //if (!(e.Argument is int) || (int) e.Argument != 4 && (int) e.Argument != 5)
                //  return;
                //this.CalcAddres((int) e.Argument);
            }
        }

        private void CheckedRejection(List<Urav> uravNodeVetv, double[] matrV, ref double[] matrE)
        {
            Nabl nabl = new Nabl(AddinStaticRgm.ID);
            List<Urav> uravList = new List<Urav>();
            int num1 = 0;
            uint num2 = 0;
            uint num3 = 0;
            foreach (Urav urav in uravNodeVetv)
            {
                if (!urav.Collapsed && urav.Brak == 0 && (urav.Vars[0].Name == 'p' || urav.Vars[0].Name == 'q') && urav.Vars[0].Value != 0.0)
                {
                    string str = "";
                    if (urav.Vars[0].Iq != 0)
                        str = urav.Vars[0].Dr["ip"].Equals((object)urav.Vars[0].Ip) ? "ip" : "iq";
                    string index = str + (object)urav.Vars[0].Name + (urav.Vars[0].Iq != 0 ? (object)"" : (object)"ras");
                    double num4 = Math.Min(Math.Abs((double)urav.Vars[0].Dr[index]), Math.Abs(urav.Vars[0].Value));
                    double num5;
                    if ((num5 = Math.Abs(((double)urav.Vars[0].Dr[index] - urav.Vars[0].Value) / num4 * 100.0)) >= StaticRgm.porog)
                    {
                        urav.Tag = (object)num5;
                        urav.IdBrak = num1;
                        uravList.Add(urav);
                    }
                }
                if (!urav.Collapsed)
                    ++num1;
            }
            uravList.Sort((Comparison<Urav>)((a, b) => ((double)a.Tag).CompareTo((double)b.Tag) * -1));
            foreach (Urav urav1 in uravList)
            {
                if (urav1.Vars[0].Iq != 0)
                {
                    if (++num3 > StaticRgm.vetvpred)
                        continue;
                }
                else if (++num2 > StaticRgm.nodepred)
                    continue;
                string str = "";
                if (urav1.Vars[0].Iq != 0)
                    str = urav1.Vars[0].Dr["ip"].Equals((object)urav1.Vars[0].Ip) ? "ip" : "iq";
                string index = str + (object)urav1.Vars[0].Name + "brak";
                if (nabl.GetNablReject() == -1)
                {
                    matrE[urav1.IdBrak] *= StaticRgm.mbrak;
                    urav1.Brak = 1;
                    urav1.Vars[0].Dr[index] = (object)1;
                }
                else
                {
                    urav1.Vars[0].Dr[index] = (object)-1;
                    List<double> doubleList = new List<double>((IEnumerable<double>)matrE);
                    doubleList.RemoveAt(urav1.IdBrak);
                    matrE = doubleList.ToArray();
                    foreach (Urav urav2 in uravList)
                    {
                        if (urav2.IdBrak > urav1.IdBrak)
                            --urav2.IdBrak;
                    }
                    uravNodeVetv.Remove(urav1);
                }
                //AddinStaticRgm.IP.AddMessage(1, "Отбраковано измерение " + (object)urav1.Vars[0], 2, 1);
            }
        }

        private void CalcAddresNode(DataRow nr)
        {
            int num = 0;
            if (nr["output"] != DBNull.Value)
                return;
            DataRow[] dataRowArray = this.vetv.Select("sta = true and (ip = " + nr["ny"] + " or iq = " + nr["ny"] + ")");
            foreach (DataRow dataRow in dataRowArray)
            {
                if (dataRow["ip"].Equals(nr["ny"]) && (double)dataRow["ipp"] <= 0.0)
                    ++num;
                else if (dataRow["iq"].Equals(nr["ny"]) && (double)dataRow["iqp"] <= 0.0)
                    ++num;
            }
            if (num != dataRowArray.Length)
                return;
            nr["output"] = nr["input"];
            foreach (DataRow vr in dataRowArray)
            {
                vr["output"] = (object)Math.Abs((double)nr["input"] * (double)vr[vr["ip"].Equals(nr["ny"]) ? "ipp" : "iqp"] / (double)nr["pras"]);
                this.CalcAddresVetv(vr["ip"].Equals(nr["ny"]) ? (int)vr["iq"] : (int)vr["ip"], vr);
            }
        }

        private void CalcAddresVetv(int n, DataRow vr)
        {
            int num1 = 0;
            double num2 = 0.0;
            DataRow[] dataRowArray = this.vetv.Select("sta = true and (ip = " + (object)n + " or iq = " + (object)n + ")");
            foreach (DataRow dataRow in dataRowArray)
            {
                if (dataRow.Equals((object)vr))
                    ++num1;
                else if (dataRow["ip"].Equals((object)n) && (double)dataRow["ipp"] <= 0.0 || dataRow["output"] != DBNull.Value)
                    ++num1;
                else if (dataRow["iq"].Equals((object)n) && (double)dataRow["iqp"] <= 0.0 || dataRow["output"] != DBNull.Value)
                    ++num1;
            }
            foreach (DataRow dataRow in dataRowArray)
            {
                if (dataRow["output"] == DBNull.Value)
                    num2 += Math.Abs((double)dataRow["ipp"]);
            }
            DataRow dataRow1 = this.node.Select("sta = true and ny = " + (object)n)[0];
            if (dataRow1["pn"] != DBNull.Value)
                num2 += Math.Abs((double)dataRow1["pn"]);
            if (num1 == dataRowArray.Length)
            {
                double num3 = 0.0;
                foreach (DataRow dataRow2 in dataRowArray)
                {
                    if (dataRow2["output"] != DBNull.Value)
                        num3 += (double)dataRow2["output"] + (double)dataRow2["input"];
                }
                if (dataRow1["pn"] != DBNull.Value)
                {
                    dataRow1["output"] = (object)(num3 * (double)dataRow1["pn"] / num2 + (double)dataRow1["input"] * (double)dataRow1["pn"] / num2);
                    foreach (DataRow vr1 in dataRowArray)
                    {
                        if (!vr1.Equals((object)vr) && vr1["output"] == DBNull.Value)
                        {
                            vr1["output"] = (object)(Math.Abs(num3 * (double)vr1[vr1["ip"].Equals(dataRow1["ny"]) ? "ipp" : "iqp"] / num2) + Math.Abs((double)dataRow1["input"] * (double)vr1[vr1["ip"].Equals(dataRow1["ny"]) ? "ipp" : "iqp"] / num2));
                            this.CalcAddresVetv(vr1["ip"].Equals((object)n) ? (int)vr1["iq"] : (int)vr1["ip"], vr1);
                        }
                    }
                }
                else if (dataRow1["pg"] != DBNull.Value)
                {
                    dataRow1["output"] = dataRow1["input"];
                    foreach (DataRow vr1 in dataRowArray)
                    {
                        if (!vr1.Equals((object)vr) && vr1["output"] == DBNull.Value)
                        {
                            vr1["output"] = (object)(Math.Abs(num3 * (double)vr1[vr1["ip"].Equals(dataRow1["ny"]) ? "ipp" : "iqp"] / num2) + Math.Abs((double)dataRow1["input"] * (double)vr1[vr1["ip"].Equals(dataRow1["ny"]) ? "ipp" : "iqp"] / num2));
                            this.CalcAddresVetv(vr1["ip"].Equals((object)n) ? (int)vr1["iq"] : (int)vr1["ip"], vr1);
                        }
                    }
                }
            }
            double num4 = 0.0;
            if (dataRow1["pn"] == DBNull.Value)
                return;
            foreach (DataRow dataRow2 in dataRowArray)
            {
                if (dataRow2["output"] != DBNull.Value)
                {
                    if (dataRow2["ip"].Equals((object)n) && (double)dataRow2["ipp"] <= 0.0)
                    {
                        num4 = 0.0;
                        break;
                    }
                    if (dataRow2["iq"].Equals((object)n) && (double)dataRow2["iqp"] <= 0.0)
                    {
                        num4 = 0.0;
                        break;
                    }
                    num4 += Math.Abs((double)dataRow2["output"]) + (double)dataRow2["input"];
                }
                else
                {
                    num4 = 0.0;
                    break;
                }
            }
            if (num4 == 0.0)
                return;
            dataRow1["output"] = (object)((double)dataRow1["input"] + num4);
        }

        private void CalcObjectiveFunction(List<Urav> uravNodeVetv, double[] V, double[] E)
        {
            int index = 0;
            using (List<Urav>.Enumerator enumerator = uravNodeVetv.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Urav urav = enumerator.Current;
                    if (!Array.Exists<Urav>(this.uravCollapsed.ToArray(), (Predicate<Urav>)(uc => uc.Id == urav.Id)))
                    {
                        if (urav.Vars[0].Name != 'u' && urav.Vars[0].Name != '0')
                        {
                            StaticRgm.unbalSum += Math.Abs(V[index] * V[index] * E[index] * E[index]);
                            double num = Math.Abs(V[index]);
                            StaticRgm.unbalSum += num;
                            if (StaticRgm.unbalMax < num)
                            {
                                StaticRgm.unbalMax = num;
                                StaticRgm.unbalMaxVU = urav.Vars[0];
                            }
                        }
                        ++index;
                    }
                }
            }
        }

        private void GetBeginApproximation(List<UravVal> ba)
        {
            ba.Clear();
            foreach (DataRow row in (InternalDataCollectionBase)this.vetv.Rows)
            {
                if (row["sta"].Equals((object)true))
                {
                    UravVal uravVal1 = new UravVal('p', (int)row["ip"], (int)row["iq"], (int)row["np"], 1, 1, row);
                    double pogr1 = uravVal1.Pogr;
                    uravVal1.Pribl = uravVal1.Value;
                    if (StaticRgm.onprib == 1 && row["ipp"] != DBNull.Value)
                        uravVal1.Pribl = StaticRgm.wu == 0 ? (double)row["ipp"] : (double)row["ipp"] / 1000.0;
                    ba.Add(uravVal1);
                    if (StaticRgm.rq == 1)
                    {
                        UravVal uravVal2 = new UravVal('q', (int)row["ip"], (int)row["iq"], (int)row["np"], 1, 1, row);
                        double pogr2 = uravVal2.Pogr;
                        uravVal2.Pribl = uravVal2.Value;
                        if (StaticRgm.onprib == 1 && row["ipq"] != DBNull.Value)
                            uravVal2.Pribl = StaticRgm.wu == 0 ? (double)row["ipq"] : (double)row["ipq"] / 1000.0;
                        ba.Add(uravVal2);
                    }
                }
            }
            foreach (DataRow row in (InternalDataCollectionBase)this.node.Rows)
            {
                if (row["sta"].Equals((object)true))
                    ba.Add(new UravVal('u', (int)row["ny"], 1, 1, row)
                    {
                        Pribl = StaticRgm.onprib != 1 || row["vras"] == DBNull.Value ? (double)row["_uhom"] / (double)row["_kt"] : (double)row["vras"] / (double)row["_kt"]
                    });
            }
        }

        private void GetUravNodeVetv(List<Urav> uravNodeVetv)
        {
            uravNodeVetv.Clear();
            int num = 0;
            foreach (DataRow dr in this.node.Select("sta = true"))
            {
                if ((int)dr["pbrak"] >= 0 && (!dr["pizmp"].Equals((object)DBNull.Value) || !dr["pizmo"].Equals((object)DBNull.Value)))
                {
                    Urav urav = new Urav(num++);
                    int ip = (int)dr["ny"];
                    UravVal uravVal = new UravVal('p', ip, 1, 1, dr);
                    urav.Vars.Add(uravVal);
                    uravVal.rows = this.vetv.Select("sta = true AND (ip = " + (object)ip + " OR iq = " + (object)ip + ")");
                    foreach (DataRow row in uravVal.rows)
                    {
                        if (row["ip"].Equals((object)ip))
                        {
                            urav.Vars.Add(new UravVal('p', (int)row["ip"], (int)row["iq"], (int)row["np"], -1, 1, row));
                        }
                        else
                        {
                            urav.Vars.Add(new UravVal('p', (int)row["ip"], (int)row["iq"], (int)row["np"], 1, 1, row));
                            urav.Vars.Add(new UravVal('P', (int)row["ip"], (int)row["iq"], (int)row["np"], 1, 1, row));
                        }
                    }
                    uravNodeVetv.Add(urav);
                }
                if (StaticRgm.rq == 1 && (int)dr["qbrak"] >= 0 && (!dr["qizmp"].Equals((object)DBNull.Value) || !dr["qizmo"].Equals((object)DBNull.Value)))
                {
                    Urav urav = new Urav(num++);
                    int ip = (int)dr["ny"];
                    UravVal uravVal = new UravVal('q', ip, 1, 1, dr);
                    urav.Vars.Add(uravVal);
                    uravVal.rows = this.vetv.Select("sta = true AND (ip = " + (object)ip + " OR iq = " + (object)ip + ")");
                    foreach (DataRow row in uravVal.rows)
                    {
                        if (row["ip"].Equals((object)ip))
                        {
                            urav.Vars.Add(new UravVal('q', (int)row["ip"], (int)row["iq"], (int)row["np"], -1, 1, row));
                        }
                        else
                        {
                            urav.Vars.Add(new UravVal('q', (int)row["ip"], (int)row["iq"], (int)row["np"], 1, 1, row));
                            urav.Vars.Add(new UravVal('Q', (int)row["ip"], (int)row["iq"], (int)row["np"], 1, 1, row));
                        }
                    }
                    uravNodeVetv.Add(urav);
                }
            }
            foreach (DataRow dr in this.vetv.Select("sta = true"))
            {
                if ((int)dr["ippbrak"] >= 0 && (!dr["ippizmp"].Equals((object)DBNull.Value) || !dr["ippizmo"].Equals((object)DBNull.Value)))
                    uravNodeVetv.Add(new Urav(num++)
                    {
                        Vars = {
              new UravVal('p', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], 1, 1, dr),
              new UravVal('p', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], 1, 1, dr)
            }
                    });
                if (StaticRgm.rq == 1 && (int)dr["ipqbrak"] >= 0 && (!dr["ipqizmp"].Equals((object)DBNull.Value) || !dr["ipqizmo"].Equals((object)DBNull.Value)))
                    uravNodeVetv.Add(new Urav(num++)
                    {
                        Vars = {
              new UravVal('q', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], 1, 1, dr),
              new UravVal('q', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], 1, 1, dr)
            }
                    });
                if ((int)dr["iqpbrak"] >= 0 && (!dr["iqpizmp"].Equals((object)DBNull.Value) || !dr["iqpizmo"].Equals((object)DBNull.Value)))
                    uravNodeVetv.Add(new Urav(num++)
                    {
                        Vars = {
              new UravVal('p', (int) dr["iq"], (int) dr["ip"], (int) dr["np"], 1, 1, dr),
              new UravVal('p', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], -1, 1, dr),
              new UravVal('P', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], -1, 1, dr)
            }
                    });
                if (StaticRgm.rq == 1 && (int)dr["iqqbrak"] >= 0 && (!dr["iqqizmp"].Equals((object)DBNull.Value) || !dr["iqqizmo"].Equals((object)DBNull.Value)))
                    uravNodeVetv.Add(new Urav(num++)
                    {
                        Vars = {
              new UravVal('q', (int) dr["iq"], (int) dr["ip"], (int) dr["np"], 1, 1, dr),
              new UravVal('q', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], -1, 1, dr),
              new UravVal('Q', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], -1, 1, dr)
            }
                    });
            }
            if (StaticRgm.ru == 1)
            {
                foreach (DataRow dr in this.vetv.Select("sta = true"))
                    uravNodeVetv.Add(new Urav(num++)
                    {
                        Vars = {
              new UravVal('0', 0, 0, 0, dr),
              new UravVal('u', (int) dr["iq"], 1, 1, dr),
              new UravVal('G', (int) dr["ip"], (int) dr["iq"], (int) dr["np"], -1, 1, dr)
            }
                    });
            }
            if (StaticRgm.ru != 1)
                return;
            foreach (DataRow dr in this.node.Select("sta = true AND uizm IS NOT Null"))
                uravNodeVetv.Add(new Urav(num++)
                {
                    Vars = {
            new UravVal('u', (int) dr["ny"], 0, 0, dr),
            new UravVal('u', (int) dr["ny"], 1, 1, dr)
          }
                });
        }

        private void GetValueProiz(List<UravVal> vp)
        {
            vp.Clear();
            foreach (DataRow row in (InternalDataCollectionBase)this.vetv.Rows)
            {
                if (row["sta"].Equals((object)true))
                {
                    vp.Add(new UravVal('p', (int)row["ip"], (int)row["iq"], (int)row["np"], 1, 1, row));
                    if (StaticRgm.rq == 1)
                        vp.Add(new UravVal('q', (int)row["ip"], (int)row["iq"], (int)row["np"], 1, 1, row));
                }
            }
            if (StaticRgm.ru != 1)
                return;
            foreach (DataRow row in (InternalDataCollectionBase)this.node.Rows)
            {
                if (row["sta"].Equals((object)true))
                    vp.Add(new UravVal('u', (int)row["ny"], 1, 1, row));
            }
        }

        private double[] GetMatrV(List<Urav> uravNodeVetv)
        {
            StaticRgm.unbalSum = 0.0;
            StaticRgm.unbalMax = 0.0;
            int num1 = 0;
            double[] numArray = new double[uravNodeVetv.Count];
            foreach (Urav urav in uravNodeVetv)
            {
                double num2 = urav.Vars[0].Value - this.GetGB(urav.Vars[0]) - this.GetPodstUrav(urav);
                numArray[num1++] = num2;
            }
            return numArray;
        }

        private double GetGB(UravVal var)
        {
            double num1 = 0.0;
            int num2 = 0;
            if (var.Ip == 0 && var.Iq == 0 || var.Name == 'u')
                return 0.0;
            if (var.Iq == 0)
            {
                if (var.Name == 'p' && !var.Dr["_g"].Equals((object)DBNull.Value))
                {
                    if (!var.Dr["_g"].Equals((object)0) && var.d1 == 0.0)
                        var.d1 = (double)var.Dr["_g"] / 1000000.0 * StaticRgm.rt;
                    double pribl = this.GetPribl('u', var.Ip, 0, 0);
                    num1 += var.d1 * pribl * pribl;
                }
                if (var.Name == 'q' && !var.Dr["_b"].Equals((object)DBNull.Value))
                {
                    if (!var.Dr["_b"].Equals((object)0) && var.d1 == 0.0)
                        var.d1 = (double)var.Dr["_b"] / 1000000.0 * StaticRgm.rt;
                    double pribl = this.GetPribl('u', var.Ip, 0, 0);
                    num1 += var.d1 * pribl * pribl;
                }
                if (var.Name == 'p' && (!var.Dr["pizmp"].Equals((object)DBNull.Value) || !var.Dr["pizmo"].Equals((object)DBNull.Value)))
                {
                    if (var.d2 == 0.0)
                    {
                        foreach (DataRow row in var.rows)
                        {
                            if (!row["_g"].Equals((object)0))
                                var.d2 += (double)row["_g"] / 2000000.0 * (StaticRgm.rt - (double)row["_totk"]);
                        }
                    }
                    double pribl = this.GetPribl('u', var.Ip, 0, 0);
                    num1 += var.d2 * pribl * pribl;
                }
                if (var.Name == 'q' && (!var.Dr["qizmp"].Equals((object)DBNull.Value) || !var.Dr["qizmo"].Equals((object)DBNull.Value)))
                {
                    if (var.d2 == 0.0)
                    {
                        foreach (DataRow row in var.rows)
                        {
                            if (!row["_b"].Equals((object)0))
                                var.d2 += (double)row["_b"] / 2000000.0 * (StaticRgm.rt - (double)row["_totk"]);
                        }
                    }
                    double pribl = this.GetPribl('u', var.Ip, 0, 0);
                    num1 += var.d2 * pribl * pribl;
                }
            }
            else
            {
                double pribl1;
                int num3;
                if (var.Name == 'p')
                {
                    if ((pribl1 = this.GetPribl('p', var.Ip, var.Iq, var.Np)) == 0.0)
                    {
                        num3 = var.Iq;
                        pribl1 = this.GetPribl('p', var.Iq, var.Ip, var.Np);
                    }
                    else
                        num3 = var.Ip;
                }
                else if ((pribl1 = this.GetPribl('q', var.Ip, var.Iq, var.Np)) == 0.0)
                {
                    num3 = var.Iq;
                    pribl1 = this.GetPribl('q', var.Iq, var.Ip, var.Np);
                }
                else
                    num3 = var.Ip;
                if (pribl1 > 0.0)
                    num2 = num3 != var.Ip ? var.Ip : var.Iq;
                if (var.Name == 'p' && (!var.Dr["ippizmp"].Equals((object)DBNull.Value) || !var.Dr["ippizmo"].Equals((object)DBNull.Value) || (!var.Dr["iqpizmp"].Equals((object)DBNull.Value) || !var.Dr["iqpizmo"].Equals((object)DBNull.Value))))
                {
                    if (!var.Dr["_g"].Equals((object)0) && var.d1 == 0.0)
                        var.d1 = (double)var.Dr["_g"] / 2000000.0 * (StaticRgm.rt - (double)var.Dr["_totk"]);
                    double pribl2 = this.GetPribl('u', var.Ip, 0, 0);
                    num1 -= var.d1 * pribl2 * pribl2;
                }
                else if (var.Name == 'q' && (!var.Dr["ipqizmp"].Equals((object)DBNull.Value) || !var.Dr["ipqizmo"].Equals((object)DBNull.Value) || (!var.Dr["iqqizmp"].Equals((object)DBNull.Value) || !var.Dr["iqqizmo"].Equals((object)DBNull.Value))))
                {
                    if (var.d1 == 0.0)
                        var.d1 = (double)var.Dr["_b"] / 2000000.0 * (StaticRgm.rt - (double)var.Dr["_totk"]);
                    double pribl2 = this.GetPribl('u', var.Ip, 0, 0);
                    num1 -= var.d1 * pribl2 * pribl2;
                }
            }
            return num1;
        }

        private double GetPribl(char name, int ip, int iq, int np)
        {
            foreach (UravVal uravVal in this.valuePribl)
            {
                if ((int)uravVal.Name == (int)name && uravVal.Ip == ip && (uravVal.Iq == iq && uravVal.Np == np))
                    return uravVal.Pribl;
            }
            return 0.0;
        }

        private double GetPribl(char name, int ip, int iq, int np, double[] matr)
        {
            int index = 0;
            using (List<UravVal>.Enumerator enumerator = this.valuePribl.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    UravVal vp = enumerator.Current;
                    if ((int)vp.Name == (int)name && vp.Ip == ip && (vp.Iq == iq && vp.Np == np))
                    {
                        UravVal[] array = this.valueCollapsed.ToArray();
                        Predicate<UravVal> match = (Predicate<UravVal>)(vc => vc.Equals(vp));
                        UravVal uravVal;
                        return (uravVal = Array.Find<UravVal>(array, match)) != null ? uravVal.Fix : matr[index];
                    }
                    if (!Array.Exists<UravVal>(this.valueCollapsed.ToArray(), (Predicate<UravVal>)(vc => vc.Equals(vp))))
                        ++index;
                }
            }
            return 0.0;
        }

        private double GetPodstUrav(Urav urav)
        {
            double num = 0.0;
            for (int index = 1; index < urav.Vars.Count; ++index)
            {
                UravVal var = urav.Vars[index];
                if (var.Name == 'p' || var.Name == 'q' || var.Name == 'u')
                    num += this.GetPribl(var.Name, var.Ip, var.Iq, var.Np) * (double)var.Sign;
                else if (var.Name == 'P' || var.Name == 'Q')
                    num += this.GetWQ(var) * (double)var.Sign;
                else if (var.Name == 'G')
                    num += this.GetG(var) * (double)var.Sign;
            }
            return num;
        }

        private double GetWQ(UravVal varU)
        {
            double num1 = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = StaticRgm.rt - (double)varU.Dr["_totk"];
            string index = varU.Name == 'P' ? "dpf" : "dqf";
            TypePoter typePoter = (TypePoter)varU.Dr[index];
            if (typePoter == TypePoter.Auto && varU.Dr["_" + index].Equals((object)DBNull.Value))
                varU.Dr["_" + index] = (object)(typePoter = this.AutoSerchPoter(varU.Name, varU.Ip, varU.Iq, varU.Np, varU.Dr));
            else if (!varU.Dr["_" + index].Equals((object)DBNull.Value))
                typePoter = (TypePoter)varU.Dr["_" + index];
            switch (typePoter)
            {
                case TypePoter.dPQ:
                    num3 = (varU.Name == 'P' ? (double)varU.Dr["dpconst"] : (double)varU.Dr["dqconst"]) * num4;
                    break;

                case TypePoter.WQ:
                    double num5 = this.GetPribl('p', varU.Ip, varU.Iq, varU.Np) / num4;
                    double num6 = num5 * num5;
                    double num7 = this.GetPribl('q', varU.Ip, varU.Iq, varU.Np) / num4;
                    double num8 = num7 * num7;
                    double num9 = varU.Dr["ckor"].Equals((object)DBNull.Value) ? 0.0 : (double)varU.Dr["ckor"];
                    double num10 = num9 * num9;
                    double num11 = varU.Dr["ckoi"].Equals((object)DBNull.Value) ? 0.0 : (double)varU.Dr["ckoi"];
                    double num12 = num11 * num11;
                    double num13 = (num6 + num8 + num10 + num12) * num4 * (varU.Name == 'P' ? (double)varU.Dr["_r"] : (double)varU.Dr["_x"]);
                    double pribl1 = this.GetPribl('u', varU.Ip, 0, 0);
                    num3 = num13 / (pribl1 * pribl1);
                    break;

                case TypePoter.Kf:
                    double num14 = this.GetPribl('p', varU.Ip, varU.Iq, varU.Np) / num4;
                    double num15 = num14 * num14;
                    double num16 = this.GetPribl('q', varU.Ip, varU.Iq, varU.Np) / num4;
                    double num17 = num16 * num16;
                    double num18 = (num15 + num17) * num4 * (varU.Name == 'P' ? (double)varU.Dr["_r"] : (double)varU.Dr["_x"]);
                    double pribl2 = this.GetPribl('u', varU.Ip, 0, 0);
                    num3 = num18 / (pribl2 * pribl2) * (StaticRgm.kf * StaticRgm.kf);
                    break;

                case TypePoter.Tgf:
                    double num19 = this.GetPribl('p', varU.Ip, varU.Iq, varU.Np) / num4;
                    double num20 = num19 * num19;
                    double num21 = this.GetPribl('p', varU.Ip, varU.Iq, varU.Np) * (double)this.regim.Rows[0]["tgf"] / num4;
                    double num22 = num21 * num21;
                    double num23 = (num20 + num22) * num4 * (varU.Name == 'P' ? (double)varU.Dr["_r"] : (double)varU.Dr["_x"]);
                    double pribl3 = this.GetPribl('u', varU.Ip, 0, 0);
                    num3 = num23 / (pribl3 * pribl3);
                    break;

                case TypePoter.Razn:
                    if (varU.Dr["ip"].Equals((object)varU.Ip))
                    {
                        if (!varU.Dr["ippizmp"].Equals((object)DBNull.Value) && !varU.Dr["ippizmo"].Equals((object)DBNull.Value) && ((double)varU.Dr["ippizmp"] > 0.0 && (double)varU.Dr["ippizmo"] > 0.0 || (double)varU.Dr["ippizmp"] < 0.0 && (double)varU.Dr["ippizmo"] < 0.0))
                            num1 = (double)varU.Dr["ippizmp"] / ((double)varU.Dr["ippizmp"] + (double)varU.Dr["ippizmo"]) * (double)varU.Dr["ippizmp"] + (double)varU.Dr["ippizmo"] / ((double)varU.Dr["ippizmp"] + (double)varU.Dr["ippizmo"]) * (double)varU.Dr["ippizmo"];
                        if (!varU.Dr["ipqizmp"].Equals((object)DBNull.Value) && !varU.Dr["ipqizmo"].Equals((object)DBNull.Value) && ((double)varU.Dr["ipqizmp"] > 0.0 && (double)varU.Dr["ipqizmo"] > 0.0 || (double)varU.Dr["ipqizmp"] > 0.0 && (double)varU.Dr["ipqizmo"] > 0.0))
                            num2 = (double)varU.Dr["ipqizmp"] / ((double)varU.Dr["ipqizmp"] + (double)varU.Dr["ipqizmo"]) * (double)varU.Dr["ipqizmp"] + (double)varU.Dr["ipqizmo"] / ((double)varU.Dr["ipqizmp"] + (double)varU.Dr["ipqizmo"]) * (double)varU.Dr["ipqizmo"];
                    }
                    else
                    {
                        if (!varU.Dr["iqpizmp"].Equals((object)DBNull.Value) && !varU.Dr["iqpizmo"].Equals((object)DBNull.Value) && ((double)varU.Dr["iqpizmp"] > 0.0 && (double)varU.Dr["iqpizmo"] > 0.0 || (double)varU.Dr["iqpizmp"] < 0.0 && (double)varU.Dr["iqpizmo"] < 0.0))
                            num1 = (double)varU.Dr["iqpizmp"] / ((double)varU.Dr["iqpizmp"] + (double)varU.Dr["iqpizmo"]) * (double)varU.Dr["iqpizmp"] + (double)varU.Dr["iqpizmo"] / ((double)varU.Dr["iqpizmp"] + (double)varU.Dr["iqpizmo"]) * (double)varU.Dr["iqpizmo"];
                        if (!varU.Dr["iqqizmp"].Equals((object)DBNull.Value) && !varU.Dr["iqqizmo"].Equals((object)DBNull.Value) && ((double)varU.Dr["iqqizmp"] > 0.0 && (double)varU.Dr["iqqizmo"] > 0.0 || (double)varU.Dr["iqqizmp"] < 0.0 && (double)varU.Dr["iqqizmo"] < 0.0))
                            num2 = (double)varU.Dr["iqqizmp"] / ((double)varU.Dr["iqqizmp"] + (double)varU.Dr["iqqizmo"]) * (double)varU.Dr["iqqizmp"] + (double)varU.Dr["iqqizmo"] / ((double)varU.Dr["iqqizmp"] + (double)varU.Dr["iqqizmo"]) * (double)varU.Dr["iqqizmo"];
                    }
                    double num24 = num1 == 0.0 ? this.GetPribl('p', varU.Ip, varU.Iq, varU.Np) : num1;
                    double num25 = num2 == 0.0 ? this.GetPribl('q', varU.Ip, varU.Iq, varU.Np) : num2;
                    double num26 = num24 / num4;
                    double num27 = num26 * num26;
                    double num28 = num25 / num4;
                    double num29 = num28 * num28;
                    double num30 = (num27 + num29) * num4 * (varU.Name == 'P' ? (double)varU.Dr["_r"] : (double)varU.Dr["_x"]);
                    double pribl4 = this.GetPribl('u', varU.Ip, 0, 0);
                    num3 = num30 / (pribl4 * pribl4);
                    break;
            }
            return num3;
        }

        private TypePoter AutoSerchPoter(char name, int ip, int iq, int np, DataRow dr)
        {
            if (name == 'P' && !dr["dpconst"].Equals((object)DBNull.Value) || name == 'Q' && !dr["dqconst"].Equals((object)DBNull.Value))
                return TypePoter.dPQ;
            if (name == 'P' && !dr["ckor"].Equals((object)DBNull.Value) || name == 'Q' && !dr["ckoi"].Equals((object)DBNull.Value))
                return TypePoter.WQ;
            return this.regim.Rows[0]["rq"].Equals((object)0) ? TypePoter.Tgf : TypePoter.Kf;
        }

        private double GetG(UravVal var)
        {
            double num1 = StaticRgm.rt - (double)var.Dr["_totk"];
            double num2 = (double)var.Dr["_r"];
            double num3 = (double)var.Dr["_x"];
            double pribl1 = this.GetPribl('p', var.Ip, var.Iq, var.Np);
            double pribl2 = this.GetPribl('q', var.Ip, var.Iq, var.Np);
            double pribl3 = this.GetPribl('u', var.Ip, 0, 0);
            double num4 = pribl3 + (pribl1 * num2 + num3 * pribl2) / (num1 * pribl3);
            double num5 = (pribl1 * num3 - pribl2 * num2) / (num1 * pribl3);
            return Math.Sqrt(num4 * num4 + num5 * num5);
        }

        private MatrRC GetMatrJ(List<Urav> uravNodeVetv, List<UravVal> valueProiz)
        {
            MatrRC matrRc = new MatrRC(uravNodeVetv.Count, valueProiz.Count);
            MatrVal[] matrValArray = new MatrVal[valueProiz.Count];
            MatrVal matrVal = (MatrVal)null;
            int row = 0;
            foreach (Urav urav in uravNodeVetv)
            {
                int col = 0;
                foreach (UravVal varP in valueProiz)
                {
                    double proizv = this.GetProizv(urav, varP);
                    if (proizv != 0.0)
                    {
                        matrVal = matrRc.Rows[row] == null ? (matrRc.Rows[row] = new MatrVal(row, col, proizv)) : (matrVal.NextCol = new MatrVal(row, col, proizv));
                        if (matrRc.Cols[col] != null)
                        {
                            matrValArray[col].NextRow = matrVal;
                            matrValArray[col] = matrVal;
                        }
                        else
                        {
                            matrRc.Cols[col] = matrVal;
                            matrValArray[col] = matrRc.Cols[col];
                        }
                    }
                    ++col;
                }
                ++row;
            }
            matrRc.Row = row;
            matrRc.Col = valueProiz.Count;
            return matrRc;
        }

        private double GetProizv(Urav urav, UravVal varP)
        {
            double num1 = 0.0;
            for (int index1 = 1; index1 < urav.Vars.Count; ++index1)
            {
                UravVal var = urav.Vars[index1];
                if (var.Ip == varP.Ip)
                {
                    if (var.Name == 'u' && (int)var.Name == (int)varP.Name)
                        num1 += (double)var.Sign;
                    else if (var.Equals(varP))
                        num1 += (double)var.Sign;
                    else if ((var.Name == 'P' && varP.Name == 'p' || var.Name == 'Q' && varP.Name == 'q') && (var.Iq == varP.Iq && var.Np == varP.Np))
                    {
                        string index2 = var.Name == 'P' ? "dpf" : "dqf";
                        TypePoter typePoter = (TypePoter)var.Dr[index2];
                        if (typePoter == TypePoter.Auto && var.Dr["_" + index2].Equals((object)DBNull.Value))
                            var.Dr["_" + index2] = (object)(typePoter = this.AutoSerchPoter(var.Name, var.Ip, var.Iq, var.Np, var.Dr));
                        else if (!var.Dr["_" + index2].Equals((object)DBNull.Value))
                            typePoter = (TypePoter)var.Dr["_" + index2];
                        switch (typePoter)
                        {
                            case TypePoter.dPQ:
                                double num2 = StaticRgm.rt - (double)var.Dr["_totk"];
                                if (var.Name == 'P')
                                    num1 += (double)var.Dr["dpconst"] * num2 * (double)var.Sign;
                                if (var.Name == 'Q')
                                {
                                    num1 += (double)var.Dr["dqconst"] * num2 * (double)var.Sign;
                                    continue;
                                }
                                continue;
                            case TypePoter.WQ:
                                double pribl1 = this.GetPribl(varP.Name, varP.Ip, varP.Iq, varP.Np);
                                if (pribl1 != 0.0)
                                {
                                    double num3 = pribl1 * 2.0 / (StaticRgm.rt - (double)var.Dr["_totk"]) * (varP.Name == 'p' ? (double)var.Dr["_r"] : (double)var.Dr["_x"]);
                                    double pribl2 = this.GetPribl('u', varP.Ip, 0, 0);
                                    double num4 = num3 / (pribl2 * pribl2);
                                    num1 += num4 * (double)var.Sign;
                                    continue;
                                }
                                continue;
                            case TypePoter.Kf:
                                double pribl3 = this.GetPribl(varP.Name, varP.Ip, varP.Iq, varP.Np);
                                if (pribl3 != 0.0)
                                {
                                    double num3 = pribl3 * 2.0 / (StaticRgm.rt - (double)var.Dr["_totk"]) * (varP.Name == 'p' ? (double)var.Dr["_r"] : (double)var.Dr["_x"]);
                                    double pribl2 = this.GetPribl('u', varP.Ip, 0, 0);
                                    double num4 = num3 / (pribl2 * pribl2);
                                    num1 += num4 * (double)var.Sign * (StaticRgm.kf * StaticRgm.kf);
                                    continue;
                                }
                                continue;
                            case TypePoter.Tgf:
                                double pribl4 = this.GetPribl('p', varP.Ip, varP.Iq, varP.Np);
                                if (pribl4 != 0.0)
                                {
                                    double num3 = pribl4 * 2.0;
                                    if (varP.Name == 'q')
                                        num3 *= StaticRgm.tgf;
                                    double num4 = StaticRgm.rt - (double)var.Dr["_totk"];
                                    double num5 = num3 / num4 * (varP.Name == 'p' ? (double)var.Dr["_r"] : (double)var.Dr["_x"]);
                                    double pribl2 = this.GetPribl('u', varP.Ip, 0, 0);
                                    double num6 = num5 / (pribl2 * pribl2);
                                    num1 += num6 * (double)var.Sign;
                                    continue;
                                }
                                continue;
                            case TypePoter.Razn:
                                double num7 = 0.0;
                                double num8 = 0.0;
                                if (var.Dr["ip"].Equals((object)varP.Ip))
                                {
                                    if (!var.Dr["ippizmp"].Equals((object)DBNull.Value) && !var.Dr["ippizmo"].Equals((object)DBNull.Value) && ((double)var.Dr["ippizmp"] > 0.0 && (double)var.Dr["ippizmo"] > 0.0 || (double)var.Dr["ippizmp"] < 0.0 && (double)var.Dr["ippizmo"] < 0.0))
                                        num7 = (double)var.Dr["ippizmp"] / ((double)var.Dr["ippizmp"] + (double)var.Dr["ippizmo"]) * (double)var.Dr["ippizmp"] + (double)var.Dr["ippizmo"] / ((double)var.Dr["ippizmp"] + (double)var.Dr["ippizmo"]) * (double)var.Dr["ippizmo"];
                                    if (!var.Dr["ipqizmp"].Equals((object)DBNull.Value) && !var.Dr["ipqizmo"].Equals((object)DBNull.Value) && ((double)var.Dr["ipqizmp"] > 0.0 && (double)var.Dr["ipqizmo"] > 0.0 || (double)var.Dr["ipqizmp"] < 0.0 && (double)var.Dr["ipqizmo"] < 0.0))
                                        num8 = (double)var.Dr["ipqizmp"] / ((double)var.Dr["ipqizmp"] + (double)var.Dr["IPqizmo"]) * (double)var.Dr["ipqizmp"] + (double)var.Dr["ipqizmo"] / ((double)var.Dr["ipqizmp"] + (double)var.Dr["ipqizmo"]) * (double)var.Dr["ipqizmo"];
                                }
                                else
                                {
                                    if (!var.Dr["iqpizmp"].Equals((object)DBNull.Value) && !var.Dr["iqpizmo"].Equals((object)DBNull.Value) && ((double)var.Dr["iqpizmp"] > 0.0 && (double)var.Dr["iqpizmo"] > 0.0 || (double)var.Dr["iqpizmp"] < 0.0 && (double)var.Dr["iqpizmo"] < 0.0))
                                        num7 = (double)var.Dr["iqpizmp"] / ((double)var.Dr["iqpizmp"] + (double)var.Dr["iqpizmo"]) * (double)var.Dr["iqpizmp"] + (double)var.Dr["iqpizmo"] / ((double)var.Dr["iqpizmp"] + (double)var.Dr["iqpizmo"]) * (double)var.Dr["iqpizmo"];
                                    if (!var.Dr["iqqizmp"].Equals((object)DBNull.Value) && !var.Dr["iqqizmo"].Equals((object)DBNull.Value) && ((double)var.Dr["iqqizmp"] > 0.0 && (double)var.Dr["iqqizmo"] > 0.0 || (double)var.Dr["iqqizmp"] < 0.0 && (double)var.Dr["iqqizmo"] < 0.0))
                                        num8 = (double)var.Dr["iqqizmp"] / ((double)var.Dr["iqqizmp"] + (double)var.Dr["iqqizmo"]) * (double)var.Dr["iqqizmp"] + (double)var.Dr["iqqizmo"] / ((double)var.Dr["iqqizmp"] + (double)var.Dr["iqqizmo"]) * (double)var.Dr["iqqizmo"];
                                }
                                double num9 = varP.Name != 'p' || num7 == 0.0 ? (varP.Name != 'q' || num8 == 0.0 ? this.GetPribl(varP.Name, varP.Ip, varP.Iq, varP.Np) * 2.0 : num8 * 2.0) : num7 * 2.0;
                                if (num9 != 0.0)
                                {
                                    double num3 = StaticRgm.rt - (double)var.Dr["_totk"];
                                    double num4 = num9 / num3 * (varP.Name == 'p' ? (double)var.Dr["_r"] : (double)var.Dr["_x"]);
                                    double pribl2 = this.GetPribl('u', varP.Ip, 0, 0);
                                    double num5 = num4 / (pribl2 * pribl2);
                                    num1 += num5 * (double)var.Sign;
                                    continue;
                                }
                                continue;
                            default:
                                continue;
                        }
                    }
                    else if ((var.Name == 'P' || var.Name == 'Q') && varP.Name == 'u')
                    {
                        double pribl1 = this.GetPribl('p', var.Ip, var.Iq, var.Np);
                        double pribl2 = this.GetPribl('q', var.Ip, var.Iq, var.Np);
                        double num2 = StaticRgm.rt - (double)var.Dr["_totk"];
                        double num3 = pribl1 * pribl1 / (num2 * num2) + pribl2 * pribl2 / (num2 * num2);
                        double num4 = var.Dr["ckor"].Equals((object)DBNull.Value) ? 0.0 : (double)var.Dr["ckor"];
                        double num5 = var.Dr["ckoi"].Equals((object)DBNull.Value) ? 0.0 : (double)var.Dr["ckoi"];
                        double num6 = num4 * num4;
                        double num7 = num5 * num5;
                        double num8 = -2.0 * (num3 + num6 + num7) * num2 * (var.Name == 'P' ? (double)var.Dr["_r"] : (double)var.Dr["_x"]);
                        double pribl3 = this.GetPribl('u', varP.Ip, 0, 0);
                        double num9 = num8 / (pribl3 * pribl3 * pribl3);
                        num1 += num9 * (double)var.Sign;
                    }
                    else if (var.Name == 'G' && (varP.Name == 'p' || varP.Name == 'q') && (var.Iq == varP.Iq && var.Np == varP.Np))
                    {
                        double pribl1 = this.GetPribl('p', varP.Ip, varP.Iq, varP.Np);
                        double pribl2 = this.GetPribl('q', varP.Ip, varP.Iq, varP.Np);
                        double fT = StaticRgm.rt - (double)var.Dr["_totk"];
                        double fR_a = (double)var.Dr["_r"];
                        double fR_r = (double)var.Dr["_x"];
                        double pribl3 = this.GetPribl('u', varP.Ip, 0, 0);
                        num1 += this.GetProizPQ(pribl1, pribl2, fR_a, fR_r, pribl3, fT, varP.Name == 'p') * (double)var.Sign;
                    }
                    else if (var.Name == 'G' && varP.Name == 'u')
                    {
                        double pribl1 = this.GetPribl('p', var.Ip, var.Iq, var.Np);
                        double pribl2 = this.GetPribl('q', var.Ip, var.Iq, var.Np);
                        double fT = StaticRgm.rt - (double)var.Dr["_totk"];
                        double fR_a = (double)var.Dr["_r"];
                        double fR_r = (double)var.Dr["_x"];
                        double pribl3 = this.GetPribl('u', varP.Ip, 0, 0);
                        num1 += this.GetProizU(pribl1, pribl2, fR_a, fR_r, pribl3, fT) * (double)var.Sign;
                    }
                }
            }
            return num1;
        }

        private double GetProizPQ(
          double fW_a,
          double fW_r,
          double fR_a,
          double fR_r,
          double fU,
          double fT,
          bool bAct_Reac)
        {
            double num1 = fU + (fW_a * fR_a + fW_r * fR_r) / (fT * fU);
            double num2 = num1 * num1;
            double num3 = (fW_a * fR_r - fW_r * fR_a) / (fT * fU);
            double num4 = num3 * num3;
            double num5 = 1.0 / (2.0 * Math.Sqrt(num2 + num4));
            double num6 = 2.0 * (fU + (fW_a * fR_a + fW_r * fR_r) / (fT * fU)) / (fT * fU);
            double num7 = !bAct_Reac ? num6 * fR_r : num6 * fR_a;
            double num8 = 2.0 * (fW_a * fR_r - fW_r * fR_a) / (fT * fU * fT * fU);
            double num9 = !bAct_Reac ? num8 * fR_a : num8 * fR_r;
            return !bAct_Reac ? num5 * (num7 - num9) : num5 * (num7 + num9);
        }

        private double GetProizU(
          double fW_a,
          double fW_r,
          double fR_a,
          double fR_r,
          double fU,
          double fT)
        {
            if (fT == 0.0 || fU == 0.0)
                return 0.0;
            double num1 = fU + (fW_a * fR_a + fW_r * fR_r) / (fT * fU);
            double num2 = num1 * num1;
            double num3 = (fW_a * fR_r - fW_r * fR_a) / (fT * fU);
            double num4 = num3 * num3;
            return 1.0 / (2.0 * Math.Sqrt(num2 + num4)) * (2.0 * (fU + (fW_a * fR_a + fW_r * fR_r) / (fT * fU)) * (1.0 - (fW_a * fR_a + fW_r * fR_r) / (fT * fU * fU)) + -2.0 * (Math.Pow(fW_a * fR_r - fW_r * fR_a, 2.0) / (fT * fT * fU * fU * fU)));
        }

        private double[] GetMatrE(List<Urav> uravNodeVetv)
        {
            List<double> doubleList = new List<double>();
            double num1 = (double)this.ves.Rows[0]["gm"];
            double num2 = (double)this.ves.Rows[0]["facmax"];
            double num3 = (double)this.ves.Rows[0]["vesp"];
            double num4 = (double)this.ves.Rows[0]["vesq"];
            double num5 = (double)this.ves.Rows[0]["vesu"];
            double num6 = (double)this.ves.Rows[0]["vesppsev"];
            double num7 = (double)this.ves.Rows[0]["vesqpsev"];
            int num8 = (int)this.regim.Rows[0]["onves"];
            double averageIzmer = this.GetAverageIzmer(uravNodeVetv);
            double num9 = num1 != 0.0 ? averageIzmer / (num1 * num1) : 0.0;
            double num10 = averageIzmer * (num1 * num1);
            using (List<Urav>.Enumerator enumerator = uravNodeVetv.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Urav urav = enumerator.Current;
                    if (!Array.Exists<Urav>(this.uravCollapsed.ToArray(), (Predicate<Urav>)(uc => uc.Id == urav.Id)))
                    {
                        double num11 = urav.Vars[0].Pogr;
                        double num12 = urav.Vars[0].Value;
                        if (urav.Vars[0].Name != '0')
                        {
                            if (Math.Abs(num12) < num9)
                                num11 /= num9 * num9;
                            else if (Math.Abs(num12) > num10)
                                num11 /= num10 * num10;
                            else
                                num11 /= num12 * num12;
                        }
                        if (num11 == 0.0)
                            num11 = 2.0;
                        if (urav.Vars[0].Name == 'p')
                        {
                            num11 *= !urav.Vars[0].Dr.Table.Columns.Contains("IsPsevdop") || !urav.Vars[0].Dr["IsPsevdop"].Equals((object)true) ? num3 : num6;
                            if (urav.Vars[0].Iq == 0)
                                urav.Vars[0].Dr["pmainvessred"] = (object)num11;
                            else if (urav.Vars[0].Dr["ip"].Equals((object)urav.Vars[0].Ip))
                                urav.Vars[0].Dr["ippmainvessred"] = (object)num11;
                            else
                                urav.Vars[0].Dr["iqpmainvessred"] = (object)num11;
                        }
                        else if (urav.Vars[0].Name == 'q')
                        {
                            num11 *= !urav.Vars[0].Dr.Table.Columns.Contains("IsPsevdop") || !urav.Vars[0].Dr["IsPsevdoq"].Equals((object)true) ? num4 : num7;
                            if (urav.Vars[0].Iq == 0)
                                urav.Vars[0].Dr["qmainvessred"] = (object)num11;
                            else if (urav.Vars[0].Dr["ip"].Equals((object)urav.Vars[0].Ip))
                                urav.Vars[0].Dr["ipqmainvessred"] = (object)num11;
                            else
                                urav.Vars[0].Dr["iqqmainvessred"] = (object)num11;
                        }
                        else if (urav.Vars[0].Name == 'u')
                        {
                            num11 *= num5;
                            urav.Vars[0].Dr["pogtnves"] = (object)num11;
                        }
                        if (num8 == 1)
                            doubleList.Add(num11);
                        else
                            doubleList.Add(1.0);
                    }
                }
            }
            return doubleList.ToArray();
        }

        private double GetAverageIzmer(List<Urav> uravNodeVetv)
        {
            double num1 = 0.0;
            int num2 = 0;
            foreach (Urav urav in uravNodeVetv)
            {
                if (urav.Vars[0].Name == 'p' || urav.Vars[0].Name == 'q')
                {
                    num1 += Math.Abs(urav.Vars[0].Value);
                    ++num2;
                }
            }
            return num1 / (double)num2;
        }

        private void ChangePribl(List<UravVal> valuePribl, double[] B)
        {
            int num = 0;
            using (List<UravVal>.Enumerator enumerator = valuePribl.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    UravVal pribl = enumerator.Current;
                    UravVal uravVal;
                    if ((uravVal = Array.Find<UravVal>(this.valueCollapsed.ToArray(), (Predicate<UravVal>)(vc => vc.Equals(pribl)))) != null)
                        pribl.Pribl += uravVal.Fix;
                    else if (B.Length > num)
                        pribl.Pribl += B[num++];
                }
            }
        }

        public void GetMatrCB(MatrRC H, double[] E, double[] V, ref MatrR C, ref double[] B)
        {
            MatrVal matrVal1 = (MatrVal)null;
            MatrVal matrVal2 = (MatrVal)null;
            for (int row = 0; row < H.Col; ++row)
            {
                MatrVal matrVal3 = (MatrVal)null;
                for (MatrVal matrVal4 = H.Cols[row]; matrVal4 != null; matrVal4 = matrVal4.NextRow)
                {
                    if (matrVal3 == null)
                        matrVal3 = matrVal1 = new MatrVal(row, matrVal4.Row, E[matrVal4.Row] * matrVal4.Value);
                    else
                        matrVal1 = matrVal1.NextCol = new MatrVal(row, matrVal4.Row, E[matrVal4.Row] * matrVal4.Value);
                }
                for (int col = 0; col < H.Col; ++col)
                {
                    if (H.Cols[col] != null && matrVal3 != null && matrVal3.Row <= col)
                    {
                        MatrVal matrVal4 = matrVal3;
                        MatrVal matrVal5 = H.Cols[col];
                        double num = 0.0;
                        while (matrVal4 != null && matrVal5 != null)
                        {
                            if (matrVal4.Col == matrVal5.Row)
                            {
                                num += matrVal4.Value * matrVal5.Value;
                                matrVal4 = matrVal4.NextCol;
                                matrVal5 = matrVal5.NextRow;
                            }
                            else if (matrVal4.Col < matrVal5.Row)
                                matrVal4 = matrVal4.NextCol;
                            else
                                matrVal5 = matrVal5.NextRow;
                        }
                        if (num != 0.0)
                            matrVal2 = C.Rows[row] != null ? (matrVal2.NextCol = new MatrVal(row, col, num)) : (C.Rows[row] = new MatrVal(row, col, num));
                    }
                }
                double num1 = 0.0;
                for (MatrVal matrVal4 = matrVal3; matrVal4 != null; matrVal4 = matrVal4.NextCol)
                    num1 += matrVal4.Value * V[matrVal4.Col];
                B[row] = num1;
            }
        }

        public static void GetMatrResh(MatrR C, double[] B)
        {
            MatrVal matrVal1 = (MatrVal)null;
            for (int index = 0; index < C.Row; ++index)
            {
                if (C.Rows[index] != null)
                {
                    MatrVal matrVal2 = (MatrVal)null;
                    double num1 = -C.Rows[index].Value;
                    for (MatrVal nextCol = C.Rows[index].NextCol; nextCol != null; nextCol = nextCol.NextCol)
                    {
                        if (matrVal2 == null)
                            matrVal2 = matrVal1 = new MatrVal(nextCol.Col, nextCol.Value / num1);
                        else
                            matrVal1 = matrVal1.NextCol = new MatrVal(nextCol.Col, nextCol.Value / num1);
                    }
                    double num2 = B[index] / num1;
                    for (MatrVal nextCol = C.Rows[index].NextCol; nextCol != null; nextCol = nextCol.NextCol)
                    {
                        MatrVal matrVal3 = C.Rows[nextCol.Col];
                        matrVal1 = matrVal2;
                        while (matrVal1 != null && matrVal1.Col < matrVal3.Col)
                            matrVal1 = matrVal1.NextCol;
                        while (matrVal1 != null)
                        {
                            if (matrVal3 == null)
                            {
                                C.AddVal(nextCol.Col, matrVal1.Col, nextCol.Value * matrVal1.Value);
                                matrVal1 = matrVal1.NextCol;
                            }
                            else if (matrVal3.Col == matrVal1.Col)
                            {
                                matrVal3.Value += nextCol.Value * matrVal1.Value;
                                matrVal3 = matrVal3.NextCol;
                                matrVal1 = matrVal1.NextCol;
                            }
                            else if (matrVal3.Col < matrVal1.Col)
                                matrVal3 = matrVal3.NextCol;
                            else if (matrVal3.Col > matrVal1.Col)
                            {
                                C.AddVal(nextCol.Col, matrVal1.Col, nextCol.Value * matrVal1.Value);
                                matrVal1 = matrVal1.NextCol;
                            }
                        }
                        B[nextCol.Col] += num2 * nextCol.Value;
                    }
                }
            }
            for (int index = C.Row - 1; index >= 0; --index)
            {
                if (C.Rows[index] != null)
                {
                    MatrVal row = C.Rows[index];
                    double num = 0.0;
                    for (MatrVal nextCol = row.NextCol; nextCol != null; nextCol = nextCol.NextCol)
                        num += nextCol.Value * B[nextCol.Col];
                    B[index] = (B[index] - num) / row.Value;
                }
            }
        }

        private bool TestV(List<UravVal> pribl)
        {
            int num1 = (int)this.regim.Rows[0]["vmax"];
            int num2 = (int)this.regim.Rows[0]["vmin"];
            foreach (UravVal uravVal in pribl)
            {
                if (uravVal.Name == 'u')
                {
                    double num3 = (double)uravVal.Dr["_kt"] * uravVal.Pribl;
                    double num4 = (double)uravVal.Dr["uhom"];
                    if (num3 > num4 + num4 / 100.0 * (double)num1)
                        uravVal.Pribl = ((double)uravVal.Dr["_uhom"] + (double)uravVal.Dr["_uhom"] / 100.0 * (double)num2) / (double)uravVal.Dr["_kt"];
                    if (num3 < num4 - num4 / 100.0 * (double)num2)
                        uravVal.Pribl = ((double)uravVal.Dr["_uhom"] - (double)uravVal.Dr["_uhom"] / 100.0 * (double)num2) / (double)uravVal.Dr["_kt"];
                }
            }
            return false;
        }

        private void CalcPQ(List<UravVal> pribl)
        {
            double num1 = (double)this.regim.Rows[0]["rt"];
            foreach (DataRow dataRow in this.node.Select("sta = false"))
                dataRow["pg"] = dataRow["pn"] = dataRow["qg"] = dataRow["qn"] = (object)DBNull.Value;
            foreach (DataRow dataRow in this.vetv.Select("sta = false"))
                dataRow["ipp"] = dataRow["ipq"] = dataRow["iqp"] = dataRow["iqq"] = (object)DBNull.Value;
            foreach (DataRow dataRow in this.node.Select("sta = true"))
            {
                foreach (DataRow dr in this.vetv.Select("sta = true AND ip =" + dataRow["ny"]))
                {
                    double num2 = num1 - (double)dr["_totk"];
                    double pribl1 = this.GetPribl('p', (int)dr["ip"], (int)dr["iq"], (int)dr["np"]);
                    double wq1 = this.GetWQ(new UravVal('P', (int)dr["ip"], (int)dr["iq"], (int)dr["np"], 1, 1, dr));
                    double pribl2 = this.GetPribl('q', (int)dr["ip"], (int)dr["iq"], (int)dr["np"]);
                    double wq2 = this.GetWQ(new UravVal('Q', (int)dr["ip"], (int)dr["iq"], (int)dr["np"], 1, 1, dr));
                    if (StaticRgm.wu == 1)
                    {
                        pribl1 *= 1000.0;
                        wq1 *= 1000.0;
                        pribl2 *= 1000.0;
                        wq2 *= 1000.0;
                    }
                    double pribl3 = this.GetPribl('u', (int)dr["ip"], 0, 0);
                    double pribl4 = this.GetPribl('u', (int)dr["iq"], 0, 0);
                    double num3 = (double)dr["_g"] * pribl3 * pribl3 / 2000000.0 * num2;
                    double num4 = (double)dr["_b"] * pribl3 * pribl3 / 2000000.0 * num2;
                    double num5 = (double)dr["_g"] * pribl4 * pribl4 / 2000000.0 * num2;
                    double num6 = (double)dr["_b"] * pribl4 * pribl4 / 2000000.0 * num2;
                    if (StaticRgm.wu == 1)
                    {
                        num3 *= 1000.0;
                        num4 *= 1000.0;
                        num5 *= 1000.0;
                        num6 *= 1000.0;
                    }
                    dr["ipp"] = (object)(pribl1 - num3);
                    dr["ipq"] = (object)(pribl2 - num4);
                    dr["dp"] = (object)wq1;
                    dr["dq"] = (object)wq2;
                    dr["iqp"] = (object)-(pribl1 + wq1 + num5);
                    dr["iqq"] = (object)-(pribl2 + wq2 + num6);
                    dr["psh"] = (object)(num3 + num5);
                    dr["qsh"] = (object)(num4 + num6);
                }
            }
            foreach (DataRow dataRow1 in this.node.Select("sta = true"))
            {
                double pribl1 = this.GetPribl('u', (int)dataRow1["ny"], 0, 0);
                double num2 = (double)dataRow1["_g"] * pribl1 * pribl1 / 1000000.0 * num1;
                double num3 = (double)dataRow1["_b"] * pribl1 * pribl1 / 1000000.0 * num1;
                double num4 = 0.0;
                double num5 = 0.0;
                if (StaticRgm.wu == 1)
                {
                    num2 *= 1000.0;
                    num3 *= 1000.0;
                }
                DataTable vetv = this.vetv;
                string filterExpression = "sta = true AND (ip = " + dataRow1["ny"] + " OR iq = " + dataRow1["ny"] + ")";
                foreach (DataRow dataRow2 in vetv.Select(filterExpression))
                {
                    if (dataRow2["ip"].Equals(dataRow1["ny"]))
                    {
                        num4 += (double)dataRow2["ipp"];
                        num5 += (double)dataRow2["ipq"];
                    }
                    else
                    {
                        num4 += (double)dataRow2["iqp"];
                        num5 += (double)dataRow2["iqq"];
                    }
                }
                dataRow1["pras"] = (object)(-num4 + num2);
                dataRow1["pg"] = num4 <= 0.0 ? (object)(-num4 + num2) : (object)DBNull.Value;
                dataRow1["pn"] = num4 > 0.0 ? (object)(num4 - num2) : (object)DBNull.Value;
                dataRow1["qras"] = (object)(-num5 + num3);
                dataRow1["qg"] = num5 <= 0.0 ? (object)(-num5 + num3) : (object)DBNull.Value;
                dataRow1["qn"] = num5 > 0.0 ? (object)(num5 - num3) : (object)DBNull.Value;
                dataRow1["vras"] = (object)((double)dataRow1["_kt"] * pribl1);
                dataRow1["psh"] = (object)num2;
                dataRow1["qsh"] = (object)num3;
            }
            if (StaticRgm.rq == 0)
            {
                foreach (DataRow row in (InternalDataCollectionBase)this.node.Rows)
                    row["qras"] = row["qg"] = row["qn"] = row["qsh"] = row["qmainves"] = row["qmainvesd"] = (object)DBNull.Value;
            }
            if (StaticRgm.rq == 0)
            {
                foreach (DataRow row in (InternalDataCollectionBase)this.vetv.Rows)
                    row["ipq"] = row["iqq"] = row["dq"] = row["qsh"] = (object)DBNull.Value;
            }
            if (StaticRgm.ru != 0)
                return;
            foreach (DataRow row in (InternalDataCollectionBase)this.node.Rows)
                row["vras"] = (object)DBNull.Value;
        }

        private void CalcAreaNodeLoss()
        {
            foreach (DataRow dataRow1 in this.area.Select("na is not null"))
            {
                dataRow1["IsUnom"] = (object)true;
                List<double> doubleList = new List<double>();
                foreach (DataRow dataRow2 in this.node.Select("sta = true and na = " + dataRow1["na"]))
                {
                    if (!doubleList.Contains((double)dataRow2["_uhom"]))
                    {
                        dataRow2["IsUnom"] = (object)true;
                        doubleList.Add((double)dataRow2["_uhom"]);
                    }
                }
            }
            foreach (DataRow dataRow1 in this.area.Select("na is not null"))
            {
                Dictionary<double, double> dictionary1 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary2 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary3 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary4 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary5 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary6 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary7 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary8 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary9 = new Dictionary<double, double>();
                Dictionary<double, double> dictionary10 = new Dictionary<double, double>();
                foreach (DataRow dataRow2 in this.node.Select("sta = true and na = " + dataRow1["na"]))
                {
                    double key1 = (double)dataRow2["_uhom"];
                    if (!dictionary1.ContainsKey(key1))
                        dictionary1.Add(key1, 0.0);
                    Dictionary<double, double> dictionary11;
                    double index1;
                    (dictionary11 = dictionary1)[index1 = key1] = dictionary11[index1] + (double)dataRow2["psh"];
                    if (StaticRgm.rq == 1)
                    {
                        if (!dictionary2.ContainsKey(key1))
                            dictionary2.Add(key1, 0.0);
                        Dictionary<double, double> dictionary12;
                        double index2;
                        (dictionary12 = dictionary2)[index2 = key1] = dictionary12[index2] + (double)dataRow2["qsh"];
                    }
                    DataTable vetv = this.vetv;
                    string filterExpression = "sta = true and (ip = " + dataRow2["ny"] + " or iq = " + dataRow2["ny"] + ")";
                    foreach (DataRow dataRow3 in vetv.Select(filterExpression))
                    {
                        DataRow dataRow4 = this.node.Select("ny = " + dataRow3["ip"])[0];
                        DataRow dataRow5 = this.node.Select("ny = " + dataRow3["iq"])[0];
                        double num = dataRow4["na"] == DBNull.Value || dataRow5["na"] == DBNull.Value || (dataRow1["na"] == DBNull.Value || (int)dataRow4["na"] != (int)dataRow1["na"]) || (int)dataRow5["na"] != (int)dataRow1["na"] ? 2.0 : 1.0;
                        double key2 = (double)dataRow4["_uhom"];
                        double key3 = (double)dataRow5["_uhom"];
                        if (dataRow3["ktr"] == DBNull.Value)
                        {
                            if (!dictionary3.ContainsKey(key2))
                                dictionary3.Add(key2, 0.0);
                            Dictionary<double, double> dictionary12;
                            double index2;
                            (dictionary12 = dictionary3)[index2 = key2] = dictionary12[index2] + (double)dataRow3["dp"] / 2.0 / num;
                            if (!dictionary3.ContainsKey(key3))
                                dictionary3.Add(key3, 0.0);
                            Dictionary<double, double> dictionary13;
                            double index3;
                            (dictionary13 = dictionary3)[index3 = key3] = dictionary13[index3] + (double)dataRow3["dp"] / 2.0 / num;
                            if (StaticRgm.rq == 1)
                            {
                                if (!dictionary4.ContainsKey(key2))
                                    dictionary4.Add(key2, 0.0);
                                Dictionary<double, double> dictionary14;
                                double index4;
                                (dictionary14 = dictionary4)[index4 = key2] = dictionary14[index4] + (double)dataRow3["dq"] / 2.0 / num;
                                if (!dictionary4.ContainsKey(key3))
                                    dictionary4.Add(key3, 0.0);
                                Dictionary<double, double> dictionary15;
                                double index5;
                                (dictionary15 = dictionary4)[index5 = key3] = dictionary15[index5] + (double)dataRow3["dq"] / 2.0 / num;
                            }
                            if (!dictionary5.ContainsKey(key2))
                                dictionary5.Add(key2, 0.0);
                            Dictionary<double, double> dictionary16;
                            double index6;
                            (dictionary16 = dictionary5)[index6 = key2] = dictionary16[index6] + (double)dataRow3["psh"] / 2.0 / num;
                            if (!dictionary5.ContainsKey(key3))
                                dictionary5.Add(key3, 0.0);
                            Dictionary<double, double> dictionary17;
                            double index7;
                            (dictionary17 = dictionary5)[index7 = key3] = dictionary17[index7] + (double)dataRow3["psh"] / 2.0 / num;
                            if (StaticRgm.rq == 1)
                            {
                                if (!dictionary6.ContainsKey(key2))
                                    dictionary6.Add(key2, 0.0);
                                Dictionary<double, double> dictionary14;
                                double index4;
                                (dictionary14 = dictionary6)[index4 = key2] = dictionary14[index4] + (double)dataRow3["qsh"] / 2.0 / num;
                                if (!dictionary6.ContainsKey(key3))
                                    dictionary6.Add(key3, 0.0);
                                Dictionary<double, double> dictionary15;
                                double index5;
                                (dictionary15 = dictionary6)[index5 = key3] = dictionary15[index5] + (double)dataRow3["qsh"] / 2.0 / num;
                            }
                        }
                        else
                        {
                            if (!dictionary7.ContainsKey(key2))
                                dictionary7.Add(key2, 0.0);
                            Dictionary<double, double> dictionary12;
                            double index2;
                            (dictionary12 = dictionary7)[index2 = key2] = dictionary12[index2] + (double)dataRow3["dp"] / 2.0 / num;
                            if (!dictionary7.ContainsKey(key3))
                                dictionary7.Add(key3, 0.0);
                            Dictionary<double, double> dictionary13;
                            double index3;
                            (dictionary13 = dictionary7)[index3 = key3] = dictionary13[index3] + (double)dataRow3["dp"] / 2.0 / num;
                            if (StaticRgm.rq == 1)
                            {
                                if (!dictionary8.ContainsKey(key2))
                                    dictionary8.Add(key2, 0.0);
                                Dictionary<double, double> dictionary14;
                                double index4;
                                (dictionary14 = dictionary8)[index4 = key2] = dictionary14[index4] + (double)dataRow3["dq"] / 2.0 / num;
                                if (!dictionary8.ContainsKey(key3))
                                    dictionary8.Add(key3, 0.0);
                                Dictionary<double, double> dictionary15;
                                double index5;
                                (dictionary15 = dictionary8)[index5 = key3] = dictionary15[index5] + (double)dataRow3["dq"] / 2.0 / num;
                            }
                            if (!dictionary9.ContainsKey(key2))
                                dictionary9.Add(key2, 0.0);
                            Dictionary<double, double> dictionary16;
                            double index6;
                            (dictionary16 = dictionary9)[index6 = key2] = dictionary16[index6] + (double)dataRow3["psh"] / 2.0 / num;
                            if (!dictionary9.ContainsKey(key3))
                                dictionary9.Add(key3, 0.0);
                            Dictionary<double, double> dictionary17;
                            double index7;
                            (dictionary17 = dictionary9)[index7 = key3] = dictionary17[index7] + (double)dataRow3["psh"] / 2.0 / num;
                            if (StaticRgm.rq == 1)
                            {
                                if (!dictionary10.ContainsKey(key2))
                                    dictionary10.Add(key2, 0.0);
                                Dictionary<double, double> dictionary14;
                                double index4;
                                (dictionary14 = dictionary10)[index4 = key2] = dictionary14[index4] + (double)dataRow3["qsh"] / 2.0 / num;
                                if (!dictionary10.ContainsKey(key3))
                                    dictionary10.Add(key3, 0.0);
                                Dictionary<double, double> dictionary15;
                                double index5;
                                (dictionary15 = dictionary10)[index5 = key3] = dictionary15[index5] + (double)dataRow3["qsh"] / 2.0 / num;
                            }
                        }
                    }
                }
                foreach (DataRow dataRow2 in this.node.Select("sta = true and IsUnom is not null and na = " + dataRow1["na"]))
                {
                    double key = (double)dataRow2["_uhom"];
                    dataRow2["dp_shunt"] = (object)(dictionary1.ContainsKey(key) ? dictionary1[key] : 0.0);
                    if (StaticRgm.rq == 1)
                        dataRow2["dq_shunt"] = (object)(dictionary2.ContainsKey(key) ? dictionary2[key] : 0.0);
                    dataRow2["dp_line"] = (object)(dictionary3.ContainsKey(key) ? dictionary3[key] : 0.0);
                    if (StaticRgm.rq == 1)
                        dataRow2["dq_line"] = (object)(dictionary4.ContainsKey(key) ? dictionary4[key] : 0.0);
                    dataRow2["shp_line"] = (object)(dictionary5.ContainsKey(key) ? dictionary5[key] : 0.0);
                    if (StaticRgm.rq == 1)
                        dataRow2["shq_line"] = (object)(dictionary6.ContainsKey(key) ? dictionary6[key] : 0.0);
                    dataRow2["dp_tran"] = (object)(dictionary7.ContainsKey(key) ? dictionary7[key] : 0.0);
                    if (StaticRgm.rq == 1)
                        dataRow2["dq_tran"] = (object)(dictionary8.ContainsKey(key) ? dictionary8[key] : 0.0);
                    dataRow2["shp_tran"] = (object)(dictionary9.ContainsKey(key) ? dictionary9[key] : 0.0);
                    if (StaticRgm.rq == 1)
                        dataRow2["shq_tran"] = (object)(dictionary10.ContainsKey(key) ? dictionary10[key] : 0.0);
                }
                double num1 = 0.0;
                foreach (double num2 in dictionary1.Values)
                    num1 += num2;
                dataRow1["dp_shunt"] = (object)num1;
                if (StaticRgm.rq == 1)
                {
                    double num2 = 0.0;
                    foreach (double num3 in dictionary2.Values)
                        num2 += num3;
                    dataRow1["dq_shunt"] = (object)num2;
                }
                double num4 = 0.0;
                foreach (double num2 in dictionary3.Values)
                    num4 += num2;
                dataRow1["dp_line"] = (object)num4;
                if (StaticRgm.rq == 1)
                {
                    double num2 = 0.0;
                    foreach (double num3 in dictionary4.Values)
                        num2 += num3;
                    dataRow1["dq_line"] = (object)num2;
                }
                double num5 = 0.0;
                foreach (double num2 in dictionary5.Values)
                    num5 += num2;
                dataRow1["shp_line"] = (object)num5;
                if (StaticRgm.rq == 1)
                {
                    double num2 = 0.0;
                    foreach (double num3 in dictionary6.Values)
                        num2 += num3;
                    dataRow1["shq_line"] = (object)num2;
                }
                double num6 = 0.0;
                foreach (double num2 in dictionary7.Values)
                    num6 += num2;
                dataRow1["dp_tran"] = (object)num6;
                if (StaticRgm.rq == 1)
                {
                    double num2 = 0.0;
                    foreach (double num3 in dictionary8.Values)
                        num2 += num3;
                    dataRow1["dq_tran"] = (object)num2;
                }
                double num7 = 0.0;
                foreach (double num2 in dictionary9.Values)
                    num7 += num2;
                dataRow1["shp_tran"] = (object)num7;
                if (StaticRgm.rq == 1)
                {
                    double num2 = 0.0;
                    foreach (double num3 in dictionary10.Values)
                        num2 += num3;
                    dataRow1["shq_tran"] = (object)num2;
                }
                dataRow1["dp"] = (object)((double)dataRow1["dp_shunt"] + (double)dataRow1["dp_line"] + (double)dataRow1["shp_line"] + (double)dataRow1["dp_tran"] + (double)dataRow1["shp_tran"]);
            }
        }

        private void CalcBalance()
        {
            foreach (double num1 in Const.UNom)
            {
                DataRow[] dataRowArray = this.node.Select("sta = true AND _uhom = " + /*(object) */num1.ToString(CultureInfo.InvariantCulture));
                if (dataRowArray.Length > 0)
                {
                    double num2 = 0.0;
                    double num3 = 0.0;
                    foreach (DataRow dataRow in dataRowArray)
                    {
                        if (dataRow["pg"] != DBNull.Value)
                            num2 += (double)dataRow["pg"];
                        if (dataRow["pn"] != DBNull.Value)
                            num3 += (double)dataRow["pn"];
                    }
                    this.balance.Rows[0]["wpos"] = (object)num2;
                    this.balance.Rows[0]["wotp"] = (object)num3;
                    this.balance.Rows[0]["wsaldo"] = (object)(num2 - num3);
                    break;
                }
            }
            for (int index = Const.UNom.Length - 1; index >= 0; --index)
            {
                DataRow[] dataRowArray = this.node.Select("sta = true AND _uhom = " + Const.UNom[index].ToString((IFormatProvider)new CultureInfo("en-US")));
                if (dataRowArray.Length > 0)
                {
                    double num = 0.0;
                    foreach (DataRow dataRow in dataRowArray)
                    {
                        if (dataRow["pn"] != DBNull.Value)
                            num += (double)dataRow["pn"];
                    }
                    this.balance.Rows[0]["wotppol"] = (object)num;
                    break;
                }
            }
            this.balance.Rows[0]["wotch"] = (object)((double)this.balance.Rows[0]["wsaldo"] - (double)this.balance.Rows[0]["wotppol"]);
        }

        private void CalcAreaVetv()
        {
            foreach (DataRow row in (InternalDataCollectionBase)this.vetv.Rows)
            {
                DataRow[] dataRowArray1 = this.node.Select("ny = " + row["ip"]);
                DataRow[] dataRowArray2 = this.node.Select("ny = " + row["iq"]);
                if (dataRowArray1.Length > 0 && dataRowArray2.Length > 0 && (dataRowArray1[0]["na"] != DBNull.Value && dataRowArray2[0]["na"] != DBNull.Value) && (int)dataRowArray1[0]["na"] != (int)dataRowArray2[0]["na"] || dataRowArray1.Length > 0 && dataRowArray2.Length > 0 && (dataRowArray1[0]["na"] == DBNull.Value || dataRowArray2[0]["na"] == DBNull.Value))
                {
                    row["ipna"] = dataRowArray1[0]["na"];
                    row["iqna"] = dataRowArray2[0]["na"];
                }
                else
                    row["ipna"] = row["iqna"] = (object)DBNull.Value;
            }
            foreach (DataRow row in (InternalDataCollectionBase)this.area.Rows)
            {
                double num1 = 0.0;
                double num2 = 0.0;
                DataTable vetv = this.vetv;
                string filterExpression = "ipna = " + row["na"] + " OR iqna = " + row["na"];
                foreach (DataRow dataRow in vetv.Select(filterExpression))
                {
                    if (dataRow["ipna"].Equals(row["na"]))
                    {
                        if ((double)dataRow["ipp"] > 0.0)
                            num1 += (double)dataRow["ipp"];
                        else
                            num2 += -(double)dataRow["ipp"];
                    }
                    else if ((double)dataRow["iqp"] > 0.0)
                        num1 += (double)dataRow["iqp"];
                    else
                        num2 += -(double)dataRow["iqp"];
                }
                row["pg_line"] = (object)num1;
                row["pn_line"] = (object)num2;
            }
        }

        private void CalcAreaNode()
        {
            foreach (DataRow row in (InternalDataCollectionBase)this.area.Rows)
            {
                double num1 = 0.0;
                double num2 = 0.0;
                double num3 = 0.0;
                double num4 = 0.0;
                double num5 = 0.0;
                double num6 = 0.0;
                foreach (DataRow dataRow in this.node.Select("sta = true AND na = " + row["na"]))
                {
                    if (dataRow["psred"] != DBNull.Value && (double)dataRow["psred"] > 0.0)
                        num2 += (double)dataRow["psred"];
                    if (dataRow["qsred"] != DBNull.Value && (double)dataRow["qsred"] > 0.0)
                        num4 += (double)dataRow["qsred"];
                    if (dataRow["pras"] != DBNull.Value && (double)dataRow["pras"] > 0.0)
                        num1 += (double)dataRow["pras"];
                    if (dataRow["qras"] != DBNull.Value && (double)dataRow["qras"] > 0.0)
                        num3 += (double)dataRow["qras"];
                    if (dataRow["pras"] != DBNull.Value && (double)dataRow["pras"] < 0.0)
                        num5 -= (double)dataRow["pras"];
                    if (dataRow["qras"] != DBNull.Value && (double)dataRow["qras"] < 0.0)
                        num6 -= (double)dataRow["qras"];
                }
                row["pvirras"] = (object)num1;
                row["qvirras"] = (object)num3;
                row["pvirizm"] = (object)num2;
                row["qvirizm"] = (object)num4;
                row["potpusk"] = (object)(num1 + (double)row["pg_line"]);
                row["pfullpost"] = (object)(num1 + (double)row["pg_line"] - (double)row["pn_line"]);
                row["ppolotpusk"] = (object)num5;
                row["otchetloss"] = (object)((double)row["pfullpost"] - num5);
                row["dpkom"] = (object)((double)row["otchetloss"] - (row["dp"] is double ? (double)row["dp"] : 0.0));
            }
        }

        private int CollapseJV(List<Urav> uravNodeVetv, ref MatrRC H, ref double[] V)
        {
            int row = 0;
            this.uravCollapsed.Clear();
            this.valueCollapsed.Clear();
            foreach (Urav urav1 in uravNodeVetv)
            {
                if (urav1.NumCollapsed == 0 && (urav1.Vars[0].Name == 'p' || urav1.Vars[0].Name == 'q') && (urav1.Vars[0].Ip != 0 && urav1.Vars[0].Iq == 0))
                {
                    urav1.Collapsed = false;
                    if (Math.Abs(urav1.Vars[0].Name == 'p' ? (double)urav1.Vars[0].Dr["_p"] : (double)urav1.Vars[0].Dr["_q"]) < StaticRgm.pf || urav1.Vars[0].Dr["tip"].Equals((object)3))
                    {
                        Urav urav2 = new Urav(urav1.Id);
                        int col1 = 0;
                        double num1 = 0.0;
                        int col2 = -1;
                        using (List<UravVal>.Enumerator enumerator = this.valueProiz.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                UravVal vp = enumerator.Current;
                                if (!Array.Exists<UravVal>(this.valueCollapsed.ToArray(), (Predicate<UravVal>)(vc => vc.Equals(vp))))
                                {
                                    if (!urav1.Collapsed && vp.Iq != 0 && (int)vp.Name == (int)urav1.Vars[0].Name && (vp.Ip == urav1.Vars[0].Ip || vp.Iq == urav1.Vars[0].Ip))
                                    {
                                        this.valueCollapsed.Insert(0, new UravVal(vp));
                                        col2 = col1;
                                        urav1.Collapsed = true;
                                    }
                                    double num2 = H.GetValue(row, col1);
                                    if (num2 != 0.0)
                                    {
                                        if (col2 == col1)
                                        {
                                            urav2.Vars.Insert(0, new UravVal(vp, num2, col1));
                                            num1 = num2;
                                        }
                                        else
                                            urav2.Vars.Add(new UravVal(vp, num2, col1));
                                    }
                                    ++col1;
                                }
                            }
                        }
                        if (!urav1.Collapsed)
                        {
                            AddinStaticRgm.IP.AddMessage(1, "Достигнут предел сворачивания по ветвям: " + (object)urav1.Vars[0], 0, 2);
                            ++urav1.NumCollapsed;
                        }
                        else if (num1 != 0.0)
                        {
                            foreach (UravVal var in urav2.Vars)
                                var.Value /= num1;
                            urav2.Value = V[row] / num1;
                            this.uravCollapsed.Insert(0, urav2);
                            this.TurnHV(ref H, ref V, urav2, row, col2);
                            if (StaticRgm.TurnRowBreak >= H.Row || StaticRgm.TurnColBreak >= H.Col)
                            {
                                if (StaticRgm.TurnRowBreak <= H.Row && StaticRgm.TurnColBreak <= H.Col)
                                    return 0;
                                AddinStaticRgm.IP.AddMessage(1, "Достигнут предел сворачивания матрицы: " + (object)urav1.Vars[0], 0, 2);
                                ++urav1.NumCollapsed;
                                StaticRgm.TurnRowBreak = StaticRgm.TurnColBreak = -1;
                                urav1.Collapsed = false;
                                return -1;
                            }
                            --row;
                        }
                        else if (num1 == 0.0 && urav1.Collapsed)
                        {
                            this.valueCollapsed.RemoveAt(0);
                            urav1.Collapsed = false;
                        }
                    }
                }
                ++row;
            }
            return 0;
        }

        private void TurnHV(ref MatrRC H, ref double[] V, Urav urav, int row, int col)
        {
            for (MatrVal matrVal = H.Cols[col]; matrVal != null; matrVal = matrVal.NextRow)
            {
                if (matrVal.Row != row && matrVal.Value != 0.0)
                {
                    double num = -matrVal.Value;
                    foreach (UravVal var in urav.Vars)
                    {
                        MatrVal val = H.GetVal(matrVal.Row, var.col);
                        if (val != null)
                        {
                            val.Value += num * var.Value;
                            if (val.Value == 0.0)
                                H.DelVal(matrVal.Row, var.col);
                        }
                        else
                            H.CreateVal(matrVal.Row, var.col, num * var.Value);
                    }
                    V[matrVal.Row] += urav.Value * num;
                }
            }
            for (MatrVal matrVal = H.Rows[row]; matrVal != null; matrVal = matrVal.NextCol)
                H.DelVal(row, matrVal.Col);
            for (MatrVal matrVal = H.Cols[col]; matrVal != null; matrVal = matrVal.NextRow)
                H.DelVal(matrVal.Row, col);
            for (int index = row + 1; index < H.Row; ++index)
            {
                for (MatrVal matrVal = H.Rows[index]; matrVal != null; matrVal = matrVal.NextCol)
                    --matrVal.Row;
            }
            for (int index = col + 1; index < H.Col; ++index)
            {
                for (MatrVal matrVal = H.Cols[index]; matrVal != null; matrVal = matrVal.NextRow)
                    --matrVal.Col;
            }
            for (int index = 0; index < H.Row; ++index)
            {
                if (row != index && H.Rows[index] == null)
                    StaticRgm.TurnRowBreak = H.Row;
            }
            for (int index = 0; index < H.Col; ++index)
            {
                if (col != index && H.Cols[index] == null)
                    StaticRgm.TurnColBreak = H.Col;
            }
            H.Rows = Array.FindAll<MatrVal>(H.Rows, (Predicate<MatrVal>)(mv => mv != null));
            H.Cols = Array.FindAll<MatrVal>(H.Cols, (Predicate<MatrVal>)(mv => mv != null));
            --H.Row;
            --H.Col;
            List<double> doubleList = new List<double>((IEnumerable<double>)V);
            doubleList.RemoveAt(row);
            V = doubleList.ToArray();
        }

        private void CalcUravCollapse(List<Urav> uravCollapsed, double[] matr)
        {
            using (List<Urav>.Enumerator enumerator = uravCollapsed.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Urav urav = enumerator.Current;
                    double num = 0.0;
                    for (int index = 1; index < urav.Vars.Count; ++index)
                    {
                        UravVal var = urav.Vars[index];
                        num += var.Value * this.GetPribl(var.Name, var.Ip, var.Iq, var.Np, matr);
                    }
                    UravVal uravVal = Array.Find<UravVal>(this.valueCollapsed.ToArray(), (Predicate<UravVal>)(vp => vp.Equals(urav.Vars[0])));
                    if (uravVal != null)
                        uravVal.Fix = urav.Value - num;
                }
            }
        }
    }

    internal class CalcSopr
    {
        private DataTable node;
        private DataTable vetv;
        private DataTable area;

        internal void GetSopr()
        {
            this.node = AddinStaticRgm.ID.GetTable("node");
            this.vetv = AddinStaticRgm.ID.GetTable("vetv");
            this.area = AddinStaticRgm.ID.GetTable("area");
            foreach (DataRow dataRow in this.node.Select("sta = true"))
            {
                dataRow["_g"] = dataRow["g"].Equals((object)DBNull.Value) ? (object)0.0 : dataRow["g"];
                dataRow["_b"] = dataRow["b"].Equals((object)DBNull.Value) ? (object)0.0 : dataRow["b"];
                dataRow["_uhom"] = (object)this.GetUnom((double)dataRow["uhom"]);
                dataRow["_eq"] = (object)false;
                dataRow["IsUnom"] = (object)DBNull.Value;
            }
            foreach (DataRow dataRow in this.vetv.Select("sta = true"))
            {
                dataRow["_r"] = dataRow["r"].Equals((object)DBNull.Value) ? (object)0.0 : dataRow["r"];
                dataRow["_x"] = dataRow["x"].Equals((object)DBNull.Value) ? (object)0.0 : dataRow["x"];
                dataRow["_g"] = dataRow["g"].Equals((object)DBNull.Value) ? (object)0.0 : dataRow["g"];
                dataRow["_b"] = dataRow["b"].Equals((object)DBNull.Value) ? (object)0.0 : dataRow["b"];
                dataRow["_eq"] = (object)false;
                dataRow["_dpf"] = (object)DBNull.Value;
                dataRow["_dqf"] = (object)DBNull.Value;
                dataRow["_totk"] = dataRow["totk"].Equals((object)DBNull.Value) ? (object)0.0 : dataRow["totk"];
            }
            for (uint index = 1; this.node.Select("sta = true AND gsvn = " + (object)index).Length > 0; ++index)
            {
                foreach (double num in Const.UNom)
                {
                    DataTable node = this.node;
                    string filterExpression = "sta = true AND _eq = false AND gsvn = " + (object)index + " AND _uhom = " + num.ToString("G", (IFormatProvider)CultureInfo.InvariantCulture);
                    foreach (DataRow nr in node.Select(filterExpression))
                        this.Equivalent(nr, 1.0);
                }
            }
        }

        private double GetUnom(double Uhom)
        {
            if (Uhom <= 3.35)
                return 0.4;
            if (Uhom >= 3.35 && Uhom <= 8.4)
                return 6.3;
            if (Uhom >= 8.4 && Uhom <= 22.75)
                return 10.5;
            if (Uhom >= 22.75 && Uhom <= 72.5)
                return 35.0;
            if (Uhom >= 72.5 && Uhom <= 165.0)
                return 110.0;
            if (Uhom >= 165.0 && Uhom <= 275.0)
                return 220.0;
            if (Uhom >= 275.0 && Uhom <= 415.0)
                return 330.0;
            if (Uhom >= 415.0 && Uhom <= 625.0)
                return 500.0;
            if (Uhom >= 625.0 && Uhom <= 825.0)
                return 750.0;
            return Uhom >= 950.0 ? 1150.0 : 0.0;
        }

        private void Equivalent(DataRow nr, double ktr)
        {
            object obj = nr["ny"];
            nr["_kt"] = (object)ktr;
            nr["_eq"] = (object)true;
            DataTable vetv = this.vetv;
            string filterExpression = "sta = true AND _eq = false AND (ip = " + obj + " OR iq = " + obj + ")";
            foreach (DataRow dataRow in vetv.Select(filterExpression))
            {
                if (!dataRow["_eq"].Equals((object)true))
                {
                    DataRow nr1 = this.node.Select("ny = " + (dataRow["ip"].Equals(obj) ? dataRow["iq"] : dataRow["ip"]))[0];
                    double ktr1 = ktr;
                    double num1 = dataRow[nameof(ktr)].Equals((object)DBNull.Value) ? 1.0 : (double)dataRow[nameof(ktr)];
                    double num2 = (double)nr["_uhom"];
                    double num3 = (double)nr1["_uhom"];
                    if (num2 != num3 || ktr != 1.0)
                    {
                        if (num2 > num3 && num1 < 1.0)
                            ktr1 = ktr * num1;
                        else if (num2 < num3 && num1 < 1.0)
                            ktr1 = ktr / num1;
                        else if (num2 > num3 && num1 > 1.0)
                            ktr1 = ktr / num1;
                        else if (num2 < num3 && num1 > 1.0)
                            ktr1 = ktr * num1;
                        double num4 = num1 == 1.0 || num2 <= num3 ? ktr1 * ktr1 : ktr * ktr;
                        dataRow["_r"] = (object)((double)dataRow["_r"] / num4);
                        dataRow["_x"] = (object)((double)dataRow["_x"] / num4);
                        dataRow["_g"] = (object)((double)dataRow["_g"] * num4);
                        dataRow["_b"] = (object)((double)dataRow["_b"] * num4);
                        if (nr1["_eq"].Equals((object)false))
                        {
                            nr1["_g"] = (object)((double)nr1["_g"] * num4);
                            nr1["_b"] = (object)((double)nr1["_b"] * num4);
                            nr1["_kt"] = (object)num4;
                        }
                    }
                    dataRow["_eq"] = (object)true;
                    nr1["_eq"] = (object)true;
                    this.Equivalent(nr1, ktr1);
                }
            }
        }
    }

    public class Const
    {
        public static double[] UNom = new double[10]
        {
      1150.0,
      750.0,
      500.0,
      330.0,
      220.0,
      110.0,
      35.0,
      10.5,
      6.3,
      0.4
        };
    }

    public enum TypePoter
    {
        Auto,
        dPQ,
        WQ,
        Kf,
        Tgf,
        Razn,
    }
}