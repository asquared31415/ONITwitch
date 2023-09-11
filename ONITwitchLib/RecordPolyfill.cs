using JetBrains.Annotations;
using ONITwitchLib.Attributes;

// ReSharper disable once CheckNamespace needs to be in this namespace to be the correct class for the compiler to find
namespace System.Runtime.CompilerServices;

/// <summary>
///     Allows using records in .NET Framework.
///     https://developercommunity.visualstudio.com/t/error-cs0518-predefined-type-systemruntimecompiler/1244809#TPIN-N1249582
/// </summary>
[NotPublicAPI(
	"This will always exist so you can use records, but its exact implementation details are not public API."
)]
[UsedImplicitly]
public static class IsExternalInit;
