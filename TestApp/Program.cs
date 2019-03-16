using System;
using System.IO;
using ADSD;
using SkinnyJson;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var raw = File.ReadAllText(@"C:\Temp\security.json");
            var config = Json.Defrost<SecurityConfig>(raw);

            SigningKeys.UpdateKeyCache(config.KeyDiscoveryUrl);

            Console.WriteLine("Cache is populating. Press [enter] to start test when you see them come in.");
            Console.ReadLine();

            var testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyIsImtpZCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyJ9.eyJhdWQiOiI4NzgwY2IzZC1iOWJkLTQ4MDgtYTc4OS0xZTk4NTY3ZTNjNzIiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC80ZDEzMzgwZi1iMmRjLTQwMWUtYWRlOC0wM2E2NzgwMWM2NzYvIiwiaWF0IjoxNTUyNzQ1NzEwLCJuYmYiOjE1NTI3NDU3MTAsImV4cCI6MTU1Mjc0OTYxMCwiYWlvIjoiNDJKZ1lKajltK256VEIrK3ZYeVBIMDQ4dU9wM093QT0iLCJhcHBpZCI6IjZhNWQ4NTIzLTgzZmYtNDQyYS04YWI2LTQ3YzZiOTZjOTJlNiIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzRkMTMzODBmLWIyZGMtNDAxZS1hZGU4LTAzYTY3ODAxYzY3Ni8iLCJvaWQiOiIxN2IwYzViYi0wMjQ4LTQ2MGItOTU4Ni1lNDZhNmRmNDkwZWUiLCJzdWIiOiIxN2IwYzViYi0wMjQ4LTQ2MGItOTU4Ni1lNDZhNmRmNDkwZWUiLCJ0aWQiOiI0ZDEzMzgwZi1iMmRjLTQwMWUtYWRlOC0wM2E2NzgwMWM2NzYiLCJ1dGkiOiJ0YzM1S1d3aHJrMk9QUFVLY0Rsb0FBIiwidmVyIjoiMS4wIn0.GWPxrMZXoYqFVngtZFh-dYmn80YDDQ4Cy3M-irk1KI9XddQxEzoI80frCiq-ODeGBIEdGq2KGM0heUW5XKr31fFiC66hDVQj1GYMtdqJLI1nL95a7ClTzzwqWcHu2uii4fFFsLvgsZaBNs7pSJc4jxMrgZlOh6wPzDflVEFyRRXH3QMXy7sW1mqWaPu5Rq9eKIuF_L8N5h0tMPuDs9PBH3MTGqROHOTn39803JUgfuZI0kBqm6GSecWF_p62j6o8YF5Qe_OvH73Ksm6wz0x83oGZpfUiQD3lntffjm_Kzxr-IE0aXdUFTptxvMH2z7pzHbcxxPlixIoFomJMUdc6cQ";

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
