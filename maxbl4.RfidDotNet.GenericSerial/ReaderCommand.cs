﻿namespace maxbl4.RfidDotNet.GenericSerial
{
    public enum ReaderCommand : byte
    {
	    Unrecognized = 0x00,
	    // ISO18000-6C commands
		TagInventory = 0x01,
		ReadData= 0x02,
		WriteData= 0x03,
		WriteEpcNumber = 0x04,
		KillTag = 0x05,
		SetMemoryRead = 0x06,
		BlockErase = 0x07,
		ReadProtectionConfigurationWithEpc = 0x08,
		ReadProtectionConfiguration = 0x09,
		UnlockReadProtection = 0x0a,
		ReadProtectionStatusCheck = 0x0b,
		EasConfiguration = 0x0c,
		EasAlertDetection = 0x0d,
		SingleTagInventory = 0x0f,
		WriteBlocks = 0x10,
		ObtainMonza4QTWorkingParameters = 0x11,
		ModifyMonza4QTWorkingParameters = 0x12,
		ExtendedDataReadingWithAssignedMask = 0x15,
		ExtendedDataWritingWithAssignedMask = 0x16,
		InventoryWithMemoryBuffer = 0x18,
		MixInventory = 0x19,
		InventoryWithEpcNumber = 0x1a,
		QtInventory = 0x1b,

	    // Reader defined commands
	    GetReaderInformation = 0x21,
	    /// <summary>
	    /// Set region
	    /// </summary>
	    ModifyWorkingFrequency = 0x22,
	    ModifyReaderAddress = 0x24,
	    ModifyReaderInventoryTime = 0x25,
	    ModifySerialBaudRate = 0x28,
	    ModifyRfPower = 0x2f,
	    LedOrBuzzerControl = 0x33,
	    SetupAntennaMultiplexing = 0x3f,
	    EnableBuzzer = 0x40,
	    GpioControl = 0x46,
	    ObtainGpioState = 0x47,
	    GetReaderSerialNumber = 0x4c,
	    ModifyTagCustomisedFunction = 0x3a,
	    EnableAntennaCheck = 0x66,
	    ModifyCommunicationInterface = 0x6a,
	    ModifyOrLoadAntennaReturnLossThresholdConfiguration = 0x6e,
	    ModifyMaximumEpcLengthConfigurationForMemoryBuffer = 0x70,
	    LoadTheMaximumEPCLengthConfiguration = 0x71,
	    ObtainDataFromMemoryBuffer = 0x72,
	    ClearMemoryBuffer = 0x73,
	    ObtainTotalTagAmountFromMemoryBuffer = 0x74,
	    ModifyParametersOfRealTimeInventoryMode = 0x75,
	    ModifyWorkingMode = 0x76,
	    LoadRealTimeInventoryModeParameters = 0x77,
	    LoadOrModifyHeartbeatPacketTimeBreakOfRealTimeInventory = 0x78,
	    ModifyRfPowerConfigurationSeparatelyForWriteOperations = 0x79,
	    LoadTheRfPowerConfigurationOfWriteOperations = 0x7a,
	    ModifyOrLoadMaximumWriteRetryTimeConfiguration = 0x7b,
	    ModifyPasswordOfTagCustomisedFunctions = 0x7d,
	    ObtainPasswordOfTagCustomisedFunctions = 0x7e,
	    LoadOrmodifyReaderProfile = 0x7f,
	    SynchroniseEM4325Timestamp = 0x85,
	    ObtainEM4325TemperatureData = 0x86,
	    ObtainExternalDataViaEM4325SPI = 0x87,
	    ResetEM4325Alert = 0x88,
	    ModifyOrloadDrmConfiguration = 0x90,
	    MeasureAntennaReturnLoss = 0x91,
	    GetReaderTemperature = 0x92,
    }
}