using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Miscelaneous
{
    /// <summary>
    /// A mix of useful extension methods.
    /// </summary>
    public static class Extensions
    {
        private const string isDeletedProperty = "IsDeleted";

        private static readonly MethodInfo propertyMethod = typeof(EF)
            .GetMethod(nameof(EF.Property), BindingFlags.Static | BindingFlags.Public)
            .MakeGenericMethod(typeof(bool));

        /// <summary>
        /// Converts a string to base64 string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToBase64(this string value)
        {
            var inputBytes = Encoding.UTF8.GetBytes(value);

            // Each 3 byte sequence in inputBytes must be converted to a 4 byte sequence
            var arrLength = (long)(4.0d * inputBytes.Length / 3.0d);
            if ((arrLength % 4) != 0)
            {
                // increment the array length to the next multiple of 4 if it is not already divisible by 4
                arrLength += 4 - (arrLength % 4);
            }

            var encodedCharArray = new char[arrLength];
            Convert.ToBase64CharArray(inputBytes, 0, inputBytes.Length, encodedCharArray, 0);
            var result = new string(encodedCharArray);
            return result;
        }

        /// <summary>
        /// Converts a base64 string to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string FromBase64(this string value)
        {
            var decodedCharArray = Convert.FromBase64String(value);
            var result = Encoding.UTF8.GetString(decodedCharArray);
            return result;
        }

        ///// <summary>
        ///// Cancels the last changes tracked by the specified context.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <exception cref="ArgumentNullException">context</exception>
        //public static void CancelChanges(this DbContext context)
        //{
        //    //var context = DataContextFactory.GetDataContext();

        //    var tracker = context?.ChangeTracker ?? throw new ArgumentNullException(nameof(context));
        //    var changedEntries = tracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();

        //    foreach (var entry in changedEntries)
        //    {
        //        switch (entry.State)
        //        {
        //            case EntityState.Modified:
        //                entry.CurrentValues.SetValues(entry.OriginalValues);
        //                entry.State = EntityState.Unchanged;
        //                break;

        //            case EntityState.Added:
        //                entry.State = EntityState.Detached;
        //                break;

        //            case EntityState.Deleted:
        //                entry.State = EntityState.Unchanged;
        //                break;
        //        }
        //    }
        //}

        //public static void IsSoftlyDeletable<T>(this EntityTypeBuilder<T> builder) where T : class
        //{
        //    var entities = builder.Metadata.Model.GetEntityTypes().Where(x => x.ClrType == typeof(T));
        //    foreach (var entity in entities)
        //    {
        //        entity.AddProperty(isDeletedProperty, typeof(bool));
        //    }
        //}

        //public static LambdaExpression GetIsDeletedRestriction(this Type type)
        //{
        //    var parm = Expression.Parameter(type, "it");
        //    var prop = Expression.Call(propertyMethod, parm, Expression.Constant(isDeletedProperty));
        //    var condition = Expression.MakeBinary(ExpressionType.Equal, prop, Expression.Constant(false));
        //    var lambda = Expression.Lambda(condition, parm);
        //    return lambda;
        //}
    }
}