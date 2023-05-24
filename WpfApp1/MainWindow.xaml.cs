using System;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private UserDbContext _dbContext;
        private const string SignUpPin = "212756969";
        private const string AdminPin = "0556627643";

        public MainWindow()
        {
            InitializeComponent();
            _dbContext = new UserDbContext();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                MessageBox.Show("Login successful");

                // Check if it's an admin login
                if (username == "admin")
                {
                    var adminControlPanel = new AdminControlPanel();
                    adminControlPanel.Show();
                }
                else
                {
                    var scrapeWindow = new ScrapeWindow();
                    scrapeWindow.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Username or password is incorrect");
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string pin = PinCodeBox.Password;

            if (pin == SignUpPin)
            {
                var user = new User { Username = username, Password = password };
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                MessageBox.Show("Account created successfully");
            }
            else
            {
                MessageBox.Show("Incorrect pin code");
            }
        }
    }

    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
