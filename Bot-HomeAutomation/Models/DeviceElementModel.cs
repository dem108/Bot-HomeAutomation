using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HomeAutomationPlatformAPI.Models
{
    public enum SwitchStatus
    {
        Off = 0,
        On
    }

    public enum ElementType
    {
        Cooler = 0,
        Heater,
        Light,
        Temperature,
        Humidity,
        WeatherForecast,
        Camera
    }

    public class DeviceElementModel
    {
        public ElementType ElementType { get; set; }
        public SwitchStatus SwitchStatus { get; set; }
        public double SetValue { get; set; }
        public double PowerConsumptionValue { get; set; }
        public string ToJsonString(Formatting formatting) => JsonConvert.SerializeObject(this, formatting);
    }

    public class DeviceElementValueModel
    {
        public string DeviceId;
        public ElementType ElementType { get; set; }
        public double SetValue { get; set; }
        public string ToJsonString(Formatting formatting) => JsonConvert.SerializeObject(this, formatting);
    }
    public class DeviceElementSwitchModel
    {
        public string DeviceId;
        public ElementType ElementType { get; set; }
        public SwitchStatus SwitchStatus { get; set; }
        public string ToJsonString(Formatting formatting) => JsonConvert.SerializeObject(this, formatting);
    }

    public class Location
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class DeviceModel
    {
        public string DeviceId { get; set; }
        public DateTime Time { get; set; }
        public Location Location { get; set; }
        public List<VirtualDeviceElement> Elements { get; set; }
        public string ToJsonString(Formatting formatting) => JsonConvert.SerializeObject(this, formatting);
    }
}