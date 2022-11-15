using BalanceCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

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
        private FileInfo buffer { get; set; }
        public Log Log { get; set; }

        public Form1()
        {
            InitializeComponent();
            data = this.dataPresenter;
            pathexcel = this.pathExcel;
            txtCount = this.TxtCount;
            lblCount = this.LblCount;
            Log = new Log(this.logBox);
            button1.Enabled = false;

            Log.AddMessage("Добро пожаловать в Balance Helper!\n", Log.MessageType.Info);

            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        //Это комент

        #region Table #1

        //OPEN TEMPLATE BUTTON
        private void opnTempl_Click(object sender, EventArgs e)
        {
            this.dataPresenter.Rows.Clear();

            string path = ExcelAdapter.Open(IsExcel: false)?[0];
            if (path == null) return;

            Log.AddMessage("Успешно прочитан файл: " + path, Log.MessageType.Success);

            pathTempl.Text = path;

            //TODO:Think about BaseData replace into MainButton procedure!

            FileInfo buffer = new FileInfo(path);
            if (File.Exists(path + "buffer")) File.Delete(path + "buffer");
            buffer = buffer.CopyTo(path + "buffer");

            Parser.ToXmlConverter(buffer.FullName);
            Data.InitBaseData(Parser.PrepareXml(buffer.FullName)); //Create Base Regim
            Data.BaseData.DataRepresenter();            //Represent Base Regim

            if (String.IsNullOrEmpty(pathexcel.Text) | String.IsNullOrEmpty(pathTempl.Text)) button1.Enabled = false;
            else button1.Enabled = true;
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
                Log.AddMessage("Ошибка ввода количества значений или значений в Excel", Log.MessageType.Error);
                return;
            }

            if (col <= 0)
            {
                MessageBox.Show("Ошибка ввода количества значений или значений в Excel");
                Log.AddMessage("Ошибка ввода количества значений или значений в Excel", Log.MessageType.Error);
                return;
            }

            Log.AddMessage("Подготовка данных...", Log.MessageType.Info);

            XmlDocument xml = new XmlDocument();
            path = pathTempl.Text;
            if (!File.Exists(path + "buffer"))
            {
                buffer = new FileInfo(path);
                buffer = buffer.CopyTo(path + "buffer");
            }
            else buffer = new FileInfo(path + "buffer");

            try
            {
                Parser.ToXmlConverter(buffer.FullName);
                Data.InitBaseData(Parser.PrepareXml(buffer.FullName)); //Create Base Regim
                datas = Parser.DataParserHelper(Parser.DataGridParser(data));
                datas = datas.Take(col).ToList();

                xml.Load(buffer.FullName);
            }
            catch (Exception ex)
            {
                Log.AddMessage("Ошибка данных в Excel! Возможно несовпадение структуры данных значениям в ячейках! Проверьте ссылки!", Log.MessageType.Error);
                Log.AddMessage("Данные по ошибке: " + ex.Message, Log.MessageType.Error);
                File.Delete(buffer.FullName);
                return;
            }

            //УРОВЕНЬ <Tables>
            XmlElement Tables = xml.DocumentElement;

            Tables.RemoveChild(Tables.ChildNodes[0]);

            foreach (XmlNode dat in datas)
            {
                dat.InnerXml = dat.InnerXml.Replace("<", "&lt;").Replace(">", "&gt;");
                Tables.AppendChild(xml.ImportNode(dat, true));
            }

            Log.AddMessage("Файл успешно преобразован.\n", Log.MessageType.Success);

            //try
            //{
            //    BalanceCore.BalanceCore core = new BalanceCore.BalanceCore(path, Log);
            //    Log.AddMessage("Затрачено на расчёт: " + core.Calculate() + " cек\n", Log.MessageType.Info);
            //}
            //catch (Exception ex)
            //{
            //    Log.AddMessage("Данные по ошибке: " + ex.Message, Log.MessageType.Error);
            //}

            xml.Save(path);
            Log.AddMessage("База сохранена по адресу: " + path + "\n", Log.MessageType.Info);
            datas = null;
            Data.BaseData = null;
            //File.Delete(buffer.FullName);

            try
            {
                path = path.Replace(".bbr", "_Невязки.bbr");
                Output(path);
            }
            catch (Exception)
            {
            }

            //MessageBox.Show("Готов!");
        }

        //OPEN EXCEL BUTTON
        private void opnExcel_Click(object sender, EventArgs e)
        {
            string path = ExcelAdapter.Open(IsExcel: true)?[0];
            if (path == null) return;

            lblCount.Text = String.Empty;
            txtCount.Text = String.Empty;

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
                        lblCount.Text = (col - 1).ToString();
                        txtCount.Text = (col - 1).ToString();
                    }

                    if (String.IsNullOrEmpty(txtCount.Text))
                    {
                        Log.AddMessage("Ошибка структуры файла Excel! Проверьте содержимое файла: " + path, Log.MessageType.Error);
                        pathexcel.Clear();
                        if (String.IsNullOrEmpty(pathTempl.Text) | String.IsNullOrEmpty(pathTempl.Text)) button1.Enabled = false;
                    }
                    else
                    {
                        Log.AddMessage("Успешно прочитан файл Excel: " + path, Log.MessageType.Success);
                        pathExcel.Text = path;
                        if (String.IsNullOrEmpty(pathTempl.Text) | String.IsNullOrEmpty(pathTempl.Text)) button1.Enabled = false;
                        else button1.Enabled = true;
                    }
                }
                catch (Exception)
                {
                    Log.AddMessage("Ошибка открытия файла Excel: " + path, Log.MessageType.Error);
                    pathexcel.Clear();
                    button1.Enabled = false;
                    return;
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

        #endregion Table #1

        #region Table #2

        //Template open in Table #2. *.bbr file with regims array
        private void opnTempl2_Click(object sender, EventArgs e)
        {
            string path = ExcelAdapter.Open(IsExcel: false)?[0];
            if (path == null) return;

            Output(path);
        }

        private string Output(string path)
        {
            path = Parser_2.ToXmlConverter_2(path);

            pathTempl2.Text = path;

            XmlDocument xml = new XmlDocument();
            xml.Load(path);

            //УРОВЕНЬ <Tables>
            XmlElement Tables = xml.DocumentElement;

            if(File.Exists(path))File.Delete(path);

            string output = Parser_2.Statistic(Tables);

            Log.AddMessage("База успешно выгружена в файл: " + output + "\n", Log.MessageType.Info);
            return output;
        }

        #endregion Table #2
    }
}