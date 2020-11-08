/* NAME: Ryan Clayson
 * ID: 100558837
 * NETD3202 - Lab 3
 * Date: Nov 6 2020
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
            ViewSummary();
            FillDataGrid();
            Revenue();
        }

        private void btnCreateEntry_Click(object sender, RoutedEventArgs e)
        {
            //Retrieves date from date picker as string
            string date = dpDatePurchased.SelectedDate.ToString();

            //Checks to see if buyer name has been entered
            if (txtBuyerName.Text != string.Empty)
            {
                if (txtNumOfShares.Text != string.Empty && int.TryParse(txtNumOfShares.Text, out int shares))
                {
                    string selectedShare;
                    if (rbPreferred.IsChecked == true)
                    {
                        selectedShare = "Preferred";
                    }
                    else
                    {
                        selectedShare = "Common";

                    }
                    try
                    {
                        string connect_string = Properties.Settings.Default.connect_string;
                        SqlConnection cn = new SqlConnection(connect_string);
                        cn.Open();

                        string selectionQuery = "";
                        if (selectedShare == "Common")
                        {
                            selectionQuery = "SELECT numCommonShares FROM Shares";
                        }
                        else
                        {
                            selectionQuery = "SELECT numPreferredShares FROM Shares";
                        }

                        SqlCommand secondCommand = new SqlCommand(selectionQuery, cn);
                        int availableShares = Convert.ToInt32(secondCommand.ExecuteScalar());
                        availableShares -= shares;

                        if(availableShares < 0)
                        {
                            MessageBox.Show("Not enough Shares available to buy.");
                            txtNumOfShares.SelectAll();
                            txtNumOfShares.Focus();
                        }
                        else
                        {
                            string updateQuery = "";
                            if (shareType == "Common")
                            {
                                updateQuery = "UPDATE Shares SET numCommonShares = '" + availableShares + "' ";
                                SqlCommand thirdcommand = new SqlCommand(updateQuery, cn);
                                thirdcommand.ExecuteScalar();
                            }
                            else if (shareType == "Preferred")
                            {
                                updateQuery = "UPDATE Shares SET numPreferredShares = '" + availableShares + "' ";
                                SqlCommand thirdcommand = new SqlCommand(updateQuery, cn);
                                thirdcommand.ExecuteScalar();
                            }

                            MessageBox.Show("Successfully Added Purchased Shares!");
                            txtBuyerName.Text = string.Empty;
                            txtNumOfShares.Text = string.Empty;
                            dpDatePurchased.SelectedDate = null;
                            rbCommon.IsChecked = false;
                            rbPreferred.IsChecked = false;
                            cn.Close();

                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.ToString());
                        throw;
                    }
                }
                else
                {
                    MessageBox.Show("Number of shares must not be empty and must be numeric.");
                    txtNumOfShares.Clear();
                    txtNumOfShares.Focus();
                }
            }
            else
            {
                //buyer name empty. Show error message
                MessageBox.Show("Please enter a buyer name.");
                txtBuyerName.Focus();
            }
        }

        public void FillDataGrid()
        {
            try
            {
                string connect_string = Properties.Settings.Default.connect_string;
                //Creates connection
                SqlConnection cn = new SqlConnection(connect_string);
                //Opens a connection
                cn.Open();

                string selectionQuery = "SELECT * FROM Buy";
                SqlCommand command = new SqlCommand(selectionQuery, cn);
                SqlDataAdapter sda = new SqlDataAdapter(command);
                DataTable dt = new DataTable("Buy");
                sda.Fill(dt);
                viewEntriesGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
