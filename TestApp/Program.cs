using System;
using System.IO;
using ADSD;
using ADSD.Json;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var raw = File.ReadAllText(@"C:\Temp\security.json");
            var config = JsonTool.Defrost<SecurityConfig>(raw);

            Console.WriteLine("Cache is populating.");
            SigningKeys.RefreshKeys(config.KeyDiscoveryUrl, TimeSpan.FromHours(1));


            var testToken = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyIsImtpZCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyJ9.eyJhdWQiOiI4NWRjZTY2Mi1lYTdmLTRmNTEtODEzNy1hM2QzYzRjNWNlMzEiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kMjA3MjkxNi1hMWRmLTQzZTgtYWNiMC1iZjVjN2M2MWE1NjAvIiwiaWF0IjoxNTUzMDAyMDY4LCJuYmYiOjE1NTMwMDIwNjgsImV4cCI6MTU1MzAwNTk2OCwiYWlvIjoiNDJKZ1lIalBJWFR0U0lPeXdKcnoveVhldkpQYkNBQT0iLCJhcHBpZCI6Ijg1ZGNlNjYyLWVhN2YtNGY1MS04MTM3LWEzZDNjNGM1Y2UzMSIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0L2QyMDcyOTE2LWExZGYtNDNlOC1hY2IwLWJmNWM3YzYxYTU2MC8iLCJvaWQiOiI3OWQwMTRlYi1lYzRkLTQxMmYtYTc1OS1mM2YxYmJkZjlkNGEiLCJzdWIiOiI3OWQwMTRlYi1lYzRkLTQxMmYtYTc1OS1mM2YxYmJkZjlkNGEiLCJ0aWQiOiJkMjA3MjkxNi1hMWRmLTQzZTgtYWNiMC1iZjVjN2M2MWE1NjAiLCJ1dGkiOiI3NDZHV1NIbmswTzNTWE5UdHViTUFBIiwidmVyIjoiMS4wIn0.OVSVmTBpdOKSfQIIrJ3Xom8QCHTjR7KRbiAqTIFk9E3o2JlU1jRjoJNhfVmDpVzIXELjqNA0QI9zdh4U_QBGBLMT_p8YRdcGNoZlm5ouVQcVzrR5dxfWw5aF0AbunOGnIfwomvrHj2S92eU2uzEzuChGiNAnI-WcaJrN3Fet0gvyqskn1VjAkGBqVrj5AasbhqPt4Dd-OukFsV9l-u3TdpWeSqCnGCzlbBCeUvErsKiOLcCenarcGen7zJZ9COegytzza5l_cxcO2LYFNRCnCrGk9vyNW8ZCsHqLMjm14A09QKoO6nCJpWlna9WFWvaIDsEXC_ow976ZTe2QwGqaBA";

            var subject = new AadSecurityCheck(config);

            var outcome = subject.Validate(testToken);

            switch (outcome) {
                case SecurityOutcome.Fail:
                    Console.WriteLine("Validation failed");
                    break;

                case SecurityOutcome.Pass:
                    Console.WriteLine("Validated OK!");
                    break;
            }

            Console.ReadLine();
        }
    }
}
