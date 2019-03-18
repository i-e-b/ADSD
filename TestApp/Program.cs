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


            var testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyIsImtpZCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyJ9.eyJhdWQiOiIyODY4YTdiNS1lYTY0LTQ1NTEtOWY4NC1jOTUxMTk5YmFjMTQiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC80ZDEzMzgwZi1iMmRjLTQwMWUtYWRlOC0wM2E2NzgwMWM2NzYvIiwiaWF0IjoxNTUyODk0ODI5LCJuYmYiOjE1NTI4OTQ4MjksImV4cCI6MTU1Mjg5ODcyOSwiYWlvIjoiNDJKZ1lOaTNldmZXT2U0WEdpOEtwRjVaR1AvbE13QT0iLCJhcHBpZCI6IjI4NjhhN2I1LWVhNjQtNDU1MS05Zjg0LWM5NTExOTliYWMxNCIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzRkMTMzODBmLWIyZGMtNDAxZS1hZGU4LTAzYTY3ODAxYzY3Ni8iLCJvaWQiOiI1MDhkYmEyMi0zYTJjLTQzMGQtODY3OC04YjA2MDk4Mjk3NGQiLCJzdWIiOiI1MDhkYmEyMi0zYTJjLTQzMGQtODY3OC04YjA2MDk4Mjk3NGQiLCJ0aWQiOiI0ZDEzMzgwZi1iMmRjLTQwMWUtYWRlOC0wM2E2NzgwMWM2NzYiLCJ1dGkiOiJ0TVhCVlN4WHJVZXRtbEN0SHA2YkFBIiwidmVyIjoiMS4wIn0.ImUEqfLX0-wmKcHRVrgYtO8Ijt93bXh2lPBGpeFMPDBoR18KG6T9yyZjPi4EssuLgQAOS4crYAmL9TyCAVu2gEv-JAi7kHVVgnFN2isPUZK1qgfpq2LthUt9Bvf4-nYVWn9D4lJC_f45F6q3GAr2iSvPNgFzKZoDA9V-ezSKHYE2CHaSH3UeNoK-DQpNi-XLJpOD-WQVN1qxLKGfLHIqg_t3csLHkNW09V4XBx63HEsjnJ_2r0qkr8sBm9z_M3-qlXhGpi_mD50zhOZi8886wQ70Y0-Zk5aAC5k9piastjoNr_6tW-WF72X25rqQbIMQbTVLtvbpByjHdGTSlYX1Xg";

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
