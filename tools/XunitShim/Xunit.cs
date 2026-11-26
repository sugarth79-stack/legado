using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xunit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FactAttribute : Attribute { public string? Skip { get; set; } public string? DisplayName { get; set; } }

    [AttributeUsage(AttributeTargets.Method)]
    public class TheoryAttribute : FactAttribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class InlineDataAttribute : Attribute
    {
        public InlineDataAttribute(params object?[] data) => Data = data;
        public object?[] Data { get; }
    }

    public sealed class XunitException : Exception { public XunitException(string m) : base(m) { } }

    public static class Assert
    {
        static XunitException Fail(string m) => new XunitException(m);

        public static void Equal<T>(T expected, T actual)
        {
            if (expected is IEnumerable ee && actual is IEnumerable ae && expected is not string)
            {
                var l = ee.Cast<object?>().ToList(); var r = ae.Cast<object?>().ToList();
                if (!l.SequenceEqual(r)) throw Fail($"Assert.Equal() Failure: secuencias distintas");
                return;
            }
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw Fail($"Assert.Equal() Failure\nExpected: {expected}\nActual:   {actual}");
        }
        public static void Equal(double expected, double actual, double tolerance)
        {
            if (Math.Abs(expected - actual) > tolerance)
                throw Fail($"Assert.Equal() Failure\nExpected: {expected} ± {tolerance}\nActual:   {actual}");
        }
        public static void Equal(double expected, double actual, int precision)
        {
            if (Math.Round(expected, precision) != Math.Round(actual, precision))
                throw Fail($"Assert.Equal() Failure\nExpected: {expected}\nActual:   {actual}");
        }
        public static void NotEqual<T>(T expected, T actual)
        {
            if (EqualityComparer<T>.Default.Equals(expected, actual)) throw Fail($"Assert.NotEqual() Failure: {actual}");
        }
        public static void True(bool condition, string? message = null) { if (!condition) throw Fail(message ?? "Assert.True() Failure"); }
        public static void False(bool condition, string? message = null) { if (condition) throw Fail(message ?? "Assert.False() Failure"); }
        public static void Null(object? o) { if (o != null) throw Fail($"Assert.Null() Failure: {o}"); }
        public static void NotNull(object? o) { if (o == null) throw Fail("Assert.NotNull() Failure"); }
        public static void Same(object? e, object? a) { if (!ReferenceEquals(e, a)) throw Fail("Assert.Same() Failure"); }
        public static void Empty(IEnumerable c) { if (c.Cast<object?>().Any()) throw Fail("Assert.Empty() Failure"); }
        public static void NotEmpty(IEnumerable c) { if (!c.Cast<object?>().Any()) throw Fail("Assert.NotEmpty() Failure"); }
        public static T Single<T>(IEnumerable<T> c) { var l = c.ToList(); if (l.Count != 1) throw Fail($"Assert.Single() Failure: {l.Count} elementos"); return l[0]; }
        public static void Contains<T>(T expected, IEnumerable<T> c) { if (!c.Contains(expected)) throw Fail($"Assert.Contains() Failure: {expected}"); }
        public static void DoesNotContain<T>(T expected, IEnumerable<T> c) { if (c.Contains(expected)) throw Fail($"Assert.DoesNotContain() Failure: {expected}"); }
        public static void Contains<T>(IEnumerable<T> c, Predicate<T> filter) { if (!c.Any(x => filter(x))) throw Fail("Assert.Contains() Failure: ningún elemento cumple el filtro"); }
        public static void DoesNotContain<T>(IEnumerable<T> c, Predicate<T> filter) { if (c.Any(x => filter(x))) throw Fail("Assert.DoesNotContain() Failure: algún elemento cumple el filtro"); }
        public static T Single<T>(IEnumerable<T> c, Predicate<T> filter) { var l = c.Where(x => filter(x)).ToList(); if (l.Count != 1) throw Fail($"Assert.Single() Failure: {l.Count} elementos"); return l[0]; }
        public static void Contains(string sub, string? s) { if (s == null || !s.Contains(sub)) throw Fail($"Assert.Contains() Failure: '{sub}'"); }
        public static T IsType<T>(object? o) { if (o is null || o.GetType() != typeof(T)) throw Fail($"Assert.IsType() Failure: {o?.GetType()}"); return (T)o; }
        public static T IsAssignableFrom<T>(object? o) { if (o is not T t) throw Fail($"Assert.IsAssignableFrom() Failure: {o?.GetType()}"); return t; }

        public static T Throws<T>(Action testCode) where T : Exception
        {
            try { testCode(); }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(T)) return (T)ex;
                throw Fail($"Assert.Throws() Failure\nExpected: {typeof(T)}\nActual:   {ex.GetType()}: {ex.Message}");
            }
            throw Fail($"Assert.Throws() Failure\nExpected: {typeof(T)}\nActual:   (no exception)");
        }
        public static T Throws<T>(Func<object?> testCode) where T : Exception => Throws<T>(() => { testCode(); });
    }
}
