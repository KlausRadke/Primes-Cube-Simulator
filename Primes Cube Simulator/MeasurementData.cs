using System.ComponentModel;
using System.Runtime.CompilerServices;
using Primes_Cube_Simulator.Annotations;

namespace Primes_Cube_Simulator
{
    internal class MeasurementData : MeasurementInfo, INotifyPropertyChanged
    {
        private double _absorbertemperatur;

        private double _averagePower;

        private double _battery;

        private double _burstTime;

        private double _capacity;

        private string _cubeInfo;

        private double _energy;

        private double _irradiationTime;

        private double _measureTime;

        private uint _numberOfFtdiDevices;

        private int _numberOfPulse;

        private double _offTime;

        private double _onTime;

        private double _peakPower;

        private double _power;


        private double _timeBetweenScan;

        public double Energy
        {
            set
            {
                _energy = value;
                OnPropertyChanged(nameof(Energy));
            }
            get => _energy;
        }

        public double TimeBetweenScan
        {
            set
            {
                _timeBetweenScan = value;
                OnPropertyChanged(nameof(TimeBetweenScan));
            }
            get => _timeBetweenScan;
        }

        public double MeasureTime
        {
            set
            {
                _measureTime = value;
                OnPropertyChanged(nameof(MeasureTime));
            }
            get => _measureTime;
        }

        public uint NumberOfFtdiDevices
        {
            set
            {
                _numberOfFtdiDevices = value;
                OnPropertyChanged(nameof(NumberOfFtdiDevices));
            }
            get => _numberOfFtdiDevices;
        }

        public string CubeInfo
        {
            set
            {
                _cubeInfo = value;
                OnPropertyChanged(nameof(CubeInfo));
            }
            get => _cubeInfo;
        }

        public double Capacity
        {
            set
            {
                _capacity = value;
                OnPropertyChanged(nameof(Capacity));
            }
            get => _capacity;
        }

        public double Absorbertemperatur
        {
            set
            {
                _absorbertemperatur = value;
                OnPropertyChanged(nameof(Absorbertemperatur));
            }
            get => _absorbertemperatur;
        }

        public double Power
        {
            set
            {
                _power = value;
                OnPropertyChanged(nameof(Power));
            }
            get => _power;
        }

        public double IrradiationTime
        {
            set
            {
                _irradiationTime = value;
                OnPropertyChanged(nameof(IrradiationTime));
            }
            get => _irradiationTime;
        }

        public double Battery
        {
            set
            {
                _battery = value;
                OnPropertyChanged(nameof(Battery));
            }
            get => _battery;
        }

        public double OnTime
        {
            set
            {
                _onTime = value;
                OnPropertyChanged(nameof(OnTime));
            }
            get => _onTime;
        }

        public double OffTime
        {
            set
            {
                _offTime = value;
                OnPropertyChanged(nameof(OffTime));
            }
            get => _offTime;
        }

        public double BurstTime
        {
            set
            {
                _burstTime = value;
                OnPropertyChanged(nameof(BurstTime));
            }
            get => _burstTime;
        }

        public double AveragePower
        {
            set
            {
                _averagePower = value;
                OnPropertyChanged(nameof(AveragePower));
            }
            get => _averagePower;
        }

        public double PeakPower
        {
            set
            {
                _peakPower = value;
                OnPropertyChanged(nameof(PeakPower));
            }
            get => _peakPower;
        }

        public int NumberOfPulse
        {
            set
            {
                _numberOfPulse = value;
                OnPropertyChanged(nameof(NumberOfPulse));
            }
            get => _numberOfPulse;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}