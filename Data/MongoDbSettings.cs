public class MongoDbSettings
{
    public string MongoDB { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public MongoCollections Collections { get; set; } = new();
}

public class MongoCollections
{
    public string ChatImages { get; set; } = "ChatImages";
}