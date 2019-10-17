using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.UI;
using APITest.Modal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.Serialization.Json;

namespace APITest
{
    [TestClass]
    public class UnitTest1
    {
        //Step1 : Start the fake server
        //https://github.com/typicode/json-server#getting-started

        [TestMethod]
        public void GetOperation()
        {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts/{postId}", Method.GET);
            request.AddUrlSegment("postId", 3);
            var response = client.Execute(request);

            var deserialize = new JsonDeserializer();
            var output= deserialize.Deserialize<Dictionary<string,string>>(response);           
            Assert.AreEqual(output["id"], "3", "Error");
        }

        [TestMethod]
        public void PostOperation()
        {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts", Method.POST);          
            request.AddJsonBody(new Post() {author="First", title="first 1" });

            //var response = client.Execute(request);
            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //Assert.AreEqual(output["author"], "First", "Error");

            //or

            var response = client.Execute<Post>(request).Data;          //Generic way
            Assert.AreEqual(response.author, "First", "Error");
        }

        [TestMethod]
        public void PostAsyncOperation()
        {
            var client = new RestClient("http://localhost:3000/");
            var request = new RestRequest("posts", Method.POST);
            request.AddJsonBody(new Post() { author = "First", title = "first 1" });
            
            var response = client.Execute<Post>(request).Data;          //Generic way
            Assert.AreEqual(response.author, "First", "Error");
        }

        private async Task<IRestResponse<T>> ExecuteAsyncRequest<T>(RestClient client , IRestRequest request)
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, restresponse =>
            {
                if (restresponse.ErrorException != null)
                {
                    throw new ApplicationException("Error");
                }
                taskCompletionSource.SetResult(restresponse);
            });
            return await taskCompletionSource.Task;
        }

    }
}
