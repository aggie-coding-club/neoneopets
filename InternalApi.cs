using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;

public class APIHandler {
    public APIHandler() {
    }
    // Called by application router to handle any /api/ request
    public Task HandleRequest(HttpContext ctx) {
        Task t = Task.Factory.StartNew( () => {
            Console.WriteLine("Start HandleRequest");
            // Gets url path excluding origin
            string path = ctx.Request.Path.ToString();
            List<string> route = path.Split("/").ToList();
            List<string> parms = Array.Empty<string>().ToList();
            string routeSection = route[2];
            if (routeSection.Equals("account")) {
                Account(ctx, route.GetRange(3, route.Count - 3));
            }
            Console.WriteLine("Did HandleRequest");
        } );
        return t;
    }
    void Account(HttpContext ctx, List<string> route) {
        if (route[0].Equals("login")) {
            Login(ctx, route.GetRange(1, route.Count - 1));
            return;
        }
    }
    void Login(HttpContext ctx, List<string> route) {
        Console.WriteLine("Start Login");
        if (route.Count > 1) {
            ctx.Response.StatusCode = 404;
            return;
        }
        if (!ctx.Request.Method.Equals("GET")) {
            ctx.Response.StatusCode = 405;
            return;
        }
        // Check for Authorization Header
        // Check for username/password query strings
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
    
    static Dictionary<string, string> ParmsToDict(List<string> parms) {
        Dictionary<string, string> dct = new Dictionary<string, string>();
        foreach (string p in parms) {
            string[] spl = p.Split("=", 2);
            dct.Add(spl[0], spl[1]);
        }
        return dct;
    }
}
