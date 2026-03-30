using System.Threading.Tasks;

namespace MASAR.Services;

public interface IIsotopeImportService
{
    Task<(int imported, int updated)> ImportIsotopesAsync();
}
