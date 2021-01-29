using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace Primes_Cube_Simulator
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            Message = "";
            Device = "";
            Measurement = "";
            _scanningRunning = false;
            _data = new MeasurementData {StorePath = "c:/LaserPowerCalibrationData", TimeBetweenScan = 50};
            Gd.DataContext = _data;
            _deviceList = new List<string>();
            _deviceList.Clear();
        }

        #endregion

        #region Cube_Connection

        private void ButtonUSB_OnClick(object sender, RoutedEventArgs e)
        {
            if (_cubeIsConnected) Disconnect();

            try
            {
                Message = "USB connection selected";
                Wrapper.Initialize();
                _connectionType = Wrapper.ConnectionType.ConnectionType_usb;

                if (Wrapper.SearchForDevices(_connectionType, _deviceType, ref _numberOfDevices) ==
                    Wrapper.Result.primes_ok)
                {
                    for (var i = 0; i < _numberOfDevices; ++i)
                    {
                        var info = new byte[64];
                        var infoStr = string.Empty;

                        if (Wrapper.GetDeviceInfo((uint) i, info) == Wrapper.Result.primes_ok)
                        {
                            infoStr = Encoding.ASCII.GetString(info.TakeWhile(x => x != 0).ToArray());
                            _deviceList.Add(infoStr);
                        }

                        var connectionData = new byte[64];
                        var connectionDataStr = string.Empty;

                        if (Wrapper.GetConnectionData((uint) i, connectionData) == Wrapper.Result.primes_ok)
                            connectionDataStr =
                                Encoding.ASCII.GetString(connectionData.TakeWhile(x => x != 0).ToArray());

                        Device = "Number of devices: " + _numberOfDevices + " Info: " + infoStr + " Connection data: " +
                                 connectionDataStr;
                        _cubeInfo = infoStr;
                        _primesConnectionData = connectionData;
                        _primesInfoStr = infoStr;
                    }
                }
                else
                {
                    Device = "No Primes Cube found";
                    Wrapper.DeInitialize();
                }
            }
            catch (Exception exception)
            {
                Message = "Error Cube Initialize USB " + exception;
            }
        }

        private void ButtonBluetooth_OnClick(object sender, RoutedEventArgs e)
        {
            if (_cubeIsConnected) Disconnect();

            try
            {
                Message = "Bluetooth connection selected";
                Wrapper.Initialize();
                _connectionType = Wrapper.ConnectionType.ConnectionType_btc;

                if (Wrapper.SearchForDevices(_connectionType, _deviceType, ref _numberOfDevices) ==
                    Wrapper.Result.primes_ok)
                {
                    _deviceList.Clear();
                    var primesInfoStr = string.Empty;
                    var primesConnectionDataStr = string.Empty;
                    var primesConnectionData = new byte[64];

                    for (var i = 0; i < _numberOfDevices; ++i)
                    {
                        var info = new byte[64];
                        var infoStr = string.Empty;

                        if (Wrapper.GetDeviceInfo((uint) i, info) == Wrapper.Result.primes_ok)
                        {
                            infoStr = Encoding.ASCII.GetString(info.TakeWhile(x => x != 0).ToArray());
                            _deviceList.Add(infoStr);
                        }

                        var connectionData = new byte[64];
                        var connectionDataStr = string.Empty;

                        if (Wrapper.GetConnectionData((uint) i, connectionData) == Wrapper.Result.primes_ok)
                            connectionDataStr =
                                Encoding.ASCII.GetString(connectionData.TakeWhile(x => x != 0).ToArray());

                        primesInfoStr = _deviceList.Find(x => x.Contains("PRIMES"));
                        if (!string.IsNullOrEmpty(primesInfoStr))
                        {
                            primesConnectionData = connectionData;
                            primesConnectionDataStr = connectionDataStr;
                        }
                        else
                        {
                            if (SelectedBluetoothItem.Length != 0)
                                primesConnectionData = Convert.FromBase64String(SelectedBluetoothItem);
                            primesConnectionDataStr = SelectedBluetoothItem;
                        }

                        Device = "Number of devices: " + _numberOfDevices + " Info: " + primesInfoStr +
                                 " Connection data: " + primesConnectionDataStr;
                        _cubeInfo = primesInfoStr;
                    }

                    _primesConnectionData = primesConnectionData;
                    _primesInfoStr = primesInfoStr;
                    _primesConnectionDataStr = primesConnectionDataStr;
                    DeviceList = _deviceList;
                }
                else
                {
                    Device = "No Primes Cube found";
                    Wrapper.DeInitialize();
                }
            }
            catch (Exception exception)
            {
                Message = "Error Cube Initialize USB " + exception;
                Wrapper.DeInitialize();
            }
        }

        private void DisconnectCube_OnClick(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void Disconnect()
        {
            Wrapper.DeInitialize();
            StopScanTimer();
            Message = "Cube disconnected";
            Measurement = "";
            _scanningRunning = false;
            _cubeIsConnected = false;
        }

        private void DList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DList.Items.Count > 0)
                {
                    SelectedBluetoothItem = DList.SelectedItem.ToString();
                    Message = SelectedBluetoothItem;
                }
            }
            catch
            {
                SelectedBluetoothItem = "";
            }
        }

        #endregion

        #region Measurement



        private void ScanCubeData()
        {
            var measurement = new Wrapper.CubeMeasurement();
            var deviceIndex = 0;
            var connectionData = new byte[64];

            var result = Wrapper.ReceiveCubeMeasurement(deviceIndex, ref measurement);

            if (result == Wrapper.Result.primes_ok)
            {
                Message = "Primes Cube ok";

                _data.NumberOfFtdiDevices = _numberOfDevices;
                _data.CubeInfo = _cubeInfo;
                _data.Capacity = measurement.remainingCapacity;
                _data.Absorbertemperatur = measurement.absorberTemperature;
                _data.Power = measurement.power;
                _data.IrradiationTime = measurement.irradiationTime;
                _data.Battery = measurement.battery;
                _data.OnTime = measurement.onTime;
                _data.OffTime = measurement.offTime;
                _data.BurstTime = measurement.correctedBurstTime;
                _data.AveragePower = measurement.correctedAveragePower;
                _data.PeakPower = measurement.peakPower;
                _data.NumberOfPulse = measurement.numberOfPulses;

                if (measurement.isReadyForMeasurement)
                {
                    Measurement = "Measurement is Ready = " + measurement.isReadyForMeasurement
                                                            + " -> Waiting for laser beam "
                                                            + " Is measuring"
                                                            + " Nr: " + _numberOfMeasurements;
                    _data.Comment = Measurement;

                    if (_data.TimeBetweenScan > 20) _data.TimeBetweenScan--;
                    _scanTimer.Interval = TimeSpan.FromMilliseconds(_data.TimeBetweenScan);
                }
                else if (measurement.isMeasurementRunning)
                {
                    Measurement = "MeasurementRunning = " + measurement.isReadyForMeasurement
                                                          + " -> Measurement is running "
                                                          + " Measure Nr: " + _numberOfMeasurements;
                    _data.Comment = Measurement;
                    _numberOfMeasurements++;
                    _data.MeasureTime++;
                    _storeData = true;
                }
                else if (measurement.isMeasurementFinished)
                {
                    Measurement = " -> Measurement is Finished";
                    _data.Comment = Measurement;
                    _data.MeasureTime = 0;
                    _data.Energy = _data.AveragePower * _data.BurstTime / 1000;
                    _numberOfMeasurements = 0;

                    if (_storeData) SaveMeasurementData();
                }
            }
            else if (result == Wrapper.Result.primes_notReady)
            {
                Message = "Primes Cube Not ready";
                if (_data.TimeBetweenScan < 100) _data.TimeBetweenScan++;
                _scanTimer.Interval = TimeSpan.FromMilliseconds(_data.TimeBetweenScan);
            }
            else if (result == Wrapper.Result.primes_connectionError)
            {
                Message = "Connection error";
                if (Wrapper.ConnectToDevice(_connectionType, _deviceType, connectionData,
                    ref deviceIndex) != Wrapper.Result.primes_ok)
                    Thread.Sleep(100);
            }
        }

        private void StartMeasurement_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_scanningRunning)
            {
                Measurement = string.Empty;

                if (!_cubeIsConnected)
                {
                    var deviceIndex = 0;

                    if (Wrapper.ConnectToDevice(_connectionType, _deviceType, _primesConnectionData, ref deviceIndex) ==
                        Wrapper.Result.primes_ok)
                    {
                        if (deviceIndex >= 0)
                        {
                            Device = " Info: " + _primesInfoStr;
                            _cubeIsConnected = true;
                            StartScanTimer();

                            if (Wrapper.Disconnect() == Wrapper.Result.primes_ok) Message = "Disconnect";
                        }
                    }
                    else
                    {
                        Message = "Primes not ok";
                        _scanningRunning = false;
                    }
                }
                else
                {
                    Message = "Measurement scanning is running";
                    StartScanTimer();
                }
            }
        }

        private void StartScanTimer()
        {
            _time = _data.TimeBetweenScan;
            _timeSpanBetweenScans = TimeSpan.FromMilliseconds(_time);
            _scanningRunning = true;
            _numberOfMeasurements = 0;
            _scanTimer.IsEnabled = true;
            _scanTimer.Start();
            _scanTimer.Interval = _timeSpanBetweenScans;
            _scanTimer.Tick += MeasurementTimer_Tick;
        }

        private void StopScanTimer()
        {
            _scanTimer.Stop();
            _scanTimer.IsEnabled = false;
            _scanTimer.Tick -= MeasurementTimer_Tick;
        }

        private void MeasurementTimer_Tick(object sender, EventArgs e)
        {
            ScanCubeData();
        }

        private void StopMeasurement_OnClick(object sender, RoutedEventArgs e)
        {
            _scanningRunning = false;
            Message = "the measurement has been stopped";
            _numberOfMeasurements = 0;
            Measurement = "";
            StopScanTimer();
        }

        #endregion

        #region Save Measurement

        private void SaveMeasurementData()
        {
            var foldername = $@"{DateTime.Now:yyyy_MM_dd}".Trim();
            var path = _data.StorePath;
            var info = new {_data.User, _data.Machine, _data.Comment};
            var serialNumbers = new
            {
                _data.RaylaseScanhead,
                _data.RaylaseScancard,
                Laser = _data.LaserSerialNumber,
                Cube = _data.CubeSerialNumber
            };
            var affix = "";

            var saveData = new
            {
                _data.CubeInfo,
                _data.Capacity,
                _data.Absorbertemperatur,
                _data.Power,
                _data.AveragePower,
                _data.PeakPower,
                _data.IrradiationTime,
                _data.Battery,
                _data.OnTime,
                Offtime = _data.OffTime,
                _data.BurstTime,
                _data.NumberOfPulse
            };
            SaveJsonFile(saveData, $@"{path}\{foldername}\", affix,
                header: new {Info = info, SerialNumbers = serialNumbers});
            Message = " Measurement Data is stored";
            _storeData = false;
        }

        private static void SaveJsonFile(object data, string path, string affix = "", string suffix = "",
            bool date = true, object header = null)
        {
            var json = JsonConvert.SerializeObject(header != null ? new {header, data} : data, Formatting.Indented);

            var datetimeString = "";

            try
            {
                path = Path.GetFullPath(path);
            }
            catch (ArgumentException)
            {
                path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                    .FullName;
            }

            if (date) datetimeString = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";

            if (!Directory.Exists($@"{path}\")) Directory.CreateDirectory($@"{path}\");

            File.WriteAllText($@"{path}\{affix}{datetimeString}{suffix}.json", json);
        }

        #endregion Save Measurement

        #region private properties

        private readonly MeasurementData _data;
        private List<string> _deviceList;

        private Wrapper.ConnectionType _connectionType = Wrapper.ConnectionType.ConnectionType_btc;
        private readonly Wrapper.DeviceType _deviceType = Wrapper.DeviceType.DeviceType_cube;

        private byte[] _primesConnectionData = new byte[64];
        private string _primesConnectionDataStr = string.Empty;
        private string _primesInfoStr = string.Empty;
        private bool _cubeIsConnected;

        private uint _numberOfDevices;
        private int _numberOfMeasurements;

        private readonly DispatcherTimer _scanTimer = new DispatcherTimer();
        private TimeSpan _timeSpanBetweenScans = TimeSpan.FromSeconds(_time);

        private static double _time = 2;
        private string _message;
        private string _cubeInfo;
        private string _device;
        private string _measurement;
        private bool _scanningRunning;
        private bool _storeData = true;

        #endregion

        #region public properties

        public string Message
        {
            set
            {
                _message = value;
                ConnectInfo.DataContext = Message;
            }
            get => _message;
        }

        public string Device
        {
            set
            {
                _device = value;
                FindDevices.DataContext = Device;
            }
            get => _device;
        }

        public List<string> DeviceList
        {
            set
            {
                _deviceList = value;
                DList.DataContext = DeviceList;
            }
            get => _deviceList;
        }

        public string SelectedBluetoothItem { set; get; }

        public string Measurement
        {
            set
            {
                _measurement = value;
                MeasurementInfo.DataContext = Measurement;
            }
            get => _measurement;
        }

        #endregion public properties
    }
}