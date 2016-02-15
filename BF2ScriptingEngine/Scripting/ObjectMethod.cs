using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2ScriptingEngine.Scripting
{
    /// <summary>
    /// This class represents an <see cref="Func{Token, ConFileEntry}"/> 
    /// to perform whenever an object method call is found 
    /// within a <see cref="ConFile"/>
    /// </summary>
    /// <typeparam name="T">The first method argument's type</typeparam>
    public class ObjectMethod<T> : ObjectMethod
    {
        /// <summary>
        /// The Func<> to perform when invoked
        /// </summary>
        protected Func<Token, T, ConFileEntry> Method;

        /// <summary>
        /// Creates a new instance of <see cref="ObjectMethod{T}"/>.
        /// </summary>
        /// <param name="method">The method to invoke when converting this <see cref="ObjectMethod"/>
        /// back to file format, if any</param>
        public ObjectMethod(Func<Token, T, ConFileEntry> method)
        {
            Method = method;
        }

        /// <summary>
        /// Invokes the underlying <see cref="Func{Token, T, ConFileEntry}"/>
        /// </summary>
        /// <param name="token"></param>
        public override ConFileEntry Invoke(Token token)
        {
            // Shorten this down a bit
            string[] values = token.TokenArgs.Arguments;

            // Ensure the length is correct
            if (token.TokenArgs.Arguments.Length != 1)
            {
                string error = $"Invalid values count for {token.TokenArgs.ReferenceType.Name}; Got {values.Length}, Expecting 1.";
                Logger.Error(error, token.File, token.Position);
                throw new Exception(error);
            }

            // Invoke the method
            return Method.Invoke(token, Converter.ConvertValue<T>(values[0], typeof(T)));
        }
    }

    public class ObjectMethod<T1, T2, T3, T4> : ObjectMethod
    {
        /// <summary>
        /// The Func<> to perform when invoked
        /// </summary>
        protected Func<Token, T1, T2, T3, T4, ConFileEntry> Method;

        /// <summary>
        /// Creates a new instance of <see cref="ObjectMethod{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="action">The <see cref="System.Action"/> to perform when this method is called when 
        /// parsing the <see cref="ConFileObject"/></param>
        /// <param name="format">The method to invoke when converting this <see cref="ObjectMethod"/>
        /// back to file format, if any</param>
        /// <remarks>The get method can be null, however the set method cannot be!</remarks>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="action"/> Action is null</exception>
        public ObjectMethod(Func<Token, T1, T2, T3, T4, ConFileEntry> method)
        {
            Method = method;
        }

        /// <summary>
        /// Invokes the underlying <see cref="Action{T1, T2, T3, T4}"/>
        /// </summary>
        /// <param name="token"></param>
        public override ConFileEntry Invoke(Token token)
        {
            // Shorten this down a bit
            string[] values = token.TokenArgs.Arguments;

            // Ensure the length is correct
            if (token.TokenArgs.Arguments.Length != 4)
            {
                string error = $"Invalid values count for {token.TokenArgs.ReferenceType.Name}; Got {values.Length}, Expecting 4.";
                Logger.Error(error, token.File, token.Position);
                throw new Exception(error);
            }

            // Invoke the method
            return Method.Invoke(token, 
                Converter.ConvertValue<T1>(values[0], typeof(T1)),
                Converter.ConvertValue<T2>(values[1], typeof(T2)),
                Converter.ConvertValue<T3>(values[2], typeof(T3)),
                Converter.ConvertValue<T4>(values[3], typeof(T4))
            );
        }
    }

    /// <summary>
    /// This class represents a <see cref="Func{Token, ConFileEntry}"/> 
    /// to perform whenever an object method call is found within 
    /// a <see cref="ConFile"/>
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
        /// Invokes an <see cref="Func{ConFileEntry}"/> from a derived class.
        /// </summary>
        /// <param name="token">The token of which this call is made from</param>
        /// <param name="arguments"></param>
        public abstract ConFileEntry Invoke(Token token);
    }
}
