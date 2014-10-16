using Raspberry.IO.GeneralPurpose;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace RaspberryIO
{
    public enum LedMode
    {
        Blinking,
        Single,
        Moving
    }

    public interface IRaspberry
    {
        event EventHandler<int> ActiveLedChanged;
        event EventHandler<double> TemperatureChanged;

        int ActiveLedIndex { get; set; }

        double Temperature { get; set; }
    }

    public class DummyRaspberry : IRaspberry
    {
        private Timer _timer;

        public DummyRaspberry()
        {
            _timer = new Timer(2000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            Temperature = 21;
        }

        public event EventHandler<int> ActiveLedChanged;

        public event EventHandler<double> TemperatureChanged;

        public int ActiveLedIndex { get; set; }

        public double Temperature { get; set; }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var rand = new Random((int)DateTime.Now.Ticks);

            var value = (double)rand.Next(0, 100);

            if (value % 2 == 0)
            {
                Temperature += value / 100;
            }
            else
            {
                Temperature -= value / 100;
            }

            Temperature = Math.Round(Temperature, 2);

            if (TemperatureChanged != null)
                TemperatureChanged(this, Temperature);
        }
    }

    public class Raspberry : IRaspberry, IDisposable
    {
        private volatile int _speed;
        private LedMode _mode;
        private Action _ledAction;

        private List<OutputPinConfiguration> _pins;
        private List<GpioConnection> _connections;

        public Raspberry()
        {
            this.ActiveLedIndex = 0;
            this.Mode = LedMode.Blinking;

            _pins = new List<OutputPinConfiguration>
            {
                ConnectorPin.P1Pin11.Output(),
                ConnectorPin.P1Pin12.Output(),
                ConnectorPin.P1Pin13.Output()
            };

            _connections = new List<GpioConnection>
            {
                 new GpioConnection(_pins[0]),
                 new GpioConnection(_pins[1]),
                 new GpioConnection(_pins[2])
            };

            _connections.ForEach(c => c.Open());

            Task.Factory.StartNew(() =>
            {
                BlinkTest();
            });
        }

        ~Raspberry()
        {
            Dispose(false);
        }

        public event EventHandler<double> TemperatureChanged;

        public event EventHandler<int> ActiveLedChanged;

        public int ActiveLedIndex { get; set; }

        public double Temperature { get; set; }

        public LedMode Mode
        {
            get { return _mode; }

            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    ChangeMode(_mode);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void SetSpeed(int speed)
        {
            _speed = speed;
        }

        private void BlinkTest()
        {
            var led1Config = ConnectorPin.P1Pin11.Output();

            var led1Con = new GpioConnection(led1Config);

            led1Con.Open();

            for (var i = 0; i < 30; i++)
            {
                led1Con.Toggle(led1Config);
                System.Threading.Thread.Sleep(1000);
            }

            led1Con.Close();
        }

        private void DoWork()
        {
            while(true)
            {
                _ledAction();
                System.Threading.Thread.Sleep(_speed);
            }
        }

        private void BlinkAction()
        {
            _connections[ActiveLedIndex].Toggle(_pins[ActiveLedIndex]);
        }

        private void SingleAction()
        {
        }

        private void MoveAction()
        {
        }

        private void ChangeMode(LedMode mode)
        {
            switch (mode)
            {
                case LedMode.Blinking:
                    _ledAction = BlinkAction;
                    break;

                case LedMode.Single:
                    break;

                case LedMode.Moving:
                    break;

                default:
                    break;
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connections.ForEach(c => c.Close());
            }
        }
    }
}