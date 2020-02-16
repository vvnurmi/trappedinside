using System.IO;
using YamlDotNet.Serialization;

public class TiaScript
{
    /// <summary>
    /// Human-readable name for identification.
    /// </summary>
    public string ScriptName { get; set; }

    /// <summary>
    /// If true then start executing steps immediately.
    /// </summary>
    public bool PlayOnStart { get; set; }

    public TiaStep[] Steps { get; set; }

    /// <summary>
    /// Creates a new <see cref="TiaScript"/> for serialized form.
    /// </summary>
    public static TiaScript Read(string serialized)
    {
        var input = new StringReader(serialized);

        var deserializer = new DeserializerBuilder()
            .Build();

        return deserializer.Deserialize<TiaScript>(input);
    }
}
