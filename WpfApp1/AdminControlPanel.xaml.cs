using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class AdminControlPanel : Window
    {
        private UserDbContext _dbContext;
        private const string AdminPin = "0556627643";

        public AdminControlPanel()
        {
            InitializeComponent();
            _dbContext = new UserDbContext();

            // Load the user data when the window is initialized
            LoadUserData();
        }

        private void LoadUserData()
        {
            var users = _dbContext.Users.ToList();
            dataGrid.ItemsSource = users;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = new User { Username = username, Password = password };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            MessageBox.Show("User added successfully");

            LoadUserData();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dataGrid.SelectedItem as User;
            if (selectedUser != null)
            {
                _dbContext.Users.Remove(selectedUser);
                _dbContext.SaveChanges();

                MessageBox.Show("User deleted successfully");

                LoadUserData();
            }
            else
            {
                MessageBox.Show("Please select a user to delete");
            }
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = dataGrid.SelectedItem as User;
            if (selectedUser != null)
            {
                var modifyWindow = new ModifyUserWindow(selectedUser, _dbContext); // pass _dbContext as well
                modifyWindow.ShowDialog();

                // Refresh the user data after modifying
                LoadUserData();
            }
            else
            {
                MessageBox.Show("Please select a user to modify");
            }
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
