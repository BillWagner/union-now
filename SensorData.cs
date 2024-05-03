using System.Text;

namespace UnionsNow;

public enum SensorStatus
{
    Red,
    Yellow,
    Green
}

public enum SensorModel
{
    SmokeAndTemp,
    Entryway,
    Perimeter
}

public readonly record struct SmokeTempSensor(
    int Id,
    string Location,
    SensorStatus OverallStatus, 
    double Temp, 
    SensorStatus TempStatus,
    bool SmokeDetected,
    SensorStatus SmokeStatus);

public readonly record struct EntrywaySensor(
    int Id,
    string Location,
    SensorStatus OverallStatus,
    double Temp,
    SensorStatus TempStatus,
    bool SmokeDetected,
    SensorStatus SmokeStatus,
    bool EntryOpen,
    SensorStatus EntryStatus,
    bool MotionDetected,
    SensorStatus MotionStatus);

public readonly record struct PerimeterSensor(
    int Id,
    string Location,
    SensorStatus OverallStatus,
    IEnumerable<string> OpenSensors,
    IEnumerable<string> ClosedSensors
    )
{
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append("Id = ");
        builder.Append(Id.ToString());
        builder.Append(", Location = ");
        builder.Append((object)Location);
        builder.Append(", OverallStatus = ");
        builder.Append(OverallStatus.ToString());
        builder.Append(", OpenSensors = ");
        builder.Append(string.Join(", ", OpenSensors));
        builder.Append(", ClosedSensors = ");
        builder.Append(string.Join(", ", ClosedSensors));
        return true;
    }
}

public static class Converters
{
    public static (SensorModel type, SmokeTempSensor? tempSensor, EntrywaySensor? entrySensor, PerimeterSensor? perimeterSensor)
        SensorFromNode(Sensor jsonDataSensor) => (SensorType(jsonDataSensor.model), jsonDataSensor) switch
        {
            (SensorModel.SmokeAndTemp, _) => (SensorModel.SmokeAndTemp, SensorFromSmokeNode(jsonDataSensor), null, null),
            (SensorModel.Entryway, _) => (SensorModel.Entryway, null, SensorFromEntrywayNode(jsonDataSensor), null),
            (SensorModel.Perimeter, _) => (SensorModel.Perimeter, null, null, SensorFromPerimeterNode(jsonDataSensor)),
            (_, _) => throw new InvalidOperationException("Invalid sensor type"),
        };
    private static SensorModel SensorType(string jsonData) => jsonData switch
    {
        "smoke-and-temp" => SensorModel.SmokeAndTemp,
        "entryway" => SensorModel.Entryway,
        "perimeter" => SensorModel.Perimeter,
        _ => throw new ArgumentException($"Unknown sensor model type: {jsonData}", nameof(jsonData))
    };

    private static SensorStatus StatusFromString(string jsonData) => jsonData switch
    {
        "red" => SensorStatus.Red,
        "yellow" => SensorStatus.Yellow,
        "green" => SensorStatus.Green,
        _ => throw new InvalidOperationException("Unknown sensor state value")
    };

    private static SmokeTempSensor SensorFromSmokeNode(Sensor sensor) => new SmokeTempSensor
        (
            Id: sensor.id,
            Location: sensor.location,
            OverallStatus: StatusFromString(sensor.status),
            Temp: sensor.data[0].temperature,
            TempStatus: StatusFromString(sensor.data[0].status),
            SmokeDetected: sensor.data[1].smoke,
            SmokeStatus: StatusFromString(sensor.data[1].status)
        );

    private static EntrywaySensor SensorFromEntrywayNode(Sensor sensor) => new EntrywaySensor
        (
            Id: sensor.id,
            Location: sensor.location,
            OverallStatus: StatusFromString(sensor.status),
            Temp: sensor.data[0].temperature,
            TempStatus: StatusFromString(sensor.data[0].status),
            SmokeDetected: sensor.data[1].smoke,
            SmokeStatus: StatusFromString(sensor.data[1].status),
            EntryOpen: (sensor.data[2].entry is "open"),
            EntryStatus: StatusFromString(sensor.data[2].status),
            MotionDetected: sensor.data[3].motion,
            MotionStatus: StatusFromString(sensor.data[3].status)
        );

    private static PerimeterSensor SensorFromPerimeterNode(Sensor sensor) => new PerimeterSensor(
            Id: sensor.id,
            Location: sensor.location,
            OverallStatus: StatusFromString(sensor.status),
            OpenSensors: sensor.open.ToArray(),
            ClosedSensors: sensor.closed.ToArray()
        );
}