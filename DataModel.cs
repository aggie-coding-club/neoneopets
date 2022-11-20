using System.Text.Json;
using System.Text.Json.Nodes;
using MongoDB.Bson;

namespace InternalAPI.DataModel;

public interface ISharable {
    JsonObject ToJson();
}

public interface IStorable {
    BsonDocument ToBson();
    static IStorable? FromBson(BsonDocument bson) {
        return null;
    }
}

public class UserFull : ISharable, IStorable {
    public string Username { get; init; }
    public string Password { get; init; }
    public AuthSession? Session { get; set; }
    public Dictionary<string, GameRun> Highscores { get; init; }

    private UserFull(){}

    public UserFull(string uname, string hashPwd) {
        Username = uname;
        Password = hashPwd;
        Session = null;
        Highscores = new Dictionary<string, GameRun>();
    }
    
    public UserFull(string uname, string hashPwd, AuthSession auth) {
        Username = uname;
        Password = hashPwd;
        Session = auth;
        Highscores = new Dictionary<string, GameRun>();
    }

    public JsonObject ToJson() {
        var highObj = new JsonObject();
        foreach (var (key, gameRun) in Highscores) {
            highObj![key] = new JsonObject {
                ["score"] = gameRun.Score,
                ["time"] = gameRun.Time
            };
        }

        return new JsonObject {
            ["username"] = Username,
            ["Highscores"] = highObj
        };
    }

    public BsonDocument ToBson() {
        var highObj = new Dictionary<string, Dictionary<string, object>>();
        foreach (var (key, gameRun) in Highscores) {
            highObj.Add(key, new Dictionary<string, object>() {
                { "score", gameRun.Score },
                { "time", gameRun.Time }
            });
        }
        return new BsonDocument(new Dictionary<string, object>() {
            { "username", Username },
            { "password", Password },
            { "highscores", highObj }
        });
    }
    public static UserFull FromBson(BsonDocument bson) {
        Dictionary<string, object> dct = bson.ToDictionary();
        Dictionary<string, object> highDctObj = (Dictionary<string, object>) dct["highscores"];
        Dictionary<string, GameRun> highDct = new Dictionary<string, GameRun>();
        foreach(var (key, obj) in highDctObj) {
            var objdct = (Dictionary<string, object>) obj;
            highDct.Add(key, new GameRun {
                    GameName = key,
                    Username = (string) dct["username"],
                    Score = (int) objdct["score"],
                    Time = (DateTime) objdct["time"]
            });
        }
        return new UserFull {
            Username = (string) dct["username"],
            Password = (string) dct["password"],
            Highscores = highDct
        };
    }
}

public class GameRun {
    public string GameName { get; init; }
    public string Username { get; init; }
    public int Score { get; init; }
    public DateTime Time { get; init; }

    public GameRun(){}

    public GameRun(string gameName, string username, int score, DateTime time) {
        GameName = gameName;
        Username = username;
        Score = score;
        Time = time;
    }
}

public class AuthSession {
    public string Token { get; init; }

    public AuthSession(){}

    public AuthSession(string token) {
        Token = token;
    }
}
