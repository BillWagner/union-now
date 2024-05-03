# union-now

Sample scenarios for Discriminated unions in C#

The scenario is based around a client app that reads data from a variety of nodes and sensor types. The data is returned in a JSON structur of the general shape shown in [*summary.json*](summary.json). The array of nodes contains different types of nodes, each with a different set of sensors. The sensors are of different types, each with a different set of properties. The classes used to deserialize the JSON data are shown in [JsonClasses.cs](JsonClasses.cs). These types contain properties for all possible nodes, and all possible properties in any of those nodes. That's very inefficient, and it's error-prone. For any given node type, only some of the properties are relevant. Checking for correct data requires deep analysis of each node, starting with its type.

In order to "emulate" the features that I'd want from unions, I wrote the code in the [SensorData.cs](SensorData.cs) file. This code creates different readonly record struct types for the data that comes along with each sensor. First, the code creates a transformed list of nodes. Each node is a tuple containing the type, and nullable members for each of the possible node types.

Once the nodes are transformed, the code can rely on the type system for much of the validation.

Some of the current code already becomes unwieldy even with only 3 node types. It grows quickly out of control as more node types are added.

Next steps:

1. Add error validation that creates error union types that can help diagnose missing or incorrect data from the "server".
1. Add error validation when the sensor data includes unexpected properties that parsed correctly. That might indicate some upgrades are needed to the code.
1. Model the potential equivalent code using some proposed union syntax.
