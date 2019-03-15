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

            var testToken = "eyJ0eXAiOiJKV1QiLCJub25jZSI6IkFRQUJBQUFBQUFDRWZleFh4amFtUWIzT2VHUTRHdWd2Zk9WeU1Ga2psN2ZWQ2U0Q3RiMzZjZXRRWmR2WHE0dGplMVZPc1hOclpOTThGejdwOWxuTFJpUXNDX3JaNnZoZVhONnVtVjNKeFJGa2VDdkFvVlQwTHlBQSIsImFsZyI6IlJTMjU2IiwieDV0IjoiTi1sQzBuLTlEQUxxd2h1SFluSFE2M0dlQ1hjIiwia2lkIjoiTi1sQzBuLTlEQUxxd2h1SFluSFE2M0dlQ1hjIn0.eyJhdWQiOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kMjA3MjkxNi1hMWRmLTQzZTgtYWNiMC1iZjVjN2M2MWE1NjAvIiwiaWF0IjoxNTUyNjc5MjI3LCJuYmYiOjE1NTI2NzkyMjcsImV4cCI6MTU1MjY4MzEyNywiYWlvIjoiNDJKZ1lKam90Y2t1ODhPbGF5ZllOdTIxL3ZOR0FBQT0iLCJhcHBfZGlzcGxheW5hbWUiOiJHY01haWxyb29tRmFVa1dlRGV2ZSIsImFwcGlkIjoiODVkY2U2NjItZWE3Zi00ZjUxLTgxMzctYTNkM2M0YzVjZTMxIiwiYXBwaWRhY3IiOiIxIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvZDIwNzI5MTYtYTFkZi00M2U4LWFjYjAtYmY1YzdjNjFhNTYwLyIsIm9pZCI6Ijc5ZDAxNGViLWVjNGQtNDEyZi1hNzU5LWYzZjFiYmRmOWQ0YSIsInN1YiI6Ijc5ZDAxNGViLWVjNGQtNDEyZi1hNzU5LWYzZjFiYmRmOWQ0YSIsInRpZCI6ImQyMDcyOTE2LWExZGYtNDNlOC1hY2IwLWJmNWM3YzYxYTU2MCIsInV0aSI6Im5MT1VvZkJ0VmsyazYtOHUtOTFhQUEiLCJ2ZXIiOiIxLjAiLCJ4bXNfdGNkdCI6MTUyNTc5MjA5M30.DpvUx0fgMPtXthIItPfk42piLKlcha3AQUrKTdEUmzkSY4FjQDqpcroHMGwySTAPTmhG2bvPRa__qQUYl_pJquSyEKuMSzOskAyLuxsdCBdqKb-24t7sASGpgOYiM9_8fkve8lqcPFHwWL5JJUoTFYVlfzv1WdI12GblV9Ou8YZc0ZnearXcQ-5ybkYfdOelxZ629fok13RZDsIPqFKHsWSqSHCnWFrDxM9h-5kWCDbFk33sf1TKiRaUi51lYEi9Q21w7OQ6NSDxSLnJ_vIAzgZicif8jGep-pH8stuSjic5D0arSEMRwyko1n0rnc3uUIH16zp32pQnON52dJiFLQ";

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
