using Newtonsoft.Json;
using Planday.Schedule.ApiClient;
using Planday.Schedule.Models;
using RestSharp;

namespace Planday.Schedule.Infrastructure.ApiClient
{
    public class EmailApiClient : IEmailApiClient
    {
        public EmployeeInfo? EmployeeEmail(long employeeId)
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

                if (jsonObject == null)
                {
                    return null;
                }

                // Access the "email" property
                string email = jsonObject.email;
                string name = jsonObject.name;

                var retVal = new EmployeeInfo(name, email);

                return retVal;
            }
            else
            {
                return null;
            }
        }
    }
}

