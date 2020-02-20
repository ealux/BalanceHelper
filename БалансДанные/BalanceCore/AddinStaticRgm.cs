﻿// Decompiled with JetBrains decompiler
// Type: Balance.Rgm.AddinStaticRgm
// Assembly: Rgm, Version=1.0.6136.17908, Culture=neutral, PublicKeyToken=null
// MVID: 526B4102-62C1-4880-A947-0A573489DF22
// Assembly location: D:\Program Files (x86)\Balance4\plugins\Rgm.dll

using Balance.Host;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using BalanceCore;

namespace Balance.Rgm
{
    [Export(typeof(Plugin))]
    public class AddinStaticRgm : Plugin
    {
        public Data.Data ldata;
        public Log log { get; set; }

        public class Local_Data : IPData
        {
            public Data.Data data;

            public Local_Data(Data.Data data) => this.data = data;

            public IEnumerable Tables => this.data.Tables;

            public object[] DataRegimItems => this.data.DataRegimItems;

            public event LoadDataTable LoadDataTable;

            private DataTable GetTable(string name) => throw new NotImplementedException();

            DataTable IPData.GetTable(string name) => this.data.GetTable(name);
        }

        internal static IPData ID;
        internal static IPProtocol IP;

        //internal static IPForm IF;
        //internal static IPGraph IG;
        //internal static PluginHost PH;
        private StaticRgm rgm;

        public BackgroundWorker bw;

        public override string PluginCommonName => "Режим";

        public override int UniqueID => 200;

        public override bool IsSigleInstance => false;

        public override PluginInstance GetInstance()
        {
            return (PluginInstance)null;
        }

        public override void GetPluginSettings(SettingsListData list)
        {
        }

        protected override void DoInitialize(PluginHost addinHost)
        {
        }

        public void DoInitialize2(PluginHost addinHost, Data.Data data)
        {
            ldata = data;
            AddinStaticRgm.ID = ldata as IPData;

            this.rgm = new StaticRgm();
            this.rgm.log = this.log;
        }

        private void CmdOnRgm(object sender, EventArgs e) => this.RunRgm(1);

        public void RunRgm(int param) => this.rgm.RgmThread();
    }
}