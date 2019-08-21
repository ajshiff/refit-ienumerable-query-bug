using System;
using System.Collections.Generic;
using Refit;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace RefitIEnumerableBug
{
    // Enum that will be used in example query
    enum ExampleEnum {First, Second, Third};

    // The example query object, featuring Enumerable properties
    class MyQuery
    {
        [AliasAs("ListInts")]
        public IEnumerable<int> ListOfInts {get; set;}
        [AliasAs("ListBools")]
        public IEnumerable<bool> ListOfBools {get; set;}
        [AliasAs("ListEnums")]
        public IEnumerable<ExampleEnum> ListOfEnums {get; set;}
        [AliasAs("ListStrings")]
        public IEnumerable<string> ListOfStrings {get; set;}
    }

    // Example Refit Interface
    interface MyRefitInterface
    {
        [Get("/api/v1/example/{id}")]
        Task<JObject> MyApiEndpoint (int id, MyQuery query = null);
    }

    // Override SendAsync on HttpClientHandler To See outbound message on the console before sending
    class ConsoleLoggingHttpClientHandler : HttpClientHandler 
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("=====Decoded Outbound Request Uri=====");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(WebUtility.UrlDecode(request.RequestUri.ToString()));
            Console.ResetColor();

            return await base.SendAsync(request, cancellationToken);
        }
    }

    // Running Program
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            // Declare Refit Interface
            var refit = RestService.For<MyRefitInterface>( new HttpClient(new ConsoleLoggingHttpClientHandler())
            {
                BaseAddress = new Uri("https://example.com")
            });
            // Prep Variables
            int id = 5;
            MyQuery query = new MyQuery() 
            {
                ListOfInts = new List<int>(){32,42,52},
                ListOfBools = new List<bool> {true, false},
                ListOfEnums = new List<ExampleEnum>(){ExampleEnum.Second, ExampleEnum.Third},
                ListOfStrings = new List<string>(){"Bird", "word"}
            };
            // Make the Call
            try 
            {
                refit.MyApiEndpoint(id, query);
            } 
            catch (Exception) 
            {

            }
            Console.WriteLine("Goodnight World");

        }
    }
}
