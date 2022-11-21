using System;
using UnityEngine;
using UnityEngine.Networking;
using RSG;
using System.Collections;
using System.Collections.Generic;

public class MongoConnector : MonoBehaviour
{
    public static string pswd { get; } = "$MONGO_SECRET";
    public static string connectionString;
    /*
     * TODO :: implement API transfer sequence
     * 
     * 1. Create JS request with necessary mongo info
     * 2. Have request handled by outside JS API, caught by Program.cs
     * 3. Program.cs manages request and sends the Mongo request
     * 4. Program.cs returns Mongo request to JS API which returns request to MongoConnecter
     * 5. Method returns result
    */

    public MongoConnector()
    {
        connectionString = "mongodb+srv://neoneopets:" + pswd + "@neoneopets.3l1txwu.mongodb.net/?retryWrites=true&w=majority";
    }

    /*
     * Add:
     *  INPUT:
     *      Takes a string type indicating the name of the database on mongo, and a
     *      Dictionary type, holding the items and their corresponding values
     *      To find out more about dictionaries, visit https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2?view=net-7.0
     *  OUTPUT: 
     *      Returns null (it's a void, duh)
     *      Adds the data to Mongo in the corresponding location indicated by the user
     *      Throws an error if the data isn't formatted as it's required to be
     *  USE CASE:
     *      Adding high scores, user information, passwords, etc.
     *      Built to be used within Unity. If you need to access Mongo on the frontend,
     *      visit Error.razor to see how that might work for you.
     */

    /* USER SCHEMA (Tentative)
     *      Username :: string
     *      Password: Sent as string, stored in encrypted format
     *      For each game, a high score value, stored as an integer
     *      Favorite pet, sent as a tuple of type of pet and its name, both strings
     *      Pets list, works as a list of tuples that include type of pet and name, both strings
     *      Items list, works as a list of strings
     */

    /* GAME SCHEMA (Tentative)
     *      Game name: string
     *          10 of these:
     *              Username: string
     *              Game score: integer
     *              Game time: [month, day, year, hour, minute] all of which are in integer form
     */
    void Add(string db, Dictionary<string, string> send)
    {
        AsPromise<UnityWebRequest>(AddHandler(db, send)).Then(rT =>
        {
            try
            {
                if (rT.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Addition successful.");
                }
                else
                {
                    Debug.Log(rT.GetResponseHeaders()["issue"]);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        });
    }

    string Login(string username, string password)
    {
        string sendBack = "Fatal error in output.";
        AsPromise<UnityWebRequest>(LoginHandler(username, password)).Then(rT =>
        {
            try
            {
                if (rT.result == UnityWebRequest.Result.Success)
                {
                    sendBack = rT.GetResponseHeaders()["token"];
                }
                else
                {
                    sendBack = rT.GetResponseHeaders()["issue"];
                }
            }
            catch (Exception e)
            {
                sendBack = e.ToString();
            }
            return null;
        });
        return sendBack;
    }

    IEnumerator LoginHandler(string username, string password)
    {
        /*
        var dict = new Dictionary<string, string>
        {
            {"username", username},
            {"password", password}
        };
        var toSend = JsonUtility.ToJson(dict);
        */
        var endpoint = "/api/login/?username=" + username + "&password=" + password;
        using (UnityWebRequest request = UnityWebRequest.Get(endpoint))
        {
            //UnityWebRequest request = new UnityWebRequest.Get(endpoint);

            yield return request.SendWebRequest();

            switch (request.GetResponseHeaders()["result"])
            {
                case "success":
                    Debug.Log("Login successful.");
                    break;
                case "failure":
                    Debug.Log("Login failed. API declined.");
                    break;

            }
        }
    }

    IEnumerator AddHandler(string db, Dictionary<string, string> send)
    {
        var toSend = JsonUtility.ToJson(send);
        var endpoint = "/api/scores";
        using (UnityWebRequest request = UnityWebRequest.Put(endpoint, toSend))
        {

            yield return request.SendWebRequest();

            switch (request.GetResponseHeaders()["result"])
            {
                case "success":
                    Debug.Log("Addition successful.");
                    break;
                case "failure":
                    Debug.Log(request.GetResponseHeaders()["issue"]);
                    break;
            }
        }
    }

    Promise<rT> AsPromise<rT>(IEnumerator coroutine)
    {
        Promise<rT> promise = new Promise<rT>();
        this.StartCoroutine(RunCoroutine(promise, coroutine));
        return promise;
    }

    static IEnumerator RunCoroutine<rT>(Promise<rT> promise, IEnumerator coroutine)
    {
        while(coroutine.MoveNext())
        {
            var current = coroutine.Current;
            if (current is rT)
            {
                promise.Resolve((rT) current);
                break;
            }
            yield return current;
        }
    }

}

/* 
 * SCRAP CODE
 * var settings = MongoClientSettings.FromConnectionString("mongodb+srv://neoneopets:"+MongoConnector.pswd+"@neoneopets.3l1txwu.mongodb.net/?retryWrites=true&w=majority");
   var client = new MongoClient(settings);
   var database = client.GetDatabase("Site-Data");
 */