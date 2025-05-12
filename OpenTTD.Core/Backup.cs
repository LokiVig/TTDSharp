using System.Diagnostics;

namespace OpenTTD.Core;

/// <summary>
/// Class to backup a specific variable and restore it later.<br/>
/// The variable is not restored automatically, but assertions make sure it is restored.<br/>
/// You have to call either <see cref="Trash()"/> or <see cref="Restore()"/> exactly once.
/// </summary>
public class Backup<T>
{
    private T original;
    private bool valid;
    private T originalValue;

    /// <summary>
    /// Backup variable.
    /// </summary>
    /// <param name="original">Variable to backup.</param>
    public Backup( T original )
    {
        this.original = original;
        valid = true;
        originalValue = original;
    }

    /// <summary>
    /// Backup variable and switch to new value.
    /// </summary>
    /// <param name="original">Variable to backup.</param>
    /// <param name="newValue">New value for variable.</param>
    public Backup( T original, T newValue )
    {
        valid = true;
        originalValue = original;

        this.original = newValue;
    }

    /// <summary>
    /// Check whether the variable was restored on object destruction
    /// </summary>
    ~Backup()
    {
        // Check whether restoration was done
        if ( valid )
        {
            // We cannot assert here, as missing restoration is 'normal' when exceptions are thrown
            // Exceptions are especially used to abort world generation
            // Debug
            Restore();
        }
    }

    /// <summary>
    /// Checks whether the variable was already restored.
    /// </summary>
    /// <returns><see langword="true"/> if variable has already been restored.</returns>
    public bool IsValid()
    {
        return valid;
    }

    /// <summary>
    /// Returns the backupped value.
    /// </summary>
    /// <returns>Value from the backup.</returns>
    public T GetOriginalValue()
    {
        Debug.Assert( valid );
        return originalValue;
    }

    /// <summary>
    /// Change the value of the variable.<br/>
    /// While this does not touch the backup at all, it ensures that the variable is only modified while backupped.
    /// </summary>
    /// <param name="newValue">New value for variable.</param>
    public void Change( T newValue )
    {
        Debug.Assert( valid );
        original = newValue;
    }

    /// <summary>
    /// Revert the variable to its original value, but do not mark it as restored.
    /// </summary>
    public void Revert()
    {
        Debug.Assert( valid );
        original = originalValue;
    }

    /// <summary>
    /// Trash the backup. The variable shall not be restored anymore.
    /// </summary>
    public void Trash()
    {
        Debug.Assert( valid );
        valid = false;
    }

    /// <summary>
    /// Restore the variable.
    /// </summary>
    public void Restore()
    {
        Revert();
        Trash();
    }

    /// <summary>
    /// Update the backup.<br/>
    /// That is trash the old value and make the current value of the variable the value to be restored later.
    /// </summary>
    public void Update()
    {
        Debug.Assert( valid );
        originalValue = original;
    }

    /// <summary>
    /// Check whether the variable is currently equals the backup.
    /// </summary>
    /// <returns><see langword="true"/> if equal.</returns>
    public bool Verify()
    {
        Debug.Assert( valid );
        return originalValue == original;
    }
}

/// <summary>
/// Class to backup a specific variable and restore it upon destruction of this object to prevent<br/>
/// stack values going out of scope before resetting the global to its original value. Contrary to<br/>
/// <see cref="Backup{T}"/> this restores the variable automatically and there is no manual option to restore.
/// </summary>
public class AutoRestoreBackup<T>
{
    private T original;
    private T originalValue;

    /// <summary>
    /// Backup variable and switch to new value.
    /// </summary>
    /// <param name="original">Variable to backup.</param>
    /// <param name="newValue">New value for variable.</param>
    public AutoRestoreBackup( T original, T newValue )
    {
        originalValue = original;
        this.original = newValue;
    }

    /// <summary>
    /// Restore the variable upon object destruction.
    /// </summary>
    ~AutoRestoreBackup()
    {
        original = originalValue;
    }
}