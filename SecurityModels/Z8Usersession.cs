using System;
using System.Collections.Generic;
namespace Enterprise.Models;

public partial class Usersession
{
    public int Id { get; set; }

    public int? Userid { get; set; }

    public string? Token { get; set; }

    public string? GoogleToken { get; set; }
    public string? FacebookToken { get; set; }
    public string? MicrosoftToken { get; set; }
    public string? Targetcipher { get; set; }

    public int? Acknowledged { get; set; }

    public int? Actionpriority { get; set; }

    public string? Sessionstart { get; set; }

    public string? Sessionend { get; set; }

    public int? Sessionrecorded { get; set; }

    public string? Sessionrecordurl { get; set; }

    public string? Sessiondescription { get; set; }

    public string? Sessionusername { get; set; }

    public string? Sessionemail { get; set; }

    public string? Sessionfirstname { get; set; }

    public string? Sessionlastname { get; set; }

    public string? Sessionfullname { get; set; }

    public int? Sessioncomplete { get; set; }

    public string? Twofactorkey { get; set; }

    public string? Twofactorkeysmsdestination { get; set; }

    public string? Twofactorkeyemaildestination { get; set; }

    public string? Twofactorprovider { get; set; }

    public string? Twofactorprovidertoken { get; set; }

    public string? Twofactorproviderauthstring { get; set; }

    public string? Useridasstring { get; set; }

    public virtual ICollection<Keyassignment> Keyassignments { get; set; } = new List<Keyassignment>();
}
