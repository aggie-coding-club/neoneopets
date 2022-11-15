using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collection.Generic;
using System.Security.Cryptography;
using System;

public class APIHandler {
    // Called by application router to handle any /api/ request
    public static Task HandleRequest(HttpContext ctx) {
        Task t = Task.Factory.StartNew( () => {
            Console.WriteLine("Start HandleRequest");
            // Gets url path excluding origin
            string path = ctx.Request.Path.ToString();
            List<string> route = path.Split("/").ToList();
            List<string> parms = Array.Empty<string>().ToList();
            { // This scope is here to keep the outer scope clean
                List<string> route2 = route.GetRange(0, route.Count - 1);
                // Assumes that url has only one `?`
                List<string> routeEnd = route[route.Count - 1].Split("?").ToList();
                if (routeEnd.Count > 1) {
                    parms = routeEnd[1].Split("&").ToList();
                }
                route2.Add(routeEnd[0]);
                route = route2;
            }
            string routeSection = route[2];
            if (routeSection.Equals("account")) {
                Account(ctx, (route.GetRange(3, route.Count - 3), parms));
            }
            Console.WriteLine("Did HandleRequest");
        } );
        return t;
    }
    static void Account(HttpContext ctx, (List<string>, List<string>) routeData) {
        if (routeData.Item1[0].Equals("login")) {
            Login(ctx, (routeData.Item1.GetRange(1, routeData.Item1.Count - 1), routeData.Item2);
            return;
        }
    }
    static void Login(HttpContext ctx, (List<string>, List<string>) routeData) {
        Console.WriteLine("Start Login");
        if (routeData.Item1.Count > 1) {
            ctx.Response.StatusCode = 404;
            ctx.Response.Status = "login is an endpoint. there are no subroutings";
            return;
        }
        if (!ctx.Request.HttpMethod.Equals("GET")) {
            ctx.Response.StatusCode = 405;
            ctx.Response.Status = "login only supports GET operations.";
            return;
        }
        Dictionary<string, string> parms = ParmsToDict(routeData.Item2);

    }
    static string HashPassword(string pswd) {
        // Generate 16 byte salt
        byte[] salt = RandomNumberGenerator.GetBytes(16); // divide by 8 to convert bits to bytes

        // Generate 32 byte hash using password and salt with SHA-256
        byte[] hash = KeyDerivation.Pbkdf2(
            password: pswd,
            salt: salt,
            prf: KeyDerivation.KeyDerivationPrf.HMACSHA256,
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
            prf: KeyDerivation.KeyDerivationPrf.HMACSHA256,
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
            (string name, string val) = p.Split("=", 2);
            dct.Add(name, val);
        }
        return dct;
    }
}
