public class LoginResponse
{
    public int id;
    public bool imposter;
    public int color;

    public LoginResponse(int id)
    {
        this.id = id;
    }

    public LoginResponse(int id, bool imposter, int color)
    {
        this.id = id;
        this.imposter = imposter;
        this.color = color;
    }

}