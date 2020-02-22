﻿using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class TiaScript
{
    /// <summary>
    /// Human-readable name for identification.
    /// </summary>
    [YamlMember(Alias = "Name")]
    public string ScriptName { get; set; }

    /// <summary>
    /// If true then start executing steps immediately.
    /// </summary>
    [YamlMember(Alias = "AutoPlay")]
    public bool PlayOnStart { get; set; }

    public TiaStep[] Steps { get; set; }

    /// <summary>
    /// Creates a new <see cref="TiaScript"/> from serialized form.
    /// </summary>
    public static TiaScript Read(string serialized)
    {
        var deserializerBuilder = new DeserializerBuilder()
            .WithNamingConvention(new PascalCaseNamingConvention());

        // Register a tag for each TIA action type to help the deserializer
        // determine the correct derived type of each serialized ITiaAction.
        foreach (var type in typeof(TiaScript).Module.GetTypes())
        {
            if (!typeof(ITiaAction).IsAssignableFrom(type)) continue;
            if (ReferenceEquals(type, typeof(ITiaAction))) continue;

            var actionPrefix = "Tia";
            Debug.Assert(type.Name.StartsWith(actionPrefix), $"{type.FullName} doesn't have name prefix '{actionPrefix}'");
            var tag = "!" + type.Name.Substring(actionPrefix.Length);
            deserializerBuilder.WithTagMapping(tag, type);
        }

        var deserializer = deserializerBuilder.Build();
        return deserializer.Deserialize<TiaScript>(serialized);
    }
}
