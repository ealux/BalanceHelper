using System.IO;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace БалансДанные
{
    static class Parser_2
    {
        public static string ToXmlConverter_2(string path)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);

            path = path + "_buffer"; //Rename PATH

            xml.Save(path);
            xml = null;

            StreamReader streamR = new StreamReader(path);

            string s;
            s = streamR.ReadToEnd();

            s = s.Replace("\u0026lt;", "\u003C");
            s = s.Replace("\u0026gt;", "\u003E");

            streamR.Close();

            FileStream flStream = new FileStream(path, FileMode.Create);
            StreamWriter streamW = new StreamWriter(flStream);
            streamW.WriteLine(s);
            streamW.Close();

            return path;

        }
        
        public static void Statistic(XmlNode tables)
        {
            int regimsCount = tables.ChildNodes.Count; //Get table/regim count

            XmlNode firstTableNode = tables.ChildNodes[0]; //Get first regim - <Table/> level
            
            Data firstData = Data.InitData(firstTableNode);

            ExcelPackage p = new ExcelPackage();

            ExcelAdapter.Pre_ExcelOutputDesiger(p, regimsCount);
            var pwb = p.Workbook;

            int col = 2;

            //Nodes header filling
            foreach (Node node in firstData.Nodes)
            {
                pwb.Worksheets["Nodes"].Cells[1, col, 1, col+4].Merge = true;
                pwb.Worksheets["Nodes"].Cells[1, col, 1, col + 4].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Center;
                
                //Set borders
                pwb.Worksheets["Nodes"].Column(col).Style.Border.BorderAround(ExcelBorderStyle.Medium);
                pwb.Worksheets["Nodes"].Column(col).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                pwb.Worksheets["Nodes"].Column(col+1).Style.Border.BorderAround(ExcelBorderStyle.Thin);
                pwb.Worksheets["Nodes"].Column(col+2).Style.Border.BorderAround(ExcelBorderStyle.Thin);
                pwb.Worksheets["Nodes"].Column(col+3).Style.Border.BorderAround(ExcelBorderStyle.Thin);
                pwb.Worksheets["Nodes"].Column(col+4).Style.Border.Right.Style = ExcelBorderStyle.Medium; //Last column border - Medium

                pwb.Worksheets["Nodes"].Cells[1, col, 1, col+4].Value = $"Узел: {node.Number} Имя: {node.Name}";

                var filler = pwb.Worksheets["Nodes"].Cells[2, col];

                filler.Value = "Ризм";
                filler.Offset(0, 1).Value = "Ррасч";
                filler.Offset(0, 2).Value = "Ризм-Ррасч";
                filler.Offset(0, 3).Value = "Ризм-Ррасч %";
                filler.Offset(0, 4).Value = "δ %";

                col += 5;
            }

            //pwb.Worksheets["Nodes"].Cells[1,col,1, ]
            pwb.Worksheets["Nodes"].DeleteColumn(col, pwb.Worksheets["Nodes"].Cells.Columns - col);
            //pwb.Worksheets["Nodes"].Cells[1, col, 1, pwb.Worksheets["Nodes"].Cells.Columns - col].Columns

            col = 2;

            //Branches header filling
            foreach (Branch node in firstData.Branches)
            {
                pwb.Worksheets["Branches"].Cells[1, col, 1, col+5].Merge = true;
                pwb.Worksheets["Branches"].Cells[1, col, 1, col+5].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Center;

                //Set borders
                pwb.Worksheets["Branches"].Column(col).Style.Border.BorderAround(ExcelBorderStyle.Medium);
                pwb.Worksheets["Branches"].Column(col).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                pwb.Worksheets["Branches"].Column(col + 1).Style.Border.BorderAround(ExcelBorderStyle.Thin);
                pwb.Worksheets["Branches"].Column(col + 2).Style.Border.BorderAround(ExcelBorderStyle.Thin);
                pwb.Worksheets["Branches"].Column(col + 3).Style.Border.BorderAround(ExcelBorderStyle.Thin);
                pwb.Worksheets["Branches"].Column(col + 4).Style.Border.BorderAround(ExcelBorderStyle.Thin);
                pwb.Worksheets["Branches"].Column(col + 5).Style.Border.Right.Style = ExcelBorderStyle.Medium; //Last column border - Medium

                pwb.Worksheets["Branches"].Cells[1, col, 1, col+5].Value = $"Ветвь: {node.NumberStart}-{node.NumberEnd} Имя: {node.Name}";

                var filler = pwb.Worksheets["Branches"].Cells[2, col];

                filler.Value = "Ризм";
                filler.Offset(0, 1).Value = "Ррасч";
                filler.Offset(0, 2).Value = "Ризм-Ррасч";
                filler.Offset(0, 3).Value = "Ризм-Ррасч %";
                filler.Offset(0, 4).Value = "δ %";
                filler.Offset(0, 5).Value = "dP";

                col += 6;
            }

            //Headers ("Nodes") style tunning
            pwb.Worksheets["Nodes"].Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            pwb.Worksheets["Nodes"].Row(1).Style.Font.Bold = true;
            pwb.Worksheets["Nodes"].Row(1).Style.Border.BorderAround(ExcelBorderStyle.Medium);
            pwb.Worksheets["Nodes"].Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            pwb.Worksheets["Nodes"].Row(2).Style.Font.Bold = true;
            pwb.Worksheets["Nodes"].Row(2).Style.Border.BorderAround(ExcelBorderStyle.Medium);

            //Headers ("Branches") style tunning
            pwb.Worksheets["Branches"].Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            pwb.Worksheets["Branches"].Row(1).Style.Font.Bold = true;
            pwb.Worksheets["Branches"].Row(1).Style.Border.BorderAround(ExcelBorderStyle.Medium);
            pwb.Worksheets["Branches"].Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            pwb.Worksheets["Branches"].Row(2).Style.Font.Bold = true;
            pwb.Worksheets["Branches"].Row(2).Style.Border.BorderAround(ExcelBorderStyle.Medium);

            int row = 3;
            foreach (XmlNode table in tables.ChildNodes)
            {
                XmlNode _static = table.ChildNodes[0];

                FillData(pwb, _static, firstData, row);

                row++;
            }

            pwb.Worksheets["Nodes"].Cells.AutoFitColumns();
            pwb.Worksheets["Branches"].Cells.AutoFitColumns();

            ExcelAdapter.Save(p);
        }

        private static void FillData(ExcelWorkbook pwb, XmlNode _static, Data data, int row)
        {
            int col = 2;

            //Nodes
            foreach (Node node in data.Nodes)
            {
                XmlNode oprNode = _static.ChildNodes[0];

                foreach (XmlNode n in _static.ChildNodes)
                {
                    if (n.Name != "node") continue;

                    if (n["ny"].InnerText == node.Number.ToString())
                    {
                        oprNode = n;
                        break;
                    }
                }

                var pws = pwb.Worksheets["Nodes"];

                double pizm = oprNode["_p"] == null
                    ? 0.0
                    : oprNode["_p"].InnerText.Contains(".")
                        ? double.Parse(oprNode["_p"].InnerText.Replace(".", ","))
                        : double.Parse(oprNode["_p"].InnerText);

                double pras = oprNode["pras"] == null
                    ? 0.0
                    : oprNode["pras"].InnerText.Contains(".")
                        ? double.Parse(oprNode["pras"].InnerText.Replace(".", ","))
                        : double.Parse(oprNode["pras"].InnerText);

                double pizm_pras = pizm - pras;

                double pizm_pras_proc = pizm != 0.0 ? (pizm_pras / pizm) : 0.0;

                double ppogfull = oprNode["ppogfull"] == null
                    ? 0.0
                    : oprNode["ppogfull"].InnerText.Contains(".")
                        ? double.Parse(oprNode["ppogfull"].InnerText.Replace(".", ","))
                        : double.Parse(oprNode["ppogfull"].InnerText);

                pws.Cells[row, col].Value = pizm;
                    pws.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                pws.Cells[row, col+1].Value = pras;
                    pws.Cells[row, col+1].Style.Numberformat.Format = "#,##0.00";
                pws.Cells[row, col+2].Value = pizm_pras;
                    pws.Cells[row, col+2].Style.Numberformat.Format = "#,##0.00";
                pws.Cells[row, col+3].Value = pizm_pras_proc;
                    pws.Cells[row, col + 3].Style.Numberformat.Format = "#0.00%";
                pws.Cells[row, col+4].Value = ppogfull/100;
                    pws.Cells[row, col+4].Style.Numberformat.Format = "#0.00%";

                col += 5;
            }

            col = 2;

            //Branches
            foreach (Branch branch in data.Branches)
            {

                XmlNode oprNode = _static.ChildNodes[0];

                foreach (XmlNode n in _static.ChildNodes)
                {
                    if (n.Name != "vetv") continue;

                    if (n["ip"].InnerText == branch.NumberStart.ToString() &
                        n["iq"].InnerText == branch.NumberEnd.ToString())
                    {
                        oprNode = n;
                        break;
                    }
                }

                var pws = pwb.Worksheets["Branches"];

                double pizm = oprNode["_ipp"] == null
                    ? 0.0
                    : oprNode["_ipp"].InnerText.Contains(".")
                        ? double.Parse(oprNode["_ipp"].InnerText.Replace(".", ","))
                        : double.Parse(oprNode["_ipp"].InnerText);

                double pras = oprNode["ipp"] == null
                    ? 0.0
                    : oprNode["ipp"].InnerText.Contains(".")
                        ? double.Parse(oprNode["ipp"].InnerText.Replace(".", ","))
                        : double.Parse(oprNode["ipp"].InnerText);

                double pizm_pras = pizm - pras;

                double pizm_pras_proc = pizm != 0.0 ? (pizm_pras / pizm) : 0.0;

                double ppogfull = oprNode["ipppogfull"] == null
                    ? 0.0
                    : oprNode["ipppogfull"].InnerText.Contains(".")
                        ? double.Parse(oprNode["ipppogfull"].InnerText.Replace(".", ","))
                        : double.Parse(oprNode["ipppogfull"].InnerText);

                double dp = oprNode["dp"] == null
                    ? 0.0
                    : oprNode["dp"].InnerText.Contains(".")
                        ? double.Parse(oprNode["dp"].InnerText.Replace(".", ","))
                        : double.Parse(oprNode["dp"].InnerText);

                pws.Cells[row, col].Value = pizm;
                    pws.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                pws.Cells[row, col + 1].Value = pras;
                    pws.Cells[row, col + 1].Style.Numberformat.Format = "#,##0.00";
                pws.Cells[row, col + 2].Value = pizm_pras;
                    pws.Cells[row, col + 2].Style.Numberformat.Format = "#,##0.00";
                pws.Cells[row, col + 3].Value = pizm_pras_proc;
                    pws.Cells[row, col + 3].Style.Numberformat.Format = "#0.00%";
                pws.Cells[row, col + 4].Value = ppogfull/100;
                    pws.Cells[row, col + 4].Style.Numberformat.Format = "#0.00%";
                pws.Cells[row, col + 5].Value = dp;
                    pws.Cells[row, col + 5].Style.Numberformat.Format = "#,##0.00";

                col += 6;
            }
        }

    }
}
