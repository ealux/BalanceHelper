using Balance.Host;
using BalanceCore;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;

namespace Balance.Data
{
    [Export(typeof(Plugin))]
    public class Data : Plugin, IPData
    {
        public void CmdNextBaseItemOnAction()
        {
            BarEditItem barItem = ((PluginCommand)this.riSelectedRegim.Tag).BarItem as BarEditItem;
            if (!(barItem.EditValue is DataRegimItem editValue))
                return;
            int num = this.dataRegimItems.IndexOf(editValue);
            if (this.dataRegimItems.Count <= num + 1)
                return;
            barItem.EditValue = (object)this.dataRegimItems[num + 1];
        }

        public Log log { get; set; }
        private object syncObject = new object();
        internal List<DataRegimItem> dataRegimItems = new List<DataRegimItem>();
        internal RepositoryItemGridLookUpEdit riSelectedRegim;
        private GridView riSelectedRegimView;
        public DataBase dataBase;
        private string pathSave;

        //internal static IPProtocol IP;
        internal static IPForm IF;

        //internal static IPGraph IG;

        private void InitializeComponent()
        {
            this.riSelectedRegim = new RepositoryItemGridLookUpEdit();
            this.riSelectedRegimView = new GridView
            {
                FocusRectStyle = DrawFocusRectStyle.RowFocus,
                Name = "riSelectedRegimView"
            };
            this.riSelectedRegimView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.riSelectedRegimView.OptionsView.ShowGroupPanel = false;
            this.riSelectedRegimView.OptionsView.ShowAutoFilterRow = true;
            this.riSelectedRegimView.OptionsBehavior.AutoPopulateColumns = false;
            this.riSelectedRegimView.OptionsView.ColumnAutoWidth = false;
            this.riSelectedRegimView.Columns.Add(new GridColumn()
            {
                Name = "Id",
                Caption = "Id",
                FieldName = "Id",
                Visible = true,
                VisibleIndex = 0,
                Width = 20
            });
            this.riSelectedRegimView.Columns.Add(new GridColumn()
            {
                Name = "Name",
                Caption = "Имя",
                FieldName = "Name",
                Visible = true,
                VisibleIndex = 1,
                Width = 50
            });
            this.riSelectedRegimView.Columns.Add(new GridColumn()
            {
                Name = "Description",
                Caption = "Описание",
                FieldName = "Description",
                Visible = true,
                VisibleIndex = 2,
                Width = 200
            });
            this.riSelectedRegimView.Columns.Add(new GridColumn()
            {
                Name = "TimeStart",
                Caption = "Время нач. изм.",
                FieldName = "TimeStartS",
                Visible = true,
                VisibleIndex = 3,
                Width = 100
            });
            this.riSelectedRegimView.Columns.Add(new GridColumn()
            {
                Name = "TimeEnd",
                Caption = "Время кон. изм.",
                FieldName = "TimeEndS",
                Visible = true,
                VisibleIndex = 4,
                Width = 100
            });
            this.riSelectedRegim.AutoHeight = false;
            this.riSelectedRegim.Name = "riSelectedRegim";
            this.riSelectedRegim.View = this.riSelectedRegimView;
            this.riSelectedRegim.DataSource = (object)this.dataRegimItems;
            this.riSelectedRegim.BeginInit();
            this.riSelectedRegimView.BeginInit();
        }

        public override string PluginCommonName => "Таблицы";

        public override int UniqueID => 100;

        public override bool IsSigleInstance => true;

        public override PluginInstance GetInstance()
        {
            if (this.dataBase == null) lock (this.syncObject) if (this.dataBase == null) this.dataBase = new DataBase((Plugin)this);
            return (PluginInstance)this.dataBase;
        }

        public override void GetPluginSettings(SettingsListData list)
        {
        }

        protected override void DoInitialize(PluginHost addinHost)
        {
        }

        public void DoInitialize2(PluginHost addinHost)
        {
            this.InitializeComponent();
            this.GetInstance();
            this.dataBase.LoadTables();
            this.dataBase.LoadTablesDefault();
            this.InitInfo();
            this.AddChangedEventPQ();
            PluginCommand pluginCommand13 = this.AddEditCommand(14, "EditBaseItem", (RepositoryItem)this.riSelectedRegim);
            this.riSelectedRegim.Tag = (object)pluginCommand13;
            pluginCommand13.Caption = "Активный режим";
            pluginCommand13.Description = "Активный режим";
            ((BarEditItem)pluginCommand13.BarItem).EditValueChanged += new EventHandler(this.CmdEditBaseItemChangedOnAction);
            PluginCommand pluginCommand14 = this.AddCommand(15, "NextBaseItem");
            pluginCommand14.Caption = "Следующий режим в базе";
            pluginCommand14.Description = "Следующий режим в базе";
            pluginCommand14.Action += new EventHandler(this.CmdNextBaseItemOnAction);
        }

        private void InitInfo()
        {
            System.Data.DataTable table1 = this.dataBase.GetTable("node");
            System.Data.DataTable table2 = this.dataBase.GetTable("vetv");
            System.Data.DataTable table3 = this.dataBase.GetTable("area");
            if (this.dataBase.GetTable("info") == null) return;
            if (table1 != null) table1.RowChanged += new DataRowChangeEventHandler(this.NodeOnRowChanged);
            if (table2 != null) table2.RowChanged += new DataRowChangeEventHandler(this.VetvOnRowChanged);
            if (table3 == null) return;
            table3.RowChanged += new DataRowChangeEventHandler(this.AreaOnRowChanged);
        }

        //private void ClearDataBaseItems()
        //{
        //  this.dataRegimItems.Clear();
        //  //(((PluginCommand) this.riSelectedRegim.Tag).BarItem as BarEditItem).EditValue = (object) null;
        //}

        public void LoadFile(string file)
        {
            //Balance.Data.Data.IF.BeginUpdate();
            this.BeginLoadDataTableAny();
            //this.ClearDataBaseItems();
            switch (Path.GetExtension(file).ToLower())
            {
                default:
                    this.dataBase.OpenDB(file);
                    //this.dataBase.LoadTablesDefault();
                    //this.cmdSave.Enabled = true;
                    this.pathSave = file;
                    break;
            }
            this.EndLoadDataTableAny();
            //Balance.Data.Data.IF.EndUpdate();
            //if (Balance.Data.Data.IG != null && Path.GetExtension(file).ToLower() == ".bgr")
            //  Balance.Data.Data.IG.RefreshGraphAllForms();
            this.AddToRecentFilesList(file);
            //this.PluginHost.ProgramMainForm.Text = "Balance4 - [" + file + "]";
            log.AddMessage("Загружен файл \"" + file + "\"", Log.MessageType.Info);
            //Balance.Data.Data.IP.AddMessage(0, "Загружен файл \"" + file + "\"", 0, 0);
        }

        private System.Data.DataTable GetActiveTable(ref int index)
        {
            string activeTable = Balance.Data.Data.IF.GetActiveTable(ref index);
            return !string.IsNullOrEmpty(activeTable) ? this.dataBase.GetTable(activeTable) : (System.Data.DataTable)null;
        }

        private void AddChangedEventPQ()
        {
            this.dataBase.GetTable("node").ColumnChanged += new DataColumnChangeEventHandler(this.NodeOnColumnChanged);
            this.dataBase.GetTable("vetv").ColumnChanged += new DataColumnChangeEventHandler(this.VetvOnColumnChanged);
        }

        private void RemoveChangedEventPQ()
        {
            this.dataBase.GetTable("node").ColumnChanged -= new DataColumnChangeEventHandler(this.NodeOnColumnChanged);
            this.dataBase.GetTable("vetv").ColumnChanged -= new DataColumnChangeEventHandler(this.VetvOnColumnChanged);
        }

        public void NodeOnColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "pizmp" && e.Row["pizmo"].Equals((object)DBNull.Value) || e.Column.ColumnName == "pizmo" && e.Row["pizmp"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "p");
                else
                    this.SetNullPog(e.Row, "p");
            }
            else if (e.Column.ColumnName == "qizmp" && e.Row["qizmo"].Equals((object)DBNull.Value) || e.Column.ColumnName == "qizmo" && e.Row["qizmp"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "q");
                else
                    this.SetNullPog(e.Row, "q");
            }
            else if (e.Column.ColumnName == "pizmpd" && e.Row["pizmod"].Equals((object)DBNull.Value) || e.Column.ColumnName == "pizmod" && e.Row["pizmpd"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "p", "d");
                else
                    this.SetNullPog(e.Row, "p", "d");
            }
            else
            {
                if ((!(e.Column.ColumnName == "qizmpd") || !e.Row["qizmod"].Equals((object)DBNull.Value)) && (!(e.Column.ColumnName == "qizmod") || !e.Row["qizmpd"].Equals((object)DBNull.Value)))
                    return;
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "q", "d");
                else
                    this.SetNullPog(e.Row, "q", "d");
            }
        }

        private void SetDefaultPog(DataRow row, string pq)
        {
            row[pq + "pogtt"] = (object)0.5;
            row[pq + "pogtn"] = (object)0.5;
            row[pq + "pogstn"] = (object)0.25;
            row[pq + "pogprib"] = (object)0.5;
            row[pq + "pogdop"] = (object)1.0;
        }

        private void SetDefaultPog(DataRow row, string pq, string d)
        {
            row[pq + "pogtt" + d] = (object)0.5;
            row[pq + "pogtn" + d] = (object)0.5;
            row[pq + "pogstn" + d] = (object)0.25;
            row[pq + "pogprib" + d] = (object)0.5;
            row[pq + "pogdop" + d] = (object)1.0;
        }

        private void SetNullPog(DataRow row, string pq)
        {
            row[pq + "pogtt"] = (object)DBNull.Value;
            row[pq + "pogtn"] = (object)DBNull.Value;
            row[pq + "pogstn"] = (object)DBNull.Value;
            row[pq + "pogprib"] = (object)DBNull.Value;
            row[pq + "pogdop"] = (object)DBNull.Value;
        }

        private void SetNullPog(DataRow row, string pq, string d)
        {
            row[pq + "pogtt" + d] = (object)DBNull.Value;
            row[pq + "pogtn" + d] = (object)DBNull.Value;
            row[pq + "pogstn" + d] = (object)DBNull.Value;
            row[pq + "pogprib" + d] = (object)DBNull.Value;
            row[pq + "pogdop" + d] = (object)DBNull.Value;
        }

        public void VetvOnColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "ippizmp" && e.Row["ippizmo"].Equals((object)DBNull.Value) || e.Column.ColumnName == "ippizmo" && e.Row["ippizmp"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "ipp");
                else
                    this.SetNullPog(e.Row, "ipp");
            }
            else if (e.Column.ColumnName == "iqpizmp" && e.Row["iqpizmo"].Equals((object)DBNull.Value) || e.Column.ColumnName == "iqpizmo" && e.Row["iqpizmp"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "iqp");
                else
                    this.SetNullPog(e.Row, "iqp");
            }
            else if (e.Column.ColumnName == "ipqizmp" && e.Row["ipqizmo"].Equals((object)DBNull.Value) || e.Column.ColumnName == "ipqizmo" && e.Row["ipqizmp"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "ipq");
                else
                    this.SetNullPog(e.Row, "ipq");
            }
            else if (e.Column.ColumnName == "iqqizmp" && e.Row["iqqizmo"].Equals((object)DBNull.Value) || e.Column.ColumnName == "iqqizmo" && e.Row["iqqizmp"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "iqq");
                else
                    this.SetNullPog(e.Row, "iqq");
            }
            if (e.Column.ColumnName == "ippizmpd" && e.Row["ippizmod"].Equals((object)DBNull.Value) || e.Column.ColumnName == "ippizmod" && e.Row["ippizmpd"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "ipp", "d");
                else
                    this.SetNullPog(e.Row, "ipp", "d");
            }
            else if (e.Column.ColumnName == "iqpizmpd" && e.Row["iqpizmod"].Equals((object)DBNull.Value) || e.Column.ColumnName == "iqpizmod" && e.Row["iqpizmpd"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "iqp", "d");
                else
                    this.SetNullPog(e.Row, "iqp", "d");
            }
            else if (e.Column.ColumnName == "ipqizmpd" && e.Row["ipqizmod"].Equals((object)DBNull.Value) || e.Column.ColumnName == "ipqizmod" && e.Row["ipqizmpd"].Equals((object)DBNull.Value))
            {
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "ipq", "d");
                else
                    this.SetNullPog(e.Row, "ipq", "d");
            }
            else
            {
                if ((!(e.Column.ColumnName == "iqqizmpd") || !e.Row["iqqizmod"].Equals((object)DBNull.Value)) && (!(e.Column.ColumnName == "iqqizmod") || !e.Row["iqqizmpd"].Equals((object)DBNull.Value)))
                    return;
                if (!DBNull.Value.Equals(e.ProposedValue))
                    this.SetDefaultPog(e.Row, "iqq", "d");
                else
                    this.SetNullPog(e.Row, "iqq", "d");
            }
        }

        private void AddToRecentFilesList(string filePath)
        {
            string str1 = filePath;
            string keyName = "HKEY_CURRENT_USER\\Software\\Balance4\\LastDoc";
            object obj = Registry.GetValue(keyName, "DocumentCount", (object)null);
            int num1 = obj == null ? 0 : (int)obj;
            if (num1 <= 0)
            {
                Registry.SetValue(keyName, "DocumentCount", (object)1);
                Registry.SetValue(keyName, "Document1", (object)str1);
            }
            else
            {
                List<string> stringList = new List<string>();
                int num2 = -1;
                for (int index = 0; index < 20 && Registry.GetValue(keyName, string.Format("Document{0}", (object)(index + 1)), (object)null) is string str2; ++index)
                {
                    stringList.Add(str2);
                    if (stringList[index].ToLower() == filePath.ToLower())
                        num2 = index;
                }
                if (num2 != -1)
                {
                    stringList.Insert(0, str1);
                    stringList.RemoveAt(num2 + 1);
                }
                else if (num1 > stringList.Count)
                {
                    stringList.Insert(0, str1);
                }
                else
                {
                    for (int index = stringList.Count - 1; index > 0; --index)
                        stringList[index] = stringList[index - 1];
                    stringList[0] = str1;
                }
                for (int index = 0; index < stringList.Count; ++index)
                    Registry.SetValue(keyName, string.Format("Document{0}", (object)(index + 1)), (object)stringList[index]);
            }
        }

        private void CmdEditBaseItemChangedOnAction(object sender, EventArgs e)
        {
            if (!((sender as BarEditItem).EditValue is DataRegimItem editValue))
                return;
            this.RemoveChangedEventPQ();
            this.dataBase.SetDataSet("static.brg", editValue.DataSetItem);
            this.AddChangedEventPQ();
        }

        private void CmdNextBaseItemOnAction(object sender, EventArgs e)
        {
            BarEditItem barItem = ((PluginCommand)this.riSelectedRegim.Tag).BarItem as BarEditItem;
            if (!(barItem.EditValue is DataRegimItem editValue))
                return;
            int num = this.dataRegimItems.IndexOf(editValue);
            if (this.dataRegimItems.Count <= num + 1)
                return;
            barItem.EditValue = (object)this.dataRegimItems[num + 1];
        }

        public bool IsBaseRegim => this.dataRegimItems.Count > 0;

        private void BeginLoadDataTableAny()
        {
            if (this.LoadDataTable == null)
                return;
            foreach (System.Data.DataTable table in this.dataBase.Tables)
                this.LoadDataTable(table.TableName, true);
        }

        private void EndLoadDataTableAny()
        {
            if (this.LoadDataTable == null)
                return;
            foreach (System.Data.DataTable table in this.dataBase.Tables)
                this.LoadDataTable(table.TableName, false);
        }

        private void NodeOnRowChanged(object sender, DataRowChangeEventArgs e) => this.RefreshRowColl((System.Data.DataTable)sender, "ny");

        private void VetvOnRowChanged(object sender, DataRowChangeEventArgs e) => this.RefreshRowColl((System.Data.DataTable)sender, "nv");

        private void AreaOnRowChanged(object sender, DataRowChangeEventArgs e) => this.RefreshRowColl((System.Data.DataTable)sender, "na");

        private void RefreshRowColl(System.Data.DataTable table, string colName)
        {
            System.Data.DataTable table1 = this.dataBase.GetTable("info");
            DataColumn column = table1.Columns[colName];
            if (table1 == null || column == null || table1.Rows.Count <= 0)
                return;
            table1.Rows[0][column] = (object)table.Rows.Count;
        }

        public event Balance.Host.LoadDataTable LoadDataTable;

        public IEnumerable Tables => this.dataBase.Tables;

        public System.Data.DataTable GetTable(string name)
        {
            return this.dataBase.GetTable(name);
        }

        public object[] DataRegimItems => (object[])new List<DataRegimItem>((IEnumerable<DataRegimItem>)this.dataRegimItems).ToArray();
    }

    public class DataRegimItem : IDataRegimItem
    {
        public DataRegimItem(int id, DataSet dataSetItem, DateTime timeStart, DateTime timeEnd)
        {
            this.Id = id;
            this.DataSetItem = dataSetItem;
            this.TimeStart = timeStart;
            this.TimeEnd = timeEnd;
        }

        public DataRegimItem(
                              int id,
                              DataSet dataSetItem,
                              DateTime timeStart,
                              DateTime timeEnd,
                              string name,
                              string description,
                              string dataFile,
                              string ColId)
                              : this(id, dataSetItem, timeStart, timeEnd)
        {
            this.Name = name;
            this.Description = description;
            this.DataFile = dataFile;
            this.ColId = ColId;
        }

        public DataSet DataSetItem { get; }

        public int Id { get; }

        public DateTime TimeStart { get; set; }

        public DateTime TimeEnd { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DataFile { get; set; }

        public string ColId { get; set; }

        public string TimeStartS => this.TimeStart.ToString("dd/MM/yyyy H:m t");

        public string TimeEndS => this.TimeEnd.ToString("dd/MM/yyyy H:m t");

        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.Name)
                    ? this.Name
                    : string.Format("{0}:{1}-{2}",
                                    (object)this.Id,
                                    (object)this.TimeStart.ToString("dd/MM/yyyy H:m t"),
                                    (object)this.TimeEnd.ToString("dd/MM/yyyy H:m t"));
        }
    }
}