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

namespace NETD3202_Lab3_RyanClayson
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                //Checks to see if user has left textbox's empty
                if (txtBuyerName.Text == string.Empty || txtNumOfShares.Text == string.Empty || dpDatePurchased.SelectedDate == null)
                {
                    MessageBox.Show("Must fill in all textfields");
                    //Reset's all fields, date selection, and radio buttons
                    txtBuyerName.Text = string.Empty;
                    txtNumOfShares.Text = string.Empty;
                    dpDatePurchased.SelectedDate = null;
                    rbCommon.IsChecked = false;
                    rbPreferred.IsChecked = false;
                }
                else if (rbCommon.IsChecked == false && rbPreferred.IsChecked == false)
                {
                    MessageBox.Show("Share type has to be selected");
                    rbCommon.IsChecked = false;
                    rbPreferred.IsChecked = false;
                }
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
                        //Check the radio buttons for share type
                        if (rbCommon.IsChecked == true)
                        {
                            shareType = "Common";
                        }
                        else if (rbPreferred.IsChecked == true)
                        {
                            shareType = "Preferred";
                        }

                        //connects to the database
                        string connectString = Properties.Settings.Default.connect_string;
                        SqlConnection cn = new SqlConnection(connectString);
                        cn.Open();

                        //Insert Query
                        string insertString = "INSERT INTO buyers (name, shares, datePurchased, shareType) VALUES ('" + name + "', '" + shares + "', '" + date + "', '" + shareType + "')";
                        SqlCommand command = new SqlCommand(insertString, cn);
                        command.ExecuteNonQuery();

                        string selectionQuery = "";
                        //If Common is the shareType
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
                                updateQuery = "UPDATE shares SET numCommonShares = '" + availableShares + "' ";
                                SqlCommand thirdCommand = new SqlCommand(updateQuery, cn);
                                thirdCommand.ExecuteScalar();
                            }
                            else if (shareType == "Preferred")
                            {
                                updateQuery = "UPDATE shares SET numPreferredShares = '" + availableShares + "' ";
                                SqlCommand thirdCommand = new SqlCommand(updateQuery, cn);
                                thirdCommand.ExecuteScalar();
                            }
                            //Show's that the update was a success
                            MessageBox.Show("Successfully added share purchase!");
                            //Reset's all fields, date selection, and radio buttons
                            txtBuyerName.Text = string.Empty; txtNumOfShares.Text = string.Empty; dpDatePurchased.SelectedDate = null;
                            rbCommon.IsChecked = false; rbPreferred.IsChecked = false;
                            cn.Close();
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
        private void FillDataGrid()
        {
            try
            {
                //Connect to the db
                string connectString = Properties.Settings.Default.connect_string;
                SqlConnection cn = new SqlConnection(connectString);
                cn.Open();

                //Query the db
                string selectQuery = "SELECT * FROM buyers";
                SqlCommand commandQuery = new SqlCommand(selectQuery, cn);

                //Retrieve the data
                SqlDataAdapter sda = new SqlDataAdapter(commandQuery);
                DataTable dt = new DataTable("Buyers");

                //Set the data
                sda.Fill(dt);
                viewEntriesGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
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

                        //Prepare and Execute Queries
                        //Common Shares
                        string retrieveCommonSharesSold = "SELECT SUM(shares) FROM buyers WHERE shareType = 'Common'";
                        SqlCommand fourthCommand = new SqlCommand(retrieveCommonSharesSold, cn);

                        //Preferred Shares
                        string retrievePreferredSharesSold = "SELECT SUM(shares) FROM buyers WHERE shareType = 'Preferred'";
                        SqlCommand fifthCommand = new SqlCommand(retrievePreferredSharesSold, cn);

                        //Revenue Generated
                        string retrieveDate = "SELECT shares, DATEDIFF(DAY, '2000-01-01', datePurchased) AS days FROM buyers WHERE shareType = 'Common' OR shareType = 'Preferred'";
                        //Creating a hidden table to pull the data from later
                        SqlDataAdapter dateCommand = new SqlDataAdapter(retrieveDate, cn);
                        DataTable dt = new DataTable();
                        dateCommand.Fill(dt);

                        //variables to store data in foreach loop
                        string date = "";
                        int revenue = 0;
                        int numShares;
                        //Run through each row on the hidden table
                        foreach (DataRow row in dt.Rows)
                        {
                            //Stores an int based on the date in the row
                            date = row["days"].ToString();
                            //Stores the number of shares in a row
                            numShares = (int)row["shares"];
                            //Generates random price for stocks
                            Random rand = new Random();
                            int money = rand.Next(int.Parse(date));
                            //Calculates the money made
                            revenue += money * numShares;
                        }

                        //Common Available
                        string availableCommonShares = "SELECT numCommonShares FROM shares";
                        SqlCommand seventhCommand = new SqlCommand(availableCommonShares, cn);

                        //Preferred Available
                        string availablePreferredShares = "SELECT numPreferredShares FROM shares";
                        SqlCommand eigthCommand = new SqlCommand(availablePreferredShares, cn);

                        //Updates the Text Blocks
                        //Common Shares Sold
                        int commonShares = Convert.ToInt32(fourthCommand.ExecuteScalar());
                        txtNumCommonSold.Text = commonShares.ToString();
                        //Preferred Shares Sold
                        int preferredShares = Convert.ToInt32(fifthCommand.ExecuteScalar());
                        txtNumPreferredSold.Text = preferredShares.ToString();
                        //Shows revenue as string 
                        txtRevenue.Text = revenue.ToString();
                        //Common Available
                        int commonSharesAvailable = Convert.ToInt32(seventhCommand.ExecuteScalar());
                        commonSharesAvailable = commonSharesAvailable - int.Parse(txtNumCommonSold.Text);
                        txtCommonSharesAvailable.Text = commonSharesAvailable.ToString();
                        //Preferred Available
                        int preferredSharesAvailable = Convert.ToInt32(eigthCommand.ExecuteScalar());
                        preferredSharesAvailable = preferredSharesAvailable - int.Parse(txtNumPreferredSold.Text);
                        txtPreferredSharesAvailable.Text = preferredSharesAvailable.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    break;

                case "View Entries":

                    FillDataGrid();

                    break;

                case "Create Entry":

                    break;
            }
        }
    }
}
