using System;
using System.Collections.Generic;

namespace StaGeGames.BestFit.EditorSpace.HierarchyIcons
{
    /// <summary>
    /// Components mapped to their corresponding icon (a character in icon font).
    /// </summary>
    static class IconMapping
    {
        public static readonly Dictionary<Type, char> componentIcons = new Dictionary<Type, char>()
        {
            { typeof(SmartMotion),'S' },
            { typeof(BestResize),'e' },
            { typeof(BestFitter),'e' },
            { typeof(SmartManager),'T'},
            { typeof(SmartGridLayout),'1' },
            { typeof(SmartScreenOrientationChecker), 's' },
            { typeof(EditorActiveOnly),  'f' },
        };

        public static readonly Dictionary<string, char> tagIcons = new Dictionary<string, char>()
        {
            //{ "Player", 'q' }
        };
    }
}
