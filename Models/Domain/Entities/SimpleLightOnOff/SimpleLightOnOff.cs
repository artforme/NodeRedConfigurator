﻿using Models.Domain.ValueObjects;

namespace Models.Domain.Entities.RgbStrip;

public class SimpleLightOnOff : Chain
{
    public SimpleLightOnOff(Dictionary<string, object> properties) : base(new Info("SimpleLightOnOff"), properties) { }

    public override (string search, object replace)[] GetProperties()
    {
        var keyMapping = new Dictionary<string, string>
        {
            { "Name", "%%name%%" },
            { "Device", "%%device%%" },
            { "DevAddress", "%%dev_address%%" },
            { "Room", "%%room%%" },
            { "Control", "%%control%%" }
        };

        var propertiesList = new List<(string search, object replace)>();

        foreach (var readableKey in keyMapping.Keys)
        {
            if (Properties.TryGetValue(readableKey, out var value))
            {
                propertiesList.Add((keyMapping[readableKey], value));
            }
            else
            {
                throw new KeyNotFoundException($"Property '{readableKey}' not found in the properties dictionary.");
            }
        }

        return propertiesList.ToArray();
    }
}