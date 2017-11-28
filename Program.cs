using System;
using System.Threading.Tasks;

// Network I\O
using System.Net.Http;
using System.Net.Http.Headers;

// Json
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

using static HttpGetJsonParsing.NYTimesJsonConverted;
using System.IO;

// Tools NuGet Package Manager Console : 
/* 
 *  Install-Package Microsoft.AspNet.WebApi.Client
 * 
 *  Install-Package NewtonSoft.Json
 */

namespace HttpGetJsonParsing
{
	/*
	*open API provided by the NYTimes. http://developer.nytimes.com/
	* *https://api.nytimes.com/svc/books/v3/lists/overview.json?api-key=<your-api-key>
    * * my-api-key : 02e195f687414c238cfc492dd6cef131
	* * 
	* */

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

		}

		// -------------------------------Json to Object
		private static T DeSerializedJsonData<T>(string json_data) where T : new()
		{

			// if string with JSON data is not empty, deserialize it to class and return its instance 
			if (!string.IsNullOrEmpty(json_data))
				{return  JsonConvert.DeserializeObject<T>(json_data) ; }
			else { throw new ArgumentNullException(nameof (DeSerializedJsonData)) ; }
			
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
				// Get Json
				//TODO: check connectivity
				String answer = await GetResponseAsync(nyApi);
				ShowResponse(answer);

				// for demo purpose only, stop before Parsing
				Console.ReadLine();


				// extract higher class from json--------------------------------------
				NYTimesJsonConverted resultFromNYTimeApi = new NYTimesJsonConverted();
				resultFromNYTimeApi = DeSerializedJsonData<NYTimesJsonConverted>(answer);
				
				// get inside	-------------------------------------------			
				JObject o = new JObject();
				o = JObject.Parse(answer);

				// status should be OK
				// TODO: if not OK?
				string status = (string)o["status"];
				Console.WriteLine(status);
				Console.ReadLine();

				int nbResults = (int)o["num_results"];
				Console.WriteLine(nbResults);
				Console.ReadLine();

				//// book extraction-------------------------------------------
				JsonTextReader reader = new JsonTextReader(new StringReader(answer));
				int i = 0;
				string[] listOfBook = new string[nbResults];
				while (reader.Read())
				{
					if (reader.Value!=null)
					{
						if ((reader.Value.Equals("title")))
						{
							listOfBook[i] = reader.ReadAsString();
							//---
							Console.WriteLine(listOfBook[i]);
							i++;
							
						}

					}

				}
				;

				// for demo purpose onlywith console, stop before quiting
				Console.ReadLine();

				}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Console.ReadLine();
			}

			



		}
	}
}
