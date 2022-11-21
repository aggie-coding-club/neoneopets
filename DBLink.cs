using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using InternalAPI.DataModel;
using MongoDB.Bson;
using System;

namespace InternalAPI.DBLink;

public class UserStore {
    MongoConnector mongo;
    Dictionary<string, UserFull> documents;
    Dictionary<string, string> authMap;

    public UserStore(MongoConnector mongoConnector) {
        mongo = mongoConnector;
        documents = new Dictionary<string, UserFull>();
        authMap = new Dictionary<string, string>();
    }

    public UserFull Create(string username, string password) {
        if (mongo.UserExists(username)) {
            throw new CreateException(CreateException.IssueType.UsernameExists);
        }
        string token = Guid.NewGuid().ToString();
        UserFull doc = new UserFull(
            username, 
            HashPassword(password),
            new AuthSession {
                Token = token
            }
        );
        documents.Add(username, doc);
        authMap.Add(token, username);
        return doc;
    }

    public UserFull Load(string username) {
        if (!mongo.UserExists(username)) {
            throw new LoadException(LoadException.IssueType.UsernameNotExist);
        }
        BsonDocument user = mongo.GetUser(username);
        UserFull udoc = UserFull.FromBson(user);
        documents.Add(username, udoc);
        return udoc;
    }

    public string Authorize(string username, string password) {
        UserFull udoc;
        if (!documents.ContainsKey(username)) {
            udoc = Load(username);
        } else {
            udoc = documents[username];
        }
        if (!VerifyPassword(password, udoc.Password)) {
            throw new AuthorizeException(AuthorizeException.IssueType.NoPasswordMatch);
        }
        string token = Guid.NewGuid().ToString();
        udoc.Session = new AuthSession {
            Token = token
        };
        return token;
    }

    public (bool, UserFull?) GetUserDoc(string token) {
        if (!authMap.ContainsKey(token)) return (false, null);
        return (true, documents[authMap[token]]);
    }

    static string HashPassword(string pswd) {
        // Generate 16 byte salt
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // Generate 32 byte hash using password and salt with SHA-256
        byte[] hash = KeyDerivation.Pbkdf2(
            password: pswd,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32
        );
        byte[] fullHash = new byte[48];
        Array.Copy(salt, 0, fullHash, 0, 16);
        Array.Copy(hash, 0, fullHash, 16, 32);
        // Convert to base64 for database and easier handling
        return Convert.ToBase64String(fullHash);
    }

    static bool VerifyPassword(string pswd, string hash) {
        // Decode base64 back into bytes
        byte[] hashBytes = Convert.FromBase64String(hash);
        // Copy salt from stored bytes
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);
        // Compute hash using new password and existing salt
        byte[] hashed = KeyDerivation.Pbkdf2(
            password: pswd,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32
        );
        // Ensure that new hash and old hash match
        for (int i=0; i < 32; i++)
            if (hashBytes[i+16] != hashed[i])
                return false;
        return true;
    }
}

public class CreateException : System.Exception {
    public enum IssueType {
        UsernameExists,
        InvalidUsername,
        InvalidPassword,
        UnknownOriginator
    }
    public IssueType Issue { get; }
    
    public CreateException(IssueType issue) {
        Issue = issue;
    }
    
    public CreateException(IssueType issue, string message) : base(message) {
        Issue = issue;
    }
    
    public CreateException(string message, Exception origin) : base(message, origin) {
        Issue = IssueType.UnknownOriginator;
    }
}

public class LoadException : System.Exception {
    public enum IssueType {
        UsernameNotExist,
        InvalidUsername,
        UnknownOriginator,
    }
    public IssueType Issue { get; }
    
    public LoadException(IssueType issue) {
        Issue = issue;
    }
    public LoadException(IssueType issue, string message) : base(message) {
        Issue = issue;
    }
    public LoadException(string message, Exception origin) : base(message, origin) {
        Issue = IssueType.UnknownOriginator;
    }
    
    public string ToString() {
        return base.ToString() + "\nLoadException: " + this.Message;
    }
}

public class AuthorizeException : System.Exception {
    public enum IssueType {
        PasswordInvalid,
        NoPasswordMatch,
        UnknownOriginator
    }
    
    public IssueType Issue { get; }
    
    public AuthorizeException(IssueType issue) {
        Issue = issue;
    }

    public AuthorizeException(IssueType issue, string message) : base(message) {
        Issue = issue;
    }
    
    public AuthorizeException(string message, Exception origin) : base(message, origin) {
        Issue = IssueType.UnknownOriginator;
    }

    public string ToString() {
        return base.ToString() + "\nAuthorizeException: " + this.Message;
    }
}
