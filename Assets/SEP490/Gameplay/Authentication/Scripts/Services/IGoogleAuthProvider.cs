using System.Threading.Tasks;

public interface IGoogleAuthProvider 
{
    Task<string> GetIdTokenAsync();
}
