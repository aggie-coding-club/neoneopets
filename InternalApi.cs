using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace InternalAPI;

public class APIHandler {
    DBLink.UserStore userStore;
    public APIHandler(DBLink.UserStore store) {
        userStore = store;
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
        if (route.Count > 0) {
            ctx.Response.StatusCode = 404;
            WriteBody(ctx.Response, "Login is an endpoint. There should not be further routing");
            return;
        }
        if (!ctx.Request.Method.Equals("GET")) {
            ctx.Response.StatusCode = 405;
            WriteBody(ctx.Response, "Login only accepts GET requests");
            return;
        }
        // Check for Authorization Header
        // Check for username/password query strings
        if (!ctx.Request.Query.ContainsKey("username") || !ctx.Request.Query.ContainsKey("password")) {
            ctx.Response.StatusCode = 400;
            WriteBody(ctx.Response, "Missing username or password field");
            return;
        }
        string username = ctx.Request.Query["username"];
        string password = ctx.Request.Query["password"];
        try {
            var token = userStore.Authorize(username, password);
        } catch (DBLink.LoadException err) {
            if (err.Issue == DBLink.LoadException.IssueType.InvalidUsername) {
                ctx.Response.StatusCode = 400;
                WriteBody(ctx.Response, "Invalid username");
                return;
            }
            if (err.Issue == DBLink.LoadException.IssueType.UsernameNotExist) {
                ctx.Response.StatusCode = 400;
                WriteBody(ctx.Response, "Username does not exist");
                return;
            }
            if (err.Issue == DBLink.LoadException.IssueType.UnknownOriginator) {
                ctx.Response.StatusCode = 500;
                return;
            }
        } catch (DBLink.AuthorizeException err) {
            if (err.Issue == DBLink.AuthorizeException.IssueType.NoPasswordMatch) {
                ctx.Response.StatusCode = 401;
                WriteBody(ctx.Response, "Password does not match");
                return;
            }
            if (err.Issue == DBLink.AuthorizeException.IssueType.PasswordInvalid) {
                ctx.Response.StatusCode = 401;
                WriteBody(ctx.Response, "Invalid password");
                return;
            }
            if (err.Issue == DBLink.AuthorizeException.IssueType.UnknownOriginator) {
                ctx.Response.StatusCode = 500;
                return;
            }
        }
        ctx.Response.Headers.Add("cdata-token", token);
    }
    void AccountCreate(HttpContext ctx, List<string> route) {
        Console.WriteLine("Start account create");
        if (route.Count > 0) {
            ctx.Response.StatusCode = 404;
            return;
        }
        if (!ctx.Request.Method.Equals("PUT")) {
            ctx.Response.StatusCode = 405;
            WriteBody(ctx.Response, "Login only accepts PUT requests");
            return;
        }
        string rbraw = ctx.Request.Body.ReadToEnd();
    }

    static void WriteBody(HttpResponse response, string content) {
        var bytes = Encoding.UTF8.GetBytes(content);
        response.Body.WriteAsync(bytes, 0, bytes.Length);
    }
}
