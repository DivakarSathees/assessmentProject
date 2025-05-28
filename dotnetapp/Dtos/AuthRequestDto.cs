namespace dotnetapp.Dtos{
public class AuthRequestDto { 
    public string Username {get; set;}
    // password should be at least 8 characters
    [System.ComponentModel.DataAnnotations.MinLength(8)]
    public string Password {get; set;}
}
}
