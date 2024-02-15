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
using System.Data.SQLite;
using System.Data;
using System.Windows.Forms.DataVisualization; //for graphing


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
            LoadDataBase();
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
        private void LoadDataBase()
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

                GGrid.ItemsSource = dt.DefaultView;
                GGrid_Copy.ItemsSource = dt.DefaultView;

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured");
            }
        
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Entry_Click(object sender, RoutedEventArgs e)
        {
            String entry = Glucose_Entry.Text;
            DateTime currentDate = DateTime.Now;
            String date = currentDate.ToString("yyyy-MM-dd");
            String time = currentDate.ToString("t");
           //MessageBox.Show(entry);

            try
            {
                dbPath = Directory.GetCurrentDirectory() + "\\glucose_database.db";
                SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};");
                conn.Open();
                string query = "Insert into records (Glucose, Date, Time) values (@entry, @date, @time)";//Glucose, Date, Time

                using (SQLiteCommand command = new SQLiteCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@entry", entry);
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
    }
}
