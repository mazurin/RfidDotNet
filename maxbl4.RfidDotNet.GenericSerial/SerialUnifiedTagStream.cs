using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using maxbl4.RfidDotNet.GenericSerial.Exceptions;
using maxbl4.RfidDotNet.GenericSerial.Ext;
using maxbl4.RfidDotNet.GenericSerial.Model;

namespace maxbl4.RfidDotNet.GenericSerial
{
    public class SerialUnifiedTagStream : IUniversalTagStream
    {
        private readonly ConnectionString connectionString;
        private readonly SerialReader serialReader;
        readonly ConcurrentQueue<TagInventoryResult> inventoryResults = new ConcurrentQueue<TagInventoryResult>();

        public const int DefaultTemperatureLimitCheckInterval = 30000;
        public int TemperatureLimitCheckInterval { get; set; } = DefaultTemperatureLimitCheckInterval;

        public SerialUnifiedTagStream(ConnectionString cs)
        {
            connectionString = cs.Clone();
            serialReader = new SerialReader(new SerialConnectionString(connectionString).Connect());
            serialReader.ThrowOnIllegalCommandError = false;
        }

        public void Dispose()
        {
            doInventory = false;
            serialReader?.DisposeSafe();
        }

        readonly Subject<Tag> tags = new Subject<Tag>();
        public IObservable<Tag> Tags => tags;
        readonly Subject<Exception> errors = new Subject<Exception>();
        public IObservable<Exception> Errors => errors;
        readonly BehaviorSubject<bool> connected = new BehaviorSubject<bool>(false);
        private Task pollingTask;
        private bool doInventory = true;
        private Task tagStreamingTask;
        public IObservable<bool> Connected => connected;
        public async Task Start()
        {
            await serialReader.ActivateOnDemandInventoryMode(true);
            await serialReader.SetAntennaConfiguration((GenAntennaConfiguration) connectionString.AntennaConfiguration);
            await serialReader.SetRFPower((byte)connectionString.RFPower);
            await serialReader.SetInventoryScanInterval(TimeSpan.FromMilliseconds(connectionString.InventoryDuration));
            serialReader.Errors.Subscribe(e =>
            {
                connected.OnNext(false);
                errors.OnNext(e);
            });
            pollingTask = StartPolling();
            tagStreamingTask = StartStreamingTags();
            connected.OnNext(true);
        }

        private Task StartPolling()
        {
            var task = new Task(async () => { 
                var sw = new Stopwatch();
                if (connectionString.ThermalLimit > 0)
                    sw.Start();
                while (doInventory)
                {
                    try
                    {
                        var res = await serialReader.TagInventory(new TagInventoryParams
                        {
                            QValue = (byte)connectionString.QValue,
                            Session = (SessionValue)connectionString.Session
                        });
                        inventoryResults.Enqueue(res);
                        if (sw.ElapsedMilliseconds > TemperatureLimitCheckInterval)
                        {
                            var t = await serialReader.GetReaderTemperature();
                            if (t > connectionString.ThermalLimit)
                            {
                                errors.OnNext(new TemperatureLimitExceededException(connectionString.ThermalLimit, t));
                                Dispose();
                                return;
                            }
                            sw.Restart();
                        }
                    }
                    catch (Exception e)
                    {
                        errors.OnNext(e);
                    }
                }
            }, TaskCreationOptions.LongRunning);
            task.Start();
            return task;
        }
        
        private Task StartStreamingTags()
        {
            var task = new Task(() => 
            { 
                while (doInventory)
                {
                    try
                    {
                        if (inventoryResults.TryDequeue(out var res))
                        {
                            if (res?.Tags != null)
                            {
                                foreach (var tag in res.Tags)
                                {
                                    tags.OnNext(tag);
                                }
                            }
                        }

                        Thread.Yield();
                    }
                    catch (Exception e)
                    {
                        errors.OnNext(e);
                    }
                }
            }, TaskCreationOptions.LongRunning);
            task.Start();
            return task;
        }

        public Task<int> QValue(int? newValue = null)
        {
            if (newValue != null) connectionString.QValue = newValue.Value;
            return Task.FromResult(connectionString.QValue);
        }

        public Task<int> Session(int? newValue = null)
        {
            if (newValue != null) connectionString.Session = newValue.Value;
            return Task.FromResult(connectionString.Session);
        }

        public async Task<int> RFPower(int? newValue = null)
        {
            if (newValue != null)
            {
                connectionString.RFPower = newValue.Value;
                await serialReader.SetRFPower((byte) newValue.Value);
            }

            var info = await serialReader.GetReaderInfo();
            return info.RFPower;
        }

        public async Task<AntennaConfiguration> AntennaConfiguration(AntennaConfiguration? newValue = null)
        {
            if (newValue != null)
            {
                connectionString.AntennaConfiguration = newValue.Value;
                await serialReader.SetAntennaConfiguration((GenAntennaConfiguration) newValue.Value);
            }
            var info = await serialReader.GetReaderInfo();
            return (AntennaConfiguration)info.AntennaConfiguration;
        }
    }
}