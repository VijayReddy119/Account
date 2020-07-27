using System.Windows;

namespace AccountFinance
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DataAccess dataAccess = new DataAccess();
            dataAccess.InitializeDb();
        }
    }
}
