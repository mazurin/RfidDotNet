using System.Diagnostics;
using System.Linq;
using System.Threading;
using RJCP.IO.Ports;
using Shouldly;
using Xunit;

namespace maxbl4.RfidDotNet.GenericSerial.Tests
{
    [Collection("Hardware")]
    [Trait("Hardware", "True")]
    public class SerialPortTests
    {
        private SerialConnectionString serial;

        public SerialPortTests()
        {
            serial = TestSettings.Instance.GetConnectionStrings().FirstOrDefault(x => x.Type == ConnectionType.Serial);
            Skip.If(serial == null);
        }

        [SkippableFact]
        public void Check_port_timeouts()
        {
            using (var port = new SerialPortStream(serial.ConnectionString.Serial.Port, serial.ConnectionString.Serial.BaudRate, 8, Parity.None, StopBits.One))
            {
                port.Open();
                
                port.CanTimeout.ShouldBeTrue();
                port.ReadTimeout.ShouldBe(Timeout.Infinite);
                port.WriteTimeout.ShouldBe(Timeout.Infinite);

                port.ReadTimeout = 1000;
                port.WriteTimeout = 1000;
                port.ReadTimeout.ShouldBe(1000);
                port.WriteTimeout.ShouldBe(1000);

                var buf = new byte[10];
                var sw = Stopwatch.StartNew();
                port.Read(buf, 0, 10).ShouldBe(0);
                sw.Stop();
                sw.ElapsedMilliseconds.ShouldBeGreaterThan(900);
            }
        }
        
        [SkippableFact]
        public void Read_should_return_even_when_read_less_then_buffer()
        {
            using (var port = new SerialPortStream(serial.ConnectionString.Serial.Port, serial.ConnectionString.Serial.BaudRate, 8, Parity.None, StopBits.One))
            {
                port.Open();
                port.Write(new byte[]{0x04, 0x00, 0x4c, 0x3a, 0xd2}, 0, 5);
                var b = new byte[100];
                var read = port.Read(b, 0, 100);
                read.ShouldBe(10);
            }
        }

        [SkippableFact]
        public void Should_get_serial_raw_multiple_times()
        {
            // Get Reader Serial Number command
            byte[] command = {0x04, 0x00, 0x4c, 0x3a, 0xd2};
            // Response from particular hardware reader
            byte[] expectedResponse = {0x09, 0x00, 0x4c, 0x00, 0x17, 0x43, 0x90, 0x15, 0x49, 0xc0};
            for (var i = 0; i < 10; i++)
            {
                using (var port = new SerialPortStream(serial.ConnectionString.Serial.Port, serial.ConnectionString.Serial.BaudRate, 8, Parity.None, StopBits.One))
                {
                    port.Open();
                    port.Write(command, 0, command.Length);
                    var b = new byte[100];
                    var read = port.Read(b, 0, 100);
                    read.ShouldBe(expectedResponse.Length);
                    expectedResponse.ShouldBe(b.Take(expectedResponse.Length));
                }    
            }
        }
    }
}