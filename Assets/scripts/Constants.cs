using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spacegame
{
    // so tell me how it felt when you walked on water? did you get your wish?
    public static class Constants
    {
        public static class Input
        {
            // i forgot why these were camelcased
            public const string HORIZONTAL_KEY_DOWN = "horizontalKeyDown";
            public const string HORIZONTAL_KEY_HELD = "horizontalKeyHeld";
            public const string HORIZONTAL_KEY_RELEASED = "horizontalKeyReleased";

            public const string VERTICAL_KEY_DOWN = "verticalKeyDown";
            public const string VERTICAL_KEY_HELD = "verticalKeyHeld";
            public const string VERTICAL_KEY_RELEASED = "verticalKeyReleased";

            public const string SELECT_KEY_DOWN = "selectKeyDown";
            public const string SELECT_KEY_HELD = "selectKeyHeld";
            public const string SELECT_KEY_RELEASED = "selectKeyReleased";
        }
        public static class Meta
        {
            public const string VERSION = "alpha 1.1";
        }
    }
}
