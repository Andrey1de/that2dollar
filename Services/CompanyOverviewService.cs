using that2dollar.Data;
using that2dollar.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace that2dollar.Services
{



    public interface ICompanyOverviewService : ISpooledService<CompanyOverview>
    {
    }
    public class CompanyOverviewService :
        AbstractRemoteService<CompanyOverview>, ICompanyOverviewService
    {
        public override string ConvertorUrl =>
             "https://www.alphavantage.co/query?function=OVERVIEW&symbol=";

        public override string GetKey(CompanyOverview item) => item.Symbol;

        public CompanyOverviewService(HttpClient _httpClient,
                           ILogger<CompanyOverviewService> logger)
            : base(_httpClient, logger, "CompanyOverview")
        {

        }

        public override CompanyOverview DecodeBody(string symbol, string jsonBody)
        {

            try
            {
                JObject o = JObject.Parse(jsonBody);


                CompanyOverview ret = new CompanyOverview();
                ret.Symbol = (string)o["Symbol"];
                if (string.IsNullOrWhiteSpace(ret.Symbol)
                    || ret.Symbol != symbol)
                {
                    Log.LogWarning($"Format CompanyOverview invalid");
                    return null;
                }

                Func<string, string> fs = (string name) => (string)o[name] ?? "";
                Func<string, int> fi = (string name) => int.Parse((string)o[name] ?? "0");

                ret.AssetType = fs("AssetType"); //Common Stock",
                ret.Name = fs("Name"); //International Business Machines Corporation",
                ret.Description = fs("Description"); //International Business Machines Corporation provides integrated solutions and services worldwide. Its Cloud & Cognitive Software segment offers software for vertical and domain-specific solutions in health, financial services, supply chain, and asset management, weather, and security software and services application areas; and customer information control system and storage, and analytics and integration software solutions to support client mission critical on-premise workloads in banking, airline, and retail industries. It also offers middleware and data platform software, including Red Hat that enables the operation of clients' hybrid multi-cloud environments; and Cloud Paks, WebSphere distributed, and analytics platform software, such as DB2 distributed, information integration, and enterprise content management, as well as IoT, Blockchain and AI/Watson platforms. The company's Global Business Services segment offers business consulting services; system integration, application management, maintenance, and support services for packaged software; and finance, procurement, talent and engagement, and industry-specific business process outsourcing services. Its Global Technology Services segment provides IT infrastructure and platform services; and project, managed, outsourcing, and cloud-delivered services for enterprise IT infrastructure environments; and IT infrastructure support services. The company's Systems segment offers servers for businesses, cloud service providers, and scientific computing organizations; data storage products and solutions; and z/OS, an enterprise operating system, as well as Linux. Its Global Financing segment provides lease, installment payment, loan financing, short-term working capital financing, and remanufacturing and remarketing services. The company was formerly known as Computing-Tabulating-Recording Co. The company was incorporated in 1911 and is headquartered in Armonk, New York.",
                ret.CIK = fs("CIK"); //0000051143",
                ret.Exchange = fs("Exchange"); //NYSE",
                ret.Currency = fs("Currency"); //USD",
                ret.Country = fs("Country"); //USA",
                ret.Sector = fs("Sector"); //Technology",
                ret.Industry = fs("Industry"); //Information Technology Services",
                ret.Address = fs("Address"); //One New Orchard Road, Armonk, NY, United States, 10504",
                ret.FullTimeEmployees = fi("FullTimeEmployees"); //345900",
                ret.FiscalYearEnd = fs("FiscalYearEnd"); //December",
                ret.LatestQuarter = fs("LatestQuarter"); //2020-12-31",


                return ret;
            }
            catch (Exception ex)
            {

                LogError(ex);
            }
            return null;
        }

        //public override SpoolItem<CompanyOverview> ToSpoolItem(CompanyOverview item)
        //{
        //    return new SpoolItem<CompanyOverview>()
        //    {
        //        Key = item.Symbol,
        //        Data = item,
        //        ActualUntil = DateTime.Now.AddSeconds(MaxReadDelaySec)

        //    };
        //}
    
        
    }


}



