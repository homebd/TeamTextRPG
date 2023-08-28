/// <summary
/// 다른 매니저들을 static으로 생성해주는 싱글톤 클래스
/// </summary>

namespace TeamTextRPG.Managers
{
    internal class GameManager
    {
        private static GameManager? _instance;
        public static GameManager Instance
        {
            get
            {
                if(_instance == null) _instance = new GameManager();
                return _instance;
            }
        }
        public SceneManager SceneManager { get; private set; }
        public DataManager DataManager { get; private set; }
        public UIManager UIManager { get; private set; }

        public GameManager()
        {
            SceneManager = new SceneManager();
            DataManager = new DataManager();
            UIManager = new UIManager();
        }
    }
}
