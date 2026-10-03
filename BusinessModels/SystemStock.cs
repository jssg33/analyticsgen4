using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class SystemStock
{
    public int Id { get; set; }

    public string? Ticker { get; set; }

    public string? CompanyName { get; set; }

    public int Source { get; set; }

    public string? Country { get; set; }

    public string? PrimaryExchange { get; set; }

    public string? SecondaryExchange { get; set; }

    public string? Company { get; set; }

    public double? Price2016 { get; set; }

    public double? Price2017 { get; set; }

    public double? Price2018 { get; set; }

    public double? Price2019 { get; set; }

    public double? Price2020 { get; set; }

    public double? Price2021 { get; set; }

    public double? Price2022 { get; set; }

    public double? Price2023 { get; set; }

    public double? Price2024 { get; set; }

    public double? Price2025 { get; set; }

    public double? Price2026 { get; set; }

    public double? Div2016 { get; set; }

    public double? Div2017 { get; set; }

    public double? Div2018 { get; set; }

    public double? Div2019 { get; set; }

    public double? Div2020 { get; set; }

    public double? Div2021 { get; set; }

    public double? Div2022 { get; set; }

    public double? Div2023 { get; set; }

    public double? Div2024 { get; set; }

    public double? Div2025 { get; set; }

    public double? Div2026 { get; set; }

    public string? Sector { get; set; }

    public double? Avgdividend { get; set; }

    public double? Totaldividends { get; set; }

    public double? PriceStart { get; set; }

    public double? PriceEnd { get; set; }

    public double? Change { get; set; }

    public double? Totalreturn { get; set; }

    public double? Totalreturnover10 { get; set; }

    public double? Shares500 { get; set; }

    public double? Totalspend { get; set; }

    public double? Fiveyearequityproj { get; set; }

    public double? Fiveyeardivproj { get; set; }

    public double? Totalfiveyearview { get; set; }

    public string? Selected { get; set; }
}
