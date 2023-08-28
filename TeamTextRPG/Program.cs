/// <summary
/// 게임 데이터를 세팅해두고 스택 메모리를 적절히 반환받으며 굴려주는 클래스
/// </summary>

using TeamTextRPG.Managers;

internal class Program
{
    static void Main(string[] args)
    {
        GameManager.Instance.DataManager.GameDataSetting();

        while (true)
        {
            GameManager.Instance.SceneManager.LoadScene();
        }
    }
}

