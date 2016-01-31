using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This class represents an <see cref="System.Action"/> and 
    /// <see cref="Func{String}"/> to perform whenever an object 
    /// method call is found within a <see cref="ConFile"/>
    /// </summary>
    /// <typeparam name="T">The first method argument's type</typeparam>
    public class ObjectMethod<T> : ObjectMethod
    {
        protected Func<Token, string> Format;
        protected Action<Token, T> Action;

        /// <summary>
        /// Creates a new instance of <see cref="ObjectMethod{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="System.Action"/> to perform when this method is called when 
        /// parsing the <see cref="ConFileObject"/></param>
        /// <param name="format">The method to invoke when converting this <see cref="ObjectMethod"/>
        /// back to file format, if any</param>
        /// <remarks>The get method can be null, however the set method cannot be!</remarks>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="action"/> Action is null</exception>
        public ObjectMethod(Action<Token, T> action, Func<Token, string> format = null)
        {
            // We at least need a setter
            if (action == null)
                throw new ArgumentNullException("action");

            Format = format;
            Action = action;
        }

        /// <summary>
        /// Invokes the underlying <see cref="Action{T}"/>
        /// </summary>
        /// <param name="token"></param>
        public override void Invoke(Token token)
        {
            // Shorten this down a bit
            string[] values = token.TokenArgs.Arguments;

            // Ensure the length is correct
            if (token.TokenArgs.Arguments.Length != 1)
            {
                string error = $"Invalid values count for {token.TokenArgs.ReferenceName}; Got {values.Length}, Expecting 1.";
                Logger.Error(error, token.File, token.Position);
                throw new Exception(error);
            }

            // Invoke the method
            Action.Invoke(token, Converter.ConvertValue<T>(values[0], typeof(T)));
        }

        /// <summary>
        /// Invokes the underlying <see cref="Func{T, String}"/> if set.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public override string ToFileFormat(Token token)
        {
            return (Format == null) ? null : Format.Invoke(token);
        }
    }

    /// <summary>
    /// This class represents an <see cref="Action"/> and 
    /// <see cref="Func{String}"/> to perform whenever an object 
    /// method call is found within a <see cref="ConFile"/>
    /// </summary>
    /// <remarks>
    /// Objects that derive from this type will be used to tie in 
    /// ConFile object property calls (<see cref="TokenType.ObjectProperty"/>) 
    /// to Action methods in C#. In the bf2 script engine, not all object
    /// properties are actual properties, rather functions that get performed, 
    /// and that is where ObjectMethod's come into play.
    /// </remarks>
    public abstract class ObjectMethod
    {
        /// <summary>
        /// Invokes an <see cref="Action"/> from a derived class.
        /// </summary>
        /// <param name="token">The token of which this call is made from</param>
        /// <param name="arguments"></param>
        public abstract void Invoke(Token token);

        /// <summary>
        /// Converts this method call into file format, or null if no 
        /// <see cref="Func{String}"/> was supplied.
        /// </summary>
        /// <param name="token">The token to get the Reference call from</param>
        /// <returns></returns>
        public abstract string ToFileFormat(Token token);
    }
}
