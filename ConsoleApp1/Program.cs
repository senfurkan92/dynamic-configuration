using DynamicConfiguration.Lib;

var reader = new ConfigurationReader("App1", "mongodb://root:root@localhost:27017", "configurationSettingsDb", 1000);

for (; ; )
{ 
    Console.WriteLine(reader.GetValue("Config1", default));
}