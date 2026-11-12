using System.IO;
using System.Security.Cryptography;
using System.Windows;
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        // Объявление глобальных параметровCspParmeters и RsaCryptoServiceProvider
        CspParameters  _cspp = new CspParameters();
        RSACryptoServiceProvider _rsa;

        // Переменные пути для папок источника, шифрования и дешифрования. 
        //Должно заканчиваться обратной косой чертой.
        const string EncrFolder = @"c:\Encrypt\";
        const string DecrFolder = @"c:\Decrypt\";
        const string SrcFolder = @"c:\docs\";

        // ФайлПуть к файлу для открытого ключа
        const string PubKeyFile = @"c:\encrypt\rsaPublicKey.txt";

        // Переменная для пары значений закрытого/открытого ключа
        const string KeyName = "Key01";

        private void buttonCreateAsmKeys(object sender, RoutedEventArgs e)
        {
            // Сохраняет пару ключей в контейнере ключей.
            _cspp.KeyContainerName = KeyName;
            _rsa = new RSACryptoServiceProvider(_cspp)
            {
                PersistKeyInCsp = true
            };

            label1.Content = _rsa.PublicOnly
                ? $"Key: {_cspp.KeyContainerName} - Public Only"
                : $"Key: {_cspp.KeyContainerName} - Full Key Pair";

        }
    }
}