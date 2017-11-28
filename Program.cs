using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Network I\O
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

// Json
using System.Json;



// Tools NuGet Package Manager Console : 
/* 
 *  Install-Package Microsoft.AspNet.WebApi.Client
 * 
 *  Install-Package System.Json -Version 4.0.20126.16343
 */

namespace HttpGetJsonParsing
{
	/*
	*open API provided by the NYTimes. http://developer.nytimes.com/
	* *https://api.nytimes.com/svc/books/v3/lists/overview.json?api-key=<your-api-key>
    * * my-api-key : 02e195f687414c238cfc492dd6cef131
	* * 
	* */


	public class Book
	{
		private const string authorKey = "author";
		private const string titleKey = "title";
		private const string descriptionKey = "description";

		public string author { get; set; }
		public string title { get; set; }
		public string description { get; set; }
	

		public Book()
		{
			author = "";
			title = "";
			description = "";
		}

		public Book(string jsonString) : this()
		// TODO
		{
			JsonObject jsonObject = JsonObject.Parse(jsonString);
			author = jsonObject.GetNamedString(authorKey, "");
			title = jsonObject.GetNamedString(titleKey, "");

			IJsonValue descriptionJsonValue = jsonObject.GetNamedString(descriptionKey);
			if (descriptionJsonValue.ValueType == JsonValueType.Null)
			{
				description = "No descrption, sorry";
			}
			else
			{
				description = descriptionJsonValue.GetString();
			}

		}
		//-------------AUTO GENERATE--------------
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override string ToString()
		{
			return base.ToString();
		}

	}


		class Program
	{
		// add static HttpClient property to the Program class
		static HttpClient client = new HttpClient();

		// NyApi is the String to get the json
		public static String nyApi = "https://api.nytimes.com/svc/books/v3/lists/overview.json?api-key=02e195f687414c238cfc492dd6cef131";

		// Response is a string build from Json received
		public static string Response = "";

		// Display Response in the console
		static void ShowResponse(String Answer)
		{
			if (string.IsNullOrEmpty(Answer))
			{
				throw new ArgumentException("message", nameof(Answer));
			}
			else
			{
				Console.WriteLine(Answer);
			}
		}
		//---------------------------------------GET & READ----------------------
		static async Task<String> GetResponseAsync(string path)
		{
			String jsonReceived = "";
			// GET
			// The GetAsync method sends the HTTP GET request.
			// The method is asynchronous, because it performs network I/O.
			// When the method completes, it returns an HttpResponseMessage that contains the HTTP response.
			// If the status code in the response is a success code, the response body contains the JSON representation of a product.
			HttpResponseMessage response = await client.GetAsync(path);
			// READ
			// Call ReadAsAsync to deserialize the JSON payload to a Product instance.
			// The ReadAsync method is asynchronous because the response body can be arbitrarily large.
			// HttpClient does not throw an exception when the HTTP response contains an error code.
			// Instead, the IsSuccessStatusCode property is false if the status is an error code.
			// When ReadAsAsync is called with no parameters,
			// it uses a default set of media formatters to read the response body.
			// The default formatters support JSON, XML, and Form-url-encoded datas
			if (response.IsSuccessStatusCode)
			{
				jsonReceived = await response.Content.ReadAsStringAsync();
			}
			return jsonReceived;


			// Instead of using the default formatters, you can provide a list of formatters to the ReadAsync method,
			// which is useful if you have a custom media-type formatter

		}

		//----------------------------MAIN-------------------------
		static void Main(string[] args)
		{
			Console.WriteLine("Data provided by The New York Times, for developement purpose only");
			Console.WriteLine("Prior written consent from The New York Times is required to use Times data without attribution.");
			RunAsync().Wait();
		}
		// network done in background thread
		// TODO: fix warning on use of await
		static async Task RunAsync()
		{
			// initialization
			// sets the base URI for HTTP requests,
			// and sets the Accept header to "application/json",
			// which tells the server to send data in JSON format.
			client.BaseAddress = new Uri(nyApi);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// hard coded demo
			try
			{
				String answer = await GetResponseAsync(nyApi);
				ShowResponse(answer);
				// for demo purpose only, stop before Parsing
				Console.ReadLine();
				// Parsing
				// extract books from json
				Book firstBook = new Book(answer);
					Console.WriteLine("First Book");
					Console.WriteLine("Title: ",firstBook.title);
					Console.WriteLine("Author", firstBook.author);
					Console.WriteLine("Description", firstBook.description);
					// for demo purpose only, stop before quiting
					Console.ReadLine();

				}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			



		}
	}
}
