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
    public partial class TenantManager : Window
    {
        public TenantManager()
        {
            InitializeComponent();
            FillListBox();
            LoadDataGrid();
        }

        // Setup the connection string to be used for connecting to the database
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        void LoadDataGrid()
        {
            // This will load existing information into the data grid
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            // Open connection to database
            try
            {
                sqliteCon.Open();
                // Use this to hide variables
                string selectQuery1 = "select ID AS 'Tenant ID', RoomNumber AS 'Room Number', Name AS 'Tenant Name', Address AS 'Tenant Address', Phone AS 'Tenant Phone', Email AS 'Tenant Email', " +
                                      "LeaseStart AS 'Lease Start', LeaseEnd AS 'Lease End', FreeWeek AS 'Free Week', FreeWeekDate AS 'Free Week Date', OperatorLicense AS 'Operator Lic', " +
                                      "OperatorLicEXPDate AS 'Operator Exp Date', MiniSalonLicense AS 'Mini Salon Lic #', MiniSalonLicEXPDate AS 'Mini Salon Exp Date' from Tenants ";
                SQLiteCommand createCommand = new SQLiteCommand(selectQuery1, sqliteCon);
                createCommand.ExecuteNonQuery();

                SQLiteDataAdapter dataAdp = new SQLiteDataAdapter(createCommand);
                DataTable dt = new DataTable("Tenants");
                dataAdp.Fill(dt);
                dataGridTenants.ItemsSource = dt.DefaultView;
                dataAdp.Update(dt);
                sqliteCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // This closes the application
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Environment.Exit(0); // Closes the background process to avoid a memory leak
        }

        // Checks for renewals within 30 days
        private void CheckForRenewals(object sender, RoutedEventArgs e)
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            StringBuilder renewalString = new StringBuilder();
            DateTime today = DateTime.Now;
            try
            {
                sqliteCon.Open();
                string renewalQuery = "select * from Tenants";
                SQLiteCommand createCommand = new SQLiteCommand(renewalQuery, sqliteCon);
                // createCommand.ExecuteNonQuery();
                SQLiteDataReader dr = createCommand.ExecuteReader();
                while (dr.Read())
                {
                    DateTime tenantLeaseEnd = Convert.ToDateTime(dr.GetString(7));
                    DateTime tenantOperatorLicRenew = Convert.ToDateTime(dr.GetString(11));
                    DateTime tenantMiniSalonRenew = Convert.ToDateTime(dr.GetString(13));
                    if ((tenantLeaseEnd <= today.AddDays(30)) || (tenantOperatorLicRenew <= today.AddDays(30)) || (tenantMiniSalonRenew <= today.AddDays(30)))
                    {
                        string tenantRoomNumber = dr.GetInt32(1).ToString();
                        string tenantName = dr.GetString(2);
                        string tenantAddress = dr.GetString(3);
                        string tenantPhone = dr.GetString(4);
                        string tenantEmail = dr.GetString(5);
                        string tenantLeaseStart = dr.GetString(6);
                        string tenantFreeWeek = dr.GetString(8);
                        string tenantFreeWeekDate = dr.GetString(9);
                        string tenantOperatorLic = dr.GetString(10);
                        string tenantMiniSalonLic = dr.GetString(12);
                        renewalString.AppendLine("Room Number: " + tenantRoomNumber);
                        renewalString.AppendLine("Name: " + tenantName);
                        renewalString.AppendLine("Address: " + tenantAddress);
                        renewalString.AppendLine("Phone: " + tenantPhone);
                        renewalString.AppendLine("Email: " + tenantEmail);
                        renewalString.AppendLine("Lease Start: " + tenantLeaseStart);
                        renewalString.AppendLine("Lease End: " + tenantLeaseEnd.ToString());
                        renewalString.AppendLine("Free Week: " + tenantFreeWeek);
                        renewalString.AppendLine("Free Week Date: " + tenantFreeWeekDate);
                        renewalString.AppendLine("Operator License: " + tenantOperatorLic);
                        renewalString.AppendLine("Operator License EXP Date: " + tenantOperatorLicRenew.ToString());
                        renewalString.AppendLine("Mini Salon License: " + tenantMiniSalonLic);
                        renewalString.AppendLine("Mini Salon License EXP Date: " + tenantMiniSalonRenew.ToString());
                        MessageBox.Show("This tenant has a renewal in 30 days:\n" + renewalString.ToString());
                        renewalString.Clear();
                    }
                }
                sqliteCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // User selects ID from the drop down combo box. A message box will be displayed showing the info to be printed, then a print dialog box will appear
        private void PrintInfo(object sender, RoutedEventArgs e)
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            StringBuilder printString = new StringBuilder();
            Label tenantLabel = new Label();
            // Open connection to database
            try
            {
                sqliteCon.Open();
                string printQuery = "select * from Tenants where (cast(ID as TEXT) || ' : ' || Name) ='" + IDandNameBox.SelectedItem + "' ";
                SQLiteCommand createCommand = new SQLiteCommand(printQuery, sqliteCon);
                // createCommand.ExecuteNonQuery();
                SQLiteDataReader dr = createCommand.ExecuteReader();
                while (dr.Read())
                {
                    string tenantRoomNumber = dr.GetInt32(1).ToString();
                    string tenantName = dr.GetString(2);
                    string tenantAddress = dr.GetString(3);
                    string tenantPhone = dr.GetString(4);
                    string tenantEmail = dr.GetString(5);
                    string tenantLeaseStart = dr.GetString(6);
                    string tenantLeaseEnd = dr.GetString(7);
                    string tenantFreeWeek = dr.GetString(8);
                    string tenantFreeWeekDate = dr.GetString(9);
                    string tenantOperatorLic = dr.GetString(10);
                    string tenantOperatorLicRenew = dr.GetString(11);
                    string tenantMiniSalonLic = dr.GetString(12);
                    string tenantMiniSalonRenew = dr.GetString(13);

                    printString.AppendLine("Room Number: " + tenantRoomNumber);
                    printString.AppendLine("Name: " + tenantName);
                    printString.AppendLine("Address: " + tenantAddress);
                    printString.AppendLine("Phone: " + tenantPhone);
                    printString.AppendLine("Email: " + tenantEmail);
                    printString.AppendLine("Lease Start: " + tenantLeaseStart);
                    printString.AppendLine("Lease End: " + tenantLeaseEnd);
                    printString.AppendLine("Free Week: " + tenantFreeWeek);
                    printString.AppendLine("Free Week Date: " + tenantFreeWeekDate);
                    printString.AppendLine("Operator License: " + tenantOperatorLic);
                    printString.AppendLine("Operator License EXP Date: " + tenantOperatorLicRenew);
                    printString.AppendLine("Mini Salon License: " + tenantMiniSalonLic);
                    printString.AppendLine("Mini Salon License EXP Date: " + tenantMiniSalonRenew);

                    tenantLabel.Content = printString;
                    tenantLabel.Margin = new Thickness(100);
                    
                    MessageBox.Show(printString.ToString());

                    PrintDialog tenantPrintDialog = new PrintDialog();
                    if (tenantPrintDialog.ShowDialog() == true)
                    {
                        tenantPrintDialog.PrintVisual(tenantLabel, "Tenants Information");
                    }
                }
                sqliteCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // This will save the tenant information into the database
        private void AddTenantSave_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                TenantsInfo t = new TenantsInfo();

                t.ID = Convert.ToInt32(AddTenantIDBox.Text);
                t.RoomNumber = Convert.ToInt32(AddTenantRoomNumberBox.Text);
                t.Name = AddTenantNameBox.Text;
                t.Address = AddTenantAddressBox.Text;
                t.Phone = AddTenantPhoneBox.Text;
                t.Email = AddTenantEmailBox.Text;
                t.LeaseStart = AddTenantLeaseStartBox.Text;
                t.LeaseEnd = AddTenantLeaseEndBox.Text;
                t.FreeWeek = AddTenantFreeWeekBox.Text;
                t.FreeWeekDate = AddTenantFreeWeekDateBox.Text;
                t.OperatorLicense = AddTenantOperatorLicBox.Text;
                t.OperatorLicEXPDate = AddTenantOperatorLicEXPDateBox.Text;
                t.MiniSalonLicense = AddTenantMiniSalonLicBox.Text;
                t.MiniSalonLicEXPDate = AddTenantMiniSalonLicEXPDateBox.Text;

                using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
                {
                    // This adds the data to the database and the data is saved
                    conn.Execute("insert into Tenants (ID, RoomNumber, Name, Address, Phone, Email, LeaseStart, LeaseEnd, FreeWeek, " +
                        "FreeWeekDate, OperatorLicense, OperatorLicEXPDate, MiniSalonLicense, MiniSalonLicEXPDate) values (@ID, @RoomNumber, @Name, @Address, @Phone, @Email, " +
                        "@LeaseStart, @LeaseEnd, @FreeWeek, @FreeWeekDate, @OperatorLicense, @OperatorLicEXPDate, @MiniSalonLicense, @MiniSalonLicEXPDate)", t);
                }
                MessageBox.Show("Tenant Added");
                // This clears the text boxes for the next entry
                AddTenantIDBox.Text = "";
                AddTenantRoomNumberBox.Text = "";
                AddTenantNameBox.Text = "";
                AddTenantAddressBox.Text = "";
                AddTenantPhoneBox.Text = "";
                AddTenantEmailBox.Text = "";
                AddTenantLeaseStartBox.Text = "";
                AddTenantLeaseEndBox.Text = "";
                AddTenantFreeWeekBox.Text = "";
                AddTenantFreeWeekDateBox.Text = "";
                AddTenantOperatorLicBox.Text = "";
                AddTenantOperatorLicEXPDateBox.Text = "";
                AddTenantMiniSalonLicBox.Text = "";
                AddTenantMiniSalonLicEXPDateBox.Text = "";
                LoadDataGrid();
                RefreshListBox();
            }
            catch
            {
                MessageBox.Show("Null values not allowed. Enter a value for each field.\nPlease enter a number for ID, Room#, OperatorLic, & MiniSalonLic.");
            }
        }

        // This will delete a tenant from the database
        private void DeleteTenantBtn(object sender, RoutedEventArgs e)
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            // Open connection to database
            try
            {
                if (MessageBox.Show("Are you sure want to delete the tenant?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    sqliteCon.Open();
                    string deleteQuery = "delete from tenants where (cast(ID as TEXT) || ' : ' || Name)='" + IDandNameBox.SelectedItem + "' ";
                    SQLiteCommand createCommand = new SQLiteCommand(deleteQuery, sqliteCon);
                    createCommand.ExecuteNonQuery();
                    MessageBox.Show("Tenant Deleted");
                    EditRoomNumber.Text = "";
                    EditName.Text = "";
                    EditAddress.Text = "";
                    EditPhone.Text = "";
                    EditEmail.Text = "";
                    EditLeaseStart.Text = "";
                    EditLeaseEnd.Text = "";
                    EditFreeWeek.Text = "";
                    EditFreeWeekDate.Text = "";
                    EditOperatorLic.Text = "";
                    EditOperatorLicEXPDate.Text = "";
                    EditMiniSalonLic.Text = "";
                    EditMiniSalonLicEXPDate.Text = "";
                    LoadDataGrid();
                    IDandNameBox.Items.Clear();
                    RefreshListBox();
                    sqliteCon.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing record from list box. \n" + ex.Message);
            }
        }

        // This will edit the information for a specific tenant
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            // Open connection to database
            try
            {
                if (MessageBox.Show("Are you sure want to edit the tenant?", "Edit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    sqliteCon.Open();
                    string editQuery = "update Tenants set RoomNumber='" + EditRoomNumber.Text + "', Name='" + EditName.Text + "', Address='" + EditAddress.Text + "', Phone='" + EditPhone.Text + "', " +
                        "Email='" + EditEmail.Text + "', LeaseStart='" + EditLeaseStart.Text + "', LeaseEnd='" + EditLeaseEnd.Text + "', FreeWeek='" + EditFreeWeek.Text + "', FreeWeekDate='" + EditFreeWeekDate.Text + "', " +
                        "OperatorLicense='" + EditOperatorLic.Text + "', OperatorLicEXPDate='" + EditOperatorLicEXPDate.Text + "', MiniSalonLicense='" + EditMiniSalonLic.Text + "', " +
                        "MiniSalonLicEXPDate='" + EditMiniSalonLicEXPDate.Text + "' where (cast(ID as TEXT) || ' : ' || Name)='" + IDandNameBox.SelectedItem + "' ";
                    SQLiteCommand createCommand = new SQLiteCommand(editQuery, sqliteCon);
                    createCommand.ExecuteNonQuery();
                    MessageBox.Show("Tenant Edited.");
                    EditRoomNumber.Text = "";
                    EditName.Text = "";
                    EditAddress.Text = "";
                    EditPhone.Text = "";
                    EditEmail.Text = "";
                    EditLeaseStart.Text = "";
                    EditLeaseEnd.Text = "";
                    EditFreeWeek.Text = "";
                    EditFreeWeekDate.Text = "";
                    EditOperatorLic.Text = "";
                    EditOperatorLicEXPDate.Text = "";
                    EditMiniSalonLic.Text = "";
                    EditMiniSalonLicEXPDate.Text = "";
                    LoadDataGrid();
                    RefreshListBox();
                    sqliteCon.Close();
                }
            }
            catch
            {
                MessageBox.Show("Make sure to use a single quote twice for proper function.");
            }
        }

        private void IDandNameBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            // Open connection to database
            try
            {
                sqliteCon.Open();
                string loadDataQuery = "select * from Tenants where (cast(ID as TEXT) || ' : ' || Name) ='" + IDandNameBox.SelectedItem + "' ";
                SQLiteCommand createCommand = new SQLiteCommand(loadDataQuery, sqliteCon);
                // createCommand.ExecuteNonQuery();
                SQLiteDataReader dr = createCommand.ExecuteReader();
                while (dr.Read())
                {
                    string tenantRoomNumber = dr.GetInt32(1).ToString();
                    string tenantName = dr.GetString(2);
                    string tenantAddress = dr.GetString(3);
                    string tenantPhone = dr.GetString(4);
                    string tenantEmail = dr.GetString(5);
                    string tenantLeaseStart = dr.GetString(6);
                    string tenantLeaseEnd = dr.GetString(7);
                    string tenantFreeWeek = dr.GetString(8);
                    string tenantFreeWeekDate = dr.GetString(9);
                    string tenantOperatorLic = dr.GetString(10);
                    string tenantOperatorLicRenew = dr.GetString(11);
                    string tenantMiniSalonLic = dr.GetString(12);
                    string tenantMiniSalonRenew = dr.GetString(13);

                    EditRoomNumber.Text = tenantRoomNumber;
                    EditName.Text = tenantName;
                    EditAddress.Text = tenantAddress;
                    EditPhone.Text = tenantPhone;
                    EditEmail.Text = tenantEmail;
                    EditLeaseStart.Text = tenantLeaseStart;
                    EditLeaseEnd.Text = tenantLeaseEnd;
                    EditFreeWeek.Text = tenantFreeWeek;
                    EditFreeWeekDate.Text = tenantFreeWeekDate;
                    EditOperatorLic.Text = tenantOperatorLic;
                    EditOperatorLicEXPDate.Text = tenantOperatorLicRenew;
                    EditMiniSalonLic.Text = tenantMiniSalonLic;
                    EditMiniSalonLicEXPDate.Text = tenantMiniSalonRenew;
                }

                sqliteCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // This will refresh the contents of the list box
        void RefreshListBox()
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            // Open connection to database
            try
            {
                sqliteCon.Open();
                string refreshListBoxQuery = "select * from Tenants";
                SQLiteCommand createCommand = new SQLiteCommand(refreshListBoxQuery, sqliteCon);
                // createCommand.ExecuteNonQuery();
                SQLiteDataReader dr = createCommand.ExecuteReader();
                while (dr.Read())
                {
                    int tenantID = dr.GetInt32(0);
                    string tenantName = dr.GetString(2);
                    IDandNameBox.Items.Remove(tenantID + " : " + tenantName);
                    IDandNameBox.Items.Add(tenantID + " : " + tenantName);
                    IDandNameBox.Items.Refresh();
                }
                sqliteCon.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // This will fill a list box allowing the user to reference what tenant they wish to update
        void FillListBox()
        {
            SQLiteConnection sqliteCon = new SQLiteConnection(LoadConnectionString());
            // Open connection to database
            try
            {
                sqliteCon.Open();
                string fillListBoxQuery = "select * from Tenants";
                SQLiteCommand createCommand = new SQLiteCommand(fillListBoxQuery, sqliteCon);
                // createCommand.ExecuteNonQuery();
                SQLiteDataReader dr = createCommand.ExecuteReader();
                while (dr.Read())
                {
                    int tenantID = dr.GetInt32(0);
                    string tenantName = dr.GetString(2);
                    IDandNameBox.Items.Add(tenantID + " : " + tenantName);
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
