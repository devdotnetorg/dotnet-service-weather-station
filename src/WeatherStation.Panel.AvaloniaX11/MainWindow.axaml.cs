using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Timers;
using WeatherStation.Panel.AvaloniaX11.Helpers;
using WeatherStation.Panel.AvaloniaX11.Services;
using WeatherStation.Panel.AvaloniaX11.Settings;
using WWeatherStation.Panel.AvaloniaX11.Helpers;

namespace WeatherStation.Panel.AvaloniaX11
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Таймер для отображения теущего времени
        /// </summary>
        private System.Timers.Timer timerCurrentTime;
        /// <summary>
        /// Таймер для отображения времени устаревания полученных данных
        /// </summary>
        private System.Timers.Timer timerLastGetData;
        private readonly object dtLastGetDataLock = new object();
        /// <summary>
        /// Время фиксирования последних полученных данных
        /// </summary>
        private DateTime dtLastGetData;
        /// <summary>
        /// Получение данных от сервера
        /// </summary>
        private IGetData _getData;        
        /// <summary>
        /// Токен закрытия окна
        /// </summary>
        private CancellationTokenSource sourceStoppingToken = new CancellationTokenSource();
        //Chart
        /// <summary>
        /// Максимальное количество точек на графике
        /// </summary>
        private int maxCountValuesChart = 5;
        /// <summary>
        /// Коллекция последних полученных значений температуры для графика
        /// </summary>
        private ObservableCollection<double> _observableValues;        
        /// <summary>
        /// Значения для оси X графика температуры
        /// </summary>
        private List<string> _labelXAxes;        
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            //Change Culture
            //Set Culture
            var vCulture = CultureInfo.GetCultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = vCulture;
            Thread.CurrentThread.CurrentUICulture = vCulture;
            CultureInfo.DefaultThreadCurrentCulture = vCulture;
            CultureInfo.DefaultThreadCurrentUICulture = vCulture;
            //Add event Load
            this.Opened += OnOpened;
            this.Closed += OnClosed;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        /// <summary>
        /// Основная инициализация окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOpened(object sender, System.EventArgs e)
        {            
            //Full Screen
#if RELEASE
             var statusConnect = this.WindowState = WindowState.FullScreen;
#endif
            //Set values
            SetStartValues();
            //Get config
            var appSettings = GetConfiguration();
            //Timer текущего времени
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
            _getData.ConnectAsync(parameters, sourceStoppingToken.Token);
            //            
            timerLastGetData = new System.Timers.Timer(10000);
            timerLastGetData.Elapsed += OnTimedLastGetDataEvent;
            timerLastGetData.AutoReset = true;
            timerLastGetData.Enabled = false;
            //Create Chart            
            _labelXAxes = new List<string>(maxCountValuesChart);            
            _observableValues = new ObservableCollection<double>();            
            //Draw
            var chart = this.FindControl<CartesianChart>("chart1");
            chart.XAxes = new List<Axis>
                       {
                            new Axis
                            {
                                Labels=_labelXAxes
                            }
                       };
            chart.Series = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = _observableValues,
                }
            };
            chart.AutoUpdateEnaled = true;   
        }
        private void OnClosed(object sender, System.EventArgs e)
        {
            sourceStoppingToken.Cancel();
            _getData.Close();
        }
        /// <summary>
        /// Установлено соединение с сервером
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConnectionOpenEvent(object sender, EventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var statusConnect = this.FindControl<Ellipse>("statusConnect");
                statusConnect.Fill = new SolidColorBrush(Colors.Green);
            }, DispatcherPriority.SystemIdle);
        }
        /// <summary>
        /// Потеря соединения с сервером
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnConnectionShutdownEvent(object sender, EventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var statusConnect = this.FindControl<Ellipse>("statusConnect");
                statusConnect.Fill = new SolidColorBrush(Colors.Red);
            }, DispatcherPriority.SystemIdle);
        }
        /// <summary>
        /// Получены данные от сервера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDataReceivedEvent(object sender, EventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var values = (SensorsEventArgs)e;
                bool isNewData = false;
                //HomeTemperature
                if (values._sensors.ContainsKey("HomeTemperature"))
                {
                    var labelsensorTemp = this.FindControl<Label>("sensorTemp");
                    var temp = (double)values._sensors["HomeTemperature"];                    
                    labelsensorTemp.Content = $"+{temp}\u00B0C";
                    isNewData = true;
                    //Add Chart                    
                    var a = (new DateTime((long)values._sensors["DateTimeNow"]).ToLocalTime())
                        .ToString("HH:mm:ss");
                    _labelXAxes.Add(a);
                    //Draw Chart
                    var chart = this.FindControl<CartesianChart>("chart1");                    
                    //Add value
                    _observableValues.Add(temp);
                    //Delete old value
                    if (_observableValues.Count > maxCountValuesChart)
                    {
                        _observableValues.RemoveAt(0);
                        _labelXAxes.RemoveAt(0);
                    }                    
                }
                //HomeHumidity
                if (values._sensors.ContainsKey("HomeHumidity"))
                {
                    var labelsensorHumidityrelative = this.FindControl<Label>("sensorHumidityrelative");
                    labelsensorHumidityrelative.Content = $"{values._sensors["HomeHumidity"]}%";
                    isNewData = true;
                }
                //HomePressure
                if (values._sensors.ContainsKey("HomePressure"))
                {
                    var pressure = (double)values._sensors["HomePressure"];
                    var labelsensorPressure1 = this.FindControl<Label>("sensorPressure1");
                    labelsensorPressure1.Content = $"{Math.Round(pressure / (double)1000, 2)} кПа";
                    var labelsensorPressure2 = this.FindControl<Label>("sensorPressure2");
                    labelsensorPressure2.Content = $"{Math.Round(pressure / (133.32), 0, MidpointRounding.AwayFromZero)} мм.рт.ст.";
                    isNewData = true;
                }
                //Command                
                if (values._sensors.ContainsKey("Command")) CommandHandler((string)values._sensors["Command"]);
                if (isNewData)
                {
                    //Timer
                    lock (dtLastGetDataLock)
                    {
                        dtLastGetData = new DateTime((long)values._sensors["DateTimeNow"]).ToLocalTime();
                    }
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        var labelHowOldData = this.FindControl<Label>("HowOldData");
                        labelHowOldData.Content = GetLabelLastData();
                    }, DispatcherPriority.SystemIdle);
                }
                timerLastGetData.Stop();
                timerLastGetData.Start();
            }, DispatcherPriority.SystemIdle);
        }
        /// <summary>
        /// Отображение текущего времени
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var currentDateTime = DateTime.Now;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var label = this.FindControl<Label>("labelCurrentDateTime");
                label.Content = currentDateTime.ToString("dd MMMM yyyy, dddd") + "\n" +
                currentDateTime.ToString("HH:mm:ss");
            }, DispatcherPriority.SystemIdle);
        }
        /// <summary>
        /// Отображение времени устаревания полученных данных
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedLastGetDataEvent(Object source, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                var labelHowOldData = this.FindControl<Label>("HowOldData");
                labelHowOldData.Content = GetLabelLastData();
            }, DispatcherPriority.SystemIdle);
        }
        /// <summary>
        /// Формирование строки для Label - отображение последних полученных данных
        /// </summary>
        /// <returns></returns>
        private string GetLabelLastData()
        {
            TimeSpan difference;
            lock (dtLastGetDataLock)
            {
                difference = DateTime.Now.Subtract(dtLastGetData);
            }
            //(0-9)Данные получены недавно
            //(10-59)Данные получены 10 секунд назад
            //(60-inf)Данные получены более 1 минут(ы) назад
            string strResult;
            switch (difference.TotalSeconds)
            {
                case < 11:
                    strResult = "Данные получены недавно";
                    break;
                case < 60:
                    strResult = $"Данные получены {Math.Truncate(difference.TotalSeconds / 10) * 10} секунд назад";
                    break;
                default:
                    strResult = $"Данные получены более {Math.Truncate(difference.TotalMinutes)} минут(ы) назад";
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
            //Get EnvironmentVariables. Read settings for RabbitMQ
            SettingsHelper.ReadSettingsforRabbitMQ(configuration, appSettings);
            return appSettings;
        }
        /// <summary>
        /// Установление начальных значений для отображения
        /// </summary>
        private void SetStartValues()
        {
            var labelsensorTemp = this.FindControl<Label>("sensorTemp");
            labelsensorTemp.Content = "Нет данных";
            var labelsensorHumidityrelative = this.FindControl<Label>("sensorHumidityrelative");
            labelsensorHumidityrelative.Content = "Нет данных";
            var labelsensorPressure1 = this.FindControl<Label>("sensorPressure1");
            labelsensorPressure1.Content = "Нет данных";
            var labelsensorPressure2 = this.FindControl<Label>("sensorPressure2");
            labelsensorPressure2.Content = "Нет данных";
            var labelHowOldData = this.FindControl<Label>("HowOldData");
            labelHowOldData.Content = "Нет данных";
        }
        /// <summary>
        /// Обработчик команд
        /// </summary>
        /// <param name="strCommand"></param>
        private void CommandHandler(string strCommand)
        {
            //Показ/скрытие экранов
            if (strCommand == "PressButton1")
            {
                var gridScreen1 = this.FindControl<Grid>("screen1");
                var gridScreen2 = this.FindControl<Grid>("screen2");
                gridScreen1.IsVisible = !gridScreen1.IsVisible;
                gridScreen2.IsVisible = !gridScreen2.IsVisible;
            }            
        }
    }
}




