using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steganografi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        } 
        /// <summary>
        /// Anahtar dosya ve gizlenecek dosya seçimi yapıldıktan sonra windowsun kendi komutu olan copy b komutu kullanarak dosyanın içine dosyayı gömüyoruz. 
        /// Fakat bu komutun tersi olmadığı için winrar kullanrak anahtar dosyanın içinden gizlenmiş dosyayı çıkartıyoruz. 
        /// Daha sonra dosya uzantısını eski haline aldığımızda ilk duruma geri dönmüş oluyoruz.
        /// Bu sayede steganografi işlemimiz gerçekleşmiş oluyor.
        /// (TXTlerde bozulma olarak gözüken kısım dosya copy /b yapılırken boş bitleri hasara uğruyor. Ama dolu olan bitler yani bizim yazdıklarımız bozukluğa uğramıyor.
        /// TXTlerde bozulma olan kısımdan öncesine bakılırsa(çok rahat farkediliyor) işlem başarılı olarak sonuçlanmış olduğu görülecektir.)
        /// </summary>
        string dosya1, dosya2, uzanti1, uzanti3, dosyayolu1, dosyayolu2, dosyayolu3, ciktidosya;//Kullanacağımız global değişkenler
        //1:Anahtar Dosya
        //2:Gizlenecek Dosya
        //3:ÇıktıDosya
        string winrar = Application.StartupPath + @"\winrar.exe";//Winrar.exe'nin dosya yolu(programın yani bizim .exe dosyamızın yanında olması gereklidir)
        private void Temizle()//bütün textboxları ve değişkenleri temizleyen fonksiyon
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            ciktidosya = "";
            dosya1 = "";
            dosya2 = ""; 
            dosyayolu1 = "";
            dosyayolu2 = "";
            dosyayolu3 = "";
            uzanti1 = ""; 
            uzanti3 = "";
        }
        public void DosyaZiple(string dosya)/// <param name="dosya">Gizlenecek dosyayı zipleme</param> 
        {
            try
            {
                using (ZipArchive zip = ZipFile.Open(Path.ChangeExtension(dosya, ".zip"), ZipArchiveMode.Create))
                {
                    zip.CreateEntryFromFile(dosya, Path.GetFileName(dosya));
                }
            }
            catch (Exception ex)
            /// <exception cref="Exception">
            /// Herhangi bir hatada kullanıcıya bildirimin gerçekleşmesi
            /// </exception>
            {
                MessageBox.Show("Hata oluştu! " + ex.Message + Environment.NewLine + "Tekrar deneyin!"); Temizle();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            button3.Enabled = false; //Form açıldığında gizle tuşunu deaktif yapma
        }
        private void button1_Click(object sender, EventArgs e)//Anahtar Dosya Seçimi Butonu
        {
            try
            {
                OpenFileDialog dosyasec = new OpenFileDialog();//Anahtar dosya seçimi-OPF nesnesi oluşturma
                dosyasec.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Desktop";//Dosya seçimi yaparken ilk açılcak dosya yolu
                if (dosyasec.ShowDialog() == DialogResult.OK)//Dosya seçimi tamamlandığında
                {
                    textBox1.Text = dosyasec.FileName;//textbox1'e dosyanın tam adı(yoluyla birlikte dosya adı)
                    string[] path = dosyasec.FileName.Split('\\');//dosyanın belli kısımlarını almak için parçalama işlemi
                    for (int i = 0; i < path.Length - 1; i++)
                        dosyayolu1 += path[i] + "\\";//dosya yolunu alma(dosya adını almıyoruz)
                    dosyayolu1 = dosyayolu1.Remove(dosyayolu1.Length - 1);//en son konulan slash(\) silme işlemi
                    dosya1 = dosyasec.SafeFileName;//Sadece dosya adı
                    uzanti1 = dosyasec.FileName.Split('.')[1];//sadece uzantısı
                }
                if (textBox1.Text != "" && textBox2.Text != "")//anahtar ve gizlenecek dosya seçildi ise:
                {
                    button3.Enabled = true;//button3 aktif yapma
                }
            }
            catch (Exception ex)
            /// <exception cref="Exception">
            /// Herhangi bir hatada kullanıcıya bildirimin gerçekleşmesi
            /// </exception>
            {
                MessageBox.Show("Hata oluştu! " + ex.Message + Environment.NewLine + "Tekrar deneyin!"); Temizle();
            }

        }
        private void button2_Click(object sender, EventArgs e)//Gizlenecek Dosya Seçimi Butonu
        {
            try
            {
                OpenFileDialog dosyasec = new OpenFileDialog();//button1 | gizlenecek dosya bilgilerini alma
                dosyasec.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Desktop";
                if (dosyasec.ShowDialog() == DialogResult.OK)
                {
                    textBox2.Text = dosyasec.FileName;
                    string[] path = dosyasec.FileName.Split('\\');
                    for (int i = 0; i < path.Length - 1; i++)
                        dosyayolu2 += path[i] + "\\";
                    dosyayolu2 = dosyayolu2.Remove(dosyayolu2.Length - 1);
                    dosya2 = dosyasec.SafeFileName;
                    dosya2 = dosya2.Split('.')[0]; 
                }
                if (textBox1.Text != "" && textBox2.Text != "")
                {
                    button3.Enabled = true;
                }
            }
            catch (Exception ex)
            /// <exception cref="Exception">
            /// Herhangi bir hatada kullanıcıya bildirimin gerçekleşmesi
            /// </exception>
            {
                MessageBox.Show("Hata oluştu! " + ex.Message + Environment.NewLine + "Tekrar deneyin!"); Temizle();
            }
        }
        private void button3_Click(object sender, EventArgs e)//Gizle Butonu
        {
            try
            {
                DosyaZiple(textBox2.Text);//gizlenecek dosyayı zipleme fonksiyonu
                Environment.CurrentDirectory = dosyayolu1;//Çalışılan mevcut konumu değiştirme
                string komut = "/C copy /b " + dosya1 + "+" + dosya2 + ".zip cikti." + uzanti1;//cmd komutu ++ copy /b anahtardosya+ziplenmiş(gizlenecek) çıktıdosyası.anahtardosyauzantı
                System.Diagnostics.Process process = new System.Diagnostics.Process();//process nesnesi oluşturma
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();//process nesnesi için başlatma(process.startinfo nesnesi) bilgileri nesnesi oluşturma
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;//processin görünmez çalışması
                startInfo.FileName = "cmd.exe";//processin çalıştıracağı uygulama
                startInfo.Arguments = komut;//processin çalıştıracağı argümanlar
                process.StartInfo = startInfo;//processin başlatma bilgilerinin atanması
                process.Start();//processi başlatma
                MessageBox.Show("Dosya Gizlendi!");
                while (!File.Exists("cikti." + uzanti1)) //çıktı dosyası oluşana kadar bekleyen ufak bir döngü
                { }
                File.Delete(dosya2+".zip");//ziplenmiş dosya olan anahtar dosyayı silme
            }
            catch (Exception ex)
            /// <exception cref="Exception">
            /// Herhangi bir hatada kullanıcıya bildirimin gerçekleşmesi
            /// </exception>
            {
                MessageBox.Show("Hata oluştu! " + ex.Message + Environment.NewLine + "Tekrar deneyin!"); Temizle();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string komut = "/c cd " + dosyayolu3 + " & " + winrar + " x " + ciktidosya; //cmd komutu ++ cd çıktıdosyayolu & winrar x çıktıdosya
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = komut;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start(); 
                MessageBox.Show("Gizlenen dosya kullanıma hazır. Anahtar dosya kullanılmak için geri getiriliyor!"); 
                File.Move(ciktidosya, Path.ChangeExtension(ciktidosya, "." + uzanti3));//Çıktıdosyanın uzantısını eski hale getirme
                Temizle();
                MessageBox.Show("Anahtar dosya kullanıma hazır!");  
            }
            catch (Exception ex)
            /// <exception cref="Exception">
            /// Herhangi bir hatada kullanıcıya bildirimin gerçekleşmesi
            /// </exception>
            {
                MessageBox.Show("Hata oluştu! " + ex.Message + Environment.NewLine + "Tekrar deneyin!"); Temizle();
            }
        }
        private void button5_Click(object sender, EventArgs e)//Çıktı Dosyası Seçimi Butonu
        {
            try
            {
                OpenFileDialog dosyasec = new OpenFileDialog();
                dosyasec.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Desktop";
                if (dosyasec.ShowDialog() == DialogResult.OK)
                {
                    textBox3.Text = dosyasec.FileName;
                    string[] path = dosyasec.FileName.Split('\\');
                    for (int i = 0; i < path.Length - 1; i++)
                        dosyayolu3 += path[i] + "\\";
                    dosyayolu3 = dosyayolu3.Remove(dosyayolu3.Length - 1); 
                    uzanti3 = dosyasec.FileName.Split('.')[1];
                }
                File.Move(dosyasec.FileName, Path.ChangeExtension(dosyasec.FileName, ".zip"));//çıktıdosyasısın uzantısını zip yapma  
                ciktidosya = dosyayolu3 + @"\" + dosyasec.SafeFileName.Split('.')[0]+".zip";//çıktıdosyasının zip uzantılı halinin dosyayolu
            }
            catch (Exception ex)
            /// <exception cref="Exception">
            /// Herhangi bir hatada kullanıcıya bildirimin gerçekleşmesi
            /// </exception>
            {
                MessageBox.Show("Hata oluştu! " + ex.Message + Environment.NewLine + "Tekrar deneyin!"); Temizle();
            }
        } 
    }
}
