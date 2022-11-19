using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;

public class PromiseDemo : MonoBehaviour
{
    Promise<List<rT>> AsPromiseAccumulating<rT>(IEnumerator coroutine) {
        Promise<List<rT>> promise = new Promise<List<rT>>();
        this.StartCoroutine(RunCoroutineAccumulating<rT>(promise, coroutine));
        return promise;
    }
    static IEnumerator RunCoroutineAccumulating<rT>(Promise<List<rT>> promise, IEnumerator coroutine) {
        List<rT> lst = new List<rT>();
        while(coroutine.MoveNext()) {
            var cr = coroutine.Current;
            lst.Add((rT) cr);
            yield return cr;
        }
        promise.Resolve(lst);
    }
    Promise<rT> AsPromise<rT>(IEnumerator coroutine) {
        Promise<rT> promise = new Promise<rT>();
        this.StartCoroutine(RunCoroutine<rT>(promise, coroutine));
        return promise;
    }
    static IEnumerator RunCoroutine<rT>(Promise<rT> promise, IEnumerator coroutine) {
        while(coroutine.MoveNext()) {
            var cr = coroutine.Current;
            if(cr is rT) {
                promise.Resolve((rT) cr);
                break;
            }
            yield return cr;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Test PromiseTest
        /* print("Will wait for 5 seconds.");
        Promise<string> promise = new Promise<string>();
        StartCoroutine(PromiseTest(promise, 5f));
        promise.Then(rv => {
            print(rv);
            return "hello";
        }).Then(rv => {
            print(rv);
        }) */;
        
        // Test AsPromise
        AsPromise<string>(Waiter(1.0f, "waiter done"))
            .Then(rt => {
                print("Finished");
                print(rt);
                foreach(object o in rt) {
                    print(o);
                }
            });
    }
    
    IEnumerator PromiseTest(Promise<string> promise, float delay) {
        print("Print Now!");
        yield return new WaitForSeconds(delay);
        promise.Resolve(string.Format("Watied for: {0:d} seconds", (int) delay));
    }
    
    IEnumerator Waiter(float delay, string rt) {
        print(string.Format("Start Waiter: {0:d}", (int) delay));
        yield return new WaitForSeconds(delay);
        yield return rt;
    }
}
