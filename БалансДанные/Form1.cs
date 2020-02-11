using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using OfficeOpenXml;

namespace БалансДанные
{
    public partial class Form1 : Form
    {
        public static DataGridView data { get; set; }
        public static TextBox pathexcel { get; set; }
        public static TextBox txtCount { get; set; }
        public static Label lblCount { get; set; }
        public static List<XmlNode> datas { get; set; }
        public static string path { get; set; }
        public Form1()
        {
            InitializeComponent();
            data = this.dataPresenter;
            pathexcel = this.pathExcel;
            txtCount = this.TxtCount;
            lblCount = this.LblCount;


            button1.Enabled = false;
        }
        //Это комент

        #region Table #1

        private void opnTempl_Click(object sender, EventArgs e)
        {
            string path = ExcelAdapter.Open(IsExcel: false)?[0] ;
            if (path == null) return;

            pathTempl.Text = path;

            //TODO:Think about BaseData replace into MainButton procedure!

            Parser.ToXmlConverter(path);
            Data.InitBaseData(Parser.PrepareXml(path)); //Create Base Regim
            Data.BaseData.DataRepresenter(); //Represent Base Regim

            button1.Enabled = true;
        }

        //MAIN WORK BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            int col = 0;

            try
            {
                col = Convert.ToInt16(txtCount.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка ввода количества значений или значений в Excel");
                return;
            }

            if (col <= 0)
            {
                MessageBox.Show("Ошибка ввода количества значений или значений в Excel");
                return;
            }

            datas = Parser.DataParserHelper(Parser.DataGridParser(data));
            datas = datas.Take(col).ToList();

            path = pathTempl.Text;

            XmlDocument xml = new XmlDocument();
            xml.Load(path);

            //УРОВЕНЬ <Tables>
            XmlElement Tables = xml.DocumentElement;

            Tables.RemoveChild(Tables.ChildNodes[0]);

            foreach (XmlNode dat in datas)
            {
                dat.InnerXml = dat.InnerXml.Replace("<", "&lt;").Replace(">", "&gt;");
                Tables.AppendChild(xml.ImportNode(dat, true));
            }


            xml.Save(path);
            datas = null;
            Data.BaseData = null;

            MessageBox.Show("Готов!");
        }

        private void opnExcel_Click(object sender, EventArgs e)
        {
            string path = ExcelAdapter.Open(IsExcel: true)?[0];
            if (path == null) return;

            using (ExcelPackage p = new ExcelPackage(new FileInfo(path)))
            {
                try
                {
                    var sheet = p.Workbook.Worksheets[1];
                    ExcelRangeBase cell = sheet.Cells[1, 1];

                    int col = 0;
                    int iter = 1;

                    while (NotInvalidText(cell.Value?.ToString()))
                    {
                        if (NotInvalidText(cell.Offset(0, 1).Value?.ToString()))
                        {
                            cell = cell.Offset(0, 1);
                            col = 2;
                            while (NotInvalidText(cell.Value?.ToString()))
                            {
                                col++;
                                cell = cell.Offset(0, 1);
                            }
                            break;
                        }
                        cell = cell.Offset(1, 0);
                        iter++;

                        if (iter > 50) break;
                    }

                    if (col != 0)
                    {
                        lblCount.Text = (col-1).ToString();
                        txtCount.Text = (col-1).ToString();
                    }

                }
                catch (Exception exception)
                {
                    MessageBox.Show("Ошибка входного файла Excel");
                    return;
                }
            }

            pathExcel.Text = path;
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
#endregion

        #region Table #2

        //Template open in Table #2. *.bbr file with regims array
        private void opnTempl2_Click(object sender, EventArgs e)
        {
            string path = ExcelAdapter.Open(IsExcel: false)?[0];
            if (path == null) return;
            path = Parser_2.ToXmlConverter_2(path);

            pathTempl2.Text = path;

            XmlDocument xml = new XmlDocument();
            xml.Load(path);

            //УРОВЕНЬ <Tables>
            XmlElement Tables = xml.DocumentElement;

            Parser_2.Statistic(Tables);

            //Parser.ToXmlConverter(path);
            //Data.InitBaseData(Parser.PrepareXml(path)); //Create Base Regim
            //Data.BaseData.DataRepresenter(); //Represent Base Regim
        }

        #endregion

    }
}
