using System.ComponentModel;
using System.Runtime.CompilerServices;
using Primes_Cube_Simulator.Annotations;

namespace Primes_Cube_Simulator
{
    internal class MeasurementInfo : INotifyPropertyChanged
    {
        private string _comment;

        private string _cubeSerialNumber;

        private string _laserSerialNumber;

        private string _machine;

        private string _raylaseScancard;

        private string _raylaseScanhead;

        private string _simlationInfo;

        private string _storePath;
        private string _user;

        public string StorePath
        {
            set
            {
                _storePath = value;
                OnPropertyChanged(nameof(StorePath));
            }
            get => _storePath;
        }

        public string SimlationInfo
        {
            set
            {
                _simlationInfo = value;
                OnPropertyChanged(nameof(SimlationInfo));
            }
            get => _simlationInfo;
        }

        public string User
        {
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
            get => _user;
        }

        public string Machine
        {
            set
            {
                _machine = value;
                OnPropertyChanged(nameof(Machine));
            }
            get => _machine;
        }

        public string Comment
        {
            set
            {
                _comment = value;
                OnPropertyChanged(nameof(Comment));
            }
            get => _comment;
        }

        public string RaylaseScanhead
        {
            set
            {
                _raylaseScanhead = value;
                OnPropertyChanged(nameof(RaylaseScanhead));
            }
            get => _raylaseScanhead;
        }

        public string RaylaseScancard
        {
            set
            {
                _raylaseScancard = value;
                OnPropertyChanged(nameof(RaylaseScancard));
            }
            get => _raylaseScancard;
        }

        public string LaserSerialNumber
        {
            set
            {
                _laserSerialNumber = value;
                OnPropertyChanged(nameof(LaserSerialNumber));
            }
            get => _laserSerialNumber;
        }

        public string CubeSerialNumber
        {
            set
            {
                _cubeSerialNumber = value;
                OnPropertyChanged(nameof(CubeSerialNumber));
            }
            get => _cubeSerialNumber;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}