using System;
using System.Collections.Generic;
using System.Xml;

namespace БалансДанные
{
    class Data
    {
        public static Data BaseData { get; set; } //Base Data to replicate

        public XmlNode SourceNode { get; set; } //Level Table - Источник XML формат

        //private XmlNode FinalNode { get; set; } //Level Table - Итоговый тип BalanceXML формат
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Branch> Branches { get; set; } = new List<Branch>();

        private static void AddData<T>(List<T> source, T t) => source.Add(t);

        //Initialize BaseData object
        public static void InitBaseData(XmlNode source) //Level Table
        {
            if (BaseData == null)
            {
                BaseData = new Data();
                BaseData.SourceNode = source;
                foreach (XmlNode s in source.ChildNodes[0]) //Level Static
                {
                    if (s.Name == "node")
                    {
                        AddData(BaseData.Nodes, new Node(s));
                    }
                    if (s.Name == "vetv")
                    {
                        AddData(BaseData.Branches, new Branch(s));
                    }
                }

            }
        }

        //Initialize Data object
        public static Data InitData(XmlNode source) //Level Table
        {
            Data data = new Data();
            data.SourceNode = source;
            foreach (XmlNode s in source.ChildNodes[0]) //Level Static
            {
                if (s.Name == "node")
                {
                    AddData(data.Nodes, new Node(s));
                }
                if (s.Name == "vetv")
                {
                    AddData(data.Branches, new Branch(s));
                }
            }

            return data;
        }

        public static Data CreateData(List<ExcData> exc, int step) //Level Static
        {
            var data = new Data();

            foreach (XmlNode source in BaseData.SourceNode.ChildNodes[0].ChildNodes)
            {
                if (source.Name == "node")
                {
                    AddData(data.Nodes, new Node(source.Clone()));
                }
                if (source.Name == "vetv")
                {
                    AddData(data.Branches, new Branch(source.Clone()));
                }
            }

            data.SourceNode = BaseData.SourceNode.Clone();

            //Regim attributes prepare
            data.SourceNode.Attributes["Id"].Value = step.ToString();
            data.SourceNode.Attributes["Name"].Value = step.ToString();
            data.SourceNode.Attributes["DataFile"].Value = Form1.pathexcel.Text;
            data.SourceNode.Attributes["ColId"].Value = (step+1).ToString();

            //Regim data prepare
            foreach (var e in exc)
            {
                if (e.Type == "node")
                {
                    foreach (Node node in data.Nodes)
                    {
                        if (node.Number == e.Start)
                        {
                            if (e.Pin) node.Pin = e.PinData[step];
                            if (e.Pout) node.Pout = e.PoutData[step];
                        }
                    }
                }
                if (e.Type == "branch")
                {
                    foreach (Branch branch in data.Branches)
                    {
                        if (branch.NumberStart == e.Start &
                            branch.NumberEnd == e.End)
                        {
                            if (e.Pin) branch.Pin = e.PinData[step];
                            if (e.Pout) branch.Pout = e.PoutData[step];
                        }
                    }
                }
            }

            //Prepare pre-final (in XML) nodes and branches
            data.ToFinalType();

            return data;
        }

        //Show data at DataGridView from Form1
        public void DataRepresenter()
        {
            int row = 0;
            Form1.data.Rows.Add(this.Nodes.Count + this.Branches.Count);
            foreach (Node node in this.Nodes)
            {
                Form1.data.Rows[row].Cells[0].Value = node.Number.ToString();
                Form1.data.Rows[row].Cells[0].ReadOnly = true;
                Form1.data.Rows[row].Cells[1].Value = node.Name.ToString();
                Form1.data.Rows[row].Cells[1].ReadOnly = true;
                Form1.data.Rows[row].Cells[2].Value = "Узел";
                Form1.data.Rows[row].Cells[2].ReadOnly = true;
                if (string.IsNullOrEmpty(node.ExcelPin)) Form1.data.Rows[row].Cells[3].Value = node.Pin.ToString();
                else Form1.data.Rows[row].Cells[3].Value = node.ExcelPin;
                if (string.IsNullOrEmpty(node.ExcelPout)) Form1.data.Rows[row].Cells[4].Value = node.Pout.ToString();
                else Form1.data.Rows[row].Cells[4].Value = node.ExcelPout;
                if (!String.IsNullOrEmpty(node.ExcelPin)) Form1.data.Rows[row].Cells[5].Value = node.ExcelPin;
                if (!String.IsNullOrEmpty(node.ExcelPout)) Form1.data.Rows[row].Cells[6].Value = node.ExcelPout;
                row++;
            }

            foreach (Branch node in this.Branches)
            {
                Form1.data.Rows[row].Cells[0].Value = node.NumberStart.ToString() + "-" + node.NumberEnd.ToString();
                Form1.data.Rows[row].Cells[0].ReadOnly = true;
                Form1.data.Rows[row].Cells[1].Value = node.Name.ToString();
                Form1.data.Rows[row].Cells[1].ReadOnly = true;
                Form1.data.Rows[row].Cells[2].Value = "Ветвь";
                Form1.data.Rows[row].Cells[2].ReadOnly = true;
                if (string.IsNullOrEmpty(node.ExcelPin)) Form1.data.Rows[row].Cells[3].Value = node.Pin.ToString();
                else Form1.data.Rows[row].Cells[3].Value = node.ExcelPin;
                if (string.IsNullOrEmpty(node.ExcelPout)) Form1.data.Rows[row].Cells[4].Value = node.Pout.ToString();
                else Form1.data.Rows[row].Cells[4].Value = node.ExcelPout;
                if (!String.IsNullOrEmpty(node.ExcelPin)) Form1.data.Rows[row].Cells[5].Value = node.ExcelPin;
                if (!String.IsNullOrEmpty(node.ExcelPout)) Form1.data.Rows[row].Cells[6].Value = node.ExcelPout;
                row++;
            }
        }

        public void ToFinalType()
        {
            //Prepare pre-final (in XML) nodes and branches
            foreach (Node n in this.Nodes)
            {
                n.PrepareSourceNode();
                foreach (XmlNode source in this.SourceNode.ChildNodes[0].ChildNodes)
                {
                    if (source.Name == "node" && source["ny"].InnerText == n.Number.ToString())
                        source.InnerXml =
                           n.SourceNode.InnerXml;
                           //SourceNode.ChildNodes[0].ReplaceChild(source, n.SourceNode.Clone());
                }
            }
            foreach (Branch b in this.Branches)
            {
                b.PrepareSourceNode();
                foreach (XmlNode source in this.SourceNode.ChildNodes[0].ChildNodes)
                {
                    if (source.Name == "vetv" && 
                        (source["ip"].InnerText == b.NumberStart.ToString()&
                         source["iq"].InnerText == b.NumberEnd.ToString())) source.InnerXml = b.SourceBranch.InnerXml;
                }
            }
        }
    }

    #region SubTypes

    public class Node
    {
        //Источник
        public XmlNode SourceNode { get; set; }
        //
        public int? Number { get; set; }
        public string Name { get; set; } = "";
        public double? Pout { get; set; }
        public double? Pin { get; set; }
        public string ExcelPin { get; set; }
        public string ExcelPout { get; set; }

        private void Initialize()
        {
            foreach (XmlNode node in SourceNode)
            {
                switch (node.Name)
                {
                    case "ny":
                        this.Number = Int32.Parse(node.InnerText);
                        break;
                    case "name":
                        this.Name = node.InnerText;
                        break;
                    case "pizmp":
                        if (node.InnerText.Contains(".")) this.Pin = Double.Parse(node.InnerText.Replace(".", ","));
                        else this.Pin = Double.Parse(node.InnerText);
                        break;
                    case "pizmo":
                        if (node.InnerText.Contains(".")) this.Pout = Double.Parse(node.InnerText.Replace(".", ","));
                        else this.Pout = Double.Parse(node.InnerText);
                        break;
                    case "pizmp_excel":
                        this.ExcelPin = node.InnerText;
                        break;
                    case "pizmo_excel":
                        this.ExcelPout = node.InnerText;
                        break;
                }
            }
        }

        public Node(){}

        public Node(XmlNode source)
        {
            this.SourceNode = source.Clone();
            Initialize();
        }

        public void PrepareSourceNode()
        {
            foreach (XmlNode node in SourceNode)
            {
                switch (node.Name)
                {
                    case "pizmp":
                        if (this.Pin.ToString().Contains(",")) node.InnerText = this.Pin.ToString().Replace(",", ".");
                        else node.InnerText = this.Pin.ToString();
                        break;
                    case "pizmo":
                        if (this.Pout.ToString().Contains(",")) node.InnerText = this.Pout.ToString().Replace(",", ".");
                        else node.InnerText = this.Pout.ToString();
                        break;
                }
            }
        }
    }

    public class Branch
    {
        //Источник
        public XmlNode SourceBranch { get; set; }
        //
        public int? NumberStart { get; set; }
        public int? NumberEnd { get; set; }
        public string Name { get; set; }
        public double? Pout { get; set; }
        public double? Pin { get; set; }
        public string ExcelPin { get; set; }
        public string ExcelPout { get; set; }

        private void Initialize()
        {
            foreach (XmlNode branch in SourceBranch)
            {
                switch (branch.Name)
                {
                    case "ip":
                        this.NumberStart = Int32.Parse(branch.InnerText);
                        break;
                    case "iq":
                        this.NumberEnd = Int32.Parse(branch.InnerText);
                        break;
                    case "name":
                        this.Name = branch.InnerText;
                        break;
                    case "iqpizmp":
                        if (branch.InnerText.Contains(".")) this.Pin = Double.Parse(branch.InnerText.Replace(".", ","));
                        else this.Pin = Double.Parse(branch.InnerText);
                        break;
                    case "ippizmo":
                        if (branch.InnerText.Contains(".")) this.Pout = Double.Parse(branch.InnerText.Replace(".", ","));
                        else this.Pout = Double.Parse(branch.InnerText);
                        break;
                    case "iqpizmp_excel":
                        this.ExcelPin = branch.InnerText;
                        break;
                    case "ippizmo_excel":
                        this.ExcelPout = branch.InnerText;
                        break;
                }
            }
        }

        public Branch(){}
        public Branch(XmlNode source)
        {
            this.SourceBranch = source.Clone();
            Initialize();
        }
        public void PrepareSourceNode()
        {
            foreach (XmlNode node in SourceBranch)
            {
                switch (node.Name)
                {
                    case "iqpizmp":
                        if (this.Pin.ToString().Contains(",")) node.InnerText = this.Pin.ToString().Replace(",", ".");
                        else node.InnerText = this.Pin.ToString();
                        break;
                    case "ippizmo":
                        if (this.Pout.ToString().Contains(",")) node.InnerText = this.Pout.ToString().Replace(",", ".");
                        else node.InnerText = this.Pout.ToString();
                        break;
                }
            }
        }

        public string Finalize()
        {
            return null;
        }
    }

    #endregion
}
