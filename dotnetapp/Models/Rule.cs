namespace dotnetapp.Models{


public class Rule {
    public string Id { get; set; }
    public string Action { get; set; }
    public string Expression { get; set; } // e.g. "user.id == resource.ownerId"
}
}
