// Decompiled with JetBrains decompiler
// Type: Balance.Data.DataBase
// Assembly: Data, Version=1.0.6132.35299, Culture=neutral, PublicKeyToken=null
// MVID: 28C34725-BFA6-4134-B141-E00F73E07A0D
// Assembly location: D:\Program Files (x86)\Balance4\plugins\Data.dll

using ClassLibrary1.Properties;
using Balance.Helpers;
using Balance.Host;
using DevExpress.XtraBars;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace Balance.Data
{
    public class DataBase : PluginInstance
    {
        public Dictionary<string, DataSet> dataSetMain = new Dictionary<string, DataSet>();

        public DataBase(Plugin addin) : base(addin)
        {
        }

        internal bool LoadTable(string path)
        {
            string fileName = Path.GetFileName(path);
            if (!this.dataSetMain.ContainsKey(fileName))
                this.dataSetMain.Add(fileName, new DataSet());
            this.dataSetMain[fileName].ReadXmlSchema(path);
            return true;
        }

        public IEnumerable Tables
        {
            get
            {
                foreach (DataSet dataSet in this.dataSetMain.Values)
                {
                    foreach (object table in (InternalDataCollectionBase)dataSet.Tables)
                        yield return table;
                }
            }
        }

        public DataTable GetTable(string name)
        {
            foreach (DataSet dataSet in this.dataSetMain.Values)
            {
                foreach (DataTable table in (InternalDataCollectionBase)dataSet.Tables)
                {
                    if (table.TableName == name)
                        return table;
                }
            }
            return (DataTable)null;
        }

        public IEnumerable Shablons
        {
            get
            {
                foreach (string key in this.dataSetMain.Keys)
                    yield return (object)key;
            }
        }

        public DataSet GetDataSet(string name) => this.dataSetMain.ContainsKey(name) ? this.dataSetMain[name] : (DataSet)null;

        public bool SetDataSet(string name, DataSet ds)
        {
            if (!this.dataSetMain.ContainsKey(name))
                return false;
            this.dataSetMain[name] = ds;
            return true;
        }

        public string PathLoadXmlSchema { get; private set; }

        internal void SaveXmlSchemaAll()
        {
            foreach (string key in this.dataSetMain.Keys) this.dataSetMain[key].WriteXmlSchema(this.PathLoadXmlSchema + "\\" + key);
        }

        internal DataSet CreateDataSet(string name)
        {
            if (!this.dataSetMain.ContainsKey(name))
                this.dataSetMain.Add(name, new DataSet());
            return this.dataSetMain[name];
        }

        public void SaveDB(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            XmlTextWriter xmlTextWriter = new XmlTextWriter((Stream)fileStream, Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            try
            {
                xmlTextWriter.WriteStartDocument();
                xmlTextWriter.WriteStartElement("Tables");
                switch (Path.GetExtension(path).ToLower())
                {
                    case ".bbr":
                        using (IEnumerator<DataRegimItem> enumerator = ((Balance.Data.Data)this.plugin).dataRegimItems.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                DataRegimItem current = enumerator.Current;
                                xmlTextWriter.WriteStartElement("Table");
                                xmlTextWriter.WriteAttributeString("NameShablon", "static.brg");
                                xmlTextWriter.WriteAttributeString("Id", current.Id.ToString());
                                xmlTextWriter.WriteAttributeString("TimeStart", current.TimeStart.ToFileTime().ToString());
                                xmlTextWriter.WriteAttributeString("TimeEnd", current.TimeEnd.ToFileTime().ToString());
                                xmlTextWriter.WriteAttributeString("Description", current.Description);
                                xmlTextWriter.WriteAttributeString("Name", current.Name);
                                xmlTextWriter.WriteAttributeString("DataFile", current.DataFile);
                                xmlTextWriter.WriteAttributeString("ColId", current.ColId);
                                MemoryStream memoryStream = new MemoryStream();
                                current.DataSetItem.WriteXml((Stream)memoryStream, XmlWriteMode.IgnoreSchema);
                                xmlTextWriter.WriteString(Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));
                                xmlTextWriter.WriteEndElement();
                            }
                            break;
                        }
                    default:
                        using (Dictionary<string, DataSet>.KeyCollection.Enumerator enumerator = this.dataSetMain.Keys.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                string current = enumerator.Current;
                                xmlTextWriter.WriteStartElement("Table");
                                xmlTextWriter.WriteAttributeString("NameShablon", current);
                                MemoryStream memoryStream = new MemoryStream();
                                this.dataSetMain[current].WriteXml((Stream)memoryStream, XmlWriteMode.IgnoreSchema);
                                xmlTextWriter.WriteString(Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length));
                                xmlTextWriter.WriteEndElement();
                            }
                            break;
                        }
                }
                xmlTextWriter.WriteEndElement();
                xmlTextWriter.WriteEndDocument();
            }
            finally
            {
                xmlTextWriter.Close();
                fileStream.Close();
            }
        }

        internal void ClearTableErrors()
        {
            foreach (DataSet dataSet in this.dataSetMain.Values)
            {
                if (dataSet.HasErrors)
                {
                    foreach (DataTable table in (InternalDataCollectionBase)dataSet.Tables)
                    {
                        foreach (DataRow error in table.GetErrors()) error.ClearErrors();
                    }
                }
            }
        }

        public void OpenDB(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                using (new WaitCursor())
                {
                    xmlDocument.Load(path);
                    switch (Path.GetExtension(path).ToLower())
                    {
                        case ".bbr":
                            foreach (XmlNode childNode in xmlDocument.SelectSingleNode("Tables").ChildNodes)
                            {
                                string index1 = childNode.Attributes["NameShablon"].Value;
                                int id = int.Parse(childNode.Attributes["Id"].Value);
                                DateTime timeStart = DateTime.FromFileTime(long.Parse(childNode.Attributes["TimeStart"].Value));
                                DateTime timeEnd = DateTime.FromFileTime(long.Parse(childNode.Attributes["TimeEnd"].Value));
                                string description = childNode.Attributes["Description"].Value;
                                string name = childNode.Attributes["Name"].Value;
                                string dataFile = childNode.Attributes["DataFile"].Value;
                                string ColId = childNode.Attributes["ColId"].Value;
                                this.dataSetMain[index1].Clear();

                                try
                                {
                                    int num = (int)this.dataSetMain[index1].ReadXml((Stream)new MemoryStream(Encoding.UTF8.GetBytes(childNode.InnerText)), XmlReadMode.IgnoreSchema);
                                }
                                catch { }

                                foreach (DataTable table in (InternalDataCollectionBase)this.dataSetMain[index1].Tables)
                                {
                                    if (table.Rows.Count == 0 && table.MinimumCapacity != 0 && table.MinimumCapacity != 50)
                                    {
                                        for (int index2 = 0; index2 < table.MinimumCapacity; ++index2)
                                            table.Rows.Add();
                                    }
                                }

                                ((Balance.Data.Data)this.plugin).dataRegimItems.Add(new DataRegimItem(id, this.dataSetMain[index1].Copy(), timeStart, timeEnd, name, description, dataFile, ColId));
                            }
                            (((PluginCommand)((Balance.Data.Data)this.plugin).riSelectedRegim.Tag).BarItem as BarEditItem).EditValue = (object)((Balance.Data.Data)this.plugin).dataRegimItems[0];
                            break;

                        default:
                            IEnumerator enumerator = xmlDocument.SelectSingleNode("Tables").ChildNodes.GetEnumerator();
                            try
                            {
                                while (enumerator.MoveNext())
                                {
                                    XmlNode current = (XmlNode)enumerator.Current;
                                    string key = current.Attributes["NameShablon"].Value;
                                    if (this.dataSetMain.ContainsKey(key))
                                    {
                                        this.dataSetMain[key].Clear();
                                        foreach (DataTable table in (InternalDataCollectionBase)this.dataSetMain[key].Tables)
                                        {
                                            if (table.Rows.Count == 0 && table.MinimumCapacity != 0 && table.MinimumCapacity != 50)
                                            {
                                                for (int index = 0; index < table.MinimumCapacity; ++index) table.Rows.Add();
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                            finally
                            {
                                if (enumerator is IDisposable disposable) disposable.Dispose();
                            }
                    }
                }
            }
            catch (Exception) { }
        }

        internal void LoadTablesDefault() => this.OpenDB((Registry.GetValue(Resources.regPathMain, Resources.userFolder, (object)string.Empty) as string) + "\\" + Resources.defaultDB);

        internal void LoadTables()
        {
            string str1 = Registry.GetValue(Resources.regPathMain, Resources.userFolder, (object)string.Empty) as string;
            if (string.IsNullOrEmpty(str1))
                return;
            this.PathLoadXmlSchema = str1 + "\\" + Resources.tableFolder;
            string str2 = Registry.GetValue(Resources.regPathMain, Resources.tableReg, (object)string.Empty) as string;
            if (string.IsNullOrEmpty(str2))
                return;
            string str3 = str2;
            char[] chArray = new char[1] { ';' };
            foreach (string str4 in str3.Split(chArray))
            {
                string path = this.PathLoadXmlSchema + "\\" + str4;
                if (File.Exists(path))
                {
                    //if (Balance.Data.Data.IP != null)
                    //  Balance.Data.Data.IP.AddMessage(0, "Загружен шаблон \"" + str4 + "\"", 0, 0);
                    this.LoadTable(path);
                }
            }
        }

        internal void ClearDB()
        {
            foreach (DataSet dataSet in this.dataSetMain.Values) dataSet.Clear();
        }

        private void ClearExcelCells(DataSet ds)
        {
            List<string> stringList = new List<string>();
            foreach (DataTable table in (InternalDataCollectionBase)ds.Tables)
            {
                stringList.Clear();
                foreach (DataColumn column in (InternalDataCollectionBase)table.Columns)
                {
                    if (table.Columns.Contains(column.ColumnName + "_excel"))
                        stringList.Add(column.ColumnName);
                }
                foreach (DataRow row in (InternalDataCollectionBase)table.Rows)
                {
                    foreach (string index in stringList)
                    {
                        if (row[index + "_excel"] != DBNull.Value)
                            row[index] = (object)DBNull.Value;
                    }
                }
            }
        }

        public override bool IsControl => false;
    }
}