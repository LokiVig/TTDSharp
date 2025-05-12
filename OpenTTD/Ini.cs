using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OpenTTD;

/// <summary>
/// Types of groups.
/// </summary>
public enum IniGroupType : byte
{
    Variables = 0, // Values of the form "landscape = hilly"
    List = 1, // A list of values, separated by \n and terminated by the next group block
    Sequence = 2 // A list of uninterpreted lines, terminated by the next group block
}

/// <summary>
/// A single "line" in an ini file.
/// </summary>
public class IniItem
{
    public string name; // The name of this item
    public string value; // The value of this item
    public string comment; // The comment associated with this item

    /// <summary>
    /// Construct a new in-memory item of an Ini-file.
    /// </summary>
    /// <param name="name">The name of the item.</param>
    public IniItem( string name )
    {
        this.name = String.StrMakeValid( name );
    }

    /// <summary>
    /// Replace the current value with another value.
    /// </summary>
    /// <param name="value">The value to replace with.</param>
    public void SetValue( string value )
    {
        this.value = value;
    }
}

/// <summary>
/// A group within an ini file.
/// </summary>
public class IniGroup
{
    public List<IniItem> items; // All items in the group
    public IniGroupType type; // Type of group
    public string name; // Name of group
    public string comment; // Comment for group

    public IniGroup( string name, IniGroupType type )
    {
        this.type = type;
        this.name = String.StrMakeValid( name );
        comment = "\n";
    }

    /// <summary>
    /// Get the item with the given name.
    /// </summary>
    /// <param name="name">Name of the item to find.</param>
    /// <returns>The requested item or <see langword="null"/> if not found.</returns>
    public IniItem GetItem( string name )
    {
        foreach ( IniItem item in items )
        {
            if ( item.name == name )
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// Get the item with the given name, and if it doesn't exist, create a new item.
    /// </summary>
    /// <param name="name">Name of the item to find.</param>
    /// <returns>The requested item.</returns>
    public IniItem GetOrCreateItem( string name )
    {
        foreach ( IniItem item in items )
        {
            if ( item.name == name )
            {
                return item;
            }
        }

        // Item doesn't exist, make a new one
        return CreateItem( name );
    }

    /// <summary>
    /// Create an item with the given name. This does not reuse an existing item of the same name.
    /// </summary>
    /// <param name="name">Name of the item to create.</param>
    /// <returns>The created item.</returns>
    public IniItem CreateItem( string name )
    {
        IniItem item = new IniItem( name );
        items.Add( item );

        return item;
    }

    /// <summary>
    /// Remove the item with the given name.
    /// </summary>
    /// <param name="name">Name of the item to remove.</param>
    public void RemoveItem( string name )
    {
        items.Remove( GetItem( name ) );
    }

    /// <summary>
    /// Clear all items in the group.
    /// </summary>
    public void Clear()
    {
        items.Clear();
    }
}

/// <summary>
/// Ini file that only supports loading.
/// </summary>
public class IniLoadFile
{
    public List<string> IniGroupNameList;

    public List<IniGroup> groups; // All groups in the ini
    public string comment; // Last comment in file

    public List<string> listGroupNames; // List of group names that are lists
    public List<string> seqGroupNames; // List of group names that are sequences

    /// <summary>
    /// Construct a new in-memory Ini file representation.
    /// </summary>
    /// <param name="listGroupNames">A list with group names that should be loaded as lists instead of variables. See <see cref="IniGroupType.List"/>.</param>
    /// <param name="seqGroupNames">A list with group names that should be loaded as lists of names. See <see cref="IniGroupType.Sequence"/>.</param>
    public IniLoadFile( List<string> listGroupNames = null, List<string> seqGroupNames = null )
    {
        this.listGroupNames = listGroupNames;
        this.seqGroupNames = seqGroupNames;
    }

    /// <summary>
    /// Get the group with the given name.
    /// </summary>
    /// <param name="name">Name of the group to find.</param>
    /// <returns>The requested group or <see langword="null"/> if not found.</returns>
    public IniGroup GetGroup( string name )
    {
        foreach ( IniGroup group in groups )
        {
            if ( group.name == name )
            {
                return group;
            }
        }

        return null;
    }

    /// <summary>
    /// Get the group with the given name, and if it doesn't exist, create a new group.
    /// </summary>
    /// <param name="name">Name of the group to find.</param>
    /// <returns>The requested group.</returns>
    public IniGroup GetOrCreateGroup( string name )
    {
        foreach ( IniGroup group in groups )
        {
            if ( group.name == name )
            {
                return group;
            }
        }

        return CreateGroup( name );
    }

    /// <summary>
    /// Create a group with the given name. This does not reuse an existing group of the same name.
    /// </summary>
    /// <param name="name">Name of the group to create.</param>
    /// <returns>The created group.</returns>
    public IniGroup CreateGroup( string name )
    {
        IniGroupType type = IniGroupType.Variables;

        if ( listGroupNames.Contains( name ) )
        {
            type = IniGroupType.List;
        }

        if ( seqGroupNames.Contains( name ) )
        {
            type = IniGroupType.Sequence;
        }

        IniGroup group = new IniGroup( name, type );
        groups.Add( group );

        return group;
    }

    /// <summary>
    /// Remove the group with the given name.
    /// </summary>
    /// <param name="name">Name of the group to remove.</param>
    public void RemoveGroup( string name )
    {
        groups.Remove( GetGroup( name ) );
    }

    /// <summary>
    /// Load the Ini file's data from the disk.
    /// </summary>
    /// <param name="filename">The file to load.</param>
    /// <param name="subdir">The subdir to load the file from.</param>
    public void LoadFromDisk( string filename, Subdirectory subdir )
    {
        Debug.Assert( groups.Count == 0 );

        using FileStream? file = OpenFile( filename, subdir, out dynamic end );

        if ( file == null || file.Length == 0 )
        {
            return;
        }

        using StreamReader reader = new StreamReader( file );
        IniGroup? group = null;
        StringBuilder comment = new();

        string? line;

        while ( ( line = reader.ReadLine() ) != null && file.Position < end )
        {
            line = line.Trim();

            if ( ( group == null || group.type != IniGroupType.Sequence ) && ( line.StartsWith( "#" ) || line.StartsWith( ";" ) || line == "" ) )
            {
                comment.AppendLine( line );
                continue;
            }

            if ( line.StartsWith( "[" ) && line.EndsWith( "]" ) )
            {
                string groupName = line[1..^1].Trim();
                group = CreateGroup( groupName );
                group.comment = comment.ToString();
                comment.Clear();
            }
            else if ( group != null )
            {
                if ( group.type == IniGroupType.Sequence )
                {
                    IniItem item = group.CreateItem( line );
                    item.comment = comment.ToString();
                    comment.Clear();
                    continue;
                }

                int equalsIndex = line.IndexOf( "=" );

                if ( equalsIndex > 0 )
                {
                    string key = line[..equalsIndex].Trim();
                    string value = line[( equalsIndex + 1 )..].Trim();

                    if ( value.StartsWith( "\"" ) && value.EndsWith( "\"" ) )
                    {
                        value = value[1..^1];
                    }

                    IniItem item = group.CreateItem( key );
                    item.comment = comment.ToString();
                    comment.Clear();

                    item.value = string.IsNullOrEmpty( value ) ? null : value;
                }
                else
                {
                    ReportFileError( $"ini: '{line}' outside of group" );
                }
            }

            this.comment = comment.ToString();
        }
    }

    /// <summary>
    /// Open the INI file.
    /// </summary>
    /// <param name="filename">Name of the INI file.</param>
    /// <param name="subdir">The subdir to load the file from.</param>
    /// <param name="size">Size of the opened file.</param>
    /// <returns>File handle of the opened file, or <see langword="null"/>.</returns>
    public virtual FileStream? OpenFile( string filename, Subdirectory subdir, ref dynamic size )
    {
        return null;
    }

    /// <summary>
    /// Report an error about the file contents.
    /// </summary>
    /// <param name="buffer">Part of the file with the error.</param>
    public virtual void ReportFileError( string buffer )
    {

    }
}

public class IniFile : IniLoadFile
{
    /// <summary>
    /// Create a new ini file with given group names.
    /// </summary>
    /// <param name="listGroupNames">A list with group names that should be loaded as lists instead of variables. See <see cref="IniGroupType.List"/>.</param>
    public IniFile( List<string> listGroupNames = null ) : base( listGroupNames )
    {

    }

    /// <summary>
    /// Save the Ini file's data to the disk.
    /// </summary>
    /// <param name="filename">The file to save to.</param>
    /// <returns><see langword="true"/> if saving succeeded.</returns>
    public bool SaveToDisk( string filename )
    {
        string tempFile = filename + ".new";

        try
        {
            using ( StreamWriter writer = new( tempFile, false, Encoding.UTF8 ) )
            {
                foreach ( IniGroup group in groups )
                {
                    writer.Write( group.comment );
                    writer.WriteLine( $"[{group.name}]" );

                    foreach ( IniItem item in group.items )
                    {
                        writer.Write( item.comment );

                        if ( item.name.Contains( " " ) || item.name.StartsWith( "[" ) )
                        {
                            writer.Write( $"\"{item.name}\"" );
                        }
                        else
                        {
                            writer.Write( item.name );
                        }

                        writer.WriteLine( $" = {item.value ?? ""}" );
                    }
                }

                writer.Write( comment );
            }

            File.Replace( tempFile, filename, null );
        }
        catch ( Exception exc )
        {
            Debug.WriteLine( $"Error saving configuration file: {exc.Message}" );
            return false;
        }

        return true;
    }

    public override FileStream? OpenFile( string filename, Subdirectory subdir, ref dynamic size )
    {
        string fullPath = Path.Combine( subdir, filename );

        if ( !File.Exists( fullPath ) )
        {
            size = 0;
            return null;
        }

        FileStream stream = new FileStream( fullPath, FileMode.Open, FileAccess.Read, FileShare.Read );
        size = stream.Length;

        return stream;
    }

    public override void ReportFileError( string buffer )
    {
        Debug.WriteLine( buffer );
    }
}