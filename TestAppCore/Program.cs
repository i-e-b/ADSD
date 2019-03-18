using System;
using System.IO;
using ADSD;
using ADSD.Json;

namespace TestAppCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var raw = File.ReadAllText(@"C:\Temp\security.json");
            var config = JsonTool.Defrost<SecurityConfig>(raw);

            Console.WriteLine("Cache is populating.");
            SigningKeys.RefreshKeys(config.KeyDiscoveryUrl, TimeSpan.FromHours(1));

            var testToken = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyIsImtpZCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyJ9.eyJhdWQiOiIyODY4YTdiNS1lYTY0LTQ1NTEtOWY4NC1jOTUxMTk5YmFjMTQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC80ZDEzMzgwZi1iMmRjLTQwMWUtYWRlOC0wM2E2NzgwMWM2NzYvIiwiaWF0IjoxNTUyOTE2MDU0LCJuYmYiOjE1NTI5MTYwNTQsImV4cCI6MTU1MjkxOTk1NCwiYWlvIjoiNDJKZ1lIajR4U3JTYWJaMndhVnpHM2N0NFg0NENRQT0iLCJhcHBpZCI6IjI4NjhhN2I1LWVhNjQtNDU1MS05Zjg0LWM5NTExOTliYWMxNCIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzRkMTMzODBmLWIyZGMtNDAxZS1hZGU4LTAzYTY3ODAxYzY3Ni8iLCJvaWQiOiI1MDhkYmEyMi0zYTJjLTQzMGQtODY3OC04YjA2MDk4Mjk3NGQiLCJzdWIiOiI1MDhkYmEyMi0zYTJjLTQzMGQtODY3OC04YjA2MDk4Mjk3NGQiLCJ0aWQiOiI0ZDEzMzgwZi1iMmRjLTQwMWUtYWRlOC0wM2E2NzgwMWM2NzYiLCJ1dGkiOiItdHdkamhGMHlVdWhhVkxwOERYQkFBIiwidmVyIjoiMS4wIn0.Y1eO8SXXHIRgQ3urJG93lLepBlxxmdWc1B7Nx8FSKeCU18saJa145wbQ2kwVzlCrzb5N7siKUU7UIeuKgjJWCJh7F-j0XWZcvgpk2YU5RsuHsdLU2HrEpXfnoKNpWPhsi864RvI7P7unQI761Jj1PTZ49-oslotgD76fHfzSEbFbWNaPb5SN1cAKdgRkIsAn9XA4bgQqPMZ4zTkoWpC37tSkHlxT3NhpWq6MLHnvvLIqssu89u5FhfHal-MXsPymb7fdzdoF14sMv7WLU_p5r_H9Gcryt2VVmdhV5GAAyt9Y7lE4HdXFCidHOFA06vdkB1Yu9rdBcO2jzqhGzDXDFg";

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
