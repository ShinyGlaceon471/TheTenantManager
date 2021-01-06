using System;
using System.Windows;
using System.Data;
using System.Data.SQLite;
using System.Configuration;
using Dapper;
using System.Windows.Controls;
using System.Text;

namespace TheTenantManager
{
    // Provide authentication and authorization of access to tenant information
    public partial class SignOnScreen : Window
    {
        TenantManager tenantManager = new TenantManager();

        public SignOnScreen()
        {
            InitializeComponent();
        }

        // Setup the connection string to be used for connecting to the database
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        private void SignOnButtonClick(object sender, RoutedEventArgs e)
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            // Open connection to database
            try
            {
                sqliteCon.Open();
                string signOnQuery = "select Username, Password from Credentials";
                SQLiteCommand createCommand = new SQLiteCommand(signOnQuery, sqliteCon);
                // createCommand.ExecuteNonQuery();
                SQLiteDataReader dr = createCommand.ExecuteReader();
                while (dr.Read())
                {
                    string username = dr.GetString(0);
                    string password = dr.GetString(1);

                    if ((username == UsernameBox.Text) || (password == PasswordBox.Password))
                    {
                        tenantManager.Show();
                        this.Close();
                    }
                }
                sqliteCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
