using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Xml;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string KeyName = "CryptoWpf.Practice12";
        private RSACryptoServiceProvider rsa;

        public MainWindow()
        {
            InitializeComponent();
        }


        // Объявление глобальных параметровCspParmeters и RsaCryptoServiceProvider
        CspParameters  _cspp = new();
        RSACryptoServiceProvider _rsa;

        // Переменные пути для папок источника, шифрования и дешифрования. 
        //Должно заканчиваться обратной косой чертой.
        const string EncrFolder = @"c:\Encrypt\";
        const string DecrFolder = @"c:\Decrypt\";
        const string SrcFolder = @"c:\docs\";

        // ФайлПуть к файлу для открытого ключа
        const string PubKeyFile = @"c:\encrypt\rsaPublicKey.txt";

        // Переменная для пары значений закрытого/открытого ключа
        const string KeyName2 = "Key01";

        private void buttonCreateAsmKeys(object sender, RoutedEventArgs e)
        {
            // Сохраняет пару ключей в контейнере ключей.
            _cspp.KeyContainerName = KeyName2;
            _rsa = new RSACryptoServiceProvider(_cspp)
            {
                PersistKeyInCsp = true
            };

            label1.Content = _rsa.PublicOnly
                ? $"Key: {_cspp.KeyContainerName} - Public Only"
                : $"Key: {_cspp.KeyContainerName} - Full Key Pair";

        }
        private void EncryptFile(FileInfo file)
        {
            // Create instance of Aes for
            // symmetric encryption of the data.
            Aes Aesaes = Aes.Create();
            ICryptoTransform transform = Aesaes.CreateEncryptor();

            // Use RSACryptoServiceProvider to
            // encrypt the AES key.
            // rsa is previously instantiated:
            //    rsa = new RSACryptoServiceProvider(cspp);
            byte[] keyEncrypted = _rsa.Encrypt(Aesaes.Key, false);

            // Create byte arrays to contain
            // the length values of the key and IV.
            int lKey = keyEncrypted.Length;
            byte[] LenK = BitConverter.GetBytes(lKey);
            int lIV = Aesaes.IV.Length;
            byte[] LenIV = BitConverter.GetBytes(lIV);

            // Write the following to the FileStream
            // for the encrypted file (outFs):
            // - length of the key
            // - length of the IV
            // - ecrypted key
            // - the IV
            // - the encrypted cipher content

            // Change the file's extension to ".enc"
            string outFile =
        Path.Combine(EncrFolder, Path.ChangeExtension(file.Name, ".enc"));

            using (var outFs = new FileStream(outFile, FileMode.Create))
            {
                outFs.Write(LenK, 0, 4);
                outFs.Write(LenIV, 0, 4);
                outFs.Write(keyEncrypted, 0, lKey);
                outFs.Write(Aesaes.IV, 0, lIV);

                // Now write the cipher text using
                // a CryptoStream for encrypting.
                using (var outStreamEncrypted =
                    new CryptoStream(outFs, transform, CryptoStreamMode.Write))
                {
                    // By encrypting a chunk at
                    // a time, you can save memory
                    // and accommodate large files.
                    int count = 0;
                    int offset = 0;

                    // blockSizeBytes can be any arbitrary size.
                    int blockSizeBytes = Aesaes.BlockSize / 8;
                    byte[] data = new byte[blockSizeBytes];
                    int bytesRead = 0;

                    using (var inFs = new FileStream(file.FullName, FileMode.Open))
                    {
                        do
                        {
                            count = inFs.Read(data, 0, blockSizeBytes);
                            offset += count;
                            outStreamEncrypted.Write(data, 0, count);
                            bytesRead += blockSizeBytes;
                        } while (count > 0);
                    }
                    outStreamEncrypted.FlushFinalBlock();
                }
            }
        }
        OpenFileDialog openFileDialog = new OpenFileDialog();
        private void buttonEncryptFile(object sender, RoutedEventArgs e)
        {
            if (_rsa is null)
            {
                MessageBox.Show("Key not set.");
            }
            else
            {
                // Отобразится диалоговое окно для выбора файла для шифрования.
                openFileDialog.InitialDirectory = SrcFolder;
                if (openFileDialog.ShowDialog(this) == true)
                {
                    string fName = openFileDialog.FileName;
                    if (fName != null)
                    {
                        // Имя файла без указания пути.
                        EncryptFile(new FileInfo(fName));
                    }
                }
            }

        }

        private void buttonDecryptFile(object sender, RoutedEventArgs e)
        {
            if (_rsa is null)
            {
                MessageBox.Show("Key not set.");
            }
            else
            {
                // Display a dialog box to select the encrypted file.
                _decryptOpenFileDialog.InitialDirectory = EncrFolder;
                if (_decryptOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fName = _decryptOpenFileDialog.FileName;
                    if (fName != null)
                    {
                        DecryptFile(new FileInfo(fName));
                    }
                }
            }

        }
    }
}