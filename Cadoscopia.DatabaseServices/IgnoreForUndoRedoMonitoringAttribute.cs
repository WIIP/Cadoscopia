using System;

namespace Cadoscopia.DatabaseServices
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreForUndoRedoMonitoringAttribute : Attribute
    {
    }
}