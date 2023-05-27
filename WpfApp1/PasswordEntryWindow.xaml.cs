using System.Windows;

namespace WpfApp1
{
    public partial class PasswordEntryWindow : Window
    {
        private readonly string _expectedPassword1;
        private readonly string _expectedPassword2;

        public bool ArePasswordsCorrect { get; private set; }

        public PasswordEntryWindow(string expectedPassword1, string expectedPassword2)
        {
            InitializeComponent();
            _expectedPassword1 = expectedPassword1;
            _expectedPassword2 = expectedPassword2;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            ArePasswordsCorrect = BCrypt.Net.BCrypt.Verify(PasswordBox1.Password, _expectedPassword1)
                && BCrypt.Net.BCrypt.Verify(PasswordBox2.Password, _expectedPassword2);
            Close();
        }

    }
}
