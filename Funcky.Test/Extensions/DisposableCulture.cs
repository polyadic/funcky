using System.Globalization;

namespace Funcky.Test.Extensions;

internal class DisposableCulture : IDisposable
{
    private readonly CultureInfo _lastCulture;
    private bool _disposedValue;

    public DisposableCulture(string culture)
    {
        _lastCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo(culture);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                CultureInfo.CurrentCulture = _lastCulture;
            }

            _disposedValue = true;
        }
    }
}
