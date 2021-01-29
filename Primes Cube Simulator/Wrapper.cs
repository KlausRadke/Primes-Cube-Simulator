using System.Runtime.InteropServices;

namespace Primes_Cube_Simulator
{
    public class Wrapper
    {
        public enum ConnectionType
        {
            ConnectionType_usb = 0,
            ConnectionType_btc,
            ConnectionType_ip
        }

        public enum DeviceType
        {
            DeviceType_cube = 0
        }

        public enum Result
        {
            primes_error = 0,
            primes_notReady = 1,
            primes_ok = 2,
            primes_connectionError = 3,
            primes_notInitialized = 4,
            primes_noDevice = 5
        }

        private const string dllName = "PrimesAPI.dll";
        //   private const string dllName = "FakePrimes.dll";

        [DllImport(dllName, EntryPoint = "initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Initialize();

        [DllImport(dllName, EntryPoint = "deInitialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DeInitialize();

        [DllImport(dllName, EntryPoint = "searchForDevices", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result SearchForDevices(ConnectionType connectionType, DeviceType deviceType,
            ref uint numberOfDevices);

        [DllImport(dllName, EntryPoint = "getDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result GetDeviceInfo(uint deviceNr, byte[] info);

        [DllImport(dllName, EntryPoint = "getConnectionData", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result GetConnectionData(uint deviceNr, byte[] connectionData);

        [DllImport(dllName, EntryPoint = "connectToDevice", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result ConnectToDevice(ConnectionType connectionType, DeviceType deviceType,
            byte[] connectionData, ref int deviceIndex);

        [DllImport(dllName, EntryPoint = "disconnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result Disconnect();

        [DllImport(dllName, EntryPoint = "receiveCubeMeasurement", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result ReceiveCubeMeasurement(int deviceIndex, ref CubeMeasurement measurement);

        [StructLayout(LayoutKind.Sequential)]
        public struct CubeMeasurement
        {
            public double power;
            public double irradiationTime;
            public double energy;
            public double absorberTemperature;
            public double remainingCapacity;
            public double battery;

            public double onTime;
            public double offTime;
            public double correctedBurstTime;
            public double correctedAveragePower;
            public double peakPower;

            public int numberOfPulses;

            [MarshalAs(UnmanagedType.U1)] public bool isReadyForMeasurement;
            [MarshalAs(UnmanagedType.U1)] public bool isMeasurementRunning;
            [MarshalAs(UnmanagedType.U1)] public bool isMeasurementFinished;
        }
    }
}