// Ejecutor mínimo de tests escritos para xUnit, para entornos sin nuget.org.
// Uso: ShimRunner <ruta/a/tests.dll> [filtro]
using System.Reflection;
using Xunit;

var asmPath = Path.GetFullPath(args[0]);
var filter = args.Length > 1 ? args[1] : null;
var dir = Path.GetDirectoryName(asmPath)!;
AppDomain.CurrentDomain.AssemblyResolve += (_, e) =>
{
    var p = Path.Combine(dir, new AssemblyName(e.Name).Name + ".dll");
    return File.Exists(p) ? Assembly.LoadFrom(p) : null;
};
var asm = Assembly.LoadFrom(asmPath);
int passed = 0, failed = 0;
foreach (var type in asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract).OrderBy(t => t.FullName))
foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).OrderBy(m => m.Name))
{
    var fact = m.GetCustomAttribute<FactAttribute>();
    if (fact == null) continue;
    var name = $"{type.FullName}.{m.Name}";
    if (filter != null && !name.Contains(filter)) continue;
    var cases = fact is TheoryAttribute
        ? m.GetCustomAttributes<InlineDataAttribute>().Select(d => d.Data).ToList()
        : new List<object?[]> { Array.Empty<object?>() };
    if (fact is TheoryAttribute && cases.Count == 0) { Console.WriteLine($"[FAIL] {name}: Theory sin InlineData"); failed++; continue; }
    foreach (var data in cases)
    {
        var label = data.Length == 0 ? name : $"{name}({string.Join(", ", data.Select(d => d ?? "null"))})";
        try
        {
            var ps = m.GetParameters();
            if (ps.Length != data.Length) throw new XunitException($"{ps.Length} parámetros y {data.Length} datos");
            var argv = ps.Select((p, i) => Coerce(data[i], p.ParameterType)).ToArray();
            var inst = m.IsStatic ? null : Activator.CreateInstance(type);
            try { m.Invoke(inst, argv); }
            finally { (inst as IDisposable)?.Dispose(); }
            passed++;
        }
        catch (Exception ex)
        {
            var real = ex is TargetInvocationException tie && tie.InnerException != null ? tie.InnerException : ex;
            Console.WriteLine($"[FAIL] {label}\n  {real.GetType().Name}: {real.Message}");
            failed++;
        }
    }
}
Console.WriteLine($"Total: {passed + failed}, Passed: {passed}, Failed: {failed}");
return failed == 0 && passed > 0 ? 0 : 1;

static object? Coerce(object? v, Type t)
{
    if (v == null) return null;
    var target = Nullable.GetUnderlyingType(t) ?? t;
    if (target.IsInstanceOfType(v)) return v;
    if (target.IsEnum) return Enum.ToObject(target, v);
    return Convert.ChangeType(v, target, System.Globalization.CultureInfo.InvariantCulture);
}
