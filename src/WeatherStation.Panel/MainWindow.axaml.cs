using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Timers;
using WeatherStation.Panel.Services;
using WeatherStation.Panel.Settings;

namespace WeatherStation.Panel
{
    public partial class MainWindow : Window
    {
        private System.Timers.Timer timerCurrentTime;
        private System.Timers.Timer timerLastGetData;
        private readonly object dtLastGetDataLock = new object();
        private DateTime dtLastGetData;

        private IGetData _getData;
        private CancellationToken stoppingToken;
        private CancellationTokenSource sourceStoppingToken = new CancellationTokenSource();        
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            //Change Culture
            CultureInfo culture = CultureInfo.GetCultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            //Add event Load
            this.Opened += OnOpened;
            this.Closed += OnClosed;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void OnOpened(object sender, System.EventArgs e)
        {
            //Set values
            SetStartValues();
            //Get config
            var appSettings = GetConfiguration();
            //Timer �������� �������
            timerCurrentTime = new System.Timers.Timer(1000);            
            timerCurrentTime.Elapsed += OnTimedEvent;
            timerCurrentTime.AutoReset = true;
            timerCurrentTime.Enabled = true;
            //Get Data RabbitMQ
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserName", appSettings.RabbitMQ.UserName);
            parameters.Add("Password", appSettings.RabbitMQ.Password);
            parameters.Add("VirtualHost", appSettings.RabbitMQ.VirtualHost);
            parameters.Add("HostName", appSettings.RabbitMQ.HostName);
            parameters.Add("ClientProvidedName", appSettings.RabbitMQ.ClientProvidedName);
            parameters.Add("QueueName", appSettings.RabbitMQ.QueueName);
            _getData = new GetDataFromRabbitMQ();       
            //Add events
            _getData.ConnectionOpen += OnConnectionOpenEvent;
            _getData.ConnectionShutdown += OnConnectionShutdownEvent;            
            _getData.DataReceived += OnDataReceivedEvent;
            //Connect
            //_getData.Connect(parameters);
            _getData.ConnectAsync(parameters, sourceStoppingToken.Token);
            //            
            timerLastGetData = new System.Timers.Timer(10000);
            timerLastGetData.Elapsed += OnTimedLastGetDataEvent;
            timerLastGetData.AutoReset = true;
            timerLastGetData.Enabled = false;
        }
        private void OnClosed(object sender, System.EventArgs e)
        {
            sourceStoppingToken.Cancel();
            _getData.Close();
        }
        void OnConnectionOpenEvent(object sender, EventArgs e)
        {
            //����������� ���������� � ��������
            Dispatcher.UIThread.InvokeAsync(() => {
                var statusConnect = this.FindControl<Ellipse>("statusConnect");
                statusConnect.Fill = new SolidColorBrush(Colors.Green);



            }, DispatcherPriority.SystemIdle);
        }
        void OnConnectionShutdownEvent(object sender, EventArgs e)
        {
            //����������� ���������� � ��������
            Dispatcher.UIThread.InvokeAsync(() => {
                var statusConnect = this.FindControl<Ellipse>("statusConnect");
                statusConnect.Fill = new SolidColorBrush(Colors.Red);


            }, DispatcherPriority.SystemIdle);
        }
        void OnDataReceivedEvent(object sender, EventArgs e)
        {
            //����������� ���������� � ��������
            Dispatcher.UIThread.InvokeAsync(() => {
                var values = (SensorsEventArgs)e;
                var labelsensorTemp = this.FindControl<Label>("sensorTemp");
                labelsensorTemp.Content = $"+{values._sensors["HomeTemperature"]}\u00B0C";                
                var labelsensorHumidityrelative = this.FindControl<Label>("sensorHumidityrelative");
                labelsensorHumidityrelative.Content= $"{values._sensors["HomeHumidity"]}%";
                var pressure = (double)values._sensors["HomePressure"];
                var labelsensorPressure1 = this.FindControl<Label>("sensorPressure1");
                labelsensorPressure1.Content = $"{Math.Round(pressure/(double)1000,2)} ���";
                var labelsensorPressure2 = this.FindControl<Label>("sensorPressure2");
                labelsensorPressure2.Content = $"{Math.Round(pressure / (133.32), 0, MidpointRounding.AwayFromZero)} ��.��.��.";
                //Timer
                lock (dtLastGetDataLock)
                {
                    dtLastGetData = new DateTime((long)values._sensors["DateTimeNow"]).ToLocalTime();
                }
                //                
                Dispatcher.UIThread.InvokeAsync(() => {
                    var labelHowOldData = this.FindControl<Label>("HowOldData");
                    labelHowOldData.Content = GetLabelLastData();
                }, DispatcherPriority.SystemIdle);
                //
                timerLastGetData.Stop();
                timerLastGetData.Start();                
            }, DispatcherPriority.SystemIdle);
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var currentDateTime = DateTime.Now;
            Dispatcher.UIThread.InvokeAsync(()=> {
                var label = this.FindControl<Label>("labelCurrentDateTime");
                label.Content = currentDateTime.ToString("dd MMMM yyyy, dddd") +"\n"+
                currentDateTime.ToString("HH:mm");
            }, DispatcherPriority.SystemIdle);            
        }

        private void OnTimedLastGetDataEvent(Object source, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() => {
                var labelHowOldData = this.FindControl<Label>("HowOldData");
                labelHowOldData.Content = GetLabelLastData();
            }, DispatcherPriority.SystemIdle);
        }
        private string GetLabelLastData()
        {
            TimeSpan difference;
            lock (dtLastGetDataLock)
            {
                difference = DateTime.Now.Subtract(dtLastGetData);
            }
            //(0-9)������ �������� �������
            //(10-59)������ �������� 10 ������ �����
            //(60-inf)������ �������� ����� 1 �����(�) �����
            string strResult;
            switch (difference.TotalSeconds)
            {                
                case < 11:
                    strResult = "������ �������� �������";
                    break;    
                case < 60:                    
                    strResult = $"������ �������� {Math.Truncate(difference.TotalSeconds / 10) * 10} ������ �����";
                    break;
                default:
                    strResult = $"������ �������� ����� {Math.Truncate(difference.TotalMinutes)} �����(�) �����";
                    break;                
            }
            return strResult;
        }


        private AppSettings GetConfiguration()
        {
            // build config          
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
            var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>() ?? new AppSettings();
            return appSettings;            
        }
        private void SetStartValues()
        {            
            var labelsensorTemp = this.FindControl<Label>("sensorTemp");
            labelsensorTemp.Content ="��� ������";
            var labelsensorHumidityrelative = this.FindControl<Label>("sensorHumidityrelative");
            labelsensorHumidityrelative.Content = "��� ������";            
            var labelsensorPressure1 = this.FindControl<Label>("sensorPressure1");
            labelsensorPressure1.Content = "��� ������";
            var labelsensorPressure2 = this.FindControl<Label>("sensorPressure2");
            labelsensorPressure2.Content = "��� ������";
            var labelHowOldData = this.FindControl<Label>("HowOldData");
            labelHowOldData.Content = "��� ������"; 
        }

    }
}
