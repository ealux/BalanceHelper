using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using OfficeOpenXml;

namespace БалансДанные
{
    static class Parser
    {
        /// <summary>
        /// Конвертация файла .bbr в нормальный XLM файл и обратно
        /// </summary>
        /// <param name="path">Источник</param>
        /// <param name="toXlm">Если true - замена lt; и gt; на скобки,
        ///                     если false - замена скобок на lt; и gt;</param>
        public static void ToXmlConverter(string path)
        {

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
        }

        //Converter to normal XML structure with first Table object (regim №1 - base) output
        public static XmlNode PrepareXml(string path)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(path);

            //УРОВЕНЬ <Tables>
            XmlElement Tables = xml.DocumentElement;

            int count = Tables.ChildNodes.Count;

            if (count > 1)
            {
                for (int i = 1; i < count; i++)
                {
                    Tables.RemoveChild(Tables.ChildNodes[1]);
                }
            }

            xml.Save(path);
            //УРОВЕНЬ <Table>
            return Tables.ChildNodes[0];
        }

        //Balance-like cells input identificator. Let to point to Excel-taken cell
        public static List<Tuple<int, int, bool, bool>> DataGridParser(DataGridView dataGrid)
        {
            Regex regex = new Regex("@(\\d*)#([A-Za-z]?)(\\d*)");
            Regex vetvRegex = new Regex("(\\d*)-(\\d*)");

            List<Tuple<int, int, bool, bool>> Rows = new List<Tuple<int, int, bool, bool>>();

            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[0].Value.ToString() == "") continue;
                if (row.Cells[3].Value == null) row.Cells[3].Value = "";
                if (row.Cells[4].Value == null) row.Cells[4].Value = "";

                Tuple<int, int, bool, bool> r;
                if (regex.IsMatch(row.Cells[3].Value.ToString()))
                {
                    if (regex.IsMatch(row.Cells[4].Value.ToString()))
                    {
                        r = new Tuple<int, int, bool, bool>
                        (
                            !vetvRegex.IsMatch(row.Cells[0].Value.ToString())
                                ? Int32.Parse(row.Cells[0].Value.ToString())
                                : Int32.Parse(row.Cells[0].Value.ToString().Split('-')[0]),
                            !vetvRegex.IsMatch(row.Cells[0].Value.ToString())
                                ? 0
                                : Int32.Parse(row.Cells[0].Value.ToString().Split('-')[1]),
                            true,
                            true
                        );
                        Rows.Add(r);
                    }
                    else
                    {
                        r = new Tuple<int, int, bool, bool>
                        (
                            !vetvRegex.IsMatch(row.Cells[0].Value.ToString())
                                ? Int32.Parse(row.Cells[0].Value.ToString())
                                : Int32.Parse(row.Cells[0].Value.ToString().Split('-')[0]),
                            !vetvRegex.IsMatch(row.Cells[0].Value.ToString())
                                ? 0
                                : Int32.Parse(row.Cells[0].Value.ToString().Split('-')[1]),
                            true,
                            false
                        );
                        Rows.Add(r);
                    }
                }
                else if (regex.IsMatch(row.Cells[4].Value.ToString()) &&
                         !regex.IsMatch(row.Cells[3].Value.ToString()))
                {
                    r = new Tuple<int, int, bool, bool>
                    (
                        !vetvRegex.IsMatch(row.Cells[0].Value.ToString())
                            ? Int32.Parse(row.Cells[0].Value.ToString())
                            : Int32.Parse(row.Cells[0].Value.ToString().Split('-')[0]),
                        !vetvRegex.IsMatch(row.Cells[0].Value.ToString())
                            ? 0
                            : Int32.Parse(row.Cells[0].Value.ToString().Split('-')[1]),
                        false,
                        true
                    );
                    Rows.Add(r);
                }
                else continue;
            }

            //Additional xmlNodes upon complete DataGrid cells
            //if (Rows.Count > 0)
            //{
            //    foreach (var row in Rows)
            //    {
            //        if (row.Item2 == 0)
            //        {

            //        }
            //    }
            //}

            return Rows;

        }

        public static List<XmlNode> DataParserHelper(List<Tuple<int, int, bool, bool>> Rows)
        {
            if (Data.BaseData == null)
            {
                //MessageBox.Show("Отсутствует базовый узел");
                throw new ArgumentException("Отсутствует базовый узел");
            }

            if (String.IsNullOrEmpty(Form1.pathexcel.Text))
            {
                //MessageBox.Show("Не выбран источник Excel!");
                throw new ArgumentException("Не выбран источник Excel!");
            }

            if (Rows.Count < 1)
            {
                //MessageBox.Show("Не заполнены данные!");
                throw new ArgumentException("Не заполнены данные!");
            }

            ExcelPackage p = new ExcelPackage(new FileInfo(Form1.pathexcel.Text)); //Connect Excel

            List<ExcData> excDatas = new List<ExcData>();

            foreach (var row in Rows)
            {
                excDatas.Add(new ExcData(row, Form1.data, p));
            }

            //Find max capacity of Excel data
            int maxCapasity = 0;

            foreach (var x in excDatas)
            {
                maxCapasity = x.PinData.Count > 0 ? x.PinData.Count : x.PoutData.Count;
                //if (maxCapasity > 0) break;
            }
            if (maxCapasity == 0) MessageBox.Show("Данные Excel отсутствуетют");

            //Create new Data
            List<XmlNode> datas = new List<XmlNode>();

            for (int i = 0; i < maxCapasity; i++)
            {
                datas.Add(Data.CreateData(excDatas, i).SourceNode);
            }

            return datas;
        }

    }
}

