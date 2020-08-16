using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using static currency.Form1;

class Valute
{
    public void CreateTable(string conn)
    {
        String command = @"CREATE TABLE  Valute
(
   
    ValuteName NVARCHAR(50),
    ValuteValue NVARCHAR(50),
 ValuteNominal NVARCHAR(50), 
ValuteNumCode NVARCHAR(50),
 ValuteCharCode NVARCHAR(50),
ValuteDate NVARCHAR(50)
  
)";

        using (SqlConnection connection = new SqlConnection(conn))
        {

            connection.Open();
            SqlCommand sqlCommand = new SqlCommand(command, connection);
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch
            {

            }
        }
    }
    public void InsertCurrency(List<CurrencyValue> list, string conn)
    {
        using (SqlConnection connection = new SqlConnection(conn))
        {
            connection.Open();

            foreach (var item in list)
            {
                try
                {
                    string insert = String.Format("INSERT INTO Valute (ValuteName,ValuteValue,ValuteNominal,ValuteNumCode,ValuteCharCode,ValuteDate) VALUES ('{0}', '{1}','{2}','{3}','{4}','{5}')", item.Name, item.Value, item.Nominal, item.NumCode, item.CharCode, item.Date);
                    SqlCommand command = new SqlCommand(insert, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception Ex)
                {
                    //throw Ex;
                    MessageBox.Show(Ex.Message);
                }
            }




        }
    }

    public void DropTable(string conn)
    {
        using (SqlConnection connection = new SqlConnection(conn))
        {
            try
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("DROP TABLE Valute", connection);
                sqlCommand.ExecuteNonQuery();
            }
            catch
            {

            }

        }
    }
    public List<CurrencyValue> ReadCurrency(string conn, CurrencyValue currency)
    {
        var list = new List<CurrencyValue>();
        using (SqlConnection connection = new SqlConnection(conn))
        {
            string sqlcommand = $"SELECT * FROM Valute WHERE ValuteName='{currency.Name}' AND ValuteDate='{currency.Date}' ";
            try
            {
                connection.Open();

                SqlCommand sqlCommand = new SqlCommand(sqlcommand, connection);
                var reader = sqlCommand.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {
                        //   object id = reader["id"];
                        object name = reader["ValuteName"];
                        object value = reader["ValuteValue"];
                        object nominal = reader["ValuteNominal"];
                        object charcode = reader["ValuteCharCode"];
                        object numcode = reader["ValuteNumCode"];
                        list.Add(new CurrencyValue() { Name = name.ToString(), Value = value.ToString(), Nominal = nominal.ToString(), CharCode = charcode.ToString(), NumCode = numcode.ToString() });
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        return list;
    }
}