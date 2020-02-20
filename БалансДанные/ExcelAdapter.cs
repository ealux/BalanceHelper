using OfficeOpenXml;
using System.IO;
using System.Windows.Forms;

namespace БалансДанные
{
    internal static class ExcelAdapter
    {
        private static string[] Files;

        /// <summary>
        /// Возвращает массив строк-путей к выбранным файлам
        /// </summary>
        /// <param name="IsExcel">Выбор только Excel файлов, если False - доступ:
        ///                        resultString[0]</param>
        /// <returns></returns>
        public static string[] Open(bool IsExcel)
        {
            string filt = IsExcel ? "Excel Files|*.xls;*.xlsx;*.xlsm" : "База режимов|*.bbr";
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = filt,
                Multiselect = false
            };

            if (fileDialog.ShowDialog() != DialogResult.Cancel)
            {
                Files = new string[] { };
                Files = fileDialog.FileNames;
                fileDialog.Dispose();
                return Files;
            }

            fileDialog.Dispose();
            return null;
        }

        public static void Save(ExcelPackage p)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls;*.xlsm",
                OverwritePrompt = true
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) return;

            string filename = saveFileDialog1.FileName;

            p.SaveAs(new FileInfo(filename));
        }

        //Prepare Excel file to data output
        public static void Pre_ExcelOutputDesiger(ExcelPackage p, int regimsCount)
        {
            var wb = p.Workbook;

            var sheetNodes = wb.Worksheets.Add("Nodes");
            var sheetBranches = wb.Worksheets.Add("Branches");

            sheetNodes.Cells[2, 1].Value = "Id";
            sheetBranches.Cells[2, 1].Value = "Id";

            for (int i = 0; i < regimsCount; i++)
            {
                sheetNodes.Cells[i + 3, 1].Value = i;
                sheetBranches.Cells[i + 3, 1].Value = i;
            }
        }
    }
}