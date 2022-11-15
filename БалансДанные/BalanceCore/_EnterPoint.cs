using Balance.Data;
using Balance.Rgm;
using System;
using System.Diagnostics;
using System.IO;

namespace BalanceCore
{
    public class BalanceCore
    {
        private Log log;
        public string Path { get; set; }
        public string ElapseTime { get; set; }
        public bool IsCalculated { get; set; }

        private Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Create calculation object
        /// </summary>
        /// <param name="path">File to calculation absolute path</param>
        public BalanceCore(string path, Log log)
        {
            Path = path;
            this.log = log;
        }

        /// <summary>
        /// Main calculation procedure
        /// </summary>
        public string Calculate()
        {
            //Check file existence
            if (String.IsNullOrEmpty(Path))
            {
                IsCalculated = false;
                log.AddMessage("Ошибка файла!", Log.MessageType.Error);
                throw new Exception("Возможно файл был удалён?");
            }

            sw.Start();

            FileInfo file = new FileInfo(Path);
            string saveName = file.FullName.Replace(".bbr", "_Невязки.bbr");
            AddinStaticRgm rgm = new AddinStaticRgm();

            try
            {
                Data data = new Data(AddinStaticRgm.ID);
                data.log = this.log;
                var data_base = data.dataBase;

                data.DoInitialize2(data.PluginHost);
                data.LoadFile(file.FullName);

                rgm.log = this.log;

                rgm.DoInitialize2(data.PluginHost, data);
                rgm.RunRgm(0);

                for (int i = 0; i < rgm.ldata.DataRegimItems.Length; i++)
                {
                    rgm.ldata.CmdNextBaseItemOnAction();
                    rgm.RunRgm(0);
                }

                log.AddMessage("Файл успешно расчитан!", Log.MessageType.Success);
                rgm.ldata.dataBase.SaveDB(saveName);
                log.AddMessage("Файл успешно сохранен:" + saveName, Log.MessageType.Success);
            }
            catch (Exception ex)
            {
                log.AddMessage("Ошибка расчета!", Log.MessageType.Error);
                File.Delete(saveName);
                throw new Exception("Ошибка расчёта! Необходимо проверить файл!\nСтек системной ошибки: " + ex.Message);
            }
            finally
            {
                sw.Stop();
                log.AddMessage("Ошибка расчёта!", Log.MessageType.Error);
                rgm.ldata.dataBase.SaveDB(saveName);
                log.AddMessage("Файл успешно сохранен:" + saveName, Log.MessageType.Info);
            }
            return sw.Elapsed.ToString();
        }
    }
}