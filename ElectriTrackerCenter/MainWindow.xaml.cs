using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using System.Device.Location;
using System.Net.Http;
using System.Net.Http.Headers;
using SKYPE4COMLib; // comentat temporar Sorin
using System.Timers;
using System.Windows.Media.Effects;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Windows.Forms;
using System.ComponentModel;

namespace ElectriTrackerCenter
{
    public partial class MainWindow : Window
    {
        public class User
        {
            public string Name { get; set; }

            public string Battery { get; set; }

            public string Location { get; set; }

            public string Status { get; set; }

            public string Urgenta { get; set; }

            public int Marime { get; set; }

            public double Telefon { get; set; }
        }
        //DECLARAREA GLOBALA
        int i;
        List<User> items = new List<User>();
        List<User> items2 = new List<User>();
        BlurEffect myBlurEffect = new BlurEffect();
        bool ok = true,ok2=true,intreaba,first=true;
        int counter = 0, counter2=0;
        string mesajeroare = "Eroare necunoscuta", mesajeroare2 = "-EROARE DE CONEXIUNE-" + Environment.NewLine + Environment.NewLine + "Conexiunea intre server si" + Environment.NewLine + "aplicatie nu a putut fi stabilita";
        string URL;
        object sender1;
        MouseButtonEventArgs e1;
        // SF. DECLARARE GLOBALA
        System.Windows.Threading.DispatcherTimer myDispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();

            myMap.Effect = myBlurEffect;
            myMap.IsEnabled = false;
            lvDataBinding.Effect = myBlurEffect;
            lvDataBinding.IsEnabled = false;

            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000); // 1 second
            myDispatcherTimer.Tick += new EventHandler(Each_Tick);
            myDispatcherTimer.Start();
        }
        public void Each_Tick(object o, EventArgs sender)
        {
            counter++;
            int afisez = 6-counter;
            eroare.Content = "Reincerc in " + afisez.ToString() + "s (" + counter2.ToString() + ")" + Environment.NewLine + "        Ultimul mesaj " + mesajeroare;
            if (counter == 5)
            {
                if (ok2)
                {
                    counter = 4;
                    ok2 = false;
                    myDispatcherTimer.Stop();
                    ReadAllSettings();
                }
                else
                {
                    counter = 0;
                    counter2++;
                    items2 = new List<User>(items);
                    foreach (Pushpin p in myMap.Children)
                    {

                        if (items2.Exists(w => w.Name == p.Uid))
                        {
                            if (items2.Exists(w => w.Name == p.Uid && w.Status == "ok-icon-hi.png"))
                                p.Background = new SolidColorBrush(Colors.Green);
                            else if (items2.Exists(w => w.Name == p.Uid && w.Status == "Plossy-red-icon-angle-md.png"))
                                p.Background = new SolidColorBrush(Colors.Red);
                            else if (items2.Exists(w => w.Name == p.Uid && w.Status == "Xlank Badge Grey.png"))
                                p.Background = new SolidColorBrush(Colors.Gray);

                        }
                    }
                    items.Clear();
                    lvDataBinding.ItemsSource = null;
                    GetData(true);
                }
            }
        }
        public void ordonare()
        {
            items.Sort(delegate(User x, User y)
            {

                return x.Name.CompareTo(y.Name);
            });
            items.Sort(delegate(User x, User y)
            {
                return x.Status.CompareTo(y.Status);
            });

            items.Sort(delegate(User x, User y)
            {
                return y.Urgenta.CompareTo(x.Urgenta);
            });
        }
        public void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    mesajeroare2 = "            -EROARE DE CONFIGURARE-" + Environment.NewLine + Environment.NewLine + "Fisierul de setari este necompletat sau inexistent." + Environment.NewLine + "       Incercati din nou restartand aplicatia.";
                    eroare2.Content = mesajeroare2;
                    eroare2.Visibility = Visibility.Visible;
                    myDispatcherTimer.Stop();
                }
                else
                {
                    URL = appSettings["ElectriTrackerServiceAddress"];
                    intreaba = Convert.ToBoolean(appSettings["Intreaba"]);
                    myDispatcherTimer.Start();
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                mesajeroare2 = "-EROARE DE CONFIGURARE-" + Environment.NewLine + Environment.NewLine + "A intervenit o eroare" + Environment.NewLine + "citind fisierul de setari." + Environment.NewLine + "Incercati din nou restartand aplicatia.";
                eroare2.Content = mesajeroare2;
                eroare2.Visibility = Visibility.Visible;
                myDispatcherTimer.Stop();
                
            }
        }
        public void succes()
        {
            myMap.Effect = null;
            checkbox2.Visibility = Visibility.Visible;
            myMap.IsEnabled = true;
            lvDataBinding.Effect = null;
            lvDataBinding.IsEnabled = true;
            eroare.Visibility = Visibility.Hidden;
            eroare2.Visibility = Visibility.Hidden;
            if (eroare2_Copy1.Visibility == Visibility.Visible ||
                eroare2xbutton.Visibility == Visibility.Visible ||
                checkbox1.Visibility == Visibility.Visible)
            {
                eroare2_Copy1.Visibility = Visibility.Hidden;
                eroare2xbutton.Visibility = Visibility.Hidden;
                checkbox1.Visibility = Visibility.Hidden;
            }
        }
        public void eroare1(string error)
        {
            checkbox2.Visibility = Visibility.Hidden;
            myMap.Effect = myBlurEffect;
            myMap.IsEnabled = false;
            lvDataBinding.Effect = myBlurEffect;
            lvDataBinding.IsEnabled = false;
            eroare.Visibility = Visibility.Visible;
            eroare2.Visibility = Visibility.Visible;
            if (error == "Baza de date incompleta")
            {
                eroare2_Copy1.Visibility = Visibility.Visible;
                eroare2xbutton.Visibility = Visibility.Visible;
                checkbox1.Visibility = Visibility.Visible;
            }
            mesajeroare = error;
        }
        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["Intreaba"].Value = "True";
                    config.Save(ConfigurationSaveMode.Modified);

                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
        void MyMethodToCallExpansiveOperation()
        {
            //Call method to show wait screen
            BackgroundWorker workertranaction = new BackgroundWorker();
            workertranaction.DoWork += new DoWorkEventHandler(workertranaction_DoWork);
            workertranaction.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                workertranaction_RunWorkerCompleted);
            workertranaction.RunWorkerAsync();
        }
        void workertranaction_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        void workertranaction_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
        public void GetData(bool refresh)
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("api/Tracker/GetAllInformation").Result;

            if (response.IsSuccessStatusCode)
            {

                string[] allInformation = response.Content.ReadAsAsync<string[]>().Result;
                for (i = 0; i < allInformation.Length; i++)
                {
                    if (refresh == false)
                    {
                        string information = allInformation[i];
                        char[] delimiterChars = { ',' };

                        string[] words = information.Split(delimiterChars);
                        if (words[0] != "" && words[1] != "" && words[2] != "" && words[3] != "" && words[4] != "" && words[5] != "" && words[6] != "")
                        {
                            string status;
                            if (words[4] == 0.ToString())
                                status = "ok-icon-hi.png";
                            else if (words[4] == 2.ToString())
                                status = "Xlank Badge Grey.png";
                            else
                                status = "Plossy-red-icon-angle-md.png";
                            string urgenta;
                            if (words[5] == 1.ToString())
                                urgenta = "Visible";
                            else
                                urgenta = "Hidden";
                            double telefon = Convert.ToDouble(words[6]);
                            items.Add(new User() { Name = words[0], Battery = words[3].ToString() + "%", Location = words[1] + "," + words[2], Status = status, Urgenta = urgenta, Marime = 100, Telefon = telefon });
                        }
                        else
                        {
                            if (intreaba)
                                succes();
                            else
                                eroare1("Baza de date incompleta");
                            break;
                        }
                    }
                    else
                    {
                        string information = allInformation[i];
                        char[] delimiterChars = { ',' };

                        string[] words = information.Split(delimiterChars);
                        if (words[0] != "" && words[1] != "" && words[2] != "" && words[3] != "" && words[4] != "" && words[5] != "" && words[6] != "")
                        {
                            string status;
                            if (words[4] == 0.ToString())
                                status = "ok-icon-hi.png";
                            else if (words[4] == 2.ToString())
                                status = "Xlank Badge Grey.png";
                            else
                                status = "Plossy-red-icon-angle-md.png";
                            string urgenta;
                            if (words[5] == 1.ToString())
                                urgenta = "Visible";
                            else
                                urgenta = "Hidden";
                            double telefon;
                            if (Regex.Matches(words[6], @"[a-zA-Z]").Count >=1)
                                telefon = 00000000;
                            else
                                telefon = Convert.ToDouble(words[6]);
                            if (items2.Exists(w => w.Name == words[0] && w.Marime == 160))
                                items.Add(new User() { Name = words[0], Battery = words[3].ToString() + "%", Location = words[1] + "," + words[2], Status = status, Urgenta = urgenta, Marime = 160, Telefon = telefon });
                            else
                                items.Add(new User() { Name = words[0], Battery = words[3].ToString() + "%", Location = words[1] + "," + words[2], Status = status, Urgenta = urgenta, Marime = 100, Telefon = telefon });
                        }
                        else
                        {
                            eroare2.Content = "-EROARE DE CONEXIUNE-" + Environment.NewLine + Environment.NewLine + "Conexiunea intre server si" + Environment.NewLine + "aplicatie nu a putut fi stabilita";
                            if (intreaba)
                                succes();
                            else
                                eroare1("Baza de date incompleta");
                            break;
                        }

                    }
                }
                if ((i == allInformation.Length) || intreaba == true)
                {
                    counter2 = 0;
                    lvDataBinding.ItemsSource = items;
                    ordonare();
                    succes();
                    eroare2.Content = "-EROARE DE CONEXIUNE-" + Environment.NewLine + Environment.NewLine + "Conexiunea intre server si" + Environment.NewLine + "aplicatie nu a putut fi stabilita";
                }
            }
            else
            {
                eroare1(" : Message - " + response.ReasonPhrase);
                eroare2.Content = "-EROARE DE CONEXIUNE-" + Environment.NewLine + Environment.NewLine + "Conexiunea intre server si" + Environment.NewLine + "aplicatie nu a putut fi stabilita";
            }

        }

        List<UIElement> toRemovex = new List<UIElement>();

        public void ShowPin(double latitudine, double longitudine, string UID)
        {
            Pushpin pushpin = new Pushpin();
            MapLayer.SetPosition(pushpin, new Location(latitudine, longitudine));
            pushpin.Uid = UID;
            myMap.Children.Add(pushpin);
            toRemovex.Add(pushpin);
            Location center = new Location(latitudine, longitudine);
            myMap.Center = center;
            myMap.ZoomLevel = 13;

        }
        public void HidePin(string UID)
        {
            foreach (UIElement p in myMap.Children)
            {
                if ((p.GetType() == typeof(Pushpin)) && p.Uid == UID)
                {
                    myMap.Children.Remove(p);
                    break;
                }
            }
        }

        private void Canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            User user = (User)canvas.DataContext;
            if (user.Marime == 100)
            {
                foreach (User d in items)
                {
                    if (d.Marime == 160)
                    { d.Marime = 100; HidePin(d.Name); }
                }
                items.Where(w => w.Name == user.Name).ToList().ForEach(s => s.Marime = 160);
                string information = user.Location;
                char[] delimiterChars = { ',' };
                string[] words = information.Split(delimiterChars);
                double longitudine = Convert.ToDouble(words[0]), latitudine = Convert.ToDouble(words[1]);
                ShowPin(longitudine, latitudine, user.Name);
            }
            else
            {
                items.Where(w => w.Name == user.Name).ToList().ForEach(s => s.Marime = 100);
                HidePin(user.Name);
            }
            lvDataBinding.ItemsSource = null;
            lvDataBinding.ItemsSource = items;
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image imagine = (Image)sender;
            User user = (User)imagine.DataContext;
            Skype skype = new Skype();
            if (skype.ActiveCalls.Count != 1)
                skype.PlaceCall(user.Telefon.ToString());
            else
                System.Windows.Forms.MessageBox.Show("Un apel este deja in desfasurare, iar apelul nu a putut fi initiat.");
        }
        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show
            ("Ignorand aceasta eroare, aplicatia s-ar putea sa nu functioneze corespunzator. Continuati?", "Error",
            System.Windows.Forms.MessageBoxButtons.YesNo, 
            System.Windows.Forms.MessageBoxIcon.Question)
            ==System.Windows.Forms.DialogResult.Yes)
            {
                string oare = checkbox1.IsChecked.ToString();
                if(oare == "True")
                    AddUpdateAppSettings("Intreaba", "true");
                intreaba = true;
                counter = 4;
            }
            else
            {

            }
            // DO stuff after 'NO is clicked'
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (User x in items)
            {
                string information = x.Location;
                char[] delimiterChars = { ',' };
                string[] words = information.Split(delimiterChars);
                double longitudine = Convert.ToDouble(words[0]), latitudine = Convert.ToDouble(words[1]);
                ShowPin(longitudine, latitudine, x.Name);
                first = false;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if(first==false)
                foreach(User x in items)
                {
                    HidePin(x.Name);
                }
        }

    }
} 