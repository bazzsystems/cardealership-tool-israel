using System;
using System.Linq;
using System.Windows;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;

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

            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                MessageBox.Show("Login successful");

                if (username == "admin")
                {
                    string hashedAdminPin = BCrypt.Net.BCrypt.HashPassword(AdminPin);
                    string hashedSignUpPin = BCrypt.Net.BCrypt.HashPassword(SignUpPin);
                    var passwordEntryWindow = new PasswordEntryWindow(hashedAdminPin, hashedSignUpPin);
                    passwordEntryWindow.ShowDialog();

                    if (!passwordEntryWindow.ArePasswordsCorrect)
                    {
                        MessageBox.Show("Incorrect passwords. Access denied.");
                        return;
                    }

                    var adminControlPanel = new AdminControlPanel();
                    adminControlPanel.Show();
                }
                else
                {
                    var scrapeWindow = new ScrapeWindow();
                    scrapeWindow.Show();
                }

                Close();
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
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                var user = new User { Username = username, Password = hashedPassword };
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                MessageBox.Show("Account created successfully");
            }
            else
            {
                MessageBox.Show("Incorrect pin code");
            }
        }


        private void OpenAdminControlPanel_Click(object sender, RoutedEventArgs e)
        {
            string hashedAdminPin = BCrypt.Net.BCrypt.HashPassword(AdminPin);
            string hashedSignUpPin = BCrypt.Net.BCrypt.HashPassword(SignUpPin);

            var passwordEntryWindow = new PasswordEntryWindow(hashedAdminPin, hashedSignUpPin);
            passwordEntryWindow.ShowDialog();

            if (!passwordEntryWindow.ArePasswordsCorrect)
            {
                MessageBox.Show("Incorrect passwords. Access denied.");
                return;
            }

            var adminControlPanel = new AdminControlPanel();
            adminControlPanel.Show();
            Close();
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("ServerHIDDEN;");
        }
    }
}
