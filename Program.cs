// Load the doc, then create things:
using System.Text.Json;
using UnionsNow;

string fileName = "summary.json";
using FileStream openStream = File.OpenRead(fileName);
Root? rawData = await JsonSerializer.DeserializeAsync<Root>(openStream);

Console.WriteLine(rawData);


// Now, build the correct sensor types:

// I really wish this was a union, not some odd-ball Tuple:
IEnumerable<(SensorModel type, SmokeTempSensor? smoke, EntrywaySensor? entryway, PerimeterSensor? perimeter)> unionOfSensors =
    from item in rawData?.sensors ?? Enumerable.Empty<Sensor>()
    select Converters.SensorFromNode(item);

foreach (var item in unionOfSensors)
{
    Console.WriteLine($"\tThis node is of type {item.type}");
    // If this were a union, it would be a single line,
    // not a switch on the type. The type might not even be needed, as
    // its only used to determine which pattern to use. Its easier than
    // the amount of null checks to add.
    Console.WriteLine($"{item.type switch
    {
        SensorModel.SmokeAndTemp => item.smoke.ToString(),
        SensorModel.Entryway => item.entryway.ToString(),
        SensorModel.Perimeter => item.perimeter.ToString(),
        _ => "Unknown",
    }}");
    Console.WriteLine();
    Console.WriteLine();
}
