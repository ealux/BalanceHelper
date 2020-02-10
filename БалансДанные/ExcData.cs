using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OfficeOpenXml;

namespace БалансДанные
{
    public class ExcData
    {
        public bool Pin { get; set; }
        public bool Pout { get; set; }
        public List<double> PinData { get; set; } = new List<double>();
        public List<double> PoutData { get; set; } = new List<double>();
        public string Type { get; set; }
        public int Start { get; set; }
        public int End { get; set; } = 0;

        //ExcData ctor
        public ExcData(Tuple<int, int, bool, bool> Row, DataGridView dataGrid, ExcelPackage p)
        {
            Regex vetvRegex = new Regex("(\\d*)-(\\d*)");

            //Prepare Type
            if (Row.Item2 == 0) this.Type = "node"; 
            else this.Type = "branch";                    

            //Preapare Pin/Pout positions 
            if (Row.Item3 & Row.Item4)
            {
                Pin = true;
                Pout = true;
            }
            else if (Row.Item3 & !Row.Item4)
            {
                Pin = true;
                Pout = false;
            }
            else if (!Row.Item3 & Row.Item4)
            {
                Pin = false;
                Pout = true;
            }

            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[0].Value.ToString() == "") continue;

                if (Type == "node")
                {
                    if (!vetvRegex.IsMatch(row.Cells[0].Value.ToString()) &&
                        Int32.Parse(row.Cells[0].Value.ToString()) == Row.Item1)
                    {
                        this.Start = Row.Item1;

                        Tuple<bool, bool> InOut = new Tuple<bool, bool>(this.Pin, this.Pout);

                        foreach (Node n in Data.BaseData.Nodes)
                        {
                            if (n.Number == this.Start)
                            {
                                if (InOut.Item1 & n.SourceNode["pizmp"] == null)
                                {
                                    n.SourceNode.InnerXml += "<pizmp></pizmp>";
                                }
                                if (InOut.Item2 & n.SourceNode["pizmo"] == null)
                                {
                                    n.SourceNode.InnerXml += "<pizmo></pizmo>";
                                }
                            }
                        }
                        CompleteLists(this, row, p);
                    }
                }

                if (Type == "branch")
                {
                    if (vetvRegex.IsMatch(row.Cells[0].Value.ToString()) &&
                        (Int32.Parse(row.Cells[0].Value.ToString().Split('-')[0]) == Row.Item1 &
                        Int32.Parse(row.Cells[0].Value.ToString().Split('-')[1]) == Row.Item2))
                    {
                        this.Start = Row.Item1;
                        this.End = Row.Item2;

                        Tuple<bool, bool> InOut = new Tuple<bool, bool>(this.Pin, this.Pout);

                        foreach (Branch b in Data.BaseData.Branches)
                        {
                            if (b.NumberStart == this.Start &&
                                b.NumberEnd == this.End)
                            {
                                if(InOut.Item1 & b.SourceBranch["iqpizmp"] == null)
                                {
                                    b.SourceBranch.InnerXml += "<iqpizmp></iqpizmp>";
                                }
                                if (InOut.Item2 & b.SourceBranch["ippizmo"] == null)
                                {
                                    b.SourceBranch.InnerXml += "<ippizmo></ippizmo>";
                                }
                            }
                        }

                        CompleteLists(this, row, p);
                    }
                }
            }
        }

        //Complete structure with excel data
        private static void CompleteLists(ExcData exc, DataGridViewRow row, ExcelPackage p)
        {
            Regex regex = new Regex("@(?<list>\\d*)#([A-Za-z]?)(?<row>\\d*)");
            int excList = 1;
            int excRow = 1;

            var Pin = exc.Pin;
            var Pout = exc.Pout;

            if (Pin)
            {
                excList = Int32.Parse(regex.Match(row.Cells[3].Value.ToString()).Groups["list"].Value);
                excRow = Int32.Parse(regex.Match(row.Cells[3].Value.ToString()).Groups["row"].Value);

                ExcelRangeBase col = p.Workbook.Worksheets[excList].Cells[excRow, 1];

                while (NotInvalidText(col.Value?.ToString()))
                {
                    try
                    {
                        double data = Double.Parse(col.Value.ToString());
                        exc.PinData.Add(data);
                    }
                    catch (Exception)
                    {
                        exc.PinData.Add(0.0);
                    }

                    col = col.Offset(0, 1);
                }
            }

            if (Pout)
            {
                excList = Int32.Parse(regex.Match(row.Cells[4].Value.ToString()).Groups["list"].Value.ToString());
                excRow = Int32.Parse(regex.Match(row.Cells[4].Value.ToString()).Groups["row"].Value);

                ExcelRangeBase col = p.Workbook.Worksheets[excList].Cells[excRow, 1];

                while (NotInvalidText(col.Value?.ToString()))
                {
                    try
                    {
                        double data = Double.Parse(col.Value.ToString());
                        exc.PoutData.Add(data);
                    }
                    catch (Exception)
                    {
                        exc.PoutData.Add(0.0);
                    }

                    col = col.Offset(0, 1);
                }
            }
        }

        //Check text correct form
        private static bool NotInvalidText(string str)
        {
            if (!string.IsNullOrWhiteSpace(str) &&
                !string.IsNullOrEmpty(str) &&
                str != "б/п" &&
                str != "-" &&
                str != "б/н" &&
                str != "" &&
                !str.StartsWith("Кор")) return true;
            return false;
        }
    }
}
