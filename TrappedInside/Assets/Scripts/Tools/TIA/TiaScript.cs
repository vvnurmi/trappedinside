using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class TiaScript : ScriptableObject
{
    /// <summary>
    /// Helper for <see cref="Read(string)"/> to deserialize a <see cref="TiaScript"/>
    /// using YamlDotNet without having to construct the instance via
    /// <see cref="ScriptableObject.CreateInstance{T}"/>.
    /// </summary>
    private class Serializable
    {
        public string Description { get; set; }
        [YamlMember(Alias = "AutoPlay")]
        public bool PlayOnStart { get; set; }
        public TiaStep[] Steps { get; set; }

        public TiaScript ToTiaScript()
        {
            var tiaScript = CreateInstance<TiaScript>();
            tiaScript.Description = Description;
            tiaScript.PlayOnStart = PlayOnStart;
            tiaScript.Steps = Steps;
            return tiaScript;
        }
    }

    private const string TiaActionTypeNamePrefix = "Tia";

    /// <summary>
    /// A TIA script that does nothing.
    /// </summary>
    public static TiaScript Empty
    {
        get
        {
            if (empty == null)
                empty = new TiaScript
                {
                    Description = "<Empty Script>",
                    Steps = new TiaStep[0],
                };
            return empty;
        }
    }
    private static TiaScript empty;

    /// <summary>
    /// Human-readable account of what the script is about.
    /// </summary>
    [field: SerializeField]
    public string Description { get; set; }

    /// <summary>
    /// If true then start executing steps immediately.
    /// </summary>
    [YamlMember(Alias = "AutoPlay")]
    [field: SerializeField]
    public bool PlayOnStart { get; set; }

    [field: SerializeReference]
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
        foreach (var type in TiaActionTypes.Value)
        {
            var tag = "!" + type.Name.Substring(TiaActionTypeNamePrefix.Length);
            deserializerBuilder.WithTagMapping(tag, type);
        }

        var deserializer = deserializerBuilder.Build();
        var serializable = deserializer.Deserialize<Serializable>(serialized);
        var tiaScript = serializable.ToTiaScript();

        tiaScript.Steps = tiaScript.Steps ?? new TiaStep[0];

        VerifyThatActionsDeserializedProperly(tiaScript);
        SetDebugNames(tiaScript);

        return tiaScript;
    }

    private static Lazy<Type[]> TiaActionTypes = new Lazy<Type[]>(() =>
        FindTiaActionTypes().OrderBy(t => t.Name).ToArray());

    private static IEnumerable<Type> FindTiaActionTypes()
    {
        foreach (var type in typeof(TiaScript).Module.GetTypes())
        {
            if (type.IsAbstract) continue;
            if (!typeof(ITiaAction).IsAssignableFrom(type)) continue;
            if (ReferenceEquals(type, typeof(ITiaAction))) continue;

            if (!type.Name.StartsWith(TiaActionTypeNamePrefix))
                Debug.LogError($"Ignoring TIA action type '{type.Name}' because its name isn't prefixed '{TiaActionTypeNamePrefix}'");
            else
                yield return type;
        }
    }

    [System.Diagnostics.Conditional("TIA_DEBUG")]
    private static void SetDebugNames(TiaScript tiaScript)
    {
        for (int stepIndex = 0; stepIndex < tiaScript.Steps.Length; stepIndex++)
        {
            var step = tiaScript.Steps[stepIndex];
            step.DebugName = $"Step #{stepIndex}";
            for (int sequenceIndex = 0; sequenceIndex < step.Sequences.Length; sequenceIndex++)
            {
                var sequence = step.Sequences[sequenceIndex];
                sequence.DebugName = $"Step #{stepIndex} Sequence #{sequenceIndex}";
                for (int actionIndex = 0; actionIndex < sequence.Actions.Length; actionIndex++)
                {
                    var action = sequence.Actions[actionIndex];
                    action.DebugName = $"Step #{stepIndex} Sequence #{sequenceIndex} Action #{actionIndex}";
                }
            }
        }
    }

    private static void VerifyThatActionsDeserializedProperly(TiaScript tiaScript)
    {
        for (int stepIndex = 0; stepIndex < tiaScript.Steps.Length; stepIndex++)
        {
            for (int sequenceIndex = 0; sequenceIndex < tiaScript.Steps[stepIndex].Sequences.Length; sequenceIndex++)
            {
                var sequence = tiaScript.Steps[stepIndex].Sequences[sequenceIndex];
                if (sequence.Actions == null)
                {
                    Debug.LogWarning($"Replacing null Actions array by an empty one in TIA script '{tiaScript.Description}'."
                        + $"\nLocation: step #{stepIndex} sequence #{sequenceIndex}."
                        + "\nPerhaps there's an Sequence that does nothing and can be removed?");
                    sequence.Actions = new ITiaAction[0];
                }
                for (int actionIndex = 0; actionIndex < sequence.Actions.Length; actionIndex++)
                {
                    if (sequence.Actions[actionIndex] != null) continue;

                    Debug.LogWarning($"Replacing null action by a no-op in TIA script '{tiaScript.Description}'."
                        + $"\nLocation: step #{stepIndex} sequence #{sequenceIndex} action #{actionIndex}."
                        + "\nPerhaps the type tag was incorrect? Valid tags are: "
                        + string.Join(", ", TiaActionTypes.Value.Select(t => "!" + t.Name)));

                    sequence.Actions[actionIndex] = new TiaPause { DurationSeconds = 0 };
                }
            }
        }
    }
}
