using Newtonsoft.Json;
using Planday.Schedule.ApiClient;
using RestSharp;

namespace Planday.Schedule.Infrastructure.ApiClient
{
    //TODO rn\ename to client form handler
    public class EmailApiClient : IEmailApiClient
    {
        public string? EmployeeEmail(long employeeId)
        {
            var url = $"http://planday-employee-api-techtest.westeurope.azurecontainer.io:5000/employee/{employeeId}";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Get);

            request.AddHeader("accept", "*");
            request.AddHeader("Authorization", "8e0ac353-5ef1-4128-9687-fb9eb8647288"); // move to env variable

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var responseData = response.Content;

                if (responseData == null)
                {
                    return null;
                }

                // Deserialize the JSON string into a dynamic object
                dynamic jsonObject = JsonConvert.DeserializeObject(responseData);

                // Access the "email" property
                var email = jsonObject.email;

                return email;
            }
            else
            {
                return null;
            }
        }
    }
}

