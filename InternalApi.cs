using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class APIHandler {
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
            string endpoint = route[2];
            if (endpoint.Equals("login")) {
                Login(ctx);
            }
            Console.WriteLine("Did HandleRequest");
        } );
        return t;
    }
    static void Login(HttpContext ctx) {
        Console.WriteLine("Start Login");
    }
}
