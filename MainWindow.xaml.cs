/* NAME: Ryan Clayson
 * ID: 100558837
 * NETD3202 - Lab 3
 * Date: Nov 6 2020
 * Resources: NETD 3202 - Week 7 - Lab 3 Hints and Tips - By: Alaadin Addas
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;

namespace NETD3202_Lab3_RyanClayson
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Creates new list
        List<Shares> list = new List<Shares>();
        public MainWindow()
        {
            InitializeComponent();
            lstShares.ItemsSource = list;
        }
        /// <summary>
        /// Create entry button logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateEntry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VALIDATIONS
                //Checks to see if user has left textbox's and selected date empty
                if (txtBuyerName.Text == string.Empty || txtNumOfShares.Text == string.Empty || dpDatePurchased.SelectedDate == null)
                {
                    MessageBox.Show("Must fill in all textfields");
                    //Reset's all fields, date selection, and radio buttons
                    txtBuyerName.Text = string.Empty; txtNumOfShares.Text = string.Empty;
                    dpDatePurchased.SelectedDate = null;
                    rbCommon.IsChecked = false; rbPreferred.IsChecked = false;
                }
                else if (rbCommon.IsChecked == false && rbPreferred.IsChecked == false)
                {
                    MessageBox.Show("Share type has to be selected");
                    rbCommon.IsChecked = false;
                    rbPreferred.IsChecked = false;
                }
                //If all validations pass retrieve user information
                else
                {
                    int shares;
                    //Checks to see if numeric input has been entered
                    if (int.TryParse(txtNumOfShares.Text, out shares))
                    {
                        //grab values
                        string name = txtBuyerName.Text;
                        string date = dpDatePurchased.SelectedDate.ToString();
                        string shareType = "";
                        //Checks the share type
                        if (rbCommon.IsChecked == true)
                        {
                            shareType = "Common";
                        }
                        else if (rbPreferred.IsChecked == true)
                        {
                            shareType = "Preferred";
                        }

                        //Connects to the database
                        string connectString = Properties.Settings.Default.connect_string;
                        SqlConnection cn = new SqlConnection(connectString);
                        cn.Open();

                        //Insert User Information into table
                        string insertString = "INSERT INTO buyers (name, shares, datePurchased, shareType) VALUES ('" + name + "', '" + shares + "', '" + date + "', '" + shareType + "')";
                        SqlCommand command = new SqlCommand(insertString, cn);
                        command.ExecuteNonQuery();

                        string selectionQuery = "";
                        //If Common is the shareType. 
                        if (shareType == "Common")
                        {
                            selectionQuery = "SELECT numCommonShares FROM shares";
                        }
                        //If Preferred is the shareType
                        else if (shareType == "Preferred")
                        {                           
                            selectionQuery = "SELECT numPreferredShares FROM shares";
                        }

                        SqlCommand secondCommand = new SqlCommand(selectionQuery, cn);
                        int availableShares = Convert.ToInt32(secondCommand.ExecuteScalar());

                        availableShares = availableShares - shares;

                        //If there is not enough shares, state message
                        if (availableShares < 0) 
                        {
                            MessageBox.Show("Sorry, there are not enough shares left.");
                        }
                        //If there are enough shares
                        else
                        {
                            string updateQuery = "";
                            if (shareType == "Common")
                            {
                                //Adds the values received to the list. Displays in View Objects tab when new entry is created
                                CommonShares commonShare = new CommonShares(txtBuyerName.Text, dpDatePurchased.Text, shares, shareType, votingPower);
                                list.Add(commonShare);

                                //updates database for common shares
                                updateQuery = "UPDATE shares SET numCommonShares = '" + availableShares + "' ";
                                SqlCommand thirdCommand = new SqlCommand(updateQuery, cn);
                                thirdCommand.ExecuteScalar();
                            }
                            else if (shareType == "Preferred")
                            {
                                //Adds the values received to the list. Displays in View Objects tab when new entry is created
                                PreferredShares preferredShare = new PreferredShares(txtBuyerName.Text, dpDatePurchased.Text, shares, shareType);
                                list.Add(preferredShare);

                                //updates database for preferred shares
                                updateQuery = "UPDATE shares SET numPreferredShares = '" + availableShares + "' ";
                                SqlCommand thirdCommand = new SqlCommand(updateQuery, cn);
                                thirdCommand.ExecuteScalar();
                            }
                            //Show's that the update was a success
                            MessageBox.Show("Successfully added share purchase!");
                            //Reset's all fields, date selection, and radio buttons
                            txtBuyerName.Text = string.Empty; txtNumOfShares.Text = string.Empty; 
                            dpDatePurchased.SelectedDate = null;
                            rbCommon.IsChecked = false; rbPreferred.IsChecked = false;
                            cn.Close();
                            lstShares.Items.Refresh();
                            //closes
                        }
                    }
                    else
                    {
                        //Error Message. Input was not numeric
                        MessageBox.Show("Shares must be numeric.");
                        txtNumOfShares.Text = string.Empty;
                        txtNumOfShares.Focus();
                    }
                }
            }
            //Catch exception
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// Retrieves data from the buyers table to display for the view entries
        /// </summary>
        private void FillDataGrid()
        {
            try
            {
                //Connect to the database
                string connectString = Properties.Settings.Default.connect_string;
                SqlConnection cn = new SqlConnection(connectString);
                cn.Open();
                //Selects all fields in buyers table
                string selectQuery = "SELECT * FROM buyers";
                SqlCommand commandQuery = new SqlCommand(selectQuery, cn);
                //Retrieves the data
                SqlDataAdapter sda = new SqlDataAdapter(commandQuery);
                DataTable dt = new DataTable("Buyers");
                //Fills and sets the data
                sda.Fill(dt);
                viewEntriesGrid.ItemsSource = dt.DefaultView;
            }
            //exception has occurred
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// Tab control settings. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            switch (tabItem)
            {
                case "View Summary":
                    try
                    {
                        //Connect to the database
                        string connectString = Properties.Settings.Default.connect_string;
                        SqlConnection cn = new SqlConnection(connectString);
                        cn.Open();

                        //Prepare Queries
                        //Common Shares
                        string retrieveCommonSharesSold = "SELECT SUM(shares) FROM buyers WHERE shareType = 'Common'";
                        //Preferred Shares
                        string retrievePreferredSharesSold = "SELECT SUM(shares) FROM buyers WHERE shareType = 'Preferred'";
                        //Revenue Generated
                        string retrievePurchaseDate = "SELECT shares, DATEDIFF(DAY, '2000-01-01', datePurchased) AS days FROM buyers WHERE shareType = 'Common' OR shareType = 'Preferred'";

                        //Execute Queries
                        //Common Shares
                        SqlCommand fourthCommand = new SqlCommand(retrieveCommonSharesSold, cn);
                        //Preferred Shares
                        SqlCommand fifthCommand = new SqlCommand(retrievePreferredSharesSold, cn);
                        //Revenue Generated
                        SqlDataAdapter dateCommand = new SqlDataAdapter(retrievePurchaseDate, cn);
                        DataTable dt = new DataTable();
                        dateCommand.Fill(dt);

                        //Variables 
                        string date = "";
                        int totalRevenue = 0;
                        int numberOfShares;
                        foreach (DataRow row in dt.Rows)
                        {
                            //Stores an int based on the date in the row
                            date = row["days"].ToString();
                            //Stores number of shares in a row
                            numberOfShares = (int)row["shares"];
                            //Generates random value of stocks
                            Random rnd = new Random();
                            int revenue = rnd.Next(int.Parse(date));
                            //Calculates the revenue
                            totalRevenue += revenue * numberOfShares;
                        }

                        //Common Available
                        string availableCommonShares = "SELECT numCommonShares FROM shares";
                        SqlCommand seventhCommand = new SqlCommand(availableCommonShares, cn);

                        //Preferred Available
                        string availablePreferredShares = "SELECT numPreferredShares FROM shares";
                        SqlCommand eigthCommand = new SqlCommand(availablePreferredShares, cn);

                        //Updates the Text Blocks
                        //Common Shares Sold
                        int commonSharesSold = Convert.ToInt32(fourthCommand.ExecuteScalar());
                        txtNumCommonSold.Text = commonSharesSold.ToString();
                        
                        //Preferred Shares Sold
                        int preferredSharesSold = Convert.ToInt32(fifthCommand.ExecuteScalar());
                        txtNumPreferredSold.Text = preferredSharesSold.ToString();
                        
                        //Shows revenue as string 
                        txtRevenue.Text = totalRevenue.ToString();
                        
                        //Common Shares Available
                        int commonSharesAvailable = Convert.ToInt32(seventhCommand.ExecuteScalar());
                        commonSharesAvailable = commonSharesAvailable - int.Parse(txtNumCommonSold.Text);
                        txtCommonSharesAvailable.Text = commonSharesAvailable.ToString();
                        
                        //Preferred Shares Available
                        int preferredSharesAvailable = Convert.ToInt32(eigthCommand.ExecuteScalar());
                        preferredSharesAvailable = preferredSharesAvailable - int.Parse(txtNumPreferredSold.Text);
                        txtPreferredSharesAvailable.Text = preferredSharesAvailable.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;
                //When View entries is selected. Shows entries
                case "View Entries":

                    FillDataGrid();

                    break;
                //Create entry tab
                case "Create Entry":

                    break;
            }
        }
    }
}
