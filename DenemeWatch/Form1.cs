using System.Diagnostics;
using System.Net.NetworkInformation;

namespace DenemeWatch
{
    public partial class Form1 : Form
    {
        private bool sanalMakina;
        private System.Windows.Forms.Timer timer;
        public Form1()
        {
            InitializeComponent();
            SanalMakinaDurumu();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 40000;
            timer.Tick += new EventHandler(timer1_Tick);
            timer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sanalmakinaAdi = "ubuntuyes";
            if (sanalMakina)
            {
                Process process = new Process();
                process.StartInfo.FileName = "VBoxManage";
                process.StartInfo.Arguments = $"controlvm \"{sanalmakinaAdi}\" poweroff";
                process.Start();
                process.WaitForExit();
                button1.BackColor = Color.Red;
            }
            else
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "VBoxManage",
                    Arguments = $"startvm \"{sanalmakinaAdi}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = processStartInfo;
                    process.Start();
                    process.WaitForExit();
                    button1.BackColor = Color.Green;
                }
            }
        }
        private void SanalMakinaDurumu()
        {

            string sanalmakinaAdi = "ubuntuyes";
            string output = VBoxManageCommand($"showvminfo \"{sanalmakinaAdi}\"");// vsbox daki sanal mankinanın ayrıntılarınuı gözlemek için 

            if (output.Contains("running"))
            {
                button1.BackColor = Color.Green;
                sanalMakina = true;
            }
            else
            {
                button1.BackColor = Color.Red;
                sanalMakina = false;
            }
        }
        private string VBoxManageCommand(string arguments)
        {
            // VBoxManage komutunu çalışır.
            Process process = new Process();
            process.StartInfo.FileName = "VBoxManage";
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            string sanalmakinaIp = "10.0.2.15";
            Ping pinggonder = new Ping();
            PingReply reply = pinggonder.Send(sanalmakinaIp);

            if (reply.Status == IPStatus.Success)
            {
                MessageBox.Show("Sanal Makina Çalışıyor");

            }
            else
            {
                //      MessageBox.Show("Sanal Makina Çalışmıyor.");
            }

            SanalMakinaDurumu();

            if (!sanalMakina)
            {

                LogToFile("Sanal Makina Kapalı.Çalışmıyor");
                MessageBox.Show("Sanal Makina Kapalı");

                Task.Run(async () =>
                {
                    await Task.Delay(40000);
                    OtomatikAcmaIslemi();
                });
            }
        }
        private void OtomatikAcmaIslemi()
        {

            string sanalmakinaAdi = "ubuntuyes";
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "VBoxManage",
                Arguments = $"startvm \"{sanalmakinaAdi}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();
            }
            LogToFile("Sanal Makina Calışıyor.");
        }
        private void LogToFile(string logMessage)
        {
            // Log mesajını belirtilen dosyaya kaydet
            string Logfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Log.txt");
            using (StreamWriter writer = File.AppendText(Logfile))
            {
                writer.WriteLine(DateTime.Now + " Sanal Makina " + logMessage);
            }
        }


    }
}