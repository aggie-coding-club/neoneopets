using MongoDB.Driver;
using MongoDB.Bson;
public class MongoConnector {
    public static string pswd { get; } = "$MONGO_SECRET";
    string connectionString;
    MongoClient client;
    IMongoCollection<BsonDocument> userCollection;
    public MongoConnector() {
        connectionString = "mongodb+srv://neoneopets:" + pswd + "@neoneopets.3l1txwu.mongodb.net/?retryWrites=true&w=majority";
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        client = new MongoClient(settings);
        var database = client.GetDatabase("Site-Data");
        userCollection = database.GetCollection<BsonDocument>("users");
    }
    public BsonDocument GetUser(string username) {
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        return userCollection.Find(filter).First();
    }
    public bool UserExists(string username) {
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        return userCollection.Find(filter).Count() > 0;
    }
    public BsonDocument ReplaceUser(string username, BsonDocument bson) {
        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        return userCollection.ReplaceOne(filter, bson).ToBsonDocument();
    }
}
