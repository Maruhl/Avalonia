﻿using System;

using Avalonia.Controls;

namespace Avalonia;

/// <summary>
/// Interface for elements that supports dynamic theme.
/// </summary>
public interface IThemeStyleable : IResourceHost
{
    /// <summary>
    /// Gets the UI theme that is used by the control (and its child elements) for resource determination.
    /// </summary>
    ElementTheme Theme { get; }

    /// <summary>
    /// Raised when the theme is changed on the element or an ancestor of the element.
    /// </summary>
    event EventHandler? ThemeChanged;
}