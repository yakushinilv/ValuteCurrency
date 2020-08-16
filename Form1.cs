using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using currency;
namespace currency
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Valute valute = new Valute();
        public string Get(string url)
        {
            string output = string.Empty;
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(1251)))
                {

                    output = reader.ReadToEnd();
                }
            }
            response.Close();
            return output;
        }

        public struct Currency
        {
            public string Name { get; set; }
            public string EngName { get; set; }
            public string Nominal { get; set; }

            public string ParentCode { get; set; }
            public string ISO_Num_Code { get; set; }
            public string ISO_Char_Code { get; set; }


        }

        public struct CurrencyValue
        {
            public string Name { get; set; }
            public string NumCode { get; set; }
            public string CharCode { get; set; }
            public string Nominal { get; set; }
            public string Value { get; set; }
            public string Date { get; set; }
        }

        public List<Currency> list = new List<Currency>();

        public List<CurrencyValue> GetCurrencyValue()
        {
            string output = Get($"http://www.cbr.ru/scripts/XML_daily.asp?date_req={dateTimePicker1.Value.ToString("dd.MM.yyyy")}");
            var listCurrencyValue = new List<CurrencyValue>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(output);
            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlElement xnode in xRoot)
            {
                string Name = "";
                string Value = "";
                string Nominal = "";
                string NumCode = "";
                string CharCode = "";

                foreach (XmlElement child in xnode.ChildNodes)
                {
                    if (child.Value != "")
                    {

                        if (child.Name.Trim() == "Name")
                        {
                            Name = child.InnerText;

                        }

                        if (child.Name.Trim() == "Value")
                        {
                            Value = child.InnerText;
                        }
                        if (child.Name.Trim() == "Nominal")
                        {
                            Nominal = child.InnerText;
                        }
                        if (child.Name.Trim() == "NumCode")
                        {
                            NumCode = child.InnerText;
                        }
                        if (child.Name.Trim() == "CharCode")
                        {
                            CharCode = child.InnerText;
                        }
                        
                      

                       
                    }
                
                }
                listCurrencyValue.Add(new CurrencyValue() { CharCode = CharCode, Value = Value, NumCode = NumCode, Nominal = Nominal, Name = Name, Date = dateTimePicker1.Value.ToString("dd.MM.yyyy") }) ;
            }
            return listCurrencyValue;    
        }


        public List<Currency> GetCurrency()
        {
            string output = Get("http://www.cbr.ru/scripts/XML_valFull.asp");

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(output);
            XmlElement xRoot = xDoc.DocumentElement;

            var currency = new List<Currency>();
            foreach (XmlElement xnode in xRoot)
            {


                //  User user = new User();
                // XmlNode attr = xnode.Attributes.GetNamedItem("Name");
                string Name = "";
                string EngName = "";
                string Nominal = "";
                string ParentCode = "";
                string ISO_Num_Code = "";
                string ISO_Char_Code = "";
                foreach (XmlElement child in xnode.ChildNodes)
                {



                    if (child.Name.Trim() == "Name")
                   
                        Name = child.InnerText;
                    if (child.Name.Trim() == "EngName")
                        EngName = child.InnerText;
                    if (child.Name.Trim() == "Nominal")
                        Nominal = child.InnerText;
                    if (child.Name.Trim() == "ParentCode")
                        ParentCode = child.InnerText;
                    if (child.Name.Trim() == "ISO_Num_Code")
                        ISO_Num_Code = child.InnerText;
                    if (child.Name.Trim() == "ISO_Char_Code")
                        ISO_Char_Code = child.InnerText;


                }
                
                currency.Add(new Currency() { Name = Name, EngName = EngName, ISO_Char_Code = ISO_Char_Code, ISO_Num_Code = ISO_Num_Code, Nominal = Nominal, ParentCode = ParentCode });



            }
            return currency;
        }

       


        private void Form1_Load(object sender, EventArgs e)
        {
           
            


            

            list = GetCurrency();
            foreach(var item in list)
                comboBox1.Items.Add(item.Name);

        }

        private void button1_Click(object sender, EventArgs e)
        {
          
            string conn = @"Server=localhost;Database=Currency;Trusted_Connection=True;";
            var listCurrencyValue=GetCurrencyValue();
            if(checkBox1.Checked)
            {
               valute.CreateTable(conn);
               valute.InsertCurrency(listCurrencyValue, conn);
            }
            var value = listCurrencyValue.FirstOrDefault(v => v.Name == comboBox1.Text);
            if (value.Value != null)
                MessageBox.Show($"{value.Name} = {value.Value} ");
            else
                MessageBox.Show("Валюта не найдена");
        }

        private void button2_Click(object sender, EventArgs e)
        {
       
            string conn = @"Server=localhost;Database=Currency;Trusted_Connection=True;";
            var list = valute.ReadCurrency(conn,new CurrencyValue() {Date=dateTimePicker1.Value.ToString("dd.MM.yyyy"),Name=comboBox1.Text });
            var value = list.FirstOrDefault(v => v.Name == comboBox1.Text);
            if (value.Value != null)
                MessageBox.Show($"{value.Name} = {value.Value} ");
            else
            {
                MessageBox.Show("Валюта не найдена");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            valute.DropTable(@"Server=localhost;Database=Currency;Trusted_Connection=True;");
        }
    }
}
