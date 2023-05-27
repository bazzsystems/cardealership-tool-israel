using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace WpfApp1
{
    public partial class ModifyUserWindow : Window
    {
        private User _user;
        private UserDbContext _dbContext;

        public ModifyUserWindow(User user, UserDbContext dbContext)
        {
            InitializeComponent();
            _user = user;
            _dbContext = dbContext;
            InitializeFields();
        }

        private void InitializeFields()
        {
            UsernameTextBox.Text = _user.Username;
            PasswordTextBox.Text = _user.Password;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = UsernameTextBox.Text;
            string newPassword = PasswordTextBox.Text;

            try
            {
                ValidateInput(newUsername, newPassword);
                UpdateUser(newUsername, newPassword);
                SaveChanges();

                MessageBox.Show("User information saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving user information: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValidateInput(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException("Username and password cannot be empty");
            }

            // Perform additional validation if needed
        }

        private void UpdateUser(string username, string password)
        {
            _user.Username = username;
            _user.Password = password;
        }

        private void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
