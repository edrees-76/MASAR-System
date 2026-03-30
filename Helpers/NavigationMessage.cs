using System;

namespace MASAR.Helpers;

public class NavigationMessage
{
    public string ViewName { get; set; } = string.Empty;
    public object? Parameter { get; set; }
}
