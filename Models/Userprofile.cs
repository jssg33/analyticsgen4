using System;
using System.Collections.Generic;

namespace Enterprise.Models;

public partial class Userprofile
{
    public int Id { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public string? Stateregion { get; set; }

    public string? Country { get; set; }

    public string? Phone { get; set; }

    public string? Cellphone { get; set; }

    public int? Sms { get; set; }

    public string? Email { get; set; }

    public string? Maritalstatus { get; set; }

    public string? University1 { get; set; }

    public string? University2 { get; set; }

    public string? Linkedinurl { get; set; }

    public string? Instagramurl { get; set; }

    public string? Vimeourl { get; set; }

    public string? Facebookurl { get; set; }

    public string? Googleurl { get; set; }

    public string? University { get; set; }

    public string? Title { get; set; }

    public string? Pronoun { get; set; }

    public string? Title2 { get; set; }

    public string? Activepictureurl { get; set; }

    public int? Userid { get; set; }

    public string? Employeeid { get; set; }

    public string? Postalzip { get; set; }

    public string? Companyid { get; set; }

    public int? Buid { get; set; }

    public int? Managerid { get; set; }

    public int? Regionid { get; set; }

    public int? Branchid { get; set; }

    public string? Fullname { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Defaultinstanceid { get; set; }

    public string? Defaultshardid { get; set; }

    public byte[]? CipherSupportId { get; set; }

    public byte[]? Targetcipher { get; set; }
}
