using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//# Company Overview
//https://www.alphavantage.co/query?function=OVERVIEW&symbol=IBM&apikey=3MEYVIGY6HV9QYMI

namespace that2dollar.Models
{
    public class CompanyOverview
    {
        public string Symbol { get; set; } //IBM",
        public string AssetType { get; set; } //Common Stock",
        public string Name { get; set; } //International Business Machines Corporation",
        public string Description { get; set; } //International Business Machines Corporation provides integrated solutions and services worldwide. Its Cloud & Cognitive Software segment offers software for vertical and domain-specific solutions in health, financial services, supply chain, and asset management, weather, and security software and services application areas; and customer information control system and storage, and analytics and integration software solutions to support client mission critical on-premise workloads in banking, airline, and retail industries. It also offers middleware and data platform software, including Red Hat that enables the operation of clients' hybrid multi-cloud environments; and Cloud Paks, WebSphere distributed, and analytics platform software, such as DB2 distributed, information integration, and enterprise content management, as well as IoT, Blockchain and AI/Watson platforms. The company's Global Business Services segment offers business consulting services; system integration, application management, maintenance, and support services for packaged software; and finance, procurement, talent and engagement, and industry-specific business process outsourcing services. Its Global Technology Services segment provides IT infrastructure and platform services; and project, managed, outsourcing, and cloud-delivered services for enterprise IT infrastructure environments; and IT infrastructure support services. The company's Systems segment offers servers for businesses, cloud service providers, and scientific computing organizations; data storage products and solutions; and z/OS, an enterprise operating system, as well as Linux. Its Global Financing segment provides lease, installment payment, loan financing, short-term working capital financing, and remanufacturing and remarketing services. The company was formerly known as Computing-Tabulating-Recording Co. The company was incorporated in 1911 and is headquartered in Armonk, New York.",
        public string CIK { get; set; } //0000051143",
        public string Exchange { get; set; } //NYSE",
        public string Currency { get; set; } //USD",
        public string Country { get; set; } //USA",
        public string Sector { get; set; } //Technology",
        public string Industry { get; set; } //Information Technology Services",
        public string Address { get; set; } //One New Orchard Road, Armonk, NY, United States, 10504",
        public int FullTimeEmployees { get; set; } //345900",
        public string FiscalYearEnd { get; set; } //December",
        public string LatestQuarter { get; set; } //2020-12-31",

        public DateTime Updated = DateTime.Now;
   
    }
}

/*
 
 {
    "Symbol": "IBM",
    "AssetType": "Common Stock",
    "Name": "International Business Machines Corporation",
    "Description": "International Business Machines Corporation provides integrated solutions and services worldwide. Its Cloud & Cognitive Software segment offers software for vertical and domain-specific solutions in health, financial services, supply chain, and asset management, weather, and security software and services application areas; and customer information control system and storage, and analytics and integration software solutions to support client mission critical on-premise workloads in banking, airline, and retail industries. It also offers middleware and data platform software, including Red Hat that enables the operation of clients' hybrid multi-cloud environments; and Cloud Paks, WebSphere distributed, and analytics platform software, such as DB2 distributed, information integration, and enterprise content management, as well as IoT, Blockchain and AI/Watson platforms. The company's Global Business Services segment offers business consulting services; system integration, application management, maintenance, and support services for packaged software; and finance, procurement, talent and engagement, and industry-specific business process outsourcing services. Its Global Technology Services segment provides IT infrastructure and platform services; and project, managed, outsourcing, and cloud-delivered services for enterprise IT infrastructure environments; and IT infrastructure support services. The company's Systems segment offers servers for businesses, cloud service providers, and scientific computing organizations; data storage products and solutions; and z/OS, an enterprise operating system, as well as Linux. Its Global Financing segment provides lease, installment payment, loan financing, short-term working capital financing, and remanufacturing and remarketing services. The company was formerly known as Computing-Tabulating-Recording Co. The company was incorporated in 1911 and is headquartered in Armonk, New York.",
    "CIK": "51143",
    "Exchange": "NYSE",
    "Currency": "USD",
    "Country": "USA",
    "Sector": "Technology",
    "Industry": "Information Technology Services",
    "Address": "One New Orchard Road, Armonk, NY, United States, 10504",
    "FullTimeEmployees": "345900",
    "FiscalYearEnd": "December",
    "LatestQuarter": "2020-12-31",
    "MarketCapitalization": "121287507968",
    "EBITDA": "15278999552",
    "PERatio": "21.776",
    "PEGRatio": "1.3853",
    "BookValue": "23.074",
    "DividendPerShare": "6.51",
    "DividendYield": "0.0482",
    "EPS": "6.233",
    "RevenuePerShareTTM": "82.688",
    "ProfitMargin": "0.0759",
    "OperatingMarginTTM": "0.1166",
    "ReturnOnAssetsTTM": "0.0348",
    "ReturnOnEquityTTM": "0.2638",
    "RevenueTTM": "73620996096",
    "GrossProfitTTM": "35575000000",
    "DilutedEPSTTM": "6.233",
    "QuarterlyEarningsGrowthYOY": "-0.631",
    "QuarterlyRevenueGrowthYOY": "-0.065",
    "AnalystTargetPrice": "138.47",
    "TrailingPE": "21.776",
    "ForwardPE": "12.1655",
    "PriceToSalesRatioTTM": "1.6345",
    "PriceToBookRatio": "5.8231",
    "EVToRevenue": "2.3444",
    "EVToEBITDA": "13.6763",
    "Beta": "1.225",
    "52WeekHigh": "137.07",
    "52WeekLow": "103.0292",
    "50DayMovingAverage": "128.0583",
    "200DayMovingAverage": "123.3557",
    "SharesOutstanding": "893593984",
    "SharesFloat": "891967749",
    "SharesShort": "29098388",
    "SharesShortPriorMonth": "29057711",
    "ShortRatio": "4.89",
    "ShortPercentOutstanding": "0.03",
    "ShortPercentFloat": "0.0326",
    "PercentInsiders": "0.133",
    "PercentInstitutions": "58.248",
    "ForwardAnnualDividendRate": "6.52",
    "ForwardAnnualDividendYield": "0.048",
    "PayoutRatio": "0.752",
    "DividendDate": "2021-03-10",
    "ExDividendDate": "2021-02-09",
    "LastSplitFactor": "2:1",
    "LastSplitDate": "1999-05-27"
}
 
 */