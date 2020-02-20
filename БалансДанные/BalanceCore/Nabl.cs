// Decompiled with JetBrains decompiler
// Type: Balance.Rgm.Nabl
// Assembly: Rgm, Version=1.0.6136.17908, Culture=neutral, PublicKeyToken=null
// MVID: 526B4102-62C1-4880-A947-0A573489DF22
// Assembly location: D:\Program Files (x86)\Balance4\plugins\Rgm.dll

using Balance.Helpers;
using Balance.Host;
using System;
using System.Collections.Generic;
using System.Data;

namespace Balance.Rgm
{
    internal class Nabl
    {
        private IPData IAD;
        private int rq;
        private readonly DataTable nodeT;
        private readonly DataTable vetvT;

        internal Nabl(IPData IAD)
        {
            this.IAD = IAD;
            this.nodeT = IAD.GetTable("node");
            this.vetvT = IAD.GetTable("vetv");
            this.rq = (int)IAD.GetTable("regim").Rows[0][nameof(rq)];
        }

        internal int GetNabl()
        {
            foreach (DataRow row in (InternalDataCollectionBase)this.nodeT.Rows)
                row["pnab"] = row["qnab"] = (object)true;
            foreach (DataRow row in (InternalDataCollectionBase)this.vetvT.Rows)
                row["pnab"] = row["qnab"] = (object)true;
            uint num1 = 1;
            while (true)
            {
                DataRow[] dataRowArray1 = this.nodeT.Select("gsvn = " + (object)num1);
                DataRow[] dataRowArray2 = this.vetvT.Select("gsvn = " + (object)num1);
                if (dataRowArray1.Length != 0)
                {
                    uint num2 = 0;
                    int[,] node = new int[dataRowArray1.Length, 3];
                    foreach (DataRow dataRow in dataRowArray1)
                    {
                        node[(int)(IntPtr)num2, 0] = (int)dataRow["ny"];
                        node[(int)(IntPtr)num2, 1] = !dataRow["pizmp"].Equals((object)DBNull.Value) || !dataRow["pizmo"].Equals((object)DBNull.Value) ? 1 : 0;
                        node[(int)(IntPtr)num2++, 2] = this.rq != 1 || !dataRow["qizmp"].Equals((object)DBNull.Value) || !dataRow["qizmo"].Equals((object)DBNull.Value) ? 1 : 0;
                    }
                    uint num3 = 0;
                    int[,] vetv = new int[dataRowArray2.Length, 5];
                    foreach (DataRow dataRow in dataRowArray2)
                    {
                        vetv[(int)(IntPtr)num3, 0] = (int)dataRow["ip"];
                        vetv[(int)(IntPtr)num3, 1] = (int)dataRow["iq"];
                        vetv[(int)(IntPtr)num3, 2] = (int)dataRow["np"];
                        vetv[(int)(IntPtr)num3, 3] = !dataRow["ippizmp"].Equals((object)DBNull.Value) || !dataRow["ippizmo"].Equals((object)DBNull.Value) || (!dataRow["iqpizmp"].Equals((object)DBNull.Value) || !dataRow["iqpizmo"].Equals((object)DBNull.Value)) ? 1 : 0;
                        vetv[(int)(IntPtr)num3++, 4] = this.rq != 1 || !dataRow["ipqizmp"].Equals((object)DBNull.Value) || (!dataRow["ipqizmo"].Equals((object)DBNull.Value) || !dataRow["iqqizmp"].Equals((object)DBNull.Value)) || !dataRow["iqqizmo"].Equals((object)DBNull.Value) ? 1 : 0;
                    }
                    for (int idNode = 0; idNode < node.Length / 3; ++idNode)
                        this.NablTest(idNode, ref node, ref vetv);
                    uint num4 = 0;
                    foreach (DataRow dataRow in dataRowArray1)
                    {
                        dataRow["precom"] = node[(int)(IntPtr)num4, 1] == 1 ? (object)"" : (object)"Добавить";
                        dataRow["qrecom"] = node[(int)(IntPtr)num4, 2] == 1 ? (object)"" : (object)"Добавить";
                        dataRow["pnab"] = (object)(node[(int)(IntPtr)num4, 1] == 1);
                        dataRow["qnab"] = (object)(node[(int)(IntPtr)num4++, 2] == 1);
                    }
                    uint num5 = 0;
                    foreach (DataRow dataRow in dataRowArray2)
                    {
                        dataRow["ipprecom"] = dataRow["iqprecom"] = vetv[(int)(IntPtr)num5, 3] == 1 ? (object)"" : (object)"Добавить";
                        dataRow["ipqrecom"] = dataRow["iqqrecom"] = vetv[(int)(IntPtr)num5, 4] == 1 ? (object)"" : (object)"Добавить";
                        dataRow["pnab"] = (object)(vetv[(int)(IntPtr)num5, 3] == 1);
                        dataRow["qnab"] = (object)(vetv[(int)(IntPtr)num5++, 4] == 1);
                    }
                    ++num1;
                }
                else
                    break;
            }
            int num6 = 0;
            if (this.nodeT.Select("sta = true AND (pnab = false OR qnab = false)").Length != 0)
                ++num6;
            if (this.vetvT.Select("sta = true AND (pnab = false OR qnab = false)").Length != 0)
                ++num6;
            return num6;
        }

        internal int GetNablReject()
        {
            uint num1 = 1;
            while (true)
            {
                DataRow[] dataRowArray1 = this.nodeT.Select("gsvn = " + (object)num1);
                DataRow[] dataRowArray2 = this.vetvT.Select("gsvn = " + (object)num1);
                if (dataRowArray1.Length != 0)
                {
                    uint num2 = 0;
                    int[,] node = new int[dataRowArray1.Length, 3];
                    foreach (DataRow dataRow in dataRowArray1)
                    {
                        node[(int)(IntPtr)num2, 0] = (int)dataRow["ny"];
                        node[(int)(IntPtr)num2, 1] = dataRow["pizmp"].Equals((object)DBNull.Value) && dataRow["pizmo"].Equals((object)DBNull.Value) || (int)dataRow["pbrak"] < 0 ? 0 : 1;
                        node[(int)(IntPtr)num2++, 2] = this.rq == 1 && dataRow["qizmp"].Equals((object)DBNull.Value) && dataRow["qizmo"].Equals((object)DBNull.Value) || (int)dataRow["qbrak"] < 0 ? 0 : 1;
                    }
                    uint num3 = 0;
                    int[,] vetv = new int[dataRowArray2.Length, 5];
                    foreach (DataRow dataRow in dataRowArray2)
                    {
                        vetv[(int)(IntPtr)num3, 0] = (int)dataRow["ip"];
                        vetv[(int)(IntPtr)num3, 1] = (int)dataRow["iq"];
                        vetv[(int)(IntPtr)num3, 2] = (int)dataRow["np"];
                        vetv[(int)(IntPtr)num3, 3] = (!dataRow["ippizmp"].Equals((object)DBNull.Value) || !dataRow["ippizmo"].Equals((object)DBNull.Value)) && (int)dataRow["ippbrak"] >= 0 || (!dataRow["iqpizmp"].Equals((object)DBNull.Value) || !dataRow["iqpizmo"].Equals((object)DBNull.Value)) && (int)dataRow["iqpbrak"] >= 0 ? 1 : 0;
                        vetv[(int)(IntPtr)num3++, 4] = this.rq != 1 || (!dataRow["ipqizmp"].Equals((object)DBNull.Value) || !dataRow["ipqizmo"].Equals((object)DBNull.Value)) && (int)dataRow["ipqbrak"] >= 0 || (!dataRow["iqqizmp"].Equals((object)DBNull.Value) || !dataRow["iqqizmo"].Equals((object)DBNull.Value)) && (int)dataRow["iqqbrak"] >= 0 ? 1 : 0;
                    }
                    for (int idNode = 0; idNode < node.Length / 3; ++idNode)
                        this.NablTest(idNode, ref node, ref vetv);
                    for (int index = 0; index < dataRowArray1.Length; ++index)
                    {
                        if (node[index, 1] != 1 || node[index, 2] != 1)
                            return -1;
                    }
                    for (int index = 0; index < dataRowArray2.Length; ++index)
                    {
                        if (vetv[index, 3] != 1 || vetv[index, 4] != 1)
                            return -1;
                    }
                    ++num1;
                }
                else
                    break;
            }
            return 0;
        }

        internal int GetNabl(int nodenonabl, int zone, char pq)
        {
            uint num1 = 1;
            while (true)
            {
                DataRow[] dataRowArray1 = this.nodeT.Select("gsvn = " + (object)num1);
                DataRow[] dataRowArray2 = this.vetvT.Select("gsvn = " + (object)num1);
                if (dataRowArray1.Length != 0)
                {
                    int index1 = 0;
                    List<int> intList1 = new List<int>();
                    intList1.Add(nodenonabl);
                    for (int index2 = 0; index2 < zone; ++index2)
                    {
                        List<int> intList2 = new List<int>();
                        foreach (int num2 in intList1)
                        {
                            foreach (DataRow dataRow in dataRowArray2)
                            {
                                if (dataRow["ip"].Equals((object)num2))
                                {
                                    if (!intList2.Contains((int)dataRow["iq"]))
                                        intList2.Add((int)dataRow["iq"]);
                                }
                                else if (dataRow["iq"].Equals((object)num2) && !intList2.Contains((int)dataRow["ip"]))
                                    intList2.Add((int)dataRow["ip"]);
                            }
                        }
                        foreach (int num2 in intList2)
                        {
                            if (!intList1.Contains(num2))
                                intList1.Add(num2);
                        }
                    }
                    if (intList1.Count != 1)
                    {
                        int[,] node = new int[intList1.Count, 3];
                        foreach (DataRow dataRow in dataRowArray1)
                        {
                            if (intList1.Contains((int)dataRow["ny"]))
                            {
                                node[index1, 0] = (int)dataRow["ny"];
                                node[index1, 1] = nodenonabl == node[index1, 0] || dataRow["pizmp"].Equals((object)DBNull.Value) && dataRow["pizmo"].Equals((object)DBNull.Value) ? 0 : 1;
                                node[index1++, 2] = this.rq != 1 || !dataRow["qizmp"].Equals((object)DBNull.Value) || !dataRow["qizmo"].Equals((object)DBNull.Value) ? 1 : 0;
                            }
                        }
                        int index2 = 0;
                        int[,] vetv = new int[dataRowArray2.Length, 5];
                        foreach (DataRow dataRow in dataRowArray2)
                        {
                            if (intList1.Contains((int)dataRow["ip"]) && intList1.Contains((int)dataRow["iq"]))
                            {
                                vetv[index2, 0] = (int)dataRow["ip"];
                                vetv[index2, 1] = (int)dataRow["iq"];
                                vetv[index2, 2] = (int)dataRow["np"];
                                vetv[index2, 3] = !dataRow["ippizmp"].Equals((object)DBNull.Value) || !dataRow["ippizmo"].Equals((object)DBNull.Value) || (!dataRow["iqpizmp"].Equals((object)DBNull.Value) || !dataRow["iqpizmo"].Equals((object)DBNull.Value)) ? 1 : 0;
                                vetv[index2++, 4] = this.rq != 1 || !dataRow["ipqizmp"].Equals((object)DBNull.Value) || (!dataRow["ipqizmo"].Equals((object)DBNull.Value) || !dataRow["iqqizmp"].Equals((object)DBNull.Value)) || !dataRow["iqqizmo"].Equals((object)DBNull.Value) ? 1 : 0;
                            }
                        }
                        vetv = (int[,])Static.ResizeArray((Array)vetv, new int[2]
                        {
                            index2,
                            5
                        });
                        for (int idNode = 0; idNode < node.Length / 3; ++idNode) this.NablTest(idNode, ref node, ref vetv);
                        for (int index3 = 0; index3 < node.Length / 3; ++index3) if (pq == 'p' && node[index3, 1] != 1 || pq == 'q' && this.rq == 1 && node[index3, 2] != 1) return 1;
                        for (int index3 = 0; index3 < vetv.Length / 5; ++index3) if (pq == 'p' && vetv[index3, 3] != 1 || pq == 'q' && this.rq == 1 && vetv[index3, 4] != 1) return 1;
                    }
                    ++num1;
                }
                else
                    break;
            }
            return 0;
        }

        internal int GetNabl(int q)
        {
            this.rq = q;
            return this.GetNabl();
        }

        private void NablTest(int idNode, ref int[,] node, ref int[,] vetv)
        {
            if (node[idNode, 1] == 0)
                node[idNode, 1] = this.NablNode(idNode, ref node, ref vetv, 3);
            else
                this.NablVetv(idNode, ref node, ref vetv, 3);
            if (node[idNode, 2] == 0)
                node[idNode, 2] = this.NablNode(idNode, ref node, ref vetv, 4);
            else
                this.NablVetv(idNode, ref node, ref vetv, 4);
        }

        private int NablNode(int idNode, ref int[,] node, ref int[,] vetv, int ar)
        {
            int num = node[idNode, 0];
            for (int index = 0; index < vetv.Length / 5; ++index)
            {
                if ((vetv[index, 0] == num || vetv[index, 1] == num) && vetv[index, ar] == 0)
                    return 0;
            }
            return 1;
        }

        private void NablVetv(int idNode, ref int[,] node, ref int[,] vetv, int ar)
        {
            int num1 = 0;
            int index1 = 0;
            int num2 = node[idNode, 0];
            for (int index2 = 0; index2 < vetv.Length / 5; ++index2)
            {
                if ((vetv[index2, 0] == num2 || vetv[index2, 1] == num2) && vetv[index2, ar] == 0)
                {
                    index1 = index2;
                    ++num1;
                    if (num1 > 1)
                        return;
                }
            }
            if (num1 != 1)
                return;
            vetv[index1, ar] = 1;
            int idNode1 = 0;
            int num3 = num2 == vetv[index1, 0] ? vetv[index1, 1] : vetv[index1, 0];
            for (int index2 = 0; index2 < node.Length / 3; ++index2)
            {
                if (num3 == node[index2, 0])
                {
                    idNode1 = index2;
                    break;
                }
            }
            this.NablTest(idNode1, ref node, ref vetv);
        }
    }
}