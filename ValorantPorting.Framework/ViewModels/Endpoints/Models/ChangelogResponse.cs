using System;

namespace ValorantPorting.Framework.ViewModels.Endpoints.Models;

public class ChangelogResponse
{
    public string Title;
    public DateTime PublishDate;
    public string Text;
    public string[] Tags;
    public string ImageURL;
}