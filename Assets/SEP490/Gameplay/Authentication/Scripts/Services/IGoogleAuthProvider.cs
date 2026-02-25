using System.Threading.Tasks;

public interface IGoogleAuthProvider 
{
    public void StartLogin();
    Task<string> GetIdTokenAsync();
}
