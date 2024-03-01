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
using System.IO;
using System.Data.SQLite; //for database
using System.Data;
using DataVis = System.Windows.Forms.DataVisualization; //for graphing
using System.Windows.Forms.DataVisualization.Charting;



/*        ID INTEGER PRIMARY KEY AUTOINCREMENT,
        Glucose INTEGER,
        Carbs INTEGER,
        Insulin INTEGER,
        Notes TEXT,
        Date TEXT,
        Time TEXT */


namespace Blood_Glucose_Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string dbPath; // Path to the SQLite database



        public MainWindow()
        {
            InitializeComponent();
            LoadDataBase(); //Load database on application launch
            LoadComboBoxes(); // Load ComboBoxes with default values
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {

        }

        /*
        private void InsertInto()
        {
            string insert = "Insert into records"
        }
        */

        // Method to load ComboBoxes with default values
        private void LoadComboBoxes()
        {
            for (int i = 1; i <= 12; i++)
            {
                hoursComboBox.Items.Add(i.ToString("00"));
            }

            // Populate minutes 
            for (int i = 0; i < 60; i++)
            {
                minutesComboBox.Items.Add(i.ToString("00"));
            }

            // Set default selection
            hoursComboBox.SelectedIndex = 0;
            minutesComboBox.SelectedIndex = 0;
        }


        private void LoadDataBase() //Method to load database with error handling. Crashes without catch.
        {
            try
            {
                // Open connection to SQLite database
                dbPath = Directory.GetCurrentDirectory() + "\\glucose_database.db";
                SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};");
                conn.Open();

                // Query to select all records from the database
                string query = "Select * from records";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);

                //Display records in Log_Grid
                Log_Grid.ItemsSource = dt.DefaultView;

                // Close database connection
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured");
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) 
        {
            //to be implemented
        }

        private void Entry_Click(object sender, RoutedEventArgs e) //Blood glucose logger
        {

            String glucoseEntry = Glucose_Entry.Text; //Extract text from UI for insertion to database
            String carbsEntry = Carbs_Entry.Text;
            String insulinEntry = Insulin_Entry.Text;
            String notesEntry = Notes_Entry.Text;

            DateTime currentDate = DateTime.Now;
            String date = currentDate.ToString("yyyy-MM-dd");
            String time = currentDate.ToString("t");

            if (cbCustomTime.IsChecked == true)
            {
                date = dpCustomDate.SelectedDate.Value.ToString("yyyy-MM-dd"); //Custom date and time will be inserted if check box is selected
                
                string hour = hoursComboBox.SelectedItem.ToString();
                string minute = minutesComboBox.SelectedItem.ToString();
                string timePeriod = ((ComboBoxItem)timePeriodComboBox.SelectedItem).Content.ToString();
                string customTime = $"{hour}:{minute} {timePeriod}";
                time = customTime; //Working on removing leading 0
            }

            try
            {
                dbPath = Directory.GetCurrentDirectory() + "\\glucose_database.db";
                SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};");
                conn.Open();
                string query = "Insert into records (Glucose, Carbs, Insulin, Notes, Date, Time) values (@glucoseEntry, @carbsEntry, @insulinEntry, @notesEntry, @date, @time)";//Glucose, Date, Time

                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@glucoseEntry", glucoseEntry);
                    command.Parameters.AddWithValue("@carbsEntry", carbsEntry);
                    command.Parameters.AddWithValue("@insulinEntry", insulinEntry);
                    command.Parameters.AddWithValue("@notesEntry", notesEntry);
                    command.Parameters.AddWithValue("@date", date);
                    command.Parameters.AddWithValue("@time", time);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured");
            }

            LoadDataBase();
        }

        private void BtnQuery_Click_1(object sender, RoutedEventArgs e)
        {
            var dateFr = dateFrom.SelectedDate;
            var dateT = dateTo.SelectedDate;

            LoadDataBase();

            try
            {
                dbPath = Directory.GetCurrentDirectory() + "\\glucose_database.db";
                SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};");
                conn.Open();
                string query = "select * from records where Date between @dateFr and @dateT";

                // Execute the SQL quary and populate the data into a data table for display
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@dateFr", dateFr.Value.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@dateT", dateT.Value.ToString("yyyy-MM-dd"));
                    command.ExecuteNonQuery();

                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    // Bind the DataTable to the data grid for displaying the queried records
                    Chart_Grid.ItemsSource = dt.DefaultView;
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured");
            }
        }

        // Method to reset log fields
        private void Reset_Log(object sender, RoutedEventArgs e)
        {
            Glucose_Entry.Clear();
            Carbs_Entry.Clear();
            Insulin_Entry.Clear();
            Notes_Entry.Clear();
            cbCustomTime.IsChecked = false; //Uncheck checkbox to toggle custom time entry
        }

        // Method to handle custom time checkbox checked event
        private void cbCustomTime_Checked(object sender, RoutedEventArgs e)
        {
            labelEnterDate.Visibility = Visibility.Visible;
            labelEnterTime.Visibility = Visibility.Visible;
            dpCustomDate.Visibility = Visibility.Visible;
            groupBoxCustomTime.Visibility = Visibility.Visible;
        }

        // Method to handle custom time checkbox unchecked event
        private void cbCustomTime_Unchecked(object sender, RoutedEventArgs e)
        {
            labelEnterDate.Visibility = Visibility.Hidden;
            labelEnterTime.Visibility = Visibility.Hidden;
            dpCustomDate.Visibility = Visibility.Hidden;
            groupBoxCustomTime.Visibility = Visibility.Hidden;
        }

        // Method to display chart when button is clicked
        private void btnDisplayChart_Click(object sender, RoutedEventArgs e)
        {
            var dateFr = dateFrom.SelectedDate;
            var dateT = dateTo.SelectedDate;

            try
            {
                // Connect to the database and retrieve glucose data within the selected date range
                dbPath = Directory.GetCurrentDirectory() + "\\glucose_database.db";
                SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};");
                conn.Open();
                string query = "select Glucose, Date, Time from records where Date between @dateFr and @dateT";

                // Execute the SQL query to fetch glucose data
                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@dateFr", dateFr.Value.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@dateT", dateT.Value.ToString("yyyy-MM-dd"));

                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int glucose = reader.GetInt32(0); // Index 0 is Glucose
                        string date = reader.GetString(1); // Index 1 is Date
                        string time = reader.GetString(2); // Index 2 is Time

                        // Combine date and time
                        DateTime dateTime = DateTime.Parse(date + " " + time);

                        // Add the data point to the chart series
                        Chart1.Series[0].Points.Add(glucose).AxisLabel = dateTime.ToString("yyyy-MM-dd HH:mm");
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                // Display error message if an exception occurs during data retrieval
                MessageBox.Show("An error has occurred");
            }
        }
    }
 }

