using System;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace ValorantPorting.ViewModels.Endpoints.Models;

public class AuthResponse
{
    [J("access_token")] public string Token;
    [J("expires_at")] public DateTime ExpireTime;
}