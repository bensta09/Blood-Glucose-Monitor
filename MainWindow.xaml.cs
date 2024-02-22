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
using System.Windows.Forms.DataVisualization; //for graphing



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

        private string dbPath;


        public MainWindow()
        {
            InitializeComponent();
            LoadDataBase(); //Load database on application launch
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
private void LoadDataBase() //Method to load database with error handling. Crashes without catch.
        {
            try
            {
                dbPath = Directory.GetCurrentDirectory() + "\\glucose_database.db";
                SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};");
                conn.Open();
                string query = "Select * from records";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);

                //Chart_Grid.ItemsSource = dt.DefaultView;
                Log_Grid.ItemsSource = dt.DefaultView;

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured");
            }
        
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) //Unused
        {

        }

        private void Entry_Click(object sender, RoutedEventArgs e) //Blood glucose logger
        {
            String glucoseEntry = Glucose_Entry.Text;
            String carbsEntry = Carbs_Entry.Text;
            String insulinEntry = Insulin_Entry.Text;
            String notesEntry = Notes_Entry.Text;

            DateTime currentDate = DateTime.Now;
            String date = currentDate.ToString("yyyy-MM-dd");
            String time = currentDate.ToString("t");
           //MessageBox.Show(entry);

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

        private void btnQuery_Click_1(object sender, RoutedEventArgs e)
        {
            var dateFr = dateFrom.SelectedDate;
            var dateT = dateTo.SelectedDate;

            LoadDataBase();

            try
            {
                dbPath = Directory.GetCurrentDirectory() + "\\glucose_database.db";
                SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};");
                conn.Open();
                string query = "select * from records where Date between @dateFr and @dateT";//Glucose, Date, Time

                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@dateFr", dateFr.Value.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@dateT", dateT.Value.ToString("yyyy-MM-dd"));
                    command.ExecuteNonQuery();

                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);
                    Chart_Grid.ItemsSource = dt.DefaultView;
                }

                conn.Close();
            }
            catch (Exception ex)
            { 
                MessageBox.Show("An error has occured");
            }
        }
    }
}
