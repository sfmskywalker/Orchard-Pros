using System;

namespace OrchardPros.Helpers {
    public static class Guard {
        public static void ArgumentNull<T>(T argument, string argumentName, string message = null) where T:class {
            if(argument == null)
                throw new ArgumentNullException(argumentName, message);
        }
    }
}