using System;
using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json.Linq;

namespace TimeTracker.Common
{
	public class Parser
	{
		public Parser() { }

		public async Task<string> GetJson(string? uriAdress)
        {
            try
			{
                if (string.IsNullOrEmpty(uriAdress))
                {
                    throw new ArgumentNullException();
                }

                Console.WriteLine("URL: " + uriAdress);
                var config = Configuration.Default.WithDefaultLoader();

                var document = await BrowsingContext.New(config).OpenAsync(uriAdress);

                if (document.Body == null)
                {
                    throw new ArgumentNullException(nameof(document.Body), "Can't be null");
                }

                var dataJson = document.Body.TextContent;
                Console.WriteLine(dataJson);
				return dataJson;
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException("Invalid argument.", exception);
            }
            catch (UriFormatException exception)
            {
                throw new UriFormatException("Invalid URL format.", exception);
            }
            catch (HttpRequestException exception)
            {
                throw new HttpRequestException("Error sending HTTP request.", exception);
            }
            catch (Exception exception)
            {
                throw new Exception("An error occurred.", exception);
            }
        }
	}
}

